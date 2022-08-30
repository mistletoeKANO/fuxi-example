using UnityEngine;

public static class ScriptTemplateUtility
{
    public static T GetComponent<T>(this MonoBehaviour behaviour, string path) where T : Component
    {
        return behaviour.transform.Find(path)?.GetComponent<T>();
    }
}