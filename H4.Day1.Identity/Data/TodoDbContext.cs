using H4.Day1.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace H4.Day1.Identity.Data;

public partial class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public virtual DbSet<Cpr> Cprs { get; set; }
    public virtual DbSet<Todolist> Todolists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cpr>(entity =>
        {
            entity.ToTable("Cpr");
            entity.Property(e => e.CprNr).HasMaxLength(500);
            entity.Property(e => e.User).HasMaxLength(500);
        });

        modelBuilder.Entity<Todolist>(entity =>
        {
            entity.ToTable("Todolist");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Item).HasMaxLength(500);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Todolist)
                .HasForeignKey<Todolist>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todolist_Cpr");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
