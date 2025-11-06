using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldCreator.Model;

public class EntryTag
{
    [Key] public int Id { get; set; }
    public string Tag { get; set; } = "";
    public ICollection<Entry>? Entries { get; set; }
}