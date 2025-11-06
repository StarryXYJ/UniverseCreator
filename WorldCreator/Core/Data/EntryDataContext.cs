using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using WorldCreator.Core.Services;
using WorldCreator.Model;
using WorldCreator.Model.Time;

namespace WorldCreator.Core.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class EntryDataContext : DbContext
{
    public DbSet<Entry> Entries { get; set; }
    public DbSet<EntryTag> EntryTags { get; set; }
    public DbSet<EntryProperty> EntryProperties { get; set; }


    /// <summary>
    /// 依赖注入初始化
    /// </summary>
    /// <param name="options"></param>
    /// <param name="serviceProvider"></param>
    public EntryDataContext(DbContextOptions<EntryDataContext> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var repositoryService = this.GetService<IRepositoryService>();
        modelBuilder.Entity<Entry>()
            .Property<CustomDateTime>(e =>
                e.EndTime)
            .HasConversion(new CustomDateTimeConverter(repositoryService.CurrentRepository.Calendar));
        modelBuilder.Entity<Entry>()
            .Property<CustomDateTime>(e =>
                e.StartTime)
            .HasConversion(new CustomDateTimeConverter(repositoryService.CurrentRepository.Calendar));
    }
}

public class CustomDateTimeConverter(CustomCalendarRule rule) : ValueConverter<CustomDateTime, string>(
    v => v.ToString(),
    v => new CustomDateTime(v, rule)
    {
    });