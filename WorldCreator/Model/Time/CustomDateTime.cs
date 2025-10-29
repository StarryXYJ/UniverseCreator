using System;
using System.Numerics;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldCreator.Model.Time;

/// <summary>
///     表示一个使用自定义历法的日期和时间。
/// </summary>
public partial class CustomDateTime : ObservableObject, IComparable<CustomDateTime>, IEquatable<CustomDateTime>
{
    private bool _suppressNotifications;

    public CustomCalendarRule Calendar { get; }


    public BigInteger TotalMilliseconds { get; private set; }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(MonthInYear))]
    private long _year;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(DaysInMonth))]
    private int _month;

    [ObservableProperty] private int _day;

    [ObservableProperty] private int _hour;

    [ObservableProperty] private int _minute;

    [ObservableProperty] private int _second;

    [ObservableProperty] private int _millisecond;

    public CustomDateTime(long year, int month, int day, CustomCalendarRule calendar)
        : this(year, month, day, 0, 0, 0, 0, calendar)
    {
    }

    public CustomDateTime(long year, int month, int day, int hour, int minute, int second, int millisecond,
        CustomCalendarRule calendar)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        _suppressNotifications = true;
        _year = year;
        _month = month;
        _day = day;
        _hour = hour;
        _minute = minute;
        Second = second;
        _millisecond = millisecond;
        _suppressNotifications = false;
        RecalculateFromComponents();
    }

    private CustomDateTime(BigInteger totalMilliseconds, CustomCalendarRule calendar)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        TotalMilliseconds = totalMilliseconds;
        UpdateComponentsFromTotal();
    }

    partial void OnYearChanged(long value) => TriggerRecalcIfNotSuppressed();
    partial void OnMonthChanged(int value) => TriggerRecalcIfNotSuppressed();
    partial void OnDayChanged(int value) => TriggerRecalcIfNotSuppressed();
    partial void OnHourChanged(int value) => TriggerRecalcIfNotSuppressed();
    partial void OnMinuteChanged(int value) => TriggerRecalcIfNotSuppressed();
    partial void OnSecondChanged(int value) => TriggerRecalcIfNotSuppressed();
    partial void OnMillisecondChanged(int value) => TriggerRecalcIfNotSuppressed();


    private void TriggerRecalcIfNotSuppressed()
    {
        if (_suppressNotifications) return;
        RecalculateFromComponents();
    }

    private void RecalculateFromComponents()
    {
        if (_suppressNotifications) return;

        _suppressNotifications = true;
        // 直接写入 backing field 避免再次触发 OnTotalMillisecondsChanged
        TotalMilliseconds = Calendar.ToTotalMillisecondsFromEpoch(Year, Month, Day, Hour, Minute, Second, Millisecond);

        // 归一化并更新各字段（直接写入 backing fields）
        (Year, Month, Day, Hour, Minute, Second, Millisecond) =
            Calendar.FromTotalMillisecondsFromEpoch(TotalMilliseconds);


        _suppressNotifications = false;

        // 逐个触发属性变化通知
        OnPropertyChanged(nameof(Year));
        OnPropertyChanged(nameof(Month));
        OnPropertyChanged(nameof(Day));
        OnPropertyChanged(nameof(Hour));
        OnPropertyChanged(nameof(Minute));
        OnPropertyChanged(nameof(Second));
        OnPropertyChanged(nameof(Millisecond));
        OnPropertyChanged(nameof(TotalMilliseconds));
    }

    private void UpdateComponentsFromTotal()
    {
        _suppressNotifications = true;
        (Year, Month, Day, Hour, Minute, Second, Millisecond) =
            Calendar.FromTotalMillisecondsFromEpoch(TotalMilliseconds);

        _suppressNotifications = false;

        OnPropertyChanged(nameof(Year));
        OnPropertyChanged(nameof(Month));
        OnPropertyChanged(nameof(Day));
        OnPropertyChanged(nameof(Hour));
        OnPropertyChanged(nameof(Minute));
        OnPropertyChanged(nameof(Second));
        OnPropertyChanged(nameof(Millisecond));
    }

    public int CompareTo(CustomDateTime? other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));
        if (Calendar != other.Calendar) throw new ArgumentException("无法比较来自不同历法的 CustomDateTime 值。");
        return TotalMilliseconds.CompareTo(other.TotalMilliseconds);
    }

    public bool Equals(CustomDateTime? other)
    {
        if (other is null) return false;
        return TotalMilliseconds == other.TotalMilliseconds && Calendar == other.Calendar;
    }

    public override bool Equals(object? obj) => obj is CustomDateTime cd && Equals(cd);

    public override int GetHashCode() => HashCode.Combine(TotalMilliseconds, Calendar);

    public static bool operator ==(CustomDateTime left, CustomDateTime right) => Equals(left, right);
    public static bool operator !=(CustomDateTime left, CustomDateTime right) => !Equals(left, right);
    public static bool operator <(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) < 0;
    public static bool operator >(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) > 0;
    public static bool operator <=(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) <= 0;
    public static bool operator >=(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) >= 0;

    public CustomDateTime AddMilliseconds(BigInteger milliseconds) =>
        new CustomDateTime(TotalMilliseconds + milliseconds, Calendar);

    public CustomDateTime AddSeconds(long seconds) => AddMilliseconds(seconds * Calendar.MillisecondsInSecond);
    public CustomDateTime AddMinutes(long minutes) => AddSeconds(minutes * Calendar.SecondsInMinute);
    public CustomDateTime AddHours(long hours) => AddMinutes(hours * Calendar.MinutesInHour);
    public CustomDateTime AddDays(long days) => AddHours(days * Calendar.HoursInDay);

    public CustomDateTime AddMonths(int months)
    {
        var newYear = Year;
        var newMonth = Month + months;
        var newDay = Day;
        var monthsInYear = Calendar.GetMonthsInYear(newYear);

        while (newMonth > monthsInYear)
        {
            newMonth -= monthsInYear;
            newYear++;
            monthsInYear = Calendar.GetMonthsInYear(newYear);
        }

        while (newMonth < 1)
        {
            newYear--;
            monthsInYear = Calendar.GetMonthsInYear(newYear);
            newMonth += monthsInYear;
        }

        var daysInNewMonth = Calendar.GetDaysInMonth(newYear, newMonth);
        if (newDay > daysInNewMonth) newDay = daysInNewMonth;

        return new CustomDateTime(newYear, newMonth, newDay, Hour, Minute, Second, Millisecond, Calendar);
    }

    public CustomDateTime AddYears(long years)
    {
        var newYear = Year + years;
        var newMonth = Month;
        var newDay = Day;
        var daysInNewMonth = Calendar.GetDaysInMonth(newYear, newMonth);
        if (newDay > daysInNewMonth) newDay = daysInNewMonth;
        return new CustomDateTime(newYear, newMonth, newDay, Hour, Minute, Second, Millisecond, Calendar);
    }

    public CustomTimeSpan Subtract(CustomDateTime other)
    {
        if (Calendar != other.Calendar) throw new ArgumentException("无法计算来自不同历法的 CustomDateTime 值之间的时间间隔。");
        return new CustomTimeSpan(TotalMilliseconds - other.TotalMilliseconds);
    }

    public static CustomDateTime operator +(CustomDateTime dt, CustomTimeSpan ts) =>
        new CustomDateTime(dt.TotalMilliseconds + ts.TotalMilliseconds, dt.Calendar);

    public static CustomDateTime operator -(CustomDateTime dt, CustomTimeSpan ts) =>
        new CustomDateTime(dt.TotalMilliseconds - ts.TotalMilliseconds, dt.Calendar);

    public static CustomTimeSpan operator -(CustomDateTime a, CustomDateTime b) => a.Subtract(b);

    public override string ToString()
    {
        var sign = Year < 0 ? "-" : "";
        var absYear = Math.Abs(Year);
        var formattedYear = absYear < 1000 ? absYear.ToString("D4") : absYear.ToString();
        return
            $"{sign}{formattedYear}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}.{Millisecond:D3} ({Calendar?.Name})";
    }

    public int MonthInYear => Calendar.GetMonthsInYear(Year);
    public int DaysInMonth => Calendar.GetDaysInMonth(Year, Month);
    public int HourInDay => Calendar.HoursInDay;
    public int MinuteInHour => Calendar.MinutesInHour;
    public int SecondInMinute => Calendar.SecondsInMinute;
    public int MillisecondInSecond => Calendar.MillisecondsInSecond;
}