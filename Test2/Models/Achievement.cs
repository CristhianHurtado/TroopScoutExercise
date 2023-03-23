using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Xml.Linq;

namespace Test2.Models
{
    public class Achievement
    {
        public int ID { get; set; }

        [RegularExpression("^\\d{4}$", ErrorMessage = "Year is exactly 4 digits (eg. 2021)")]
        public int Year { get; set; }

        [Display(Name = "Healthy Living")]
        public int HL { get; set; }

        [Display(Name = "Beliefs & Values")]
        public int BV { get; set; }

        [Display(Name = "Leadership")]
        public int LS { get; set; }

        [Display(Name = "Creative Expression")]
        public int CE { get; set; }

        [Display(Name = "Environment & Outdoors")]
        public int EO { get; set; }

        [Display(Name = "Scout")]
        public int ScoutID { get; set; }
        public Scout Scout { get; set; }
    }
}
