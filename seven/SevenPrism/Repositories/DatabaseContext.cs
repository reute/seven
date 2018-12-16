using Microsoft.EntityFrameworkCore;
using SevenPrism.Helpers;
using SevenPrism.Models;
using SevenPrism.Properties;
using System;
using System.Collections.ObjectModel;

namespace SevenPrism.Repository
{
    public class DatabaseContext : DbContext
    {  
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
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var appFolder = ApplicationInfo.ProductName;
            var databasePath = $"{appDataPath}\\{appFolder}";
            System.IO.Directory.CreateDirectory(databasePath);
       
            var databaseName = Settings.Default.DatabaseName;
            var connectionString = $"Data Source={databasePath}\\{databaseName}"; 
            optionsBuilder.UseSqlite(connectionString);
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

