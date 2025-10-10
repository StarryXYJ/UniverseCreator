using Microsoft.EntityFrameworkCore;
using WorldCreator.Model;

namespace WorldCreator.Core.Data;

public class EntryDataContext:DbContext
{
    public System.Data.Entity.DbSet<Entry> Entries { get; set; } = null!;

    public EntryDataContext(DbContextOptions<EntryDataContext> options) : base(options)
    {
        
    } 
}