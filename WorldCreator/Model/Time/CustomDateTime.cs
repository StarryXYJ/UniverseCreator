using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldCreator.Model.Time;

/// <summary>
///     表示一个使用自定义历法的日期和时间。
/// </summary>
public readonly struct CustomDateTime : IComparable<CustomDateTime>, IEquatable<CustomDateTime>
{
    private readonly long _totalMilliseconds; // 自纪元以来的总毫秒数，用于内部比较和算术
    public CustomCalendarRule Calendar { get; }

    public long Year { get; }
    public int Month { get; }
    public int Day { get; }
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }
    public int Millisecond { get; }

    /// <summary>
    ///     构造函数，创建指定日期和历法的 CustomDateTime 实例 (时间默认为 00:00:00.000)。
    /// </summary>
    public CustomDateTime(long year, int month, int day, CustomCalendarRule calendar)
        : this(year, month, day, 0, 0, 0, 0, calendar)
    {
    }

    /// <summary>
    ///     构造函数，创建指定日期时间组件和历法的 CustomDateTime 实例。
    /// </summary>
    public CustomDateTime(long year, int month, int day, int hour, int minute, int second, int millisecond,
        CustomCalendarRule calendar)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar), "历法规则不能为空。");
        _totalMilliseconds = Calendar.ToTotalMillisecondsFromEpoch(year, month, day, hour, minute, second, millisecond);

        // 重新从 _totalMilliseconds 提取组件，确保与历法规则完全一致
        (Year, Month, Day, Hour, Minute, Second, Millisecond) =
            Calendar.FromTotalMillisecondsFromEpoch(_totalMilliseconds);
    }

    /// <summary>
    ///     用于根据总毫秒数和历法创建 CustomDateTime 实例。
    /// </summary>
    private CustomDateTime(long totalMilliseconds, CustomCalendarRule calendar)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar), "历法规则不能为空。");
        _totalMilliseconds = totalMilliseconds;
        (Year, Month, Day, Hour, Minute, Second, Millisecond) =
            Calendar.FromTotalMillisecondsFromEpoch(_totalMilliseconds);
    }

    /// <summary>
    ///     获取当前日期时间 (使用指定的历法)。
    /// </summary>
    public static CustomDateTime Now(CustomCalendarRule calendar)
    {
        var utcNow = DateTime.UtcNow;
        // 注意：这里将系统时间转换为自定义历法，可能存在精度或日期映射问题，
        // 仅作为示例，实际应用可能需要更复杂的转换逻辑。
        return new CustomDateTime(
            utcNow.Year, utcNow.Month, utcNow.Day,
            utcNow.Hour, utcNow.Minute, utcNow.Second, utcNow.Millisecond,
            calendar);
    }

    // 实现 IComparable<CustomDateTime> 接口
    public int CompareTo(CustomDateTime other)
    {
        if (Calendar != other.Calendar) throw new ArgumentException("无法比较来自不同历法的 CustomDateTime 值。");
        return _totalMilliseconds.CompareTo(other._totalMilliseconds);
    }

    // 实现 IEquatable<CustomDateTime> 接口
    public bool Equals(CustomDateTime other)
    {
        // 历法实例必须相同，且总毫秒数相同
        return _totalMilliseconds == other._totalMilliseconds && Calendar == other.Calendar;
    }

    public override bool Equals(object obj)
    {
        return obj is CustomDateTime other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_totalMilliseconds, Calendar);
    }

    // 运算符重载
    public static bool operator ==(CustomDateTime left, CustomDateTime right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CustomDateTime left, CustomDateTime right)
    {
        return !(left == right);
    }

    public static bool operator <(CustomDateTime left, CustomDateTime right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(CustomDateTime left, CustomDateTime right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(CustomDateTime left, CustomDateTime right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(CustomDateTime left, CustomDateTime right)
    {
        return left.CompareTo(right) >= 0;
    }

    // 日期时间算术操作
    public CustomDateTime AddMilliseconds(long milliseconds)
    {
        return new CustomDateTime(_totalMilliseconds + milliseconds, Calendar);
    }

    public CustomDateTime AddSeconds(long seconds)
    {
        return AddMilliseconds(seconds * Calendar.MillisecondsInSecond);
    }

    public CustomDateTime AddMinutes(long minutes)
    {
        return AddSeconds(minutes * Calendar.SecondsInMinute);
    }

    public CustomDateTime AddHours(long hours)
    {
        return AddMinutes(hours * Calendar.MinutesInHour);
    }

    public CustomDateTime AddDays(long days)
    {
        return AddHours(days * Calendar.HoursInDay);
    }

    /// <summary>
    ///     添加指定的月份数。
    /// </summary>
    public CustomDateTime AddMonths(int months)
    {
        var newYear = Year;
        var newMonth = Month + months;
        var newDay = Day;

        var monthsInYear = Calendar.GetMonthsInYear(newYear);

        // 调整年份和月份
        while (newMonth > monthsInYear)
        {
            newMonth -= monthsInYear;
            newYear++;
            monthsInYear = Calendar.GetMonthsInYear(newYear); // 重新获取新年份的月份数 (如果历法支持变动)
        }

        while (newMonth < 1)
        {
            newYear--;
            monthsInYear = Calendar.GetMonthsInYear(newYear); // 重新获取新年份的月份数
            newMonth += monthsInYear;
        }

        // 调整日期，如果新月份的天数不足
        var daysInNewMonth = Calendar.GetDaysInMonth(newYear, newMonth);
        if (newDay > daysInNewMonth) newDay = daysInNewMonth;

        return new CustomDateTime(newYear, newMonth, newDay, Hour, Minute, Second, Millisecond, Calendar);
    }

    /// <summary>
    ///     添加指定的年份数。
    /// </summary>
    public CustomDateTime AddYears(long years)
    {
        var newYear = Year + years;
        var newMonth = Month;
        var newDay = Day;

        // 调整日期，如果新年份的当前月份天数不足 (例如，闰年2月29日到非闰年)
        var daysInNewMonth = Calendar.GetDaysInMonth(newYear, newMonth);
        if (newDay > daysInNewMonth) newDay = daysInNewMonth;

        return new CustomDateTime(newYear, newMonth, newDay, Hour, Minute, Second, Millisecond, Calendar);
    }


    /// <summary>
    ///     计算两个 CustomDateTime 实例之间的时间间隔。
    /// </summary>
    public CustomTimeSpan Subtract(CustomDateTime other)
    {
        if (Calendar != other.Calendar) throw new ArgumentException("无法计算来自不同历法的 CustomDateTime 值之间的时间间隔。");
        return new CustomTimeSpan(_totalMilliseconds - other._totalMilliseconds);
    }

    // 运算符重载，用于 CustomDateTime 和 CustomTimeSpan 的加减
    public static CustomDateTime operator +(CustomDateTime dt, CustomTimeSpan ts)
    {
        return new CustomDateTime(dt._totalMilliseconds + ts.TotalMilliseconds, dt.Calendar);
    }

    public static CustomDateTime operator -(CustomDateTime dt, CustomTimeSpan ts)
    {
        return new CustomDateTime(dt._totalMilliseconds - ts.TotalMilliseconds, dt.Calendar);
    }

    public static CustomTimeSpan operator -(CustomDateTime dt1, CustomDateTime dt2)
    {
        return dt1.Subtract(dt2);
    }

    public override string ToString()
    {
        var sign = Year < 0 ? "-" : "";
        var absYear = Math.Abs(Year);
        // 格式化年份，确保至少4位，并处理负号
        var formattedYear = absYear.ToString();
        if (absYear < 1000) formattedYear = absYear.ToString("D4"); // 确保至少4位
        else if (absYear > 9999) formattedYear = absYear.ToString(); // 更大的年份不强制D4

        return
            $"{sign}{formattedYear}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}.{Millisecond:D3} ({Calendar.Name})";
    }
}