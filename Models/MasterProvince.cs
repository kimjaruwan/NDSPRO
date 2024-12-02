using System.ComponentModel.DataAnnotations.Schema;
namespace NDSPRO.Models
{
    [Table("YMTG_MasterProvinces")]
    public class MasterProvince
    {
        public int Id { get; set; }          // Primary Key
        public string Provinces { get; set; }
        public string Districts { get; set; }
        public string SubDistricts { get; set; }
        public string ZipCode { get; set; }
    }
}
