using System;
using System.Collections.Generic;
using UnityEngine;

public class TypeManager : Singleton<TypeManager>
{
    Dictionary<Type, Dictionary<int, Type>> dictionary = new Dictionary<Type, Dictionary<int, Type>>();

    public void Init()
    {
        if (null == dictionary)
        {
            dictionary = new Dictionary<Type, Dictionary<int, Type>>();
        }
    }

    /// <summary>
    /// 初始化某个类型的缓存
    /// </summary>
    /// <param name="t"></param>
    /// <param name="funcToGetKey"></param>
    /// <typeparam name="T"></typeparam>
    public void InitTypeCache<T>(Func<Type, int> funcToGetKey)
    {
        if (null == funcToGetKey)
        {
            Debug.LogError("must have a func to generate typeid");
            return;
        }
        
        var t = typeof(T);
        if (!dictionary.ContainsKey(t))
        {
            var subDictionary = GetAllTypeDictionary<T>(funcToGetKey);
            if (null != subDictionary)
            {
                dictionary.Add(t, subDictionary);
                Debug.Log(string.Format("Init cache {0} : {1}", t.Name, subDictionary.Count));                
            }
        }
        else
        {
            Debug.LogError(string.Format("init duplicate type dictionary {0}", t.Name));
        }
    }

    /// <summary>
    /// 初始化某个类型的缓存
    /// </summary>
    /// <param name="t"></param>
    /// <param name="funcToGetKey"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    public void InitTypeCache<T, TAttribute>(Func<Type, TAttribute, int> funcToGetKey)
        where TAttribute : Attribute
    {
        if (null == funcToGetKey)
        {
            Debug.LogError("must have a func to generate typeid");
            return;
        }
        
        var t = typeof(T);
        if (!dictionary.ContainsKey(t))
        {
            dictionary.Add(t, GetAllTypeDictionary<T, TAttribute>(funcToGetKey));
        }
        else
        {
            Debug.LogError(string.Format("init duplicate type dictionary {0}", t.Name));
        }
    }

    /// <summary>
    /// 根据名字获取缓存类型。
    /// </summary>
    /// <param name="keyName">key</param>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    public bool TryGetCachedType<T>(int keyName, out Type result)
    {
        if (dictionary.TryGetValue(typeof(T), out var subDictionary))
        {
            if (subDictionary.TryGetValue(keyName, out var resultT))
            {
                if (null != resultT)
                {
                    result = resultT;
                    return true;
                }
            }
        }
        else
        {
            Debug.Log(string.Format("type {0} is not init", typeof(T)));
        }

        result = default;
        return false;
    }

    public bool TryGetInstance<T>(int keyName, out T result) where T : class
    {
        if (TryGetCachedType<T>(keyName, out Type t))
        {
            result = Activator.CreateInstance(t) as T;
            return true;
        }

        Debug.LogError($"no type {typeof(T).Name}: {keyName}");
        result = default;
        return false;
    }

    /// <summary>
    /// 清理某个类型的缓存。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ClearTypeCache<T>()
    {
        if (dictionary.TryGetValue(typeof(T), out var subDictionary))
        {
            if (null != subDictionary)
            {
                subDictionary.Clear();
            }
        }
    }

    /// <summary>
    /// 获取所有T类型的一个缓存字典。
    /// </summary>
    /// <param name="funcToGetKey">传入一个用于计算key的回调方法</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Dictionary<int, Type> GetAllTypeDictionary<T>(
        Func<Type, int> funcToGetKey)
    {
        if (null == funcToGetKey)
        {
            return new Dictionary<int, Type>();
        }
        
        var resultDictionary = new Dictionary<int, Type>();
        var allTypesOfT = TypeHelper.GetAllSubClass<T>();
        for (var i = 0; i < allTypesOfT.Count; i++)
        {
            var t = allTypesOfT[i];
            var key = funcToGetKey.Invoke(t);
            if (IsValidKey(key))
            {
                resultDictionary.Add(key, t);
            }
        }

        return resultDictionary;
    }

    /// <summary>
    /// 获取所带有某属性的T类型的一个缓存字典。
    /// </summary>
    /// <param name="funcToGetKey">传入一个用于计算key的回调方法</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    /// <returns></returns>
    public static Dictionary<int, Type> GetAllTypeDictionary<T, TAttribute>(
        Func<Type, TAttribute, int> funcToGetKey) where TAttribute : Attribute
    {
        if (null == funcToGetKey)
        {
            return new Dictionary<int, Type>();
        }
        var resultDictionary = new Dictionary<int, Type>();
        var allTypesOfT = TypeHelper.GetAllSubClass<T>();
        for (var i = 0; i < allTypesOfT.Count; i++)
        {
            var t = allTypesOfT[i];
            var attribute = t.GetAttribute<TAttribute>();
            if (null != attribute)
            {
                var key = funcToGetKey(t, attribute);
                if (IsValidKey(key))
                {
                    if (!resultDictionary.TryGetValue(key, out var tResult))
                    {
                        resultDictionary.Add(key, t);
                    }                    
                }
            }
        }

        return resultDictionary;
    }

    private static bool IsValidKey(int key)
    {
        return 0 <= key;
    }
}