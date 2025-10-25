using System;
using System.Globalization;
using Avalonia.Data.Converters;
using WorldCreator.Control;
using WorldCreator.ViewModels;
using DisplayMode = ExCSS.DisplayMode;

namespace WorldCreator.Core.Converters;

public class DisplayModeToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DisplayMode currentMode && parameter is DisplayMode targetMode) return currentMode == targetMode;
        return false; // 默认返回 Collapsed
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

// 用于根据 SplitLayoutDirection 和 ConverterParameter 决定 Visibility
public class SplitLayoutDirectionToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SplitLayoutDirection currentDirection && parameter is SplitLayoutDirection targetDirection)
            return currentDirection == targetDirection;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

// 用于根据 SwapPanels 决定 Grid.Column 或 Grid.Row
public class SwapPanelsToGridIndexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // parameter: "0" or "2" (表示默认位置和交换位置)
        // value: bool (SwapPanels)

        if (value is bool swapPanels && parameter is string defaultIndexString)
            if (int.TryParse(defaultIndexString, out var defaultIndex))
            {
                // 如果不交换，返回默认索引
                if (!swapPanels) return defaultIndex;
                // 如果交换，返回另一个索引 (0 变 2, 2 变 0)
                return defaultIndex == 0 ? 2 : 0;
            }

        return 0; // 默认值
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}