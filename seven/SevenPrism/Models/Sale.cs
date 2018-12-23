using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Sale : BindableBase
    {

        public int Id { get; set; }

        private DateTime _date = DateTime.Now;
        [Required]
        public DateTime Date
        {
            get => _date;

            set
            {
                SetProperty(ref _date, value);
            }
        }
        public Category Cat { get; set; }

        private Article _article = new Article();
        public Article Article {
            get
            {
                return _article;
            }
            set
            {
                if (value is Article)
                {
                    _article = value;
                    Price = value.Price;
                }                   
                else
                    _article = null;
            }
        }

        //private string _articleDescription;
        //public string ArticleDescription {
        //    get
        //    {
        //        if (_article != null)
        //            return _article.Description;
        //        else
        //            return _articleDescription;

        //    }
        //    set
        //    {
        //        _articleDescription = value;
        //    }
        //}

        public string ArticleDescription { get; set; } = string.Empty;

        public Referent Ref { get; set; }

        private int _amount;
        public int Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                    Sum = _amount * _price;
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                    Sum = _amount * _price;
            }
        }

        private decimal _sum;
        public decimal Sum
        {
            get => _sum;
            
            set
            {
                SetProperty(ref _sum, value);
            }
        }
    }
}
