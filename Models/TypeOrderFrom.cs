using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{
    [Table("YMTG_NDS_OrderType")]
    public class TypeOrderFrom
    {
        [Key]
        public int Code { get; set; }             // รหัส
        public string TypeRecapFrom { get; set; }    // ประเภทการ Recap
        public string Description { get; set; }      // รายละเอียด
        public string CodeChar { get; set; }

        public string TypeRecapShow { get; set; }
        // ตัวอักษรโค้ด
    }
}
