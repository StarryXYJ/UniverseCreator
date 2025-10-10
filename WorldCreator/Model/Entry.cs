using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldCreator.Model;

public enum EntryType
{
    Default,
    Event,
    Character
}

/// <summary>
///     词条
/// </summary>
public class Entry
{
    public Entry(string name, string description, byte priority, List<EntryProperty>? properties, DateTime startTime,
        DateTime endTime, EntryType type)
    {
        Name = name;
        Description = description;
        Priority = priority;
        Properties = properties ?? [];
        StartTime = startTime;
        EndTime = endTime;
        Type = type;
    }

    public Entry(string name, string description, byte priority, List<EntryProperty>? properties) : this(name,
        description, priority, properties, DateTime.MinValue, DateTime.MinValue, EntryType.Default)
    {
    }


    [Key] public int Id { get; set; }

    public string Name { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public List<string> SubNames { get; set; } = new();
    public byte Priority { get; set; }
    public DateTime StartTime { get; set; } = DateTime.MinValue;
    public DateTime EndTime { get; set; } = DateTime.MinValue;
    public string Description { get; set; } = "";
    public List<EntryProperty> Properties { get; set; } = new();
    public EntryType Type { get; set; } = EntryType.Default;
}