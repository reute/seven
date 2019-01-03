using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Article : ModelBase
    {

        public Article()
        {           
            Date = DateTime.Now;            
        }

        public int Id { get; set; }

        private DateTime _date;
        [Required]
        public DateTime Date
        {
            get => _date;

            set
            {
                SetPropertyAndValidate(ref _date, value);
            }
        }

        private Category _cat;
        [Required(ErrorMessage = "A Category is required")]
        public Category Cat
        {
            get => _cat;
            set
            {
                SetPropertyAndValidate(ref _cat, value);
            }
        }

        public Manufacturer Manufacturer { get; set; }

        private string _model;
        [Required(ErrorMessage = "A Model is required")]
        [StringLength(50, ErrorMessage = "Model Description not longer than [1} characters")]
        public string Model
        {
            get => _model;
            set
            {
                SetPropertyAndValidate(ref _model, value);
            }
        } 

        private decimal _price;
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Must be between {1} and {2}")]
        public decimal Price
        {
            get => _price;
            set
            {
                SetPropertyAndValidate(ref _price, value);
            }
        }

        public string Description
        {
            get
            {
                var tmp = $"{Cat?.Name} {Manufacturer?.Name} {Model}";
                return tmp;
            }
        }
    }
}
