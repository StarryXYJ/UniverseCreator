using System;
using System.Numerics;

namespace WorldCreator.Model.Time;

/// <summary>
///     格里高利历规则实现。
///     处理闰年和每月天数不同的情况。
///     纪元定义为 0001-01-01 00:00:00.000 (Proleptic Gregorian)。
///     公元前年份使用天文年编号 (1 BC 为 Year 0, 2 BC 为 Year -1)。
/// </summary>
public sealed class GregorianCalendarRule : CustomCalendarRule
{
    public static GregorianCalendarRule Instance = new();

    // 纪元定义：0001-01-01 00:00:00.000
    private const long EpochYear = 1;

    // 累计天数表 (非闰年)
    private static readonly int[] DaysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };

    // 累计天数表 (闰年)
    private static readonly int[] DaysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
    public override string Name => "Gregorian Calendar";
    public override int HoursInDay => 24;
    public override int MinutesInHour => 60;
    public override int SecondsInMinute => 60;
    public override int MillisecondsInSecond => 1000;

    /// <summary>
    ///     判断给定年份是否为闰年。
    ///     遵循天文年编号: 1 BC 为 Year 0, 2 BC 为 Year -1。
    /// </summary>
    private static bool IsLeapYear(long year)
    {
        var actualYear = year;
        if (year <= 0) // 如果是公元前年份 (Year 0, -1, ...)
        {
            actualYear++; // 转换为通常的年份编号 (1 BC -> 0, 2 BC -> -1, ... 转换为 1, 0, ...)
            if (actualYear <= 0) return false; // 0年不是闰年，负数年也不适用格里高利历的闰年规则
        }

        // 格里高利历闰年规则
        return (actualYear % 4 == 0 && actualYear % 100 != 0) || actualYear % 400 == 0;
    }

    public override int GetDaysInYear(long year)
    {
        return IsLeapYear(year) ? 366 : 365;
    }

    public override int GetMonthsInYear(long year)
    {
        return 12;
    }

    public override int GetDaysInMonth(long year, int month)
    {
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        if (month == 2) return IsLeapYear(year) ? 29 : 28;
        if (month == 4 || month == 6 || month == 9 || month == 11) return 30;
        return 31;
    }

    public override long ToTotalMillisecondsFromEpoch(long year, int month, int day, int hour, int minute, int second,
        int millisecond)
    {
        // 验证输入，报错版本
        /*if (year == 0)
            throw new ArgumentOutOfRangeException(nameof(year),
                "Year 0 不受支持。公元前年份请使用负数 (例如, 1 BC 为 Year 0, 2 BC 为 Year -1)。");
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        if (day < 1 || day > GetDaysInMonth(year, month)) throw new ArgumentOutOfRangeException(nameof(day));
        if (hour < 0 || hour >= HoursInDay) throw new ArgumentOutOfRangeException(nameof(hour));
        if (minute < 0 || minute >= MinutesInHour) throw new ArgumentOutOfRangeException(nameof(minute));
        if (second < 0 || second >= SecondsInMinute) throw new ArgumentOutOfRangeException(nameof(second));
        if (millisecond < 0 || millisecond >= MillisecondsInSecond)
            throw new ArgumentOutOfRangeException(nameof(millisecond));*/
        if (year == 0) year = 1;
        if (month < 1) month = 1;
        if (month > 12) month = 12;
        if (day < 1) day = 1;
        if (day > GetDaysInMonth(year, month)) day = GetDaysInMonth(year, month);
        if (hour < 0) hour = 0;
        if (hour >= HoursInDay) hour = HoursInDay - 1;
        if (minute < 0) minute = 0;
        if (minute >= MinutesInHour) minute = MinutesInHour - 1;
        if (second < 0) second = 0;
        if (second >= SecondsInMinute) second = SecondsInMinute - 1;
        if (millisecond < 0) millisecond = 0;
        if (millisecond >= MillisecondsInSecond) millisecond = MillisecondsInSecond - 1;
        long totalDaysFromEpoch = 0;

        // 计算从纪元年到当前年之前的总天数
        if (year >= EpochYear)
            for (var y = EpochYear; y < year; y++)
                totalDaysFromEpoch += GetDaysInYear(y);
        else // 公元前年份 (Year 0 代表 1 BC, Year -1 代表 2 BC)
            for (var y = EpochYear - 1; y >= year; y--) // 从 Year 0 (1 BC) 开始倒数
                totalDaysFromEpoch -= GetDaysInYear(y);

        // 添加当前年内的天数 (0-based)
        var daysToMonth = IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
        totalDaysFromEpoch += daysToMonth[month - 1]; // 累积到当前月之前的天数
        totalDaysFromEpoch += day - 1; // 当前月内的天数

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
        var currentDays = totalDaysFromEpoch; // 0-based from epoch start
        year = EpochYear;

        if (currentDays >= 0)
        {
            // 从纪元年开始正向计数
            while (currentDays >= GetDaysInYear(year))
            {
                currentDays -= GetDaysInYear(year);
                year++;
            }
        }
        else // 公元前年份
        {
            // 从纪元年之前一年 (Year 0 / 1 BC) 开始反向计数
            year = EpochYear - 1; // 从 Year 0 (1 BC) 开始
            while (currentDays < 0)
            {
                var daysInPrevYear = GetDaysInYear(year);
                currentDays += daysInPrevYear;
                if (currentDays < 0) // 如果加上前一年的天数后仍然为负，则年份继续递减
                    year--;
            }
        }

        // 此时 currentDays 是计算出的 `year` 内的 0-based 天数
        var isLeap = IsLeapYear(year);
        var daysToMonth = isLeap ? DaysToMonth366 : DaysToMonth365;

        month = 1;
        while (month <= 12 && currentDays >= daysToMonth[month]) month++;
        // 减去 1 得到正确的月份索引 (因为 daysToMonth[month] 是到 month 结束的累计天数)
        day = (int)(currentDays - daysToMonth[month - 1]) + 1; // 1-based 日期

        // 修正 month/day 如果 currentDays 导致 month 溢出
        if (month == 0) // 这意味着 currentDays 为 0, 对应 1月1日
        {
            month = 1;
            day = 1;
        }
    }

    private GregorianCalendarRule()
    {
    }
}