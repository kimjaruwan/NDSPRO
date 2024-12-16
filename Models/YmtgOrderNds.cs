using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NDSPRO.Models

{
    [Table("YMTG_NDS_Order")]
    public class YmtgOrderNds
    {
      
        public int Id { get; set; } = 0;                        // กำหนดค่าเริ่มต้นเป็น 0
        public string QuotationNumber { get; set; } = "";       // หมายเลขใบเสนอราคา
        public string OrderNumber { get; set; } = "";           // หมายเลขคำสั่งซื้อ
        public DateTime? OrderDate { get; set; } = DateTime.Now.Date;         // วันที่สั่งซื้อ
        public string OrderStatus { get; set; } = "";           // สถานะคำสั่งซื้อ
        public DateTime? ShipDate { get; set; } = DateTime.Now.Date;                // วันที่จัดส่ง
        public int TotalQty { get; set; } = 0;                  // จำนวนรวม กำหนดค่าเริ่มต้นเป็น 0
        public decimal TotalPrice { get; set; } = 0m;           // ราคาทั้งหมด กำหนดค่าเริ่มต้นเป็น 0
        public string CustomerName { get; set; } = "";          // ชื่อลูกค้า
        public string CustomerEmail { get; set; } = "";         // อีเมลลูกค้า
        public string CustomerAddress { get; set; } = "";       // ที่อยู่ลูกค้า
        public string CustomerAddressTax { get; set; } = "";    // ที่อยู่สำหรับภาษี
        public string CustomerPhone { get; set; } = "";         // เบอร์โทรลูกค้า
        public string Remark { get; set; } = "";                // หมายเหตุ
        public string CreateBy { get; set; } = "";              // สร้างโดย
        public DateTime CreateDate { get; set; } //วันที่ Create กำหนดแล้วใน Database MS SQL = getdate()             
        public int QuoStatus { get; set; } = 0;                 // สถานะใบเสนอราคา กำหนดค่าเริ่มต้นเป็น 0
        public string QuoType { get; set; } = "";               // ประเภทใบเสนอราคา
        public string QuoLastname { get; set; } = "";           // นามสกุลในใบเสนอราคา
        public string QuoCompanyName { get; set; } = "";        // ชื่อบริษัทในใบเสนอราคา
        public string QuoProvince { get; set; } = "";           // จังหวัดในใบเสนอราคา
        public string QuoDistricts { get; set; } = "";          // อำเภอในใบเสนอราคา
        public string QuoSubDistricts { get; set; } = "";       // ตำบลในใบเสนอราคา
        public string QuoZipCode { get; set; } = "";            // รหัสใบเสนอราคา
        public string QuoRemark { get; set; } = "";             // หมายเหตุใบเสนอราคา
        public string QuoTaxID { get; set; } = "";              // เลขประจำตัวผู้เสียภาษี

        public DateTime? QuoLastUpdate { get; set; } = DateTime.Now;

        public int QuoShippingPrice { get; set; } = 0;

        public int QuoCancel { get; set; } = 0;
    }

}
