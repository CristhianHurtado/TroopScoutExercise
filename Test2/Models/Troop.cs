using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Test2.Models
{
    public class Troop : Auditable
    {
        public int ID { get; set; }

        [Display(Name = "Troop")]
        [Required(ErrorMessage = "You cannot leave the Troop name blank.")]
        [StringLength(50, ErrorMessage = "Troop name cannot be more than 50 characters long.")]
        public string TroopName { get; set; }

        [Display(Name = "Troop Number")]
        [Required(ErrorMessage = "You cannot leave the Troop number blank.")]
        [RegularExpression("^[A-Z]{1}\\d{3}$", ErrorMessage = "Please enter a valid Troop Number (One capital letter followed by 3 digits).")]
        [StringLength(4, ErrorMessage = "Troop number cannot be more than 4 characters long.")]
        public string TroopNumber { get; set; }

        [Display(Name = "Budget")]
        [Required(ErrorMessage = "You cannot leave the Troop Budget blank.")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Currency)]
        [Range(50d, 10000d, ErrorMessage = "Budget must be between $50.00 and $10,000.00")]
        public double TroopBudget { get; set; }

        public ICollection<Scout> Scouts { get; set; } = new HashSet<Scout>();

    }
}
