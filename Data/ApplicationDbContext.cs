using Microsoft.EntityFrameworkCore;
using NDSPRO.Models;
 
namespace NDSPRO.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Item> Items { get; set; } // เพิ่ม DbSet สำหรับแต่ละ Entity
        //YMTGUser ชื่อ Model
        public DbSet<YMTGUser> YMTG_USER { get; set; }
        
        public DbSet<MasterStyle> MasterStyles { get; set; }

        public DbSet<QuotationRun> QuotationRuns { get; set; }

        public DbSet<MasterProvince> MasterProvinces { get; set; }

        public DbSet<YmtgProductNds> YmtgProducts { get; set; }

        public DbSet<YmtgOrderNds> YmtgOrders { get; set; }
     
        public DbSet<TypeOrderFrom> TypeOrder { get; set; }


        public DbSet<ProductType> ProductTypes { get; set; }
      
        public DbSet<MasterRemark> YmtgRemark { get; set; }

        public DbSet<QuotationFile> QuotationFiles { get; set; }

        public DbSet<OrderModel> OrderModel { get; set; }

        public DbSet<ProductModel> ProductModel { get; set; }

        public DbSet<AttachmentsModel> AttachmentsModel { get; set; }
        //AttachmentsModel
    }
}
