using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Test2.ViewModels
{
    public class AchievementSummaryVM
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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Troop")]
        public string TroopName { get; set; }

        [Display(Name = "Number of Years")]
        public int NumberOfYears { get; set; }

        [Display(Name = "Total Healthy Living")]
        public int TotalHL { get; set; }

        [Display(Name = "Average Leadership")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public double AverageLS { get; set; }

        [Display(Name = "Max Environment & Outdoors")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public double MaxEO { get; set; }
    }
}
