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
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now; 

        public int Amount { get; set; }       
    }
}
