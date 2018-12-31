using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Models
{
    public class Manufacturer : ModelBase
    {
        public int Id { get; set; } 

        private string _name = string.Empty;
        [Required]
        [StringLength(20, ErrorMessage = "Manufacturer not longer than [1} characters")]
        public string Name
        {
            get => _name;
            set
            {
                SetPropertyAndValidate(ref _name, value);
            }
        }
    }
}
