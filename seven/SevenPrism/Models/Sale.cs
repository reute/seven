﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Sale : BindableBase
    {
        private DateTime _date = DateTime.Now;
        public int Id { get; set; }
     
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        public Category Cat { get; set; }

        public string Detail { get; set; } = string.Empty;

        public Referent Ref { get; set; }

        private int? _amount;
        public int? Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value) && _price != null)
                    RaisePropertyChanged(nameof(Sum));
            }
        }

        private decimal? _price;
        public decimal? Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value) && _amount != null)
                    RaisePropertyChanged(nameof(Sum));
            }
        }

        public decimal? Sum
        {
            get
            {
                if (_amount != null && _price != null)
                    return _amount * _price;
                return null;
            }
        }
    }
}
