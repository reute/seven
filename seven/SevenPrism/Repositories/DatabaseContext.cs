using Microsoft.EntityFrameworkCore;
using SevenPrism.Models;
using SevenPrism.Properties;
using System;
using System.Collections.ObjectModel;

namespace SevenPrism.Repository
{
    public class DatabaseContext : DbContext
    {
        readonly string databasePath = Settings.Default.DatabasePath;
        /// <summary>
        /// Creates a new Contoso DbContext.
        /// </summary>
        public DatabaseContext() : base()
        {
            Database.EnsureCreated();
            Sales.Load();
            Deposits.Load();
            Referents.Load();
            Categories.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + databasePath);
        }  

        public DbSet<Sale> Sales
        {
            get;
            set;
        }

        public DbSet<Deposit> Deposits
        {
            get;
            set;
        }

        public DbSet<Referent> Referents
        {
            get;
            set;
        }

        public DbSet<Category> Categories
        {
            get;
            set;
        }
    }
}

