using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{
    public class ProductType
    {
        
        public int Id { get; set; } = 0;                        // กำหนดค่าเริ่มต้นเป็น 0
        public string TypeProduct { get; set; } = "";
        

    }
}
