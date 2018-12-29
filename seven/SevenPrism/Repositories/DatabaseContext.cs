using log4net;
using Microsoft.EntityFrameworkCore;
using SevenPrism.CustomControls;
using SevenPrism.Models;
using SevenPrism.Properties;
using System;
using System.Reflection;
using log4net.Config;
using System.Linq;
using System.IO;
using System.Windows;

namespace SevenPrism.Repository
{
    public class DatabaseContext : DbContext
    {
        // Logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 

        private string DataSource;

        public DatabaseContext() : base()
        {
           
            // Needed for Logger
            XmlConfigurator.Configure();
            var tmp = Database.EnsureCreated();
            if (tmp)
            {
                log.Info($"Db not found, created new {DataSource}");            
                FillDb(); 
            } else
            {
                log.Info($"Found Db at {DataSource}");       
            }
            LoadTables();
        }

        private void LoadTables()
        {
            Sales.Load();
            Deposits.Load();
            Referents.Load();
            Categories.Load();
            Articles.Load();
            Manufacturers.Load();
        }

        // fill db with initial values
        private void FillDb()
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
                Name = "Bremse"
            });
            Categories.Add(new Category
            {
                Name = "Schlauch"
            });
            Categories.Add(new Category
            {
                Name = "Kette"
            });

            Manufacturers.Add(new Manufacturer
            {
                Name = "Schwalbe"
            });

            Manufacturers.Add(new Manufacturer
            {
                Name = "Shimano"
            });

            Manufacturers.Add(new Manufacturer
            {
                Name = "Magura"
            });

            SaveChanges();

            Articles.Add(new Article
            {
                Date = DateTime.Now,
                Cat = Categories.Single(m => m.Name.Equals("Schlauch")),
                Manufacturer = Manufacturers.Single(m => m.Name.Equals("Schwalbe")),
                Model = "AV13",
                Price = 5                
            });

            Articles.Add(new Article
            {
                Date = DateTime.Now,
                Cat = Categories.Single(m => m.Name.Equals("Kette")),
                Manufacturer = Manufacturers.Single(m => m.Name.Equals("Shimano")),
                Model = "9fach",
                Price = 9
            });

            Articles.Add(new Article
            {
                Date = DateTime.Now,
                Cat = Categories.Single(m => m.Name.Equals("Bremse")),
                Manufacturer = Manufacturers.Single(m => m.Name.Equals("Magura")),
                Model = "HS33",
                Price = 80
            });

            SaveChanges();

            log.Info($"Added initial values to Db at {Application.Current.Properties["DataSource"]}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var appFolder = ApplicationInfo.ProductName;
            var databasePath = Path.Combine(appDataPath,appFolder);
           
            try
            {
                Directory.CreateDirectory(databasePath);
                var databaseName = Settings.Default.DatabaseName;               
                DataSource = Path.Combine(databasePath, databaseName);
                log.Info($"Using {databasePath} as folder for db");
            }
            // if dir cannot be created
            catch (Exception e)
            {
                // using application folder
                DataSource = Settings.Default.DatabaseName;
                log.Info($"Could not create folder {databasePath}, using application folder for db");
            }
    
            var connectionString = $"Data Source={DataSource}";
            // Using Applicaiton Property to store Path
            Application.Current.Properties["DataSource"] = DataSource;
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

        public DbSet<Manufacturer> Manufacturers
        {
            get;
            set;
        }
    }
}

