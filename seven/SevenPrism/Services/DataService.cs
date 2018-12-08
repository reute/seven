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
        public ObservableCollection<Sale> Orders { get; }

        public SqliteRepository dbContext;

        private readonly DbContextOptions<SqliteRepository> _dbOptions;

        // Sale changed event

        public DataService()
        {
            //string demoDatabasePath = Package.Current.InstalledLocation.Path + @"\Assets\Contoso.db";
            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\four\\four.db";
            //if (!File.Exists(databasePath))
            //{
            //    File.Copy(demoDatabasePath, databasePath);
            //}
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SqliteRepository>().UseSqlite("Data Source=" + databasePath);
            _dbOptions = dbContextOptionsBuilder.Options;

            dbContext = new SqliteRepository();
            dbContext.Database.EnsureCreated();

            dbContext.Orders.ToList();
            dbContext.Deposits.ToList();

            Orders = dbContext.Orders.Local.ToObservableCollection();
            Deposits = dbContext.Deposits.Local.ToObservableCollection();
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
            dbContext.SaveChanges();
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
