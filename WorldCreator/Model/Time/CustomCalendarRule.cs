using System.Numerics;

namespace WorldCreator.Model.Time;

/// <summary>
///     历法规则的抽象基类。
///     定义了历法的基本单位和日期转换逻辑。
/// </summary>
public abstract class CustomCalendarRule
{
    public abstract string Name { get; }
    public abstract int HoursInDay { get; }
    public abstract int MinutesInHour { get; } // 通常为 60
    public abstract int SecondsInMinute { get; } // 通常为 60
    public abstract int MillisecondsInSecond { get; } // 通常为 1000

    /// <summary>
    ///     计算一天中的总毫秒数。
    /// </summary>
    public long TotalMillisecondsInDay => HoursInDay * MinutesInHour * SecondsInMinute * MillisecondsInSecond;

    /// <summary>
    ///     获取给定年份的天数。
    /// </summary>
    public abstract int GetDaysInYear(long year);

    /// <summary>
    ///     获取给定年份的月份数。
    /// </summary>
    public abstract int GetMonthsInYear(long year);

    /// <summary>
    ///     获取给定年份和月份的天数。
    /// </summary>
    public abstract int GetDaysInMonth(long year, int month);

    /// <summary>
    ///     将日期时间组件转换为自纪元以来的总毫秒数。
    ///     纪元定义为 Year 1, Month 1, Day 1, 00:00:00.000。
    ///     公元前年份使用负数表示 (例如, 1 BC 为 Year 0, 2 BC 为 Year -1)。
    /// </summary>
    public abstract long ToTotalMillisecondsFromEpoch(long year, int month, int day, int hour, int minute, int second,
        int millisecond);

    /// <summary>
    ///     将自纪元以来的总毫秒数转换为日期时间组件。
    /// </summary>
    public abstract void FromTotalMillisecondsFromEpoch(BigInteger totalMilliseconds, out long year, out int month,
        out int day, out int hour, out int minute, out int second, out int millisecond);

    /// <summary>
    ///     将自纪元以来的总毫秒数转换为日期时间组件。
    /// </summary>
    public (long, int, int, int, int, int, int) FromTotalMillisecondsFromEpoch(BigInteger totalMilliseconds)
    {
        FromTotalMillisecondsFromEpoch(totalMilliseconds, out var year, out var month, out var day, out var hour,
            out var minute,
            out var second, out var millisecond);
        return (year, month, day, hour, minute, second, millisecond);
    }
}