
// ReSharper disable once CheckNamespace
public static class Debugger
{
    public static readonly int LogLevel = 0B0111;
    private static System.Diagnostics.Stopwatch mStopWatch;

    public static void Log(string message, params object[] args)
    {
        if ((LogLevel & 1) == 0) return;
        var msg = args.Length == 0 ? message : string.Format(message, args);
        UnityEngine.Debug.Log($"Frame {UnityEngine.Time.frameCount} -- {msg}");
    }

    public static void LogWarning(string message, params object[] args)
    {
        if ((LogLevel & 2) == 0) return;
        var msg = args.Length == 0 ? message : string.Format(message, args);
        UnityEngine.Debug.LogWarning($"Frame {UnityEngine.Time.frameCount} -- {msg}");
    }

    public static void LogError(string message, params object[] args)
    {
        if ((LogLevel & 4) == 0) return;
        var msg = args.Length == 0 ? message : string.Format(message, args);
        UnityEngine.Debug.LogError($"Frame {UnityEngine.Time.frameCount} -- {msg}");
    }

    public static void ColorLog(UnityEngine.Color color, string message, params object[] args)
    {
        if ((LogLevel & 1) == 0) return;
        var colorFormat = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        if (args.Length == 0)
        {
            Log($"<color=#{colorFormat}>{message}</color>");
            return;
        }

        for (int i = 0; i < args.Length; i++)
            args[i] = $"<color=#{colorFormat}>{args[i]}</color>";
        Log(message, args);
    }

    public static void ColorWarning(UnityEngine.Color color, string message, params object[] args)
    {
        if ((LogLevel & 2) == 0) return;
        var colorFormat = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        if (args.Length == 0)
        {
            LogWarning($"<color=#{colorFormat}>{message}</color>");
            return;
        }

        for (int i = 0; i < args.Length; i++)
            args[i] = $"<color=#{colorFormat}>{args[i]}</color>";
        LogWarning(message, args);
    }

    public static void ColorError(UnityEngine.Color color, string message, params object[] args)
    {
        if ((LogLevel & 4) == 0) return;
        var colorFormat = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        if (args.Length == 0)
        {
            LogError($"<color=#{colorFormat}>{message}</color>");
            return;
        }

        for (int i = 0; i < args.Length; i++)
            args[i] = $"<color=#{colorFormat}>{args[i]}</color>";
        LogError(message, args);
    }

    public static void StartWatch()
    {
        if (null == mStopWatch)
            mStopWatch = System.Diagnostics.Stopwatch.StartNew();
        mStopWatch.Restart();
    }

    public static void Watch(string title)
    {
        ColorLog(ColorStyle.Orange, "{0}耗时: {1} ms", title, mStopWatch.ElapsedMilliseconds);
    }

    public static void StopWatch(string title)
    {
        mStopWatch.Stop();
        ColorLog(ColorStyle.Orange, "{0}耗时: {1} ms", title, mStopWatch.ElapsedMilliseconds);
    }
    
    public struct ColorStyle
    {
        internal static UnityEngine.Color Cyan2 = new UnityEngine.Color(0.69f, 1f, 0.93f);
        internal static UnityEngine.Color Orange = new UnityEngine.Color(1f, 0.42f, 0.1f);
    }
}