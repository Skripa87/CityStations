using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class ReservCityStationsDbContext:DbContext
    {
        public ReservCityStationsDbContext() : base("Name = ReservCityStationsDataBase")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InformationTable>()
                .HasMany(h => h.Contents)
                .WithMany(c => c.InformationTables)
                .Map(m => m.MapLeftKey("InformationTable_Id")
                    .MapRightKey("Content_Id")
                    .ToTable("InformationTables_Contents"));
        }

        public DbSet<StationModel> Stations { get; set; }
        public DbSet<ModuleType> ModuleTypes { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<InformationTable> InformationTables { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}