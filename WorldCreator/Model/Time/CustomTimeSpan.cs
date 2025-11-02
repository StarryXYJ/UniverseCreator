using System;
using System.Numerics;

namespace WorldCreator.Model.Time;

/// <summary>
///     表示一个时间间隔。
///     独立于任何历法，以总毫秒数存储。
/// </summary>
public readonly struct CustomTimeSpan(BigInteger milliseconds) : IComparable<CustomTimeSpan>, IEquatable<CustomTimeSpan>
{
    public readonly BigInteger TotalSeconds = milliseconds;

    // 静态工厂方法，用于根据特定历法创建 TimeSpan
    public static CustomTimeSpan FromDays(long days, CustomCalendarRule calendar)
    {
        return new CustomTimeSpan(days * calendar.TotalSecondsInDay);
    }

    public static CustomTimeSpan FromHours(long hours, CustomCalendarRule calendar)
    {
        return new CustomTimeSpan(hours * calendar.MinutesInHour * calendar.SecondsInMinute);
    }

    public static CustomTimeSpan FromMinutes(long minutes, CustomCalendarRule calendar)
    {
        return new CustomTimeSpan(minutes * calendar.SecondsInMinute);
    }

    public static CustomTimeSpan FromSeconds(long seconds, CustomCalendarRule calendar)
    {
        return new CustomTimeSpan(seconds);
    }

    public static CustomTimeSpan FromMilliseconds(long milliseconds)
    {
        return new CustomTimeSpan(milliseconds);
    }

    // 实现比较和相等性
    public int CompareTo(CustomTimeSpan other)
    {
        return TotalSeconds.CompareTo(other.TotalSeconds);
    }

    public bool Equals(CustomTimeSpan other)
    {
        return TotalSeconds == other.TotalSeconds;
    }

    public override bool Equals(object obj)
    {
        return obj is CustomTimeSpan other && Equals(other);
    }

    public override int GetHashCode()
    {
        return TotalSeconds.GetHashCode();
    }

    // 运算符重载
    public static bool operator ==(CustomTimeSpan left, CustomTimeSpan right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CustomTimeSpan left, CustomTimeSpan right)
    {
        return !(left == right);
    }

    public static bool operator <(CustomTimeSpan left, CustomTimeSpan right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(CustomTimeSpan left, CustomTimeSpan right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(CustomTimeSpan left, CustomTimeSpan right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(CustomTimeSpan left, CustomTimeSpan right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static CustomTimeSpan operator +(CustomTimeSpan ts1, CustomTimeSpan ts2)
    {
        return new CustomTimeSpan(ts1.TotalSeconds + ts2.TotalSeconds);
    }

    public static CustomTimeSpan operator -(CustomTimeSpan ts1, CustomTimeSpan ts2)
    {
        return new CustomTimeSpan(ts1.TotalSeconds - ts2.TotalSeconds);
    }

    public override string ToString()
    {
        // 为了显示方便，这里假设了标准的 24 小时/天、60 分钟/小时等。
        // 如果需要根据特定历法格式化，需要传入 CustomCalendarRule。
        var totalMs = TotalSeconds;
        var days = totalMs / (24L * 60 * 60 * 1000);
        totalMs %= 24L * 60 * 60 * 1000;
        var hours = totalMs / (60 * 60 * 1000);
        totalMs %= 60 * 60 * 1000;
        var minutes = totalMs / (60 * 1000);
        totalMs %= 60 * 1000;
        var seconds = totalMs / 1000;
        var milliseconds = totalMs % 1000;

        return $"{days}.{hours:D2}:{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
    }
}