using Avalonia.Controls.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldCreator.Model.Time;

public partial class BindingTime:ObservableObject
{
    [ObservableProperty,NotifyPropertyChangedFor(nameof(MonthInYear))]
    private long _year;
    [ObservableProperty,NotifyPropertyChangedFor(nameof(DaysInMonth))]
    private int _month;
    [ObservableProperty]
    private int _day;
    [ObservableProperty]
    private int _hour;
    [ObservableProperty]
    private int _minute;
    [ObservableProperty]
    private int _second;
    [ObservableProperty]
    private int _millisecond;
    public CustomCalendarRule Calendar { get; }

    public BindingTime(CustomCalendarRule calendar)
    {
        Calendar = calendar;
    }
    public int MonthInYear => Calendar.GetMonthsInYear(Year);
    public int DaysInMonth => Calendar.GetDaysInMonth(Year, Month);
    public int HourInDay => Calendar.HoursInDay;
    public int MinuteInHour => Calendar.MinutesInHour;
    public int SecondInMinute => Calendar.SecondsInMinute;
    public int MillisecondInSecond => Calendar.MillisecondsInSecond;
    public static implicit operator CustomDateTime(BindingTime bindingTime)=>
        new CustomDateTime(bindingTime.Year, bindingTime.Month, bindingTime.Day,
            bindingTime.Hour, bindingTime.Minute, bindingTime.Second,
            bindingTime.Millisecond, bindingTime.Calendar);
    public static implicit operator BindingTime(CustomDateTime customDateTime) =>
        new BindingTime(customDateTime.Calendar)
        {
            Year = customDateTime.Year,
            Month = customDateTime.Month,
            Day = customDateTime.Day,
            Hour = customDateTime.Hour,
            Minute = customDateTime.Minute,
            Second = customDateTime.Second,
            Millisecond = customDateTime.Millisecond
        };
}