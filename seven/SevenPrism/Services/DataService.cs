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

        public DatabaseContext SqliteContext;

        //private readonly DbContextOptions<SqliteRepository> _dbOptions;

        // Sale changed event

        public DataService()
        {
            string databasePath = "Seven.db";
            // string databasePath = Package.Current.InstalledLocation.Path + @"\Assets\Contoso.db";
            // string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\four\\four.db";

            //if (!File.Exists(databasePath))
            //{
            //    File.Copy(demoDatabasePath, databasePath);
            //}            
            var sqliteContextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite("Data Source=" + databasePath).Options; 
            SqliteContext = new DatabaseContext(sqliteContextOptions);
            var bo = SqliteContext.Database.EnsureCreated();

            SqliteContext.Sales.ToList();
            SqliteContext.Deposits.ToList();

            Sales = SqliteContext.Sales.Local.ToObservableCollection();
            Deposits = SqliteContext.Deposits.Local.ToObservableCollection();
        }

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

        public void Save()
        {
            SqliteContext.SaveChanges();
        }

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
