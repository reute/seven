﻿using Microsoft.EntityFrameworkCore;
using SevenPrism.Models;
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
            optionsBuilder.UseSqlite("Data Source=Seven.db");
        }

        /// <summary>
        /// Gets the orders DbSet.
        /// </summary>
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
