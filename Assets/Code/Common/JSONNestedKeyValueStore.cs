using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SoftLiu.Utilities;

public class JSONNestedKeyValueStore
{
    Dictionary<string, object> m_store = null;

    public JSONNestedKeyValueStore()
    {
        m_store = new Dictionary<string, object>();
    }

    public object this[string key]
    {
        get
        {
            object value = null;

            string[] keys = key.Split(new Char[] { '.' });

            Dictionary<string, object> currentDic = m_store;

            string currentKey = "";

            for (int i = 0; i < keys.Length; i++)
            {
                string keySection = keys[i];
                currentKey += keySection + ".";

                if (currentDic.ContainsKey(keySection))
                {
                    object currentObj = currentDic[keySection];

                    if (i != (keys.Length - 1))
                    {
                        if (currentObj is Dictionary<string, object>)
                        {
                            currentDic = currentObj as Dictionary<string, object>;
                        }
                        else
                        {
                            throw new FormatException("Object at key " + currentKey.Substring(0, currentKey.Length - 1) + " is not a dictionary and does not contain a value for: " + key);
                        }
                    }
                    else
                    {
                        value = currentObj;
                    }
                }
            }

            return value;
        }
        set
        {
            string[] keys = key.Split(new Char[] { '.' });

            Dictionary<string, object> currentDic = m_store;

            string currentKey = "";

            for (int i = 0; i < keys.Length; i++)
            {
                string keySection = keys[i];
                currentKey += keySection + ".";

                //If there is already a key matching this key
                if (currentDic.ContainsKey(keySection))
                {
                    object currentObj = currentDic[keySection];

                    //If we have more keys
                    if (i != (keys.Length - 1))
                    {
                        if (currentObj is Dictionary<string, object>)
                        {
                            currentDic = currentObj as Dictionary<string, object>;
                        }
                        else
                        {
                            throw new FormatException("Object at key " + currentKey.Substring(0, currentKey.Length - 1) + " is not a dictionary and can not store value with key: " + key);
                        }
                    }
                    else
                    {
                        if (currentObj is Dictionary<string, object>)
                        {
                            Debug.LogWarning("Overriding a key with more depth at: " + key);
                        }

                        currentDic[keySection] = value;
                    }
                }
                //Create a new key
                else
                {
                    if (i != (keys.Length - 1))
                    {
                        Dictionary<string, object> newDic = new Dictionary<string, object>();

                        currentDic[keySection] = newDic;
                        currentDic = newDic;
                    }
                    else
                    {
                        currentDic[keySection] = value;
                    }
                }
            }
        }
    }

    public void Clear()
    {
        m_store.Clear();
    }

    public bool FromJSON(TextReader json)
    {
        bool success = false;

        try
        {
            Dictionary<string, object> newStore = MiniJSON.Deserialize(json) as Dictionary<string, object>;

            if (newStore != null)
            {
                m_store = newStore;
                success = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSONNestedKeyValueStore :: Failed to parse JSON - " + e.Message);
        }

        return success;
    }

    public string ToJSON()
    {
        return MiniJSON.Serialize(m_store);
    }

    public bool Merge(string json)
    {
        bool success = false;

        try
        {
            Dictionary<string, object> newStore = MiniJSON.Deserialize(json) as Dictionary<string, object>;

            if (newStore != null)
            {
                Action<Dictionary<string, object>, Dictionary<string, object>> merge = null;
                merge = delegate (Dictionary<string, object> baseStore, Dictionary<string, object> overrideStore)
                {
                    foreach (KeyValuePair<string, object> pair in overrideStore)
                    {
                        object currentVal = baseStore.ContainsKey(pair.Key) ? baseStore[pair.Key] : null;

                        //If this is a dictionary and the base is a dictionary we need to merge on those dictionaries as well
                        if (pair.Value.GetType() == typeof(Dictionary<string, object>) && (currentVal != null && currentVal.GetType() == typeof(Dictionary<string, object>)))
                        {
                            merge(currentVal as Dictionary<string, object>, pair.Value as Dictionary<string, object>);
                        }
                        else
                        {
                            baseStore[pair.Key] = pair.Value;
                        }
                    }
                };

                merge(m_store, newStore);

                success = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("JSONNestedKeyValueStore :: Failed to parse JSON - {0}", e.Message));
        }

        return success;
    }

#if UNITY_EDITOR
    public Dictionary<string, object> rawData
    {
        get { return m_store; }
    }
#endif
}
