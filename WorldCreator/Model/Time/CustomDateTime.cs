using System;
using System.Dynamic;
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


    public BigInteger TotalSeconds { get; private set; }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(MonthInYear))]
    private long _year;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(DaysInMonth))]
    private int _month;

    [ObservableProperty] private int _day;

    [ObservableProperty] private int _hour;

    [ObservableProperty] private int _minute;

    [ObservableProperty] private int _second;

    partial void OnYearChanged(long oldValue, long newValue) => SyncTotal();

    partial void OnMonthChanged(int oldValue, int newValue)
    {
        if (newValue <= 0)
        {
            Month = 1;
        }
        else if (newValue >= Calendar.GetMonthsInYear(Year))
        {
            Month = Calendar.GetMonthsInYear(Year);
        }

        SyncTotal();
    }

    partial void OnDayChanged(int oldValue, int newValue)
    {
        if (newValue <= 0)
        {
            Day = 1;
        }
        else if (newValue >= Calendar.GetDaysInMonth(Year, Month))
        {
            Day = Calendar.GetDaysInMonth(Year, Month);
        }

        SyncTotal();
    }

    partial void OnHourChanged(int oldValue, int newValue)
    {
        if (newValue < 0)
        {
            Hour = 0;
        }
        else if (newValue >= Calendar.HoursInDay)
        {
            Hour = Calendar.HoursInDay - 1;
        }

        SyncTotal();
    }

    partial void OnMinuteChanged(int oldValue, int newValue)
    {
        if (newValue < 0)
        {
            Minute = 0;
        }
        else if (newValue >= Calendar.MinutesInHour)
        {
            Minute = Calendar.MinutesInHour - 1;
        }

        SyncTotal();
    }

    partial void OnSecondChanged(int oldValue, int newValue)
    {
        if (newValue < 0)
        {
            Second = 0;
        }
        else if (newValue >= Calendar.SecondsInMinute)
        {
            Second = Calendar.SecondsInMinute - 1;
        }

        SyncTotal();
    }

    protected void SyncTotal() =>
        TotalSeconds = Calendar.ToTotalSecondsFromEpoch(Year, Month, Day, Hour, Minute, Second);

    public CustomDateTime(long year, int month, int day, CustomCalendarRule calendar)
        : this(year, month, day, 0, 0, 0, calendar)
    {
    }

    public CustomDateTime(long year, int month, int day, int hour, int minute, int second,
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
        _suppressNotifications = false;
        RecalculateFromComponents();
    }

    public CustomDateTime(BigInteger totalSeconds, CustomCalendarRule calendar)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        TotalSeconds = totalSeconds;
        UpdateComponentsFromTotal();
    }

    public CustomDateTime(string totalSecondsString, CustomCalendarRule calendar) :
        this(BigInteger.Parse(totalSecondsString), calendar)
    {
    }

    private void RecalculateFromComponents()
    {
        if (_suppressNotifications) return;
        _suppressNotifications = true;
        // 直接写入 backing field 避免再次触发 OnTotalSecondsChanged
        TotalSeconds = Calendar.ToTotalSecondsFromEpoch(Year, Month, Day, Hour, Minute, Second);

        // 归一化并更新各字段（直接写入 backing fields）
        (Year, Month, Day, Hour, Minute, Second) =
            Calendar.FromTotalSecondsToEpoch(TotalSeconds);

        _suppressNotifications = false;

        // 逐个触发属性变化通知
        OnPropertyChanged(nameof(Year));
        OnPropertyChanged(nameof(Month));
        OnPropertyChanged(nameof(Day));
        OnPropertyChanged(nameof(Hour));
        OnPropertyChanged(nameof(Minute));
        OnPropertyChanged(nameof(Second));
        OnPropertyChanged(nameof(TotalSeconds));
    }

    private void UpdateComponentsFromTotal()
    {
        _suppressNotifications = true;
        (Year, Month, Day, Hour, Minute, Second) =
            Calendar.FromTotalSecondsToEpoch(TotalSeconds);
        _suppressNotifications = false;
        OnPropertyChanged(nameof(Year));
        OnPropertyChanged(nameof(Month));
        OnPropertyChanged(nameof(Day));
        OnPropertyChanged(nameof(Hour));
        OnPropertyChanged(nameof(Minute));
        OnPropertyChanged(nameof(Second));
    }

    public int CompareTo(CustomDateTime? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        //if (Calendar != other.Calendar) throw new ArgumentException("无法比较来自不同历法的 CustomDateTime 值。");
        return TotalSeconds.CompareTo(other.TotalSeconds);
    }

    public bool Equals(CustomDateTime? other)
    {
        if (other is null) return false;
        return TotalSeconds == other.TotalSeconds && Calendar == other.Calendar;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((CustomDateTime)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Calendar);
        hashCode.Add(TotalSeconds);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(CustomDateTime left, CustomDateTime right) => Equals(left, right);
    public static bool operator !=(CustomDateTime left, CustomDateTime right) => !Equals(left, right);
    public static bool operator <(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) < 0;
    public static bool operator >(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) > 0;
    public static bool operator <=(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) <= 0;
    public static bool operator >=(CustomDateTime left, CustomDateTime right) => left.CompareTo(right) >= 0;

    public CustomTimeSpan Subtract(CustomDateTime other)
    {
        //if (Calendar != other.Calendar) throw new ArgumentException("无法计算来自不同历法的 CustomDateTime 值之间的时间间隔。");
        return new CustomTimeSpan(TotalSeconds - other.TotalSeconds);
    }

    public static CustomDateTime operator +(CustomDateTime dt, CustomTimeSpan ts) =>
        new CustomDateTime(dt.TotalSeconds + ts.TotalSeconds, dt.Calendar);

    public static CustomDateTime operator -(CustomDateTime dt, CustomTimeSpan ts) =>
        new CustomDateTime(dt.TotalSeconds - ts.TotalSeconds, dt.Calendar);

    public static CustomTimeSpan operator -(CustomDateTime a, CustomDateTime b) => a.Subtract(b);

    public override string ToString()
    {
        var sign = Year < 0 ? "-" : "";
        var absYear = Math.Abs(Year);
        var formattedYear = absYear < 1000 ? absYear.ToString("D4") : absYear.ToString();
        return
            $"{sign}{formattedYear}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2} ({Calendar?.Name})";
    }

    public int MonthInYear => Calendar.GetMonthsInYear(Year);
    public int DaysInMonth => Calendar.GetDaysInMonth(Year, Month);
    public int HourInDay => Calendar.HoursInDay;
    public int MinuteInHour => Calendar.MinutesInHour;
    public int SecondInMinute => Calendar.SecondsInMinute;
}