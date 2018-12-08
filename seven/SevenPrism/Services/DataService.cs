using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenPrism.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using SevenPrism.Models;

namespace SevenPrism.Services
{
    public class DataService
    {
        public ObservableCollection<Deposit> Deposits { get; }
        public ObservableCollection<Sale> Sales { get; }          

        //private readonly DbContextOptions<SqliteRepository> _dbOptions;

        // Sale changed event

        public DataService()
        {  
            SqliteContext = new DatabaseContext(GetSqliteContextOptions());
            SqliteContext.Database.EnsureCreated();

            SqliteContext.Sales.Load();
            SqliteContext.Deposits.Load();

            Sales = SqliteContext.Sales.Local.ToObservableCollection();
            Deposits = SqliteContext.Deposits.Local.ToObservableCollection();
        }

        public void Save()
        {
            SqliteContext.SaveChanges();
        }

        private DbContextOptions<DatabaseContext>  GetSqliteContextOptions()
        {
            //if (!File.Exists(databasePath))
            //{
            //    File.Copy(demoDatabasePath, databasePath);
            //}       
            string databasePath = "Seven.db";
            // string databasePath = Package.Current.InstalledLocation.Path + @"\Assets\Contoso.db";
            // string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\four\\four.db";
            return new DbContextOptionsBuilder<DatabaseContext>().UseSqlite("Data Source=" + databasePath).Options;
        }

        private DatabaseContext SqliteContext;

        //public List<Sale> GetOrders()
        //{
        //    return dbContext.Orders.ToList();
        //}


        //public void Upsert(Sale Sale)
        //{
        //    var existing = dbContext.Orders.FirstOrDefault(_order => _order.Id == Sale.Id);
        //    if (existing == null)
        //    {
        //        dbContext.Orders.Add(Sale);
        //    }
        //    else
        //    {
        //        dbContext.Entry(existing).CurrentValues.SetValues(Sale);
        //    }          
        //}

      

        //public void Delete(Guid orderId)
        //{
        //    var match = dbContext.Orders.Find(orderId);
        //    if (match != null)
        //    {
        //        dbContext.Orders.Remove(match);
        //    }
        //}
    }
}
