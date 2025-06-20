using BattleCatsEntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace BattleCatsEntityFramework.Data;
public class BattleCatsContext : Microsoft.EntityFrameworkCore.DbContext
{
    public BattleCatsContext(DbContextOptions<BattleCatsContext> options) : base(options) { }

    public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<BattleCatsCard> BattleCatsCards { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<UserCard> UserCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCard>()
            .HasKey(uc => new { uc.UserId, uc.CardId });
    }
}
