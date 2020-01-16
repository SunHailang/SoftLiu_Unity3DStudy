using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Definitions;

// TODO: Collect fields from classes recursive to allow for using neasted classes
// TODO: Add assert for invalid .json file
// TODO: Add asserts for invalid fields/values
// TODO: Add assert if item is not a dictionary
// TODO: Make function for comparing field names
// TODO: Assert on circular dependencies
public class GameDB
{
    // Sorted by Type
    private Dictionary<System.Type, Dictionary<string, ObjectData>> m_byTypeAndKeys = new Dictionary<System.Type, Dictionary<string, ObjectData>>();

    public IEnumerator GetEnumerator<T>() where T : ObjectData
    {
        return m_byTypeAndKeys[typeof(T)].Values.GetEnumerator();
    }

    public IEnumerator GetEnumeratorByType(System.Type type)
    {
        return m_byTypeAndKeys[type].Values.GetEnumerator();
    }

    public ICollection<ObjectData> GetCollection<T>() where T : ObjectData
    {
        return m_byTypeAndKeys[typeof(T)].Values as ICollection<ObjectData>;
    }

    public int GetNumItems<T>() where T : ObjectData
    {
        return m_byTypeAndKeys[typeof(T)].Values.Count;
    }

    public int GetNumItems<T>(System.Predicate<T> predicate) where T : ObjectData
    {
        int count = 0;
        Dictionary<string, ObjectData>.ValueCollection values = m_byTypeAndKeys[typeof(T)].Values;
        foreach (ObjectData value in values)
        {
            if (predicate((T)value))
            {
                count++;
            }
        }
        return count;
    }

    public bool HasKey<T>(string key) where T : ObjectData
    {
        System.Type type = typeof(T);
        Dictionary<string, ObjectData> dict;
        if (m_byTypeAndKeys.TryGetValue(type, out dict))
        {
            if (dict.ContainsKey(key))
            {
                return true;
            }
        }
        return false;
    }
    public bool TryGetItem<T>(string key, out T data) where T : ObjectData
    {
        System.Type type = typeof(T);
        Dictionary<string, ObjectData> dict;
        if (m_byTypeAndKeys.TryGetValue(type, out dict))
        {
            ObjectData val;
            if (dict.TryGetValue(key, out val))
            {
                data = val as T;
                return true;
            }
        }
        data = default(T);
        return false;
    }

    public T GetItem<T>(string key) where T : ObjectData
    {
        System.Type type = typeof(T);
        T data = null;
        Dictionary<string, ObjectData> dict;
        if (m_byTypeAndKeys.TryGetValue(type, out dict))
        {
            ObjectData val;
            if (dict.TryGetValue(key, out val))
            {
                data = val as T;
            }
            else
            {
                Debug.LogWarning(string.Format("Item {0} of type {1} not found in game database", key, type.ToString()));
            }
        }
        else
        {
            Debug.LogError(string.Format("Unknown type {0} in game database", type.ToString()));
        }
        return data;
    }

    public List<T> GetItems<T>(System.Predicate<T> predicate) where T : ObjectData
    {
        List<T> list = new List<T>();
        Dictionary<string, ObjectData>.ValueCollection values = m_byTypeAndKeys[typeof(T)].Values;
        foreach (ObjectData value in values)
        {
            if (predicate((T)value))
            {
                list.Add((T)value);
            }
        }
        return list;
    }

    public T GetItem<T>(System.Predicate<T> predicate) where T : ObjectData
    {
        Dictionary<string, ObjectData>.ValueCollection values = m_byTypeAndKeys[typeof(T)].Values;
        foreach (ObjectData value in values)
        {
            if (predicate((T)value))
            {
                return (T)value;
            }
        }
        return null;
    }

