using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using WorldCreator.Model.Time;

namespace WorldCreator.Model;

/// <summary>
///     词条
/// </summary>
public partial class Entry : ObservableObject
{
    public Entry(string name, string description, byte priority, AvaloniaList<EntryProperty> properties,
        CustomDateTime startTime,
        CustomDateTime endTime)
    {
        Name = name;
        Description = description;
        Priority = priority;
        Properties = properties;
        StartTime = startTime;
        EndTime = endTime;
    }

    public Entry(string name, string description, byte priority, AvaloniaList<EntryProperty> properties) : this(
        name,
        description, priority, properties,
        new CustomDateTime(1, 1, 1, GregorianCalendarRule.Instance),
        new CustomDateTime(1, 1, 1, GregorianCalendarRule.Instance))
    {
    }

    protected Entry()
    {
    }

    [Key] public int Id { get; set; }

    [ObservableProperty] private string _name;
    public ICollection<EntryProperty> Properties { get; set; } = new List<EntryProperty>();
    public ICollection<EntryTag> Tags { get; set; } = new List<EntryTag>();

    [ObservableProperty] private byte _priority;
    [ObservableProperty] private CustomDateTime _startTime;
    [ObservableProperty] private CustomDateTime _endTime;
    [ObservableProperty] private string _description;

    [NotMapped]
    public AvaloniaList<EntryProperty> UIProperties
    {
        get
        {
            var list = new AvaloniaList<EntryProperty>();
            list.AddRange(Properties);
            return list;
        }
        set { Properties = new List<EntryProperty>(value ?? new AvaloniaList<EntryProperty>()); }
    }

    [NotMapped]
    public AvaloniaList<EntryTag> UITags
    {
        get
        {
            var list = new AvaloniaList<EntryTag>();
            list.AddRange(Tags);
            return list;
        }
        set { Tags = new List<EntryTag>(value ?? new AvaloniaList<EntryTag>()); }
    }
}