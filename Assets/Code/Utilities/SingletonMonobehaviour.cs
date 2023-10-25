using UnityEngine;

namespace Code.Utilities
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _instance = null;

        public static T GetInstance()
        {
            return _instance;
        }

        protected virtual void Initialization()
        {
        
        }
    
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Debug.LogError($"{typeof(T)} is not singleton object.");
            }
            DontDestroyOnLoad(gameObject);
            Initialization();
        }
    }
}