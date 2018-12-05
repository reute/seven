using Microsoft.EntityFrameworkCore;
using System;

namespace Seven.Repository
{
    public class SevenContext : DbContext
    {
        /// <summary>
        /// Creates a new Contoso DbContext.
        /// </summary>
        public SevenContext(DbContextOptions<SevenContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets the orders DbSet.
        /// </summary>
        public DbSet<Order> Orders
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
