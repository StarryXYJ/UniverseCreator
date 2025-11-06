using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using WorldCreator.Core;
using WorldCreator.Model.Time;

namespace WorldCreator.Model;

/// <summary>
///     单一世界的仓库
/// </summary>
public partial class WorldRepository : ObservableObject, IComparable<WorldRepository>
{
    [ObservableProperty] private string _author = "";

    [ObservableProperty] private DateTime _lastEditTime = DateTime.Now;

    [ObservableProperty] private string _name = "";

    public CustomCalendarRule Calendar { get; set; } = GregorianCalendarRule.Instance;

    /// <summary>
    ///     用于按修改时间排序
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(WorldRepository? other)
    {
        return LastEditTime.CompareTo(other?.LastEditTime);
    }

    public void Save()
    {
        if (!Directory.Exists(Common.NativeRepoPath(Name))) Directory.CreateDirectory(Common.NativeRepoPath(Name));
        var path = Common.NativeConfigPath(Name);
        File.WriteAllText(path, JsonSerializer.Serialize(this));
    }

    public override string ToString()
    {
        return $"Name:{Name},author:{Author}";
    }
}