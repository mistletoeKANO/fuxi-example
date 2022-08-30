using System;

public static class TimeHelper
{
    /// <summary>
    /// 服务器时间 根据心跳获取 服务器返回时间戳，客户端按帧修正
    /// </summary>
    public static double ServerTime;
    public static long ServerLongTime => (long) ServerTime;

    /// <summary>
    /// UTC 目标 时间 格式化
    /// </summary>
    /// <param name="endSeconds"></param>
    /// <returns></returns>
    public static string Time2UTCFormat(long endSeconds)
    {
        var beginTick = (endSeconds - ServerLongTime) * 10000000L + DateTime.UtcNow.Ticks;
        var dateTime = new DateTime(beginTick, DateTimeKind.Utc);
        return dateTime.ToString("F");
    }

    /// <summary>
    /// 本地 目标 时间 格式化
    /// </summary>
    /// <param name="endSeconds"></param>
    /// <returns></returns>
    public static string Time2LocalFormat(long endSeconds)
    {
        var endTick = (endSeconds - ServerLongTime) * 10000000L + DateTime.Now.Ticks;
        var endTime = new DateTime(endTick, DateTimeKind.Local);
        return endTime.ToString("F");
    }

    private static readonly System.Text.StringBuilder sb = new System.Text.StringBuilder();
    /// <summary>
    /// 剩余时间 倒计时 格式化文本 最大单位 day
    /// </summary>
    /// <param name="endSeconds"></param>
    /// <returns></returns>
    public static string RemainingTimeDayFormat(long endSeconds)
    {
        double seconds = endSeconds - ServerLongTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        sb.Clear();
        if (timeSpan.Days > 0)
        {
            sb.Append(timeSpan.Days < 10 ? $"0{timeSpan.Days}天 " : $"{timeSpan.Days}");
        }
        sb.Append(timeSpan.Hours < 10 ? $"0{timeSpan.Hours}:" : $"{timeSpan.Hours}:");
        sb.Append(timeSpan.Minutes < 10 ? $"0{timeSpan.Minutes}:" : $"{timeSpan.Minutes}:");
        sb.Append(timeSpan.Seconds < 10 ? $"0{timeSpan.Seconds}" : $"{timeSpan.Seconds}");
        return sb.ToString();
    }

    /// <summary>
    /// 获取 最大单位为 day 的 时间跨度 结构
    /// </summary>
    /// <param name="endSeconds"></param>
    /// <returns></returns>
    public static TimeSpan DayOfTimeSpan(long endSeconds)
    {
        double seconds = endSeconds - ServerLongTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return timeSpan;
    }

    /// <summary>
    /// 是否 到达或者超出 指定时间
    /// </summary>
    /// <param name="endSeconds"></param>
    /// <returns></returns>
    public static bool IsTimeOut(int endSeconds)
    {
        return endSeconds - ServerLongTime <= 0;
    }
}