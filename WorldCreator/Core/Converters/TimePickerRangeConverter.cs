using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Data.Converters;
using SQLitePCL;
using WorldCreator.Model.Time;
using WorldCreator.ViewModels;

namespace WorldCreator.Core.Converters;



/// <summary>
/// 在CustomDataPicker中使用
/// 将DateTime转换为时间限制
/// </summary>
public class TimePickerRangeConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        
        if (value is CustomDateTime time&& parameter is string type) return type switch
        {
            "M"=>time.Calendar.GetMonthsInYear(time.Year),
            "D"=>time.Calendar.GetDaysInMonth(time.Year,time.Month),
            "h"=>time.Calendar.HoursInDay-1,
            "m"=>time.Calendar.MinutesInHour-1,
            "s"=>time.Calendar.SecondsInMinute-1,
            "ms"=>time.Calendar.MillisecondsInSecond-1,
            _=>null
        };
        
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new Exception("Fail to convert"); // 通常不需要从命令转换回来
    }
}