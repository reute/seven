using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seven.Model
{
    public class Earning : BindableBase
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }

        public Earning(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }
}
