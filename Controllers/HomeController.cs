using Microsoft.AspNetCore.Mvc;
using NDSPRO.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using NDSPRO.Data;

using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using static System.Net.WebRequestMethods;
namespace NDSPRO.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadPath;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");

            // สร้างโฟลเดอร์ Upload ถ้ายังไม่มี
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }


        public IActionResult Index()
        {
            //เริ่มต้นเป็นหน้าแรก
            return View();
        }


        public IActionResult MainLinkIndex()
        {
            //เริ่มต้นเป็นหน้าแรก
            return View();
        }
        public IActionResult OrderInformation()
        {
            //เริ่มต้นเป็นหน้าแรก
            return View();
        }

        [HttpPost]
        public ActionResult CheckUser()
        {
            var user = "jaruwan.s";
            var pass = "j1234";

            var items = _context.YMTG_USER.Where(z => z.YPTUser == user && z.YPTPass == pass).FirstOrDefault();
            return Ok(items);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult LogOff()
        {

            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] Item itemNews)
        {
            if (itemNews.Name != "")
            {
                _context.Items.Add(itemNews);
                _context.SaveChanges();

                return Ok(itemNews);
            }
            return BadRequest("Invalid data");
        }

        [HttpPost]
        public ActionResult GetItemss()
        {
            var items = _context.Items.ToList();
            return Ok(items);
        }

        public IActionResult CreateQuo()
        {

            return View();
        }

        public IActionResult EditQuo(string quotationNumber)
        {
        
            var model = new QuotationViewModel
            {
                QuotationNumber = quotationNumber
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult GetSku()
        {
            var MasterStylesDataGroup = _context.MasterStyles.GroupBy(p => p.Description).ToList();
            var MasterStylesData = MasterStylesDataGroup.Select(g => g.Key).ToList();
            return Ok(MasterStylesData);
        }


        [HttpGet]
        public ActionResult GetColors()
        {
            var colors = new List<object>();
            colors.Add("Black");
            colors.Add("Green");
            colors.Add("Gray");
            colors.Add("Navy");
            colors.Add("White");
            colors.Add("NO COLOR");
            return Ok(colors);
        }
        [HttpGet]
        public ActionResult GetSizes()
        {
            var sizes = new List<object>();
            sizes.Add("XS");
            sizes.Add("S");
            sizes.Add("M");
            sizes.Add("L");
            sizes.Add("XL");
            sizes.Add("2XL");
            sizes.Add("3XL");
            sizes.Add("4XL");
            sizes.Add("NO SIZE");
            return Ok(sizes);
        }

        public ActionResult GenerateQuotationNumber()
        {
            var GenerateQuotation = _context.QuotationRuns.GroupBy(p => p.QuotationNumber).ToList();
            var GenerateQuotations = GenerateQuotation.Select(g => g.Key).ToList();
            return Ok(GenerateQuotations);
        }



        [HttpGet]
        public ActionResult GetProvinces()
        {
            var GetProvinceList = _context.MasterProvinces.GroupBy(p => p.Provinces).ToList();
            var GetProvinceLists = GetProvinceList.Select(g => g.Key).ToList();
            return Ok(GetProvinceLists);
        }
        [HttpPost]
        public ActionResult GetDistricts([FromBody] MasterProvince Provinces)
        {
            var GetDistrict = _context.MasterProvinces.Where(z => z.Provinces == Provinces.Provinces).GroupBy(p => p.Districts).ToList();
            var GetDistrictLists = GetDistrict.Select(g => g.Key).ToList();
            return Ok(GetDistrictLists);
        }

        [HttpPost]
        public ActionResult GetListSubs([FromBody] MasterProvince request)
        {
            var GetSubDistricts = _context.MasterProvinces.Where(z => z.Districts == request.Districts && z.Provinces == request.Provinces)
                .GroupBy(p => p.SubDistricts).ToList();
            var GetSubDistrictsList = GetSubDistricts.Select(g => g.Key).ToList();
            return Ok(GetSubDistrictsList);

        }

        [HttpPost]
        public ActionResult GetListZipcode([FromBody] MasterProvince request)
        {
            var GetZipcode = _context.MasterProvinces
            .Where(z => z.SubDistricts == request.SubDistricts && z.Districts == request.Districts)
            .GroupBy(p => p.ZipCode).ToList();

            var GetZipcodeList = GetZipcode.Select(g => g.Key).FirstOrDefault();
            return Ok(GetZipcodeList);
        }



        [HttpGet]
        public ActionResult GetOrderType()
        {
            //  var GetOrderTypeLists = _context.TypeOrder
            //.Select(p => new
            //{
            //    TypeRecapFrom = p.TypeRecapFrom,
            //    CodeChar = p.CodeChar
            //})
            //.Distinct()
            //.ToList();
            //  return Ok(GetOrderTypeLists);
            var GetOrderTypeLists = _context.TypeOrder.Select(p => p.TypeRecapShow).ToList();
            return Ok(GetOrderTypeLists);

        }
        //SaveToProductTable
        [HttpPost]
        public ActionResult SaveQuotation([FromBody] YmtgOrderNds dataQuotation)

        {
            if (dataQuotation == null)
            {
                return BadRequest("ข้อมูลไม่ถูกต้อง");
            }
            string NewQuotationNumber = "";

            // Gen เลข
            var MaxQuo = _context.YmtgOrders
    .OrderByDescending(o => o.Id)
    .Select(o => o.QuotationNumber)
    .FirstOrDefault();



            if (MaxQuo != null && MaxQuo.Length >= 9)
            {
                string QuoHead1 = "QUONDS";
                string QuoHead2 = MaxQuo.Substring(6, 2); // Year, last 2 digits
                string QuoHead3 = MaxQuo.Substring(8, 2); // Month
                int nextSequence = int.Parse(MaxQuo.Substring(MaxQuo.Length - 4)) + 1; // Extract last 3 characters and increment by 1
                NewQuotationNumber = QuoHead1 + QuoHead2 + QuoHead3 + nextSequence.ToString("D4");
            }
            else
            {
                // กรณีไม่มีข้อมูลก่อนหน้านี้ กำหนดค่าเริ่มต้น
                string QuoHead1 = "QUONDS";
                string QuoHead2 = DateTime.Now.ToString("yy"); // Year, last 2 digits
                string QuoHead3 = DateTime.Now.ToString("MM"); // Month
                NewQuotationNumber = QuoHead1 + QuoHead2 + QuoHead3 + "0001";
            }

            dataQuotation.QuotationNumber = NewQuotationNumber;





            _context.YmtgOrders.Add(dataQuotation);
            _context.SaveChanges();
            return Ok(dataQuotation);
        }

        // table product

        //    if (Entries != null)
        //    {
        //        foreach (var entry in Entries)
        //        {
        //            var newProduct = new YmtgProductNds
        //            {
        //                QuotationNumber = NewQuotationNumber,
        //                ProductName = entry.SelectedStyleName,
        //                Qty = entry.Quantity,
        //                SKUCode = !string.IsNullOrEmpty(entry.SelectedSku) && entry.SelectedSku.Length > 3
        //                    ? entry.SelectedSku.Substring(0, entry.SelectedSku.Length - 3)
        //                    : null,
        //                Size = entry.SelectedSize,
        //                Color = entry.SelectedColor,
        //                Price = entry.Quantity * entry.PricePerUnit,
        //                PrintingType = !string.IsNullOrEmpty(entry.SelectedSku) && entry.SelectedSku.Length > 3
        //            ? entry.SelectedSku.Substring(entry.SelectedSku.Length - 3)
        //            : null,
        //                CreateBy = "ADMIN",
        //                CreateDate = DateTime.Now
        //            };

        //            _context.YmtgProducts.Add(newProduct);
        //        }

        //        _context.SaveChanges();
        //    }




        //    // save ลง table product



        //    return Ok();
        //}

        //แก้ไข Model
        [HttpPost]
        public ActionResult SaveToProductTable([FromBody] List<ProductList> Entries)

        {

            if (Entries != null)
            {
                foreach (var entry in Entries)
                {
                    var newProduct = new YmtgProductNds
                    {
                        QuotationNumber = entry.QuotationNumber,
                        ProductName = entry.SelectedStyleName,
                        Qty = entry.Quantity,
                        SKUCode = "",
                        SKUCodeFull = entry.SelectedSku,
                        Size = entry.SelectedSize,
                        Color = entry.SelectedColor,
                        Price = entry.Quantity * entry.PricePerUnit,
                        PrintingType = 0,
                        CreateBy = "ADMIN",
                        CreateDate = DateTime.Now
                    };

                    // ตรวจสอบและกำหนดค่า SKUCode
                    if (!string.IsNullOrEmpty(entry.SelectedSku) && entry.SelectedSku.Length > 3)
                    {
                        newProduct.SKUCode = entry.SelectedSku.Substring(0, entry.SelectedSku.Length - 3);
                    }

                    // ตรวจสอบและกำหนดค่า PrintingType
                    if (!string.IsNullOrEmpty(entry.SelectedSku) && entry.SelectedSku.Length > 3)
                    {

                            // ดึงสามอักขระสุดท้าย
                            string lastThreeChars = entry.SelectedSku.Substring(entry.SelectedSku.Length - 3);

                            // พยายามแปลงสตริงเป็นจำนวนเต็ม
                            if (int.TryParse(lastThreeChars, out int printingType))
                            {
                                newProduct.PrintingType = printingType;
                            }
                            else
                            {
                               //แปลงค่่าไม่สำเร็จ
                                newProduct.PrintingType = 0; // หรือค่าที่เหมาะสมตามบริบท
                            }

                        //newProduct.PrintingType = entry.SelectedSku.Substring(entry.SelectedSku.Length - 3);
                    }
                    _context.YmtgProducts.Add(newProduct);
                }

                _context.SaveChanges();
            }
            return Ok(Entries);
        }


        [HttpPost]
        public ActionResult GetSkuCode([FromBody] MasterStyle style)
        {
            if (style == null || string.IsNullOrWhiteSpace(style.StyleCode))
            {
                return BadRequest("StyleCode is required.");
            }
            var skuCodes = _context.MasterStyles
                .Where(z => z.StyleCode == style.StyleCode)
                .GroupBy(p => p.Style) // Column ที่เรา Select
                .Select(g => g.Key)
                .FirstOrDefault();

            if (skuCodes == null || !skuCodes.Any())
            {
                return NotFound("No matching SKU Codes found.");
            }
            return Ok(skuCodes);
        }


        [HttpPost]
        public ActionResult GetdataQuo()
        {
            
            var dataquo = _context.YmtgOrders.Where(a => a.QuoCancel == 0).ToList();
 
            return Ok(dataquo);
        }
        // For front end
        [HttpGet]
        public JsonResult GetdataQuos()
        {
            var dataquo = _context.YmtgOrders.Select(e => new
            {
                e.Id,
                e.QuotationNumber,
                e.QuoType,
                e.CustomerName,
                e.QuoLastname,
                e.CreateDate

            }).ToList();

            return Json(new { data = dataquo });

        }


        [HttpPost]
        public ActionResult GetdataQuoForEdit([FromBody] QuotationViewModel request)
        {

            var dataquoEditOrder = _context.YmtgOrders.Where(z => z.QuotationNumber == request.QuotationNumber)
                .FirstOrDefault();
     

            return Ok(dataquoEditOrder);
        }
        

        [HttpPost]
        public ActionResult GetForEditProduct([FromBody] QuotationViewModel searchQuo)
        {

       
            var dataquoEditProduct = _context.YmtgProducts.Where(z => z.QuotationNumber == searchQuo.QuotationNumber).ToList();

            return Ok(dataquoEditProduct);
        }




        //Update data
        [HttpPost]
        public ActionResult UpdateQuotation([FromBody] QuotationUpdateModel updateModel)
        {
            if (updateModel == null)
            {
                return BadRequest("Invalid data.");
            }

            // ตรวจสอบและดึงข้อมูล Order เดิมจากฐานข้อมูล
            var existingOrder = _context.YmtgOrders.FirstOrDefault(q => q.QuotationNumber == updateModel.QuotationNumber);
            if (existingOrder == null)
            {
                return NotFound("Quotation not found.");
            }

            // อัปเดตข้อมูลใน Order
            existingOrder.CustomerName = updateModel.CustomerName;
            existingOrder.OrderDate = updateModel.OrderDate;
            existingOrder.ShipDate = updateModel.ShipDate; // Map
            existingOrder.TotalQty = updateModel.TotalQty;
            existingOrder.TotalPrice = updateModel.TotalPrice;
            existingOrder.QuoRemark = updateModel.Remark;
            existingOrder.CustomerAddress = updateModel.CustomerAddress;
            existingOrder.CustomerPhone = updateModel.CustomerPhone;
            existingOrder.QuoProvince = updateModel.QuoProvince;
            existingOrder.QuoDistricts = updateModel.QuoDistricts;
            existingOrder.QuoSubDistricts = updateModel.QuoSubDistricts;
            existingOrder.QuoZipCode = updateModel.QuoZipCode;
            existingOrder.CreateBy = "ADMIN"; // ระบุผู้แก้ไขล่าสุด
            existingOrder.QuoCompanyName = updateModel.QuoCompanyName;
            existingOrder.QuoLastname = updateModel.QuoLastname;
            existingOrder.QuoTaxID = updateModel.QuoTaxID;
            existingOrder.CustomerEmail = updateModel.CustomerEmail;
            existingOrder.QuoType = updateModel.QuoType;
            existingOrder.QuoLastUpdate = DateTime.Now;
            existingOrder.QuoShippingPrice = updateModel.QuoShippingPrice;
            existingOrder.QuoStatus = updateModel.QuoStatus; // Map


            //TaxID / Email
            // บันทึกการเปลี่ยนแปลง
            _context.SaveChanges();


            // ลบข้อมูลสินค้าเก่าทั้งหมดสำหรับ QuotationNumber
            var existingProducts = _context.YmtgProducts
                .Where(p => p.QuotationNumber == updateModel.QuotationNumber)
                .ToList();
            _context.YmtgProducts.RemoveRange(existingProducts);




            //  YmtgProducts 
            if (updateModel.Entries != null)
            {
                foreach (var entry in updateModel.Entries)
                {
                    var newProduct = new YmtgProductNds
                    {
                        QuotationNumber = updateModel.QuotationNumber,
                        ProductName = entry.ProductName,
                        Qty = entry.Qty,
                        SKUCode = "",
                        SKUCodeFull = entry.SKUCodeFull,
                        Size = entry.Size,
                        Color = entry.Color,
                        Price = entry.Price,
                        PrintingType = 0,
                        CreateBy = "ADMIN",
                        CreateDate = DateTime.Now
                    };

                    // ตัดสามตัวหลังออกจาก SKUCode
                    if (!string.IsNullOrEmpty(entry.SKUCodeFull) && entry.SKUCodeFull.Length > 3)
                    {
                        newProduct.SKUCode = entry.SKUCodeFull.Substring(0, entry.SKUCodeFull.Length - 3);
                    }


                    // ตรวจสอบและกำหนดค่า PrintingType
                    if (!string.IsNullOrEmpty(entry.Sku) && entry.Sku.Length > 3)
                    {
                        string lastThreeChars = entry.Sku.Substring(entry.Sku.Length - 3);
                        if (int.TryParse(lastThreeChars, out int printingType))
                        {
                            newProduct.PrintingType = printingType;
                        }
                    }

                    _context.YmtgProducts.Add(newProduct);
                }

                _context.SaveChanges();
            }

            return Ok("Quotation and products updated successfully.");
        }

        [HttpGet]
        public ActionResult GetLoadRemark()
        {
            var GetLoadRemark = _context.YmtgRemark.Select(k=> k.RemarkQuo).ToList();
         
            return Ok(GetLoadRemark);
        }

        [HttpGet]
        public IActionResult GetQuotations()
        {
            var quotations = _context.YmtgOrders.Select(q => new
            {
                q.QuotationNumber,
                q.QuoStatus,
                q.QuoType,
                q.CustomerName,
                q.QuoLastname,
                q.CreateDate,
                q.QuoCancel
            }).Where(a => a.QuoCancel == 0).ToList();
            return Json(new { data = quotations });
            //return Json(quotations);
        }

        //Update data
        [HttpPost]
        public ActionResult DeleteQuo([FromBody] QuotationUpdateModel quoNumber)
        {
            if (quoNumber == null)
            {
                return BadRequest("Invalid data.");
            }

            
            var existingOrder = _context.YmtgOrders.FirstOrDefault(q => q.QuotationNumber == quoNumber.QuotationNumber);
            if (existingOrder == null)
            {
                return NotFound("Quotation not found.");
            }

          
            existingOrder.QuoCancel = 1;
  
            existingOrder.QuoLastUpdate = DateTime.Now;

        
            _context.SaveChanges();

            return Ok(new { quoNumber.QuotationNumber });

        }

        //Load ข้อมูล Order
        [HttpGet]
        public ActionResult GetLoadOrderInfomation()
        {

            var GetLoadRemark = _context.YmtgRemark.Select(k => k.RemarkQuo).ToList();

            return Ok(GetLoadRemark);

        }


        // File

        [HttpPost]
        public IActionResult UploadFile(IFormFile file, [FromForm] string fileDescription, [FromForm] string quotationNumber)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            if (string.IsNullOrEmpty(quotationNumber))
            {
                return BadRequest("Quotation number is required.");
            }

            if (string.IsNullOrEmpty(fileDescription))
            {
                fileDescription = "";

            }
            // สร้างโฟลเดอร์สำหรับ Quotation Number
            var quotationFolderPath = Path.Combine(_uploadPath, quotationNumber);
            if (!Directory.Exists(quotationFolderPath))
            {
                Directory.CreateDirectory(quotationFolderPath);
            }

            // บันทึกไฟล์ลงในโฟลเดอร์
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(quotationFolderPath, fileName);


            //Check ชื่อไฟล์ซ้ำ
            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileExtension = Path.GetExtension(fileName);
                fileName = $"{fileNameWithoutExtension}_{counter++}{fileExtension}";
                filePath = Path.Combine(quotationFolderPath, fileName);
            }



            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // บันทึกข้อมูลลงฐานข้อมูล
            var newFile = new QuotationFile
            {
                QuotationNumber = quotationNumber,
                FileName = fileName,
                FilePath = Path.Combine("Uploads", quotationNumber, fileName), // เก็บ Path แบบ Relative
                FileDescription = fileDescription,
                CreatedAt = DateTime.Now
            };
            _context.QuotationFiles.Add(newFile);
            _context.SaveChanges();

            //return Ok(newFile);
            return Json(new { data = newFile });
        }

        [HttpGet]
        public IActionResult DownloadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest("File path is required.");
            }
            // ลบคำว่า "Uploads" ออกจาก filePath ถ้ามี
            if (filePath.StartsWith("Uploads", StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring("Uploads".Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            // รวมเส้นทางไฟล์
            var absolutePath = Path.Combine(_uploadPath, filePath);

            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound("File not found.");
            }


            var fileName = Path.GetFileName(absolutePath);
            return PhysicalFile(absolutePath, "application/octet-stream", fileName);
        }

        [HttpDelete]
        public IActionResult DeleteFile(string filePath, string quotationNumber)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(quotationNumber))
            {
                return BadRequest("File path and Quotation number are required.");
            }

            // ลบคำว่า "Uploads" ออกจาก filePath ถ้ามี
            if (filePath.StartsWith("Uploads", StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring("Uploads".Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            var absolutePath = Path.Combine(_uploadPath, filePath);
            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }

            var formattedFilePath = "Uploads\\" + filePath;
            // ลบข้อมูลจากฐานข้อมูล
            var fileRecord = _context.QuotationFiles
                .FirstOrDefault(f => f.FilePath == formattedFilePath && f.QuotationNumber == quotationNumber);
            if (fileRecord != null)
            {
                _context.QuotationFiles.Remove(fileRecord);
                _context.SaveChanges();
            }

            return Ok("File deleted successfully.");
        }


        [HttpGet]
        public IActionResult GetQuotationFiles(string quotationNumber)
            
        {
            if (string.IsNullOrEmpty(quotationNumber))
            {
                return BadRequest("Quotation number is required.");
            }

            // ดึงข้อมูลจากฐานข้อมูลที่เกี่ยวข้องกับ QuotationNumber
            var files = _context.QuotationFiles
                .Where(f => f.QuotationNumber == quotationNumber)
                .Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.FilePath,
                    f.FileDescription,
                    CreatedAt = f.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToList();

            return Ok(files);

        }


        //Update Status Quoattion Confirm
        [HttpPost]
        public ActionResult UpdateQuoStatus([FromBody] QuotationUpdateModel quoNumber)
        {
            if (quoNumber == null)
            {
                return BadRequest("Invalid data.");
            }


            var existingOrder = _context.YmtgOrders.FirstOrDefault(q => q.QuotationNumber == quoNumber.QuotationNumber);
            if (existingOrder == null)
            {
                return NotFound("Quotation not found.");
            }


            existingOrder.QuoStatus = 1;

            existingOrder.QuoLastUpdate = DateTime.Now;

            //existingOrder.ShipDate = "";


           _context.SaveChanges();

            return Ok(new { quoNumber.QuotationNumber });

        }


    }

}
