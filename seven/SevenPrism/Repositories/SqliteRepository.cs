using Microsoft.EntityFrameworkCore;
using SevenPrism.Model;
using System;

namespace SevenPrism.Repository
{
    public class SqliteRepository : DbContext
    {
        /// <summary>
        /// Creates a new Contoso DbContext.
        /// </summary>
        public SqliteRepository() 
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=seven.db");
        }

        /// <summary>
        /// Gets the orders DbSet.
        /// </summary>
        public DbSet<Sale> Orders
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
