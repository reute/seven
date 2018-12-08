using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Deposit : BindableBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Date { get; set; } = DateTime.Now;

        private int amount;

        public int Amount
        {
            get => amount;
            set
            {
                SetProperty(ref amount, value);
                //RaisePropertyChanged(nameof(Amount));
            }
        }



        //public int Amount { get; set; }


        public Deposit() { }

        public Deposit(DateTime date, int amount)
        {
            Date = date;
            this.amount = amount;
            //Amount = amount;
        }
    }
}
