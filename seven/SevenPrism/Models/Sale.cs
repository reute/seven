using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Model
{
    public class Sale : BindableBase
    {
        private int? _number;
        private decimal? _price;


        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Date { get; set; } = DateTime.Now;

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
