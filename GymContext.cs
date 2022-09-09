using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GymWebApp.Models;

namespace GymWebApp
{
    public class GymContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Personal> Personals { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Visit> Visits { get; set; }
        
        public GymContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hall>().HasData(
                new Hall[]
                {
                    new Hall { Id=1, Name="Жопа"},
                    new Hall { Id=2, Name="Хуй"},
                    new Hall { Id=3, Name="Пизда"}
                });

            modelBuilder.Entity<Trainer>().HasData(
                new Trainer[]
                {
                    new Trainer { Id=1, Name="Tom", HallId = 1},
                    new Trainer { Id=2, Name="Alice", HallId = 2}
                });
        }

    }

}

