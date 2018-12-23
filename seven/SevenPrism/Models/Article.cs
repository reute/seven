﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Article : BindableBase
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

        [Required]
        public Category Cat { get; set; } = new Category();

        public Manufacturer Manufacturer { get; set; } = new Manufacturer();

        public string Description
        {
            get
            {
                return $"{Cat.Name} {Manufacturer.Name} {Model}";
            }
        }

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }
    }
}