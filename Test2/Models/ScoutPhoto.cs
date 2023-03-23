using System.ComponentModel.DataAnnotations;

namespace Test2.Models
{
    public class ScoutPhoto
    {
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        [StringLength(255)]
        public string MimeType { get; set; }

        public int ScoutID { get; set; }
        public Scout scout { get; set; }
    }
}
