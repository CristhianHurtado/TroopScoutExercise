using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Test2.Models
{
    public class Scout : Auditable, IValidatableObject
    {
        public int ID { get; set; }

        [Display(Name = "Scout")]
        public string FullName
        {
            get
            {
                return FirstName
                    + " " + LastName;
            }
        }

        [Display(Name = "Scout")]
        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public string DOBSummary
        {
            get
            {
                return DOB.ToString("d") + " (Age: " + Age.ToString() + ")";
            }
        }

        [Display(Name = "Age")]
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int a = today.Year - DOB.Year
                    - ((today.Month < DOB.Month || (today.Month == DOB.Month && today.Day < DOB.Day) ? 1 : 0));
                return a; /*Note: You could add .PadLeft(3) but spaces disappear in a web page. */
            }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter the date of birth.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Display(Name = "Fee Paid")]
        [Required(ErrorMessage = "The Fee paid is required.")]
        [DataType(DataType.Currency)]
        public double FeePaid { get; set; }

        [Display(Name = "Troop")]
        [Required(ErrorMessage = "You must select a troop for the Scout.")]
        public int TroopID { get; set; }

        public Troop Troop { get; set; }

        public ICollection<Achievement> Achievements { get; set; } = new HashSet<Achievement>();
        public ScoutPhoto ScoutPhoto { get; set; }
        public ScoutThumbnail ScoutThumbnail { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Age > 13)
            {
                if (FeePaid < 15d)
                {
                    yield return new ValidationResult("The minimum fee for Scouts over 13 is $15.00", new[] { "FeePaid" });
                }
            }
            else if (Age > 11)//Age is 12 or 13
            {
                if (FeePaid < 10d)
                {
                    yield return new ValidationResult("The minimum fee for 12 and 13 year old Scouts is $10.00", new[] { "FeePaid" });
                }
            }
        }
    }
}
