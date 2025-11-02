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
    public static readonly int DaysIn400Years = 146097;

    /// <summary>
    ///  0-99年 (100年): 100 * 365 + 25 闰年 (0, 4, ..., 96) = 36525 天<br/>
    /// 100-199年 (100年): 100 * 365 + 24 闰年 (104, ..., 196) = 36524 天<br/>
    /// 200-299年 (100年): 100 * 365 + 24 闰年 (204, ..., 296) = 36524 天<br/>
    /// 300-399年 (100年): 100 * 365 + 24 闰年 (304, ..., 396) = 36524 天
    /// </summary>
    public static readonly int[] AstroHundredYearBlockLengths = [36525, 36524, 36524, 36524];

    /// <summary>
    /// 一个4年周期有 4 * 365 + 1 (闰年) = 1461 天
    /// </summary>
    public static readonly int DaysIn4Years = 1461;

    public static readonly GregorianCalendarRule Instance = new();

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

    /// <summary>
    /// 判断一个long年份是否为闰年（格里高利历规则）。
    /// 适用于天文年编号，即可以处理负数年份和0年。
    /// </summary>
    /// <param name="year">要判断的年份（long）。</param>
    /// <returns>如果是闰年则返回 true，否则返回 false。</returns>
    public static bool IsLeapYear(long year)
    {
        // long % 400 == 0 works for negative numbers too.
        // E.g., (-400) % 400 == 0.
        if (year % 400 == 0)
        {
            return true;
        }

        if (year % 100 == 0)
        {
            return false;
        }

        return year % 4 == 0;
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

    /// <summary>
    /// 内部辅助函数：计算从天文年0年1月1日到指定天文年和该年剩余天数的总天数。
    /// 此函数只处理非负的天文年。
    /// </summary>
    /// <param name="astroYear">天文年份（long），必须是非负数。</param>
    /// <param name="remainingDaysInYear">该年剩余天数（BigInteger），0表示1月1日。</param>
    /// <returns>从天文年0年1月1日开始的总天数（BigInteger）。</returns>
    private static BigInteger CalculateDaysSinceAstroEpoch(long astroYear, BigInteger remainingDaysInYear)
    {
        if (astroYear < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(astroYear), "negetive astro year is not supported.");
        }

        BigInteger totalDays = 0;
        long currentAstroYear = 0; // 从天文年0年开始累加

        // --- 1. 累加完整的400年周期 ---
        // 一个400年周期有 400 * 365 + 97 (闰年) = 146097 天
        long num400YearBlocks = astroYear / 400;
        totalDays += num400YearBlocks * DaysIn400Years;
        currentAstroYear += num400YearBlocks * 400;

        // --- 2. 累加完整的100年周期 (在当前400年周期内) ---
        // 在一个400年周期内，100年块的天数模式是固定的。
        // 从天文年0年开始：
        // 0-99年 (100年): 100 * 365 + 25 闰年 (0, 4, ..., 96) = 36525 天
        // 100-199年 (100年): 100 * 365 + 24 闰年 (104, ..., 196) = 36524 天
        // 200-299年 (100年): 100 * 365 + 24 闰年 (204, ..., 296) = 36524 天
        // 300-399年 (100年): 100 * 365 + 24 闰年 (304, ..., 396) = 36524 天
        long hundredYearBlocksInCurrent400 = (astroYear - currentAstroYear) / 100;
        for (int i = 0; i < hundredYearBlocksInCurrent400; i++)
        {
            // blockIndex 确保我们使用正确的100年块天数
            int blockIndex = (int)((currentAstroYear / 100) % 4);
            totalDays += AstroHundredYearBlockLengths[blockIndex];
            currentAstroYear += 100;
        }

        // --- 3. 累加完整的4年周期 (在当前100年周期内) ---
        // 一个4年周期有 4 * 365 + 1 (闰年) = 1461 天
        BigInteger daysIn4Years = 1461;
        long num4YearBlocks = (astroYear - currentAstroYear) / 4;
        totalDays += num4YearBlocks * daysIn4Years;
        currentAstroYear += num4YearBlocks * 4;

        // --- 4. 累加单个年份的天数 (在当前4年周期内) ---
        while (currentAstroYear < astroYear)
        {
            totalDays += IsLeapYear(currentAstroYear) ? 366 : 365;
            currentAstroYear++;
        }

        // --- 5. 累加目标年份内的剩余天数 ---
        totalDays += remainingDaysInYear;

        return totalDays;
    }

    /// <summary>
    /// 将一个年份（long）和该年剩余天数（BigInteger）转换为从公元1年1月1日开始的总天数（BigInteger）。
    /// 0天表示公元1年1月1日。
    /// 年份为正数表示公元（AD），负数表示公元前（BC），例如 -1 表示 1 BC。
    /// </summary>
    /// <param name="year">年份（long）。</param>
    /// <param name="remainingDaysInYear">该年剩余天数（BigInteger），0表示1月1日。</param>
    /// <returns>从公元1年1月1日开始的总天数（BigInteger）。</returns>
    /// <exception cref="ArgumentOutOfRangeException">如果年份为0，或剩余天数不合法。</exception>
    public static BigInteger GetDaysFromYearAndRemainingDays(long year, BigInteger remainingDaysInYear)
    {
        // --- 输入校验 ---
        if (year == 0)
        {
            year = 1;
        }

        if (remainingDaysInYear < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(remainingDaysInYear), "剩余天数不能为负数。");
        }

        var daysInTargetYear = IsLeapYear(year) ? 366 : 365;
        if (remainingDaysInYear >= daysInTargetYear)
        {
            throw new ArgumentOutOfRangeException(nameof(remainingDaysInYear),
                $"剩余天数 {remainingDaysInYear} 超出了年份 {year} 的总天数 {daysInTargetYear - 1}。");
        }

        // --- 特殊情况处理：公元1年12月31日 ---
        // 根据 GetYearAndRemainingDays 的定义，totalDays = -1 对应 1 AD Dec 31。
        // 正常计算 1 AD Dec 31 应该是 totalDays = 364。
        // 为了保持反向一致性，这里做特殊处理。
        if (year == 1 && remainingDaysInYear == (IsLeapYear(1) ? 365 : 364))
        {
            return BigInteger.Parse("-1");
        }

        BigInteger totalDays;

        if (year > 0) // AD 年份 (公元1年及以后)
        {
            // 将公元年转换为天文年 (1 AD -> Astro 0, 2 AD -> Astro 1, ...)
            long astroYear = year - 1;
            totalDays = CalculateDaysSinceAstroEpoch(astroYear, remainingDaysInYear);
        }
        else // BC 年份 (公元前1年及以前)
        {
            // 这里的逻辑需要与 GetYearAndRemainingDays 函数的 BC 处理逻辑完全反向。
            // GetYearAndRemainingDays 的 BC 逻辑: totalDays = -(effectiveDaysForAstroCalc + 1)
            // 其中 effectiveDaysForAstroCalc = CalculateDaysSinceAstroEpoch(tempAstroYear, tempRemainingDays)
            // 且 tempRemainingDays = daysInTargetCalendarYear - remainingDaysInYear - 1

            // 1. 将输入的 BC 年份转换为天文年
            // -1 BC -> Astro 0, -2 BC -> Astro 1, ...
            long tempAstroYear = -year - 1;

            // 2. 反向计算 GetAstroYearAndRemainingDaysInternal 内部使用的 'tempRemainingDays'
            // remainingDaysInYear 是 GetYearAndRemainingDays 返回的 0-indexed 天数。
            // tempRemainingDays 是 GetAstroYearAndRemainingDaysInternal 接收的 0-indexed 天数。
            // 关系是: calculatedRemainingDays = daysInTargetCalendarYear - tempRemainingDays - 1
            // 所以: tempRemainingDays = daysInTargetCalendarYear - calculatedRemainingDays - 1
            BigInteger tempRemainingDays = daysInTargetYear - remainingDaysInYear - 1;

            // 3. 计算 effectiveDaysForAstroCalc
            BigInteger effectiveDaysForAstroCalc = CalculateDaysSinceAstroEpoch(tempAstroYear, tempRemainingDays);

            // 4. 根据公式计算 totalDays
            totalDays = -(effectiveDaysForAstroCalc + 1);
        }

        return totalDays;
    }

    public override BigInteger ToTotalSecondsFromEpoch(long year, int month, int day, int hour, int minute, int second)
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
        BigInteger totalDaysFromEpoch = 0;

        // // 计算从纪元年到当前年之前的总天数
        // if (year >= EpochYear)
        //     for (var y = EpochYear; y < year; y++)
        //         totalDaysFromEpoch += GetDaysInYear(y);
        // else // 公元前年份 (Year 0 代表 1 BC, Year -1 代表 2 BC)
        //     for (var y = EpochYear - 1; y >= year; y--) // 从 Year 0 (1 BC) 开始倒数
        //         totalDaysFromEpoch -= GetDaysInYear(y);
        totalDaysFromEpoch = GetDaysFromYearAndRemainingDays(year, 0);

        // 添加当前年内的天数 (0-based)
        var daysToMonth = IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
        totalDaysFromEpoch += daysToMonth[month - 1]; // 累积到当前月之前的天数
        totalDaysFromEpoch += day - 1; // 当前月内的天数

        // 添加时间组件
        var totalSeconds = totalDaysFromEpoch * TotalSecondsInDay;
        totalSeconds += hour * MinutesInHour * SecondsInMinute * totalSeconds;
        totalSeconds += minute * SecondsInMinute * totalSeconds;
        totalSeconds += second * totalSeconds;

        return totalSeconds;
    }


    /// <summary>
    /// 内部辅助函数：将一个BigInteger天数转换为对应的天文年数和该年剩余天数。
    /// 假设天数从天文年0年1月1日开始计算，0天表示天文年0年1月1日。
    /// 此函数只处理非负天数。
    /// </summary>
    /// <param name="daysSinceAstroEpoch">总天数（BigInteger），从天文年0年1月1日开始计算。</param>
    /// <returns>一个元组，包含计算出的天文年份（BigInteger）和该年剩余天数（BigInteger）。</returns>
    private static (long astroYear, int remainingDaysInYear) GetAstroYearAndRemainingDaysInternal(
        BigInteger daysSinceAstroEpoch)
    {
        if (daysSinceAstroEpoch < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(daysSinceAstroEpoch),
                "the internal function only accepts non-negative days.");
        }

        long currentAstroYear = 0; // 从天文年0年开始
        var daysRemaining = daysSinceAstroEpoch;

        // --- 1. 处理400年周期 ---
        // 一个400年周期有 400 * 365 + 97 (闰年) = 146097 天
        var num400YearBlocks = (long)(daysRemaining / DaysIn400Years);
        currentAstroYear += num400YearBlocks * 400;
        daysRemaining %= DaysIn400Years;

        // --- 2. 处理100年周期 (在剩余不足400年的天数中) ---
        // 在一个400年周期内，100年块的天数模式是固定的。
        // 从天文年0年开始：
        foreach (var t in AstroHundredYearBlockLengths)
        {
            if (daysRemaining >= t)
            {
                daysRemaining -= t;
                currentAstroYear += 100;
            }
            else
            {
                break; // 剩余天数不足以构成下一个100年块
            }
        }

        // --- 3. 处理4年周期 (在剩余不足100年的天数中) ---
        var num4YearBlocks = (long)(daysRemaining / DaysIn4Years);
        currentAstroYear += num4YearBlocks * 4;
        daysRemaining %= DaysIn4Years;

        // --- 4. 处理单个年份 (在剩余不足4年的天数中) ---
        // 此时 daysRemaining 已经很小，可以直接逐年迭代
        while (true)
        {
            int daysInThisYear = IsLeapYear(currentAstroYear) ? 366 : 365;
            if (daysRemaining < daysInThisYear)
            {
                break; // 剩余天数不足以构成完整的一年
            }

            daysRemaining -= daysInThisYear;
            currentAstroYear++;
        }

        return (currentAstroYear, (int)daysRemaining);
    }

    /// <summary>
    /// 将一个BigInteger天数转换为对应的年数（long）和该年剩余天数（int）。
    /// 假设天数从公元1年1月1日开始计算，0天表示公元1年1月1日。
    /// 年份为正数表示公元（AD），负数表示公元前（BC），例如 -1 表示 1 BC。
    /// </summary>
    /// <param name="totalDays">总天数。</param>
    /// <returns>一个元组，包含计算出的年份（long）和该年剩余天数（BigInteger）。</returns>
    /// <exception cref="ArgumentOutOfRangeException">如果天数小于long.MinValue，则抛出。</exception>
    /// <exception cref="OverflowException">如果计算出的年份超出long的范围，则抛出。</exception>
    public static (long year, int remainingDaysInYear) GetYearAndRemainingDays(BigInteger totalDays)
    {
        // 0天表示公元1年1月1日
        if (totalDays == 0)
        {
            return (1, 0); // (Year: 1 AD, Remaining Days: 0)
        }

        long calculatedCalendarYear;
        BigInteger calculatedRemainingDays;
        if (totalDays > 0)
        {
            // 对于正数天数，我们从公元1年1月1日向前计数。
            // 这相当于从天文年0年1月1日开始，向前计数 totalDays 天。
            var (astroYear, remaining) = GetAstroYearAndRemainingDaysInternal(totalDays);

            // 将天文年转换为日历年（AD）
            // 天文年0 -> 1 AD
            // 天文年1 -> 2 AD
            // ...
            calculatedCalendarYear = astroYear + 1;
            calculatedRemainingDays = remaining;
        }
        else // totalDays < 0
        {
            // 对于负数天数，我们从公元1年1月1日向后计数。
            // 例如：
            // totalDays = -1 对应 1 AD Dec 31
            // totalDays = -365 (如果1 AD非闰年) 对应 1 BC Jan 1
            // totalDays = -366 (如果1 AD非闰年) 对应 2 BC Dec 31

            var daysBeforeEpoch = BigInteger.Abs(totalDays); // 距离1 AD Jan 1 的绝对天数

            // 将其转换为从天文年0年1月1日开始，向前计数 effectiveDaysForAstroCalc 天。
            // 举例：
            // totalDays = -1 (1 AD Dec 31) -> daysBeforeEpoch = 1 -> effectiveDaysForAstroCalc = 0
            // totalDays = -365 (1 BC Jan 1) -> daysBeforeEpoch = 365 -> effectiveDaysForAstroCalc = 364
            // totalDays = -366 (2 BC Dec 31) -> daysBeforeEpoch = 366 -> effectiveDaysForAstroCalc = 365
            var effectiveDaysForAstroCalc = daysBeforeEpoch - 1;

            if (effectiveDaysForAstroCalc < 0) // 这意味着 totalDays 是 -1
            {
                // totalDays = -1 对应 1 AD Dec 31。
                // 1 AD 不是闰年，所以有365天。1 AD Dec 31 是其第365天 (0-indexed 364)。
                // 剩余天数是 364。
                return (1, IsLeapYear(1) ? 365 : 364);
            }

            var (tempAstroYear, tempRemainingDays) = GetAstroYearAndRemainingDaysInternal(effectiveDaysForAstroCalc);

            // 将天文年 (0, 1, 2...) 转换为日历 BC 年 (-1, -2, -3...)
            // 天文年0 -> 1 BC (-1)
            // 天文年1 -> 2 BC (-2)
            // ...
            calculatedCalendarYear = -(tempAstroYear + 1);

            // 将 tempRemainingDays (0-365) 转换为目标 BC 年内的剩余天数。
            // tempRemainingDays 是 tempAstroYear 内的0-indexed天数。
            // 我们需要计算该 BC 年从1月1日开始的实际天数。
            int daysInTargetCalendarYear = IsLeapYear(calculatedCalendarYear) ? 366 : 365;
            calculatedRemainingDays = daysInTargetCalendarYear - tempRemainingDays - 1;
        }

        return (calculatedCalendarYear, (int)calculatedRemainingDays);
    }

    public override void FromTotalSecondsToEpoch(BigInteger totalSeconds, out long year, out int month,
        out int day, out int hour, out int minute, out int second)
    {
        var totalDaysFromEpoch = BigInteger.DivRem(totalSeconds, TotalSecondsInDay, out var remainingMsInDay);

        // 提取时间组件
        hour = (int)BigInteger.DivRem(remainingMsInDay, MinutesInHour * SecondsInMinute,
            out remainingMsInDay);
        minute = (int)BigInteger.DivRem(remainingMsInDay, SecondsInMinute, out remainingMsInDay);
        second = (int)remainingMsInDay;

        // 提取日期组件 (年, 月, 日)
        var currentDays = totalDaysFromEpoch;
        year = EpochYear;

        (year, day) = GetYearAndRemainingDays(currentDays);

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