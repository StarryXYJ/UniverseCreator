using System;
using System.Globalization;
using Avalonia.Data.Converters;
using WorldCreator.ViewModels;

namespace WorldCreator.Core.Converters;

public class StartViewDataContextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 尝试将传入的 DataContext 转换为 StartViewModel下的命令
        if (value is StartViewModel viewModel) return viewModel.OpenRepositoryCommand;
        // 如果转换失败，或者传入的不是 StartViewModel，可以返回 null 或抛出异常
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new Exception("Fail to convert"); // 通常不需要从命令转换回来
    }
}