using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Deposit : ModelBase
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        private int _amount;
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Must be between {1} and {2}")]
        public int Amount
        {
            get => _amount;
            set
            {
                SetPropertyAndValidate(ref _amount, value);
            }
        }
    }
}
