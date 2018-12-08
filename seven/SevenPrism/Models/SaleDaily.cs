using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class SaleDaily : BindableBase
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }

        public SaleDaily(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }
}
