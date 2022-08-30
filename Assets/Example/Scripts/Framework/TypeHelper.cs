using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 类型和程序集相关的工具。
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// 尝试类型转换。
    /// </summary>
    /// <param name="obj">原实例</param>
    /// <param name="result">转换后的接国</param>
    /// <typeparam name="TResult">转换类型</typeparam>
    /// <returns>是否转换成功</returns>
    public static bool TryAs<TResult>(this object obj, out TResult result) where TResult : class
    {
        result = obj as TResult;
        return null != result;
    }
    
    /// <summary>
    /// 获取程序集中所有x类型。
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    /// <returns></returns>
    public static List<Type> GetAllSubClass<T>()
    {
        var aimType = typeof(T);
        List<Type> allTypes;
        // 这个方法在 (redmi 8A) (MIUI 11.0.3|稳定版) 上获取不到。
        // allTypes = Assembly.GetCallingAssembly().GetTypes().ToList();
        allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(o => o.GetTypes()
                .ToList())
            .ToList();
        var result = allTypes.FindAll(t => t.IsSubclassOf(aimType) && !t.IsAbstract);
        return result;
    }
    
    public static List<Type> GetAllSubClass(Type curType)
    {
        if (curType == null) return null;
        List<Type> allTypes;
        // 这个方法在 (redmi 8A) (MIUI 11.0.3|稳定版) 上获取不到。
        //allTypes = Assembly.GetCallingAssembly().GetTypes().ToList();
        allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(o => o.GetTypes()
                .ToList())
            .ToList();
        var result = allTypes.FindAll(t => t.IsSubclassOf(curType));
        return result;
    }

    /// <summary>
    /// 获取程序集中所有x类型。
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    /// <returns></returns>
    public static List<Type> GetAllSubClassOfInterface<T>()
    {
        var aimType = typeof(T);
        List<Type> allTypes;
        // 这个方法在 (redmi 8A) (MIUI 11.0.3|稳定版) 上获取不到。
//        allTypes = Assembly.GetCallingAssembly().GetTypes().ToList();
        allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.GetTypes().ToList()).ToList();
        var result = allTypes.FindAll(t => aimType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);
        return result;
    }
}