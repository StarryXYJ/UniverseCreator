using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Serilog;

namespace WorldCreator.Core.Commands;

/// <summary>
///     Markdown预览时点击超链接触发的命令
///     不知道为什么RelayCommand用不了
///     以 entry:// 开头为快速跳转到词条预览
/// </summary>
public class HyperlinkClickCommand : ICommand
{
    private bool _isExecutable = true;

    public bool IsExecutable
    {
        get => _isExecutable;
        set
        {
            if (_isExecutable != value)
            {
                _isExecutable = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return parameter is string;
    }

    public void Execute(object? parameter)
    {
        var url = parameter as string;
        GoTo(url ?? "");
    }

    public static void GoTo(string url)
    {
        try
        {
            // https://github.com/dotnet/runtime/issues/17938
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", url);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }
}