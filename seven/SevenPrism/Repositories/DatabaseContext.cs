using Microsoft.EntityFrameworkCore;
using SevenPrism.Helpers;
using SevenPrism.Models;
using SevenPrism.Properties;
using System;

namespace SevenPrism.Repository
{
    public class DatabaseContext : DbContext
    {
        public string FullDatabasePath;

        public DatabaseContext() : base()
        {
            if (Database.EnsureCreated())
            {              
                initDb(); 
            }
            Sales.Load();
            Deposits.Load();
            Referents.Load();
            Categories.Load();
            Articles.Load();
        }

        // fill db with initial values
        private void initDb()
        {
            Referents.Add(new Referent
            {
                Name = "jr"
            });
            Referents.Add(new Referent
            {
                Name = "jc"
            });
            Referents.Add(new Referent
            {
                Name = "cf"
            });

            Categories.Add(new Category
            {
                Name = "Bremsen"
            });
            Categories.Add(new Category
            {
                Name = "Schläuche"
            });
            Categories.Add(new Category
            {
                Name = "Ketten"
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {      
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var appFolder = ApplicationInfo.ProductName;
            var databasePath = $"{appDataPath}\\{appFolder}";
            System.IO.Directory.CreateDirectory(databasePath);
       
            var databaseName = Settings.Default.DatabaseName;
            FullDatabasePath = $"{databasePath}\\{databaseName}";

            var connectionString = $"Data Source={FullDatabasePath}"; 
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

        public DbSet<Article> Articles
        {
            get;
            set;
        }
    }
}

