using System;
using UnityEngine;

namespace Game.HotFix
{
    public class Singleton<T> where T : new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (null == instance) { instance = new T(); }
                return instance;
            }
        }
    }

    public class MonoSingleton : MonoBehaviour
    {
        
    }
}