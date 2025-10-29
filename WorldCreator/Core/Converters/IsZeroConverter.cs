using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WorldCreator.Core.Converters;

public class IsZeroConverter:IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is  int v) return v==0;
        if(value is  long k) return k==0;
        if(value is  float f) return Math.Abs(f)<float.Epsilon;
        if(value is  double d) return Math.Abs(d)<double.Epsilon;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is bool v) return v?0:1;
        return 0;
    }
}