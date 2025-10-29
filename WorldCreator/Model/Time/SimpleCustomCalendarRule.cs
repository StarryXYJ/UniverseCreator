using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace WorldCreator.Model.Time;

/// <summary>
///     简单的自定义历法规则。
///     允许自定义每月天数数组和每日小时数。无闰年。
///     纪元定义为 Year 1, Month 1, Day 1, 00:00:00.000。
///     公元前年份使用负数表示 (例如, 1 BC 为 Year 0, 2 BC 为 Year -1)。
/// </summary>
public sealed class SimpleCustomCalendarRule : CustomCalendarRule
{
    // 纪元定义：Year 1, Month 1, Day 1, 00:00:00.000
    private const long EpochYear = 1;
    private const int EpochMonth = 1;
    private const int EpochDay = 1;

    // 每月天数数组，作为只读列表暴露，便于数据绑定
    private readonly int[] _daysInMonths;

    private readonly int _daysInYearCalculated; // 根据 DaysInMonths 数组计算出的每年总天数

    /// <summary>
    ///     无参构造函数，为数据绑定提供默认值。
    ///     默认值：每年12个月，每月天数30，每日24小时。
    /// </summary>
    public SimpleCustomCalendarRule()
        : this(
            Enumerable.Repeat(30, 12).ToArray(), // 12个月，总360天
            24
        )
    {
    }

    /// <summary>
    ///     构造函数，允许自定义每月天数数组和每日小时数。
    /// </summary>
    /// <param name="daysInMonths">一个整数数组，表示每个月的天数。数组长度即为每年月份数。</param>
    /// <param name="hoursInDay">每日小时数。</param>
    /// <param name="minutesInHour">每小时分钟数 (默认60)。</param>
    /// <param name="secondsInMinute">每分钟秒数 (默认60)。</param>
    /// <param name="millisecondsInSecond">每秒毫秒数 (默认1000)。</param>
    public SimpleCustomCalendarRule(int[] daysInMonths, int hoursInDay, int minutesInHour = 60,
        int secondsInMinute = 60, int millisecondsInSecond = 1000)
    {
        if (daysInMonths == null || daysInMonths.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(daysInMonths), "每月天数数组不能为空。");
        if (daysInMonths.Any(d => d <= 0))
            throw new ArgumentOutOfRangeException(nameof(daysInMonths), "每月天数必须大于0。");
        if (hoursInDay <= 0)
            throw new ArgumentOutOfRangeException(nameof(hoursInDay), "每日小时数必须大于0。");
        if (minutesInHour <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutesInHour), "每小时分钟数必须大于0。");
        if (secondsInMinute <= 0)
            throw new ArgumentOutOfRangeException(nameof(secondsInMinute), "每分钟秒数必须大于0。");
        if (millisecondsInSecond <= 0)
            throw new ArgumentOutOfRangeException(nameof(millisecondsInSecond), "每秒毫秒数必须大于0。");

        _daysInMonths = (int[])daysInMonths.Clone(); // 克隆数组以防止外部修改
        HoursInDay = hoursInDay;
        MinutesInHour = minutesInHour;
        SecondsInMinute = secondsInMinute;
        MillisecondsInSecond = millisecondsInSecond;

