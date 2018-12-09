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
            Sales.Load();
            Deposits.Load();   
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
    }
}
