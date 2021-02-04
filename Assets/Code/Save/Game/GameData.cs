using Definitions;
using SoftLiu.Save;
using SoftLiu.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public partial class GameData : ScriptableObject
{
    public void Import(string jsonText)
    {
        Dictionary<string, object> jsonData = DeserializeJson(jsonText);
        if (jsonData != null)
        {
            Import(jsonData);
        }
        else
        {
            throw new System.Exception("Failed to parse gameDB json");
        }
    }

    public void ImportDictionary(Dictionary<string, object> data)
    {
        Import(data);
    }

    public Dictionary<System.Type, Dictionary<string, ObjectData>> GetAllItems()
    {
        Dictionary<System.Type, IEnumerable> allItems = new Dictionary<System.Type, IEnumerable>();
        FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fields)
        {
            System.Type key = fieldInfo.FieldType.GetGenericArguments()[0];
            IEnumerable list = fieldInfo.GetValue(this) as IEnumerable;
            allItems.Add(key, list);
        }
        Dictionary<System.Type, Dictionary<string, ObjectData>> returnDic = new Dictionary<System.Type, Dictionary<string, ObjectData>>();
        foreach (System.Type type in allItems.Keys)
        {
            Dictionary<string, ObjectData> ret = new Dictionary<string, ObjectData>();
            IEnumerable list = null;
            if (allItems.TryGetValue(type, out list) && list != null)
            {
                foreach (object obj in list)
                {
                    ObjectData objData = obj as ObjectData;
                    ret.Add(objData.key, objData);
                }
            }
            returnDic.Add(type, ret);
        }
        return returnDic;
    }

    public static Dictionary<string, object> DeserializeJson(string jsonText)
    {
        return MiniJSON.Deserialize(jsonText) as Dictionary<string, object>;
    }

    public static List<ObjectData> InstantiateItems(Dictionary<string, object> dict)
    {
        List<ObjectData> list = new List<ObjectData>();
        // Parse items into classes
        foreach (string objectType in dict.Keys)
        {
            List<object> rawData = dict[objectType] as List<object>;
            foreach (object instance in rawData)
            {
                Dictionary<string, object> data = instance as Dictionary<string, object>;
                if (data != null)
                {
                    // Create class or dictionary
                    ObjectData item = null;
                    try
                    {
                        // Try to map the item to a runtime class
                        string typeName = string.Format("Definitions.{0}", objectType);
                        System.Type type = System.Type.GetType(typeName);
                        if (type != null)
                        {
                            item = (ObjectData)System.Activator.CreateInstance(type);
                            ApplyData(item, data, "", "");
                            // Store
                            list.Add(item);
                        }
                        else
                        {
                            Debug.LogError("GameData :: (InstantiateItems) Unknown type - " + typeName);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
                else
                {
                    Debug.Log(instance);
                }
            }
        }
        return list;
    }

    public static List<T> CreateInstances<T>(List<object> objects)
    {
        List<T> list = new List<T>(objects.Count);
        foreach (object instance in objects)
        {
            Dictionary<string, object> data = instance as Dictionary<string, object>;
            T obj = (T)System.Activator.CreateInstance<T>();
            ApplyData(obj, data, "", "");
            try
            {
                (obj as ObjectData).Init();
            }
            catch (System.Exception) { }
            list.Add(obj);
        }
        return list;
    }

    // TODO: We may want to flag non serialisable (or use the Serialise tag) for variables we don't want saved out.
    public static Dictionary<string, object> GetData(object item)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        FieldInfo[] fieldList = GetFields(item.GetType());
        for (int i = 0; i < fieldList.Length; i++)
        {
            FieldInfo field = fieldList[i];
            string fieldName = field.Name;
            if (fieldName.StartsWith("m_"))
            {
                fieldName = fieldName.Substring(2);
            }
            object value = field.GetValue(item);
            System.Type valueType = value.GetType();
            if (value is IEnumerable && valueType != typeof(string))
            {
                System.Type listItemType;
                if (valueType.IsArray)
                {
                    listItemType = valueType.GetElementType();
                }
                else
                {
                    listItemType = valueType.GetGenericArguments()[0];
                }
                bool isClassType = listItemType.IsClass && listItemType != typeof(string);
                bool isGeneric = listItemType.IsGenericType;
                bool isArray = listItemType.IsArray;
                List<object> list = new List<object>();
                IEnumerator enumerator = (value as IEnumerable).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object listItem = enumerator.Current;
                    if (isClassType || isGeneric || isArray)
                    {
                        listItem = GetData(listItem);
                    }
                    list.Add(listItem);
                }
                value = list;
            }
            else if (valueType.IsClass && valueType != typeof(string))
            {
                value = GetData(value);
            }
            else if (valueType.IsEnum)
            {
                // Lets save Enums as integers. May want an option to save as strings?
                value = (int)value;
            }
            data[fieldName] = value;
        }
        return data;
    }

    public static void ApplyData(object item, Dictionary<string, object> data, string baseName, string baseNameAlt)
    {
        FieldInfo[] fieldList = GetFields(item.GetType());
        for (int i = 0; i < fieldList.Length; i++)
        {
            FieldInfo field = fieldList[i];
            object dataValue = null;
            if (data.TryGetValue(baseName + field.Name, out dataValue) || (field.Name.StartsWith("m_") && data.TryGetValue(baseName + field.Name.Substring(2), out dataValue))
               || data.TryGetValue(baseNameAlt + field.Name, out dataValue) || (field.Name.StartsWith("m_") && data.TryGetValue(baseNameAlt + field.Name.Substring(2), out dataValue)))
            {
                object fieldValue = GetValue(field.FieldType, dataValue);
                if (fieldValue != null)
                {
                    field.SetValue(item, fieldValue);
                }
            }
        }
    }

    static object GetValue(System.Type fieldType, object data)
    {
        object value = null;
        // Generics
        if (fieldType.IsGenericType)
        {
            //List<>
            if (fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (data is IList)
                {
                    // Create list
                    IList newList = System.Activator.CreateInstance(fieldType) as IList;
                    // If we are a list of Classes, 2d array/list, not primitive types
                    System.Type listItemType = fieldType.GetGenericArguments()[0];
                    bool isClassType = listItemType.IsClass && listItemType != typeof(string);
                    bool isGeneric = listItemType.IsGenericType;
                    bool isArray = listItemType.IsArray;
                    // Add from list
                    foreach (object listValue in (data as IList))
                    {
                        object v = null;
                        if (isClassType || isGeneric || isArray)
                        {
                            v = GetValue(listItemType, listValue);
                        }
                        else
                        {
                            v = ChangeType(listValue, listItemType);
                        }
                        newList.Add(v);
                    }
                    value = newList;
                }
                else
                {
                    Debug.LogError("Incorrect data format for a List<>. {0} but should be IList" + data.GetType().Name);
                }
            }
            else
            {
                Debug.LogError("No support to read in the type " + fieldType.Name);
            }
            // No support for any other generics yet
        }
        // Array
        else if (fieldType.IsArray)
        {
            if (data is IList)
            {
                // Create array
                System.Array array = System.Array.CreateInstance(fieldType.GetElementType(), (data as IList).Count);
                // If we are a list of Classes, 2d array/list, not primitive types
                System.Type arrayItemType = fieldType.GetElementType();
                bool isClassType = arrayItemType.IsClass && arrayItemType != typeof(string);
                bool isGeneric = arrayItemType.IsGenericType;
                bool isArray = arrayItemType.IsArray;
                // Add from list
                int i = 0;
                foreach (object listValue in (data as IList))
                {
                    object v = null;
                    if (isClassType || isGeneric || isArray)
                    {
                        v = GetValue(arrayItemType, listValue);
                    }
                    else
                    {
                        v = ChangeType(listValue, arrayItemType);
                    }
                    array.SetValue(v, i++);
                }
                value = array;
            }
            else
            {
                value = null; // Empty arrays are now set to null
                              //Debug.LogError("Incorrect data format for an array. {0} but should be IList" + data.GetType().Name);
            }
        }
        // Object field
        else if (fieldType.IsClass && fieldType != typeof(string))
        {
            Dictionary<string, object> objectData = data as Dictionary<string, object>;
            if (objectData != null)
            {
                value = System.Activator.CreateInstance(fieldType);
                ApplyData(value, objectData, "", "");
            }
            else
            {
                Debug.LogError("Incorrect data format for a class. {0} but should be Dictionary<string, object>" + data.GetType().Name);
            }
        }
        else if (fieldType == typeof(DateTime))
        {
            if (data != null)
            {
                string strData = (string)data;
                if (strData.Equals("")) //If it's null it's an error, if it's empty means default value.
                {
                    value = DateTime.MinValue; //If field was left empty on DB, use a default value.
                }
                else if (strData != null)
                {
                    value = (object)SaveUtilities.ParseDateTime(strData, DateTime.Now, "GameData.GetValue: ");
                }
                else
                {
                    Debug.LogError("Incorrect data format for a Date. {0} but should be a valid DateTime using format DD/MM/YYYY hh:mm:ss" + data.GetType().Name);
                }
            }
            else
            {
                Debug.LogError("Incorrect data format for a Date. {0} but should be a valid DateTime using format DD/MM/YYYY hh:mm:ss" + data.GetType().Name);
            }
        }
        // Individual field
        else
        {
            try
            {
                System.Type valueType = data.GetType();
                if (fieldType.IsEnum && (valueType == typeof(int) || valueType == typeof(long)))
                {
                    value = System.Enum.ToObject(fieldType, data);
                }
                else
                {
                    value = ChangeType(data, fieldType);
                    if (fieldType.IsEnum)
                    {
                        if (value == null)
                        {
                            Debug.LogError(string.Format("Unable to convert {0} to type {1}", data, fieldType));
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(string.Format("Unable to convert {0} to type {1}", data.GetType(), fieldType));
            }
        }
        return value;
    }

    public static FieldInfo[] GetFields(System.Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        /*if(type.BaseType != typeof(System.Object))
		{
			GetFields(fields, type.BaseType);
		}*/
    }
    static object ChangeType(object value, System.Type type)
    {
        if (type.IsEnum)
            return System.Convert.ChangeType(System.Enum.Parse(type, value as string), type);
        else
            return System.Convert.ChangeType(value, type);
    }
}