using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWidgetApplication.Models
{
    public class ContactFormViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}