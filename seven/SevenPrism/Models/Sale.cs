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
        private int? _number;
        private decimal? _price;
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

        public Category? Category { get; set; }

        public string Detail { get; set; } = string.Empty;

        public Staff? Staff { get; set; }

        public int? Number
        {
            get => _number;
            set
            {
                if (SetProperty(ref _number, value) && _price != null)
                    RaisePropertyChanged(nameof(Sum));
            }
        }

        public decimal? Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value) && _number != null)
                    RaisePropertyChanged(nameof(Sum));
            }
        }

        public decimal? Sum
        {
            get
            {
                if (_number != null && _price != null)
                    return _number * _price;
                return null;
            }
        }
    }

    //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Category
    {
        // [Description("This is Kette")]
        Kette,
        Schlauch,
        Bremse,
        Licht,
        Kurbel,
        Kasette,
        Griffe,
        Klingel
    }

    //[TypeConverter(typeof(EnumDescriptionConverter))]
    public enum Staff
    {
        //[Description("This is Jr")]
        Jr,
        Jc,
        Cf
    }
}
