using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using WorldCreator.Model.Time;

namespace WorldCreator.Model;

/// <summary>
///     词条
/// </summary>
public partial class Entry : ObservableObject
{
    public Entry(string name, string description, byte priority, ObservableCollection<EntryProperty> properties,
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

    public Entry(string name, string description, byte priority, ObservableCollection<EntryProperty> properties) : this(
        name,
        description, priority, properties,
        new CustomDateTime(1,1,1,GregorianCalendarRule.Instance), 
        new CustomDateTime(1,1,1,GregorianCalendarRule.Instance))
    {
    }


    [Key] public int Id { get; set; }

    [ObservableProperty] private string _name;
    public ObservableCollection<string> Tags { get; set; }
    public ObservableCollection<string> SubNames { get; set; }
    public ObservableCollection<EntryProperty> Properties { get; set; }
    [ObservableProperty] private double _priority;
    [ObservableProperty] private CustomDateTime? _startTime;
    [ObservableProperty] private CustomDateTime? _endTime;
    [ObservableProperty] private string _description;
}