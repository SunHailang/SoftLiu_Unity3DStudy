using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Save
{
    public abstract class SaveSystem
    {
        protected string m_systemName = string.Empty;
        private SaveData m_saveData = null;
        private Stack<string> m_keyStack = new Stack<string>();
        #region Public Properties
        public string name
        {
            get { return m_systemName; }
        }

        public SaveData data
        {
            set { m_saveData = value; }
        }

        public string version
        {
            get { return m_saveData.Version; }
        }
        #endregion

        #region Protected Properties
        protected GameDB gameDB
        {
            get { return GameDataManager.Instance.gameDB; }
        }
        #endregion

        #region Abstract Interface
        public abstract void Reset();

        public abstract void Load();

        public abstract void Save();

        public abstract bool Upgrade();

        public abstract void Downgrade();
        #endregion

        #region Scoping
        protected void PushKey(string key)
        {
            m_keyStack.Push(key);
        }

        protected void PopKey()
        {
            m_keyStack.Pop();
        }
        #endregion

        #region Getters
        protected bool KeyExists(string key, bool platformSpecific = false)
        {
            return GetObject(key, platformSpecific) != null;
        }

        protected int GetInt(string key, int defaultValue = 0, bool platformSpecific = false)
        {
            object intObj = GetObject(key, platformSpecific);

            return intObj != null ? Convert.ToInt32(intObj) : defaultValue;
        }

        protected string GetString(string key, string defaultValue = "", bool platformSpecific = false)
        {
            object stringObj = GetObject(key, platformSpecific);

            return stringObj != null ? Convert.ToString(stringObj) : defaultValue;
        }

        protected float GetFloat(string key, float defaultValue = 0, bool platformSpecific = false)
        {
            object floatObj = GetObject(key, platformSpecific);

            return floatObj != null ? Convert.ToSingle(floatObj) : defaultValue;
        }

        protected double GetDouble(string key, double defaultValue = 0, bool platformSpecific = false)
        {
            object doubleObj = GetObject(key, platformSpecific);

            return doubleObj != null ? Convert.ToDouble(doubleObj) : defaultValue;
        }

        protected bool GetBool(string key, bool defaultValue = false, bool platformSpecific = false)
        {
            object boolObj = GetObject(key, platformSpecific);

            return boolObj != null ? Convert.ToBoolean(boolObj) : defaultValue;
        }

        protected int[] GetIntArray(string key, bool platformSpecific = false)
        {
            IEnumerable iter = GetObject(key, platformSpecific) as IEnumerable;

            List<int> array = new List<int>();

            if (iter != null)
            {
                foreach (object intObj in iter)
                {
                    array.Add(Convert.ToInt32(intObj));
                }
            }

            return array.ToArray();
        }

        protected float[] GetFloatArray(string key, bool platformSpecific = false)
        {
            IEnumerable iter = GetObject(key, platformSpecific) as IEnumerable;

            List<float> array = new List<float>();

            if (iter != null)
            {
                foreach (object floatObj in iter)
                {
                    array.Add(Convert.ToSingle(floatObj));
                }
            }

            return array.ToArray();
        }

        protected bool[] GetBoolArray(string key, bool platformSpecific = false)
        {
            IEnumerable iter = GetObject(key, platformSpecific) as IEnumerable;

            List<bool> array = new List<bool>();

            if (iter != null)
            {
                foreach (object boolObj in iter)
                {
                    array.Add(Convert.ToBoolean(boolObj));
                }
            }

            return array.ToArray();
        }

        protected string[] GetStringArray(string key, bool platformSpecific = false)
        {
            IEnumerable iter = GetObject(key, platformSpecific) as IEnumerable;

            List<string> array = new List<string>();

            if (iter != null)
            {
                foreach (object strObj in iter)
                {
                    array.Add(Convert.ToString(strObj));
                }
            }

            return array.ToArray();
        }

        #endregion

        #region Setters
        protected void SetInt(string key, int value, bool platformSpecific = false)
        {
            SetObject(key, value, platformSpecific);
        }

        protected void SetFloat(string key, float value, bool platformSpecific = false)
        {
            SetObject(key, value, platformSpecific);
        }

        protected void SetDouble(string key, double value, bool platformSpecific = false)
        {
            SetObject(key, value, platformSpecific);
        }

        protected void SetBool(string key, bool value, bool platformSpecific = false)
        {
            SetObject(key, value, platformSpecific);
        }

        protected void SetString(string key, string value, bool platformSpecific = false)
        {
            SetObject(key, value, platformSpecific);
        }

        //Be very careful using the below setters this will completely replace all entries in an array with the new array and can lose data
        protected void SetStringArray(string key, string[] array, bool platformSpecific = false, bool assertOnLengthDecrease = true)
        {
            if (assertOnLengthDecrease)
            {
                string[] currentArray = GetStringArray(key, platformSpecific);
            }

            SetObject(key, array, platformSpecific);
        }

        protected void SetIntArray(string key, int[] array, bool platformSpecific = false, bool assertOnLengthDecrease = true)
        {
            if (assertOnLengthDecrease)
            {
                int[] currentArray = GetIntArray(key, platformSpecific);
            }

            SetObject(key, array, platformSpecific);
        }

        protected void SetFloatArray(string key, float[] array, bool platformSpecific = false, bool assertOnLengthDecrease = true)
        {
            if (assertOnLengthDecrease)
            {
                float[] currentArray = GetFloatArray(key, platformSpecific);
            }

            SetObject(key, array, platformSpecific);
        }

        protected void SetBoolArray(string key, bool[] array, bool platformSpecific = false, bool assertOnLengthDecrease = true)
        {
            if (assertOnLengthDecrease)
            {
                bool[] currentArray = GetBoolArray(key, platformSpecific);
            }

            SetObject(key, array, platformSpecific);
        }
        #endregion

        #region Private Methods
        private string GetKey(string key, bool platformSpecific)
        {
            m_keyStack.Push(key);

            string[] keys = m_keyStack.ToArray();

            Array.Reverse(keys);

            string saveKey = "";

            if (!string.IsNullOrEmpty(m_systemName))
            {
                saveKey += m_systemName + ".";
            }

            saveKey += (platformSpecific ? Globals.GetPlatform().ToString()+ "." : "") + string.Join(".", keys);

            m_keyStack.Pop();

            return saveKey;
        }

        private object GetObject(string key, bool platformSpecific = false)
        {
            return m_saveData != null ? m_saveData[GetKey(key, platformSpecific)] : null;
        }

        private void SetObject(string key, object value, bool platformSpecific = false)
        {
            if (m_saveData != null)
            {
                m_saveData[GetKey(key, platformSpecific)] = value;
            }
            else
            {
                Debug.LogError("SaveSystem (Set) :: Trying to set data before saveData available");
            }
        }

        private void SetObjectAtIndex(string key, int index, object value, bool platformSpecific = false)
        {
            object valueAtKey = GetObject(key, platformSpecific);
            List<object> list = valueAtKey as List<object>;

            if (valueAtKey == null || list != null)
            {
                if (list == null)
                {
                    list = new List<object>();
                    SetObject(key, list, platformSpecific);
                }

                //Fill array to be the size required to set index
                if ((list.Count - 1) < index)
                {
                    for (int i = list.Count - 1; i <= index; i++)
                    {
                        list.Add(null);
                    }
                }

                list[index] = value;
            }
            else
            {
                Debug.LogWarning("SaveSystem (SetAtIndex) :: Unable to set value at index as key is not an array");
            }

        }
        #endregion
    }
}