        _daysInYearCalculated = _daysInMonths.Sum(d => d);
    }

    public override string Name => "Simple Custom Calendar (Array-based)";

    // 历法规则属性，支持数据绑定
    public override int HoursInDay { get; }
    public override int MinutesInHour { get; }
    public override int SecondsInMinute { get; }
    public override int MillisecondsInSecond { get; }
    public IReadOnlyList<int> DaysInMonths => _daysInMonths;

    public override int GetDaysInYear(long year)
    {
        return _daysInYearCalculated;
        // 无闰年，每年天数固定
    }

    public override int GetMonthsInYear(long year)
    {
        return _daysInMonths.Length;
    }

    public override int GetDaysInMonth(long year, int month)
    {
        if (month < 1 || month > GetMonthsInYear(year))
            throw new ArgumentOutOfRangeException(nameof(month),
                $"月份 {month} 超出 {Name} 历法的有效范围 (1-{GetMonthsInYear(year)})。");

        return _daysInMonths[month - 1]; // 数组是0-based
    }

    public override long ToTotalMillisecondsFromEpoch(long year, int month, int day, int hour, int minute, int second,
        int millisecond)
    {
        // 验证输入
        if (year == 0)
            throw new ArgumentOutOfRangeException(nameof(year),
                "Year 0 不受支持。公元前年份请使用负数 (例如, 1 BC 为 Year 0, 2 BC 为 Year -1)。");
        if (month < 1 || month > GetMonthsInYear(year))
            throw new ArgumentOutOfRangeException(nameof(month),
                $"月份 {month} 超出 {Name} 历法的有效范围 (1-{GetMonthsInYear(year)})。");
        if (day < 1 || day > GetDaysInMonth(year, month))
            throw new ArgumentOutOfRangeException(nameof(day),
                $"日期 {day} 超出 {Name} 历法中月份 {month} 的有效范围 (1-{GetDaysInMonth(year, month)})。");
        if (hour < 0 || hour >= HoursInDay)
            throw new ArgumentOutOfRangeException(nameof(hour),
                $"小时 {hour} 超出 {Name} 历法中每日小时数的有效范围 (0-{HoursInDay - 1})。");
        if (minute < 0 || minute >= MinutesInHour)
            throw new ArgumentOutOfRangeException(nameof(minute), $"分钟 {minute} 超出有效范围 (0-{MinutesInHour - 1})。");
        if (second < 0 || second >= SecondsInMinute)
            throw new ArgumentOutOfRangeException(nameof(second), $"秒钟 {second} 超出有效范围 (0-{SecondsInMinute - 1})。");
        if (millisecond < 0 || millisecond >= MillisecondsInSecond)
            throw new ArgumentOutOfRangeException(nameof(millisecond),
                $"毫秒 {millisecond} 超出有效范围 (0-{MillisecondsInSecond - 1})。");

        long totalDaysFromEpoch = 0;

        // 计算从纪元年到当前年之前的总天数
        if (year >= EpochYear)
            totalDaysFromEpoch += (year - EpochYear) * _daysInYearCalculated;
        else // 公元前年份 (Year 0 代表 1 BC, Year -1 代表 2 BC)
            totalDaysFromEpoch -= (EpochYear - year) * _daysInYearCalculated;

        // 添加当前年内的天数 (0-based)
        long daysInCurrentYear = 0;
        for (var m = 1; m < month; m++) daysInCurrentYear += _daysInMonths[m - 1];
        daysInCurrentYear += day - 1;
        totalDaysFromEpoch += daysInCurrentYear;

        // 添加时间组件
        var totalMilliseconds = totalDaysFromEpoch * TotalMillisecondsInDay;
        totalMilliseconds += hour * MinutesInHour * SecondsInMinute * MillisecondsInSecond;
        totalMilliseconds += minute * SecondsInMinute * MillisecondsInSecond;
        totalMilliseconds += second * MillisecondsInSecond;
        totalMilliseconds += millisecond;

        return totalMilliseconds;
    }

    public override void FromTotalMillisecondsFromEpoch(BigInteger totalMilliseconds, out long year, out int month,
        out int day, out int hour, out int minute, out int second, out int millisecond)
    {
        var totalDaysFromEpoch = BigInteger.DivRem(totalMilliseconds, TotalMillisecondsInDay, out var remainingMsInDay);

        // 提取时间组件
        hour = (int)BigInteger.DivRem(remainingMsInDay, MinutesInHour * SecondsInMinute * MillisecondsInSecond,
            out remainingMsInDay);
        minute = (int)BigInteger.DivRem(remainingMsInDay, SecondsInMinute * MillisecondsInSecond, out remainingMsInDay);
        second = (int)BigInteger.DivRem(remainingMsInDay, MillisecondsInSecond, out remainingMsInDay);
        millisecond = (int)remainingMsInDay;

        // 提取日期组件 (年, 月, 日)
        long daysInCurrentYear; // 0-based day within the current year

        if (totalDaysFromEpoch >= 0)
        {
            var y = (long)BigInteger.DivRem(totalDaysFromEpoch, _daysInYearCalculated, out var remainder);
            year = EpochYear + y;
            daysInCurrentYear = (long)remainder;
        }
        else // totalDaysFromEpoch 为负数 (公元前)
        {
            var absoluteDays = BigInteger.Abs(totalDaysFromEpoch);
            var yearsBeforeEpoch = absoluteDays / _daysInYearCalculated;
            var daysIntoYearBeforeEpoch = absoluteDays % _daysInYearCalculated; // 0-based from end of year

            if (daysIntoYearBeforeEpoch == 0)
            {
                year = (long)((BigInteger)EpochYear - yearsBeforeEpoch);
                daysInCurrentYear = 0; // 1月1日是第0天
            }
            else
            {
                year = (long)(EpochYear - yearsBeforeEpoch - 1);
                daysInCurrentYear = (long)(_daysInYearCalculated - daysIntoYearBeforeEpoch); // 0-based 从年头开始计算
            }
        }

        // 根据 daysInCurrentYear (0-based) 计算月和日
        month = 1;
        day = 1;
        long daysCounted = 0;
        while (month <= _daysInMonths.Length)
        {
            if (daysInCurrentYear < daysCounted + _daysInMonths[month - 1])
            {
                day = EpochDay + (int)(daysInCurrentYear - daysCounted);
                break;
            }

            daysCounted += _daysInMonths[month - 1];
            month++;
        }

        if (month > _daysInMonths.Length) // Fallback for edge cases, should not happen with correct logic
        {
            month = _daysInMonths.Length;
            day = _daysInMonths[^1];
        }
    }
}