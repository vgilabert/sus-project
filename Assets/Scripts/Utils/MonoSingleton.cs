using System;
using UnityEngine;

namespace Utils
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static Action OnStarted;
        public static Action OnDestroyed;

        private static T instance;
        public static T Instance => instance;

        private bool exist = false;
        public bool Exist => instance;
        protected virtual void Awake()
        {
            if (instance)
            {
                Debug.LogError($"Only one instance of {typeof(T)} available");
            }
            Debug.Log(GetType().Name + " created");
            instance = FindObjectOfType<T>();
            exist = true;
            OnStarted?.Invoke();
        }

        protected void OnDestroy()
        {
            exist = false;
            instance = null;
            OnDestroyed?.Invoke();
        }
    }
}