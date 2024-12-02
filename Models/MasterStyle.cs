using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{
    [Table("YMTG_MasterStyle")]
    public class MasterStyle
    {
        public int Id { get; set; }
        public string Style { get; set; }
        public string StyleCode { get; set; }
        public string SKUCode { get; set; }
        public string Brand { get; set; }
        public string StyleType { get; set; }
        public string TypeName { get; set; }
        public string FabricType { get; set; }
        public string FabricDesc { get; set; }
        public string PatternCode { get; set; }
        public string PatternDetail { get; set; }
        public string UpdateBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Description { get; set; }
        public string FullDesc { get; set; }
        public string SysOwner { get; set; }
        public DateTime SysCreateDate { get; set; }
        public string Gender { get; set; }
        public string FabricSpec { get; set; }
        public string SpecialTechnical { get; set; }
    }

}
