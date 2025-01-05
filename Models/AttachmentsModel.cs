using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{
    [Table("YMTG_NDS_OtherFiles")]
    public class AttachmentsModel
    {


        public int Id { get; set; } = 0;


        public string OrderNumber { get; set; } = "";


        public string FileName { get; set; } = "";


        public string FilePath { get; set; } = "";

        public string FileDescription { get; set; } = "";


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string AddFileBy { get; set; } = "";


    }
}
