using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VolunteeringWpfApp.Models;

namespace VolunteeringWpfApp
{
    class VolunteeringDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var state = State.LoadFromFile();

            if (string.IsNullOrWhiteSpace(state.DatabaseConnectionString) || string.IsNullOrWhiteSpace(state.DatabaseName))
            {
                MessageBox.Show("Строка подключения или имя базы данных не указаны в конфигурации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            optionsBuilder.UseSqlServer($@"Server={state.DatabaseConnectionString};Database={state.DatabaseName};Trusted_Connection=True;TrustServerCertificate=True;").ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)); ;
        }

        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<VolunteerEvent> VolunteerEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VolunteerEvent>()
                .HasOne(ve => ve.Volunteer)
                .WithMany()
                .HasForeignKey(ve => ve.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VolunteerEvent>()
                .HasOne(ve => ve.Event)
                .WithMany()
                .HasForeignKey(ve => ve.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VolunteerEvent>()
                .HasOne(ve => ve.Role)
                .WithMany()
                .HasForeignKey(ve => ve.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.Region)
                .WithMany()
                .HasForeignKey(v => v.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Region)
                .WithMany()
                .HasForeignKey(e => e.RegionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}