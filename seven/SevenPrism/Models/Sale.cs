using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Sale : ModelBase
    {
        public Sale()
        {
            Date = DateTime.Now;
            ArticleDescription = string.Empty;
            Article = new Article();
            Ref = null;
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

        private Article _article;
        public Article Article
        {
            get => _article;
            set
            {
                _article = value;
                if (value is Article)
                    Price = value.Price;
            }
        }

        private string _articleDescription;
        [Required(ErrorMessage = "An ArticleDescription is required")]
        [StringLength(50, ErrorMessage = "Article Description not longer than [1} characters")]
        public string ArticleDescription
        {
            get => _articleDescription;
            set
            {
                SetPropertyAndValidate(ref _articleDescription, value);
            }
        }

        private Referent _ref;
        [Required(ErrorMessage = "A Referent is required")]
        public Referent Ref
        {
            get => _ref;
            set
            {
                SetPropertyAndValidate(ref _ref, value);                
            }
        }
  
        private int _amount;
        [Range(1, int.MaxValue, ErrorMessage = "Must be between {1} and {2}")]
        [Required]
        public int Amount
        {
            get => _amount;
            set
            {
                if (SetPropertyAndValidate(ref _amount, value))
                    Sum = _amount * _price;
            }
        }

        private decimal _price;
        [Range(0, int.MaxValue, ErrorMessage = "Must be between {1} and {2}")]
        [Required]
        public decimal Price
        {
            get => _price;
            set
            {
                if (SetPropertyAndValidate(ref _price, value))
                    Sum = _amount * _price;
            }
        }

        private decimal _sum;
        [Required]
        public decimal Sum
        {
            get => _sum;
            
            set
            {
                SetProperty(ref _sum, value);
            }
        }

        public string ToString(string format)
        {
            return string.Format(ArticleDescription, Amount, Price, Sum);
        }    
    }
}
