using System.ComponentModel.DataAnnotations.Schema;
namespace NDSPRO.Models

{
    [Table("YMTG_Order_NDS")]
    public class YmtgOrderNds
    {
        //YMTG_Order_NDS
        public int Id { get; set; }   
        // Primary Key
        public string? QuotationNumber { get; set; }         // หมายเลขใบเสนอราคา
        public string? OrderNumber  { get; set; }             // หมายเลขคำสั่งซื้อ
        public DateTime? OrderDate { get; set; }             // วันที่สั่งซื้อ
        public string? OrderStatus { get; set; }             // สถานะคำสั่งซื้อ
        public DateTime? ShipDate { get; set; }              // วันที่จัดส่ง
        public int TotalQty { get; set; }                   // จำนวนรวม
        public decimal TotalPrice { get; set; }             // ราคาทั้งหมด
        public string? CustomerName { get; set; }            // ชื่อลูกค้า
        public string? CustomerEmail { get; set; }           // อีเมลลูกค้า
        public string? CustomerAddress { get; set; }         // ที่อยู่ลูกค้า
        public string? CustomerAddressTax { get; set; }      // ที่อยู่สำหรับภาษี
        public string? CustomerPhone { get; set; }           // เบอร์โทรลูกค้า
        public string? Remark { get; set; }                  // หมายเหตุ
        public string? CreateBy { get; set; }                // สร้างโดย
        public DateTime? CreateDate { get; set; }            // วันที่สร้าง
        public int QuoStatus { get; set; }               // สถานะใบเสนอราคา
        public string? QuoType { get; set; }                 // ประเภทใบเสนอราคา
        public string? QuoLastname { get; set; }             // นามสกุลในใบเสนอราคา
        public string? QuoCompanyName { get; set; }          // ชื่อบริษัทในใบเสนอราคา
        public string? QuoProvince { get; set; }             // จังหวัดในใบเสนอราคา
        public string? QuoDistricts { get; set; }            // อำเภอในใบเสนอราคา
        public string? QuoSubDistricts { get; set; }         // ตำบลในใบเสนอราคา
        public string? QuoZipCode { get; set; } // รหัสใบเสนอราคา

        public string? QuoRemark { get; set; }
        public string? QuoTaxID { get; set; }




    }
}
