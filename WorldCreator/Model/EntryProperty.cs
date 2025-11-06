using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldCreator.Model;

/// <summary>
///     词条属性
/// </summary>
public class EntryProperty
{
    [Key] public int Id { get; set; }
    public string Description { get; set; } = "";
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ICollection<Entry>? Entries { get; set; }
}