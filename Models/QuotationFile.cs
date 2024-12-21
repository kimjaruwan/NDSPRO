using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{
    [Table("YMTG_NDS_QuotationFiles")]
    public class QuotationFile
    {

        public int Id { get; set; } = 0;


        public string QuotationNumber { get; set; } = "";


        public string FileName { get; set; } = "";


        public string FilePath { get; set; } = "";

        public string FileDescription { get; set; } = "";

      
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
