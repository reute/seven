using Microsoft.EntityFrameworkCore;
using SevenPrism.Models;
using System;

namespace SevenPrism.Repository
{
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Creates a new Contoso DbContext.
        /// </summary>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
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
