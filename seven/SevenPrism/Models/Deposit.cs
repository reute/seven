using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Deposit : BindableBase
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        private int _amount;
        [Required]
        public int Amount
        {
            get => _amount;
            set
            {
                SetProperty(ref _amount, value);
            }
        }
    }
}