    public void ForEach<T>(System.Action<T> func) where T : ObjectData
    {
        IEnumerator enumerator = m_byTypeAndKeys[typeof(T)].Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            T data = enumerator.Current as T;
            func(data);
        }
    }

    public void Initialize(Dictionary<System.Type, Dictionary<string, ObjectData>> baseData)
    {
        m_byTypeAndKeys = baseData;
    }

    public void Import(Dictionary<string, object> rawData)
    {
        List<ObjectData> allItems = GameData.InstantiateItems(rawData);
        foreach (ObjectData obj in allItems)
        {
            System.Type type = obj.GetType();
            Dictionary<string, ObjectData> dict;
            if (!m_byTypeAndKeys.TryGetValue(type, out dict))
            {
                continue;
            }
            dict[obj.key] = obj;
        }
    }

    private void OnEnable()
    {
#if UNITY_EDITOR

#endif
    }

    private void ApplyData(object item, Dictionary<string, object> data, string baseName, string baseNameAlt)
    {
        FieldInfo[] fieldList = item.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fieldList)
        {
            if (field.FieldType.IsGenericType)
            {
                // Generics
                if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // List
                    object value = null;
                    if (data.TryGetValue(baseName + field.Name, out value) || (field.Name.StartsWith("m_") && data.TryGetValue(baseName + field.Name.Substring(2), out value))
                       || data.TryGetValue(baseNameAlt + field.Name, out value) || (field.Name.StartsWith("m_") && data.TryGetValue(baseNameAlt + field.Name.Substring(2), out value)))
                    {
                        // Create list
                        IList newList = System.Activator.CreateInstance(field.FieldType) as IList;
                        if (value is IList)
                        {
                            // Add from list
                            foreach (object listValue in (value as IList))
                            {
                                newList.Add(ChangeType(listValue, field.FieldType.GetGenericArguments()[0]));
                            }
                        }
                        else
                        {
                            // Add from single
                            newList.Add(ChangeType(value, field.FieldType.GetGenericArguments()[0]));
                        }
                        field.SetValue(item, newList);
                    }
                }
            }
            else if (field.FieldType.IsArray)
            {
                // Array!
                object value = null;
                if (data.TryGetValue(baseName + field.Name, out value) || (field.Name.StartsWith("m_") && data.TryGetValue(baseName + field.Name.Substring(2), out value))
                   || data.TryGetValue(baseNameAlt + field.Name, out value) || (field.Name.StartsWith("m_") && data.TryGetValue(baseNameAlt + field.Name.Substring(2), out value)))
                {

                    // Create array
                    System.Array array;

                    if (value is IList)
                    {
                        array = System.Array.CreateInstance(field.FieldType.GetElementType(), (value as IList).Count);

                        // Add from list
                        int i = 0;
                        foreach (object listValue in (value as IList))
                        {
                            array.SetValue(ChangeType(listValue, field.FieldType.GetElementType()), i++);
                        }
                    }
                    else
                    {
                        array = System.Array.CreateInstance(field.FieldType.GetElementType(), 1);

                        // Add from single
                        array.SetValue(ChangeType(value, field.FieldType.GetElementType()), 0);
                    }
                    field.SetValue(item, array);
                }
            }
            else if (field.FieldType.IsClass && field.FieldType != typeof(string))
            {
                // If the field is a class, recurse into it
                string newBaseName = baseName + field.Name + ".";
                string newBaseNameAlt = baseNameAlt;
                if (field.Name.StartsWith("m_"))
                    newBaseNameAlt += field.Name.Substring(2) + ".";
                else
                    newBaseNameAlt += field.Name + ".";

                ApplyData(field.GetValue(item), data, newBaseName, newBaseNameAlt);
            }
            else
            {
                // Individual field
                try
                {
                    object value = null;
                    if (data.TryGetValue(baseName + field.Name, out value) || (field.Name.StartsWith("m_") && data.TryGetValue(baseName + field.Name.Substring(2), out value))
                       || data.TryGetValue(baseNameAlt + field.Name, out value) || (field.Name.StartsWith("m_") && data.TryGetValue(baseNameAlt + field.Name.Substring(2), out value)))
                    {
                        field.SetValue(item, ChangeType(value, field.FieldType));
                    }
                }
                catch (System.Exception /*e*/)
                {
                    // Invalid value/field

                }
            }
        }
    }

    private bool IsSpecialKey(string key)
    {
        if (key == "__type") return true;
        if (key == "__base") return true;
        return false;
    }
    private object ChangeType(object value, System.Type type)
    {
        if (type.IsEnum)
            return System.Convert.ChangeType(System.Enum.Parse(type, value as string), type);
        else
            return System.Convert.ChangeType(value, type);
    }

    public static void GetGameDBInEditor(ref GameDB m_gameDB)
    {
#if UNITY_EDITOR
        if (m_gameDB == null)
        {
            GameData gameData = ScriptableObject.CreateInstance<GameData>();
            TextAsset ta = UnityEditor.AssetDatabase.LoadAssetAtPath(GameDataManager.GameDBJsonPath, typeof(TextAsset)) as TextAsset;
            gameData.Import(ta.text);
            Resources.UnloadAsset(ta);
            m_gameDB = new GameDB();
            m_gameDB.Initialize(gameData.GetAllItems());
        }
#endif
    }
}
