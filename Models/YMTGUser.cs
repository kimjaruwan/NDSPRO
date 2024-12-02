using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{
    [Table("YMTG_USER")]
    public class YMTGUser
    {
        public int Id { get; set; } // Assuming Id is an integer
        public string YPTUser { get; set; }
        public string YPTName { get; set; }
        public int YPTLevel { get; set; }
        public string YPTPass { get; set; }
        public string UserGroup { get; set; }
        public string Factory { get; set; }
        public string Dept { get; set; }
        public string YPTPODepartment { get; set; }
        public string GNXPODepartment { get; set; }
        public string Email { get; set; }
        public Boolean Resign { get; set; } // Assuming Resign is a boolean
        public string Status { get; set; }
        public string EmployeeId { get; set; }
    }
}
