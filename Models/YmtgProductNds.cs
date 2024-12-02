using System.ComponentModel.DataAnnotations.Schema;

namespace NDSPRO.Models
{ 
   
    [Table("YMTG_Product_NDS")]
    public class YmtgProductNds
    {
    
        public int Id { get; set; }
        public string? QuotationNumber { get; set; }
        public string? OrderNumber { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Qty { get; set; }
        public string? SKUCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public string? ScreenShots1 { get; set; }
        public string? Url1 { get; set; }
        public string? ScreenSize1 { get; set; }
        public string? TopDistance1 { get; set; }
        public string? ScreenShots2 { get; set; }
        public string? Url2 { get; set; }
        public string? ScreenSize2 { get; set; }
        public string? TopDistance2 { get; set; }
        public string? ScreenShots3 { get; set; }
        public string? Url3 { get; set; }
        public string? ScreenSize3 { get; set; }
        public string? TopDistance3 { get; set; }
        public string? ScreenShots4 { get; set; }
        public string? Url4 { get; set; }
        public string? ScreenSize4 { get; set; }
        public string? TopDistance4 { get; set; }
        public int PrintingType { get; set; }
        public string? Remark { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        
    }
}
