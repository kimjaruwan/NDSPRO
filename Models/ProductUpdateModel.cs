namespace NDSPRO.Models
{
    public class ProductUpdateModel
    {

        public string ProductName { get; set; }

        public string Sku { get; set; }
        public string SKUCodeFull { get; set; }
        public int Qty { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
    }
}
