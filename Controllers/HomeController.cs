using Microsoft.AspNetCore.Mvc;
using NDSPRO.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using NDSPRO.Data;

using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using static System.Net.WebRequestMethods;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
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

            // ���ҧ������ Upload ����ѧ�����
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }


        public IActionResult Index()
        {
            //���������˹���á
            return View();
        }


        public IActionResult MainLinkIndex()
        {
            //���������˹���á
            return View();
        }
        public IActionResult OrderInformation()
        {
            //���������˹���á
            return View();
        }


        public IActionResult ViewPage(string quotationNumber)
        {
            // ViewPage read only
            var model = new QuotationViewModel
            {
                QuotationNumber = quotationNumber
            };

            return View(model);
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
                return BadRequest("���������١��ͧ");
            }
            string NewQuotationNumber = "";

            // Gen �Ţ
            var MaxQuo = _context.YmtgOrders
    .OrderByDescending(o => o.Id)
    .Select(o => o.QuotationNumber)
    .FirstOrDefault();



            if (MaxQuo != null && MaxQuo.Length >= 9)
            {
                string QuoHead1 = "QUONDS";
                string QuoHead2 = DateTime.Now.ToString("yy"); // Year
                string QuoHead3 = DateTime.Now.ToString("MM");  // Month// Month
                int nextSequence = int.Parse(MaxQuo.Substring(MaxQuo.Length - 4)) + 1; // Extract last 3 characters and increment by 1
                NewQuotationNumber = QuoHead1 + QuoHead2 + QuoHead3 + nextSequence.ToString("D4");
            }
            else
            {
                // �ó�����բ����š�͹˹�ҹ�� ��˹�����������
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




        //    // save ŧ table product



        //    return Ok();
        //}

        //��� Model
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

                    // ��Ǩ�ͺ��С�˹���� SKUCode
                    if (!string.IsNullOrEmpty(entry.SelectedSku) && entry.SelectedSku.Length > 3)
                    {
                        newProduct.SKUCode = entry.SelectedSku.Substring(0, entry.SelectedSku.Length - 3);
                    }

                    // ��Ǩ�ͺ��С�˹���� PrintingType
                    if (!string.IsNullOrEmpty(entry.SelectedSku) && entry.SelectedSku.Length > 3)
                    {

                            // �֧����ѡ����ش����
                            string lastThreeChars = entry.SelectedSku.Substring(entry.SelectedSku.Length - 3);

                            // �������ŧʵ�ԧ�繨ӹǹ���
                            if (int.TryParse(lastThreeChars, out int printingType))
                            {
                                newProduct.PrintingType = printingType;
                            }
                            else
                            {
                               //�ŧ������������
                                newProduct.PrintingType = 0; // ���ͤ�ҷ��������������Ժ�
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
                .Where(z => z.Description == style.StyleCode)
                .GroupBy(p => p.Style) // Column ������ Select
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
            
            var dataquo = _context.YmtgOrders
            .Where(a => a.QuoCancel == 0) 
            .OrderByDescending(a => a.Id) // ���§�ӴѺ Id �ҡ�ҡ仹���
            .ToList(); 

            return Ok(dataquo);
        }
        // For front end
        [HttpGet]
        public JsonResult GetdataQuos()
        {
            var dataquo = _context.YmtgOrders
            .OrderByDescending(e => e.Id) // ���§�ӴѺ Id �ҡ�ҡ仹���
            .Select(e => new
         {
             e.Id,
             e.QuotationNumber,
             e.QuoType,
             e.CustomerName,
             e.QuoLastname,
             e.CreateDate
         })
         .ToList();

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

            // ��Ǩ�ͺ��д֧������ Order ����ҡ�ҹ������
            var existingOrder = _context.YmtgOrders.FirstOrDefault(q => q.QuotationNumber == updateModel.QuotationNumber);
            if (existingOrder == null)
            {
                return NotFound("Quotation not found.");
            }

            // �ѻവ������� Order
            existingOrder.CustomerName = updateModel.CustomerName;
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
            existingOrder.CreateBy = "ADMIN"; // �кؼ���������ش
            existingOrder.QuoCompanyName = updateModel.QuoCompanyName;
            existingOrder.QuoLastname = updateModel.QuoLastname;
            existingOrder.QuoTaxID = updateModel.QuoTaxID;
            existingOrder.CustomerEmail = updateModel.CustomerEmail;
            existingOrder.QuoType = updateModel.QuoType;
            existingOrder.QuoLastUpdate = DateTime.Now;
            existingOrder.QuoShippingPrice = updateModel.QuoShippingPrice;
            existingOrder.QuoStatus = updateModel.QuoStatus; // Map



            var genNewOrdernumber = "";

            var MaxQuo = _context.OrderModel
            .Where(o => o.OrderNumber.Length > 4 && o.OrderNumber.Substring(3, 1) == "M") // ��Ǩ�ͺ��ҵ�Ƿ�� 4 �� 'M'
            .OrderByDescending(o => o.OrderNumber) // ���§�ӴѺ�ҡ�ҡ仹���
            .Select(o => o.OrderNumber) // ���͡੾�� Remark
            .FirstOrDefault(); // �֧��ҷ���ҡ����ش


            if (MaxQuo != null && MaxQuo.Length >= 9)
            {
                string QuoHead1 = "NDSM";
                string QuoHead2 = DateTime.Now.ToString("yy"); // Year
                string QuoHead3 = DateTime.Now.ToString("MM");  // Month
                int nextSequence = int.Parse(MaxQuo.Substring(MaxQuo.Length - 4)) + 1; // Extract last 3 characters and increment by 1
                genNewOrdernumber = QuoHead1 + QuoHead2 + QuoHead3 + nextSequence.ToString("D4");
            }
            else
            {
                // �ó�����բ����š�͹˹�ҹ�� ��˹�����������
                string QuoHead1 = "NDSM";
                string QuoHead2 = DateTime.Now.ToString("yy"); // Year, last 2 digits
                string QuoHead3 = DateTime.Now.ToString("MM"); // Month
                genNewOrdernumber = QuoHead1 + QuoHead2 + QuoHead3 + "0001";
            }

            




            // gen Order number
            if (existingOrder.QuoStatus == 1)
            {
                existingOrder.OrderNumber = genNewOrdernumber;
                existingOrder.OrderDate = updateModel.OrderDate;
                //existingOrder.Remark = genNewOrdernumber;
            }

            //TaxID / Email
            // �ѹ�֡�������¹�ŧ
            _context.SaveChanges();

            var custName = "";
            var custAddress = existingOrder.CustomerAddress + " " + existingOrder.QuoProvince +
                " " + existingOrder.QuoDistricts + " " +existingOrder.QuoSubDistricts + " " + 
                existingOrder.QuoZipCode;

            if (string.IsNullOrEmpty(existingOrder.CustomerName) && !string.IsNullOrEmpty(existingOrder.QuoCompanyName))
            {
                custName = existingOrder.QuoCompanyName;
            }
            else if (string.IsNullOrEmpty(existingOrder.QuoCompanyName) && !string.IsNullOrEmpty(existingOrder.CustomerName)) 
            {
                custName = existingOrder.CustomerName + " " + existingOrder.QuoLastname;

            }


            if (existingOrder.QuoStatus == 1)
            {

                var newOrderModel = new OrderModel
                {
                   

                    OrderNumber = genNewOrdernumber,
                    OrderDate = existingOrder.OrderDate,
                    OrderStatus = "processing", 
                    ShipDate = existingOrder.ShipDate,
                    TotalQty = existingOrder.TotalQty,
                    TotalPrice = existingOrder.TotalPrice,
                    CustomerName = custName,
                    CustomerEmail = existingOrder.CustomerEmail,
                    CustomerAddress = custAddress,
                    CustomerAddressTax = existingOrder.QuoTaxID, // �� TaxID �� CustomerAddressTax
                    CustomerPhone = existingOrder.CustomerPhone,
                    Remark = updateModel.QuotationNumber,
                    CreateBy = "ADMIN", // �кؼ����ӡ������������
                    CreateDate = DateTime.Now // �ѹ������ҧ����


                };

                // ����������ŧ� OrderModel
                _context.OrderModel.Add(newOrderModel);

                // �ѹ�֡�������¹�ŧŧ㹰ҹ������
                _context.SaveChanges();

            }
           



            // ź�������Թ�����ҷ���������Ѻ QuotationNumber
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

                    // �Ѵ��������ѧ�͡�ҡ SKUCode
                    if (!string.IsNullOrEmpty(entry.SKUCodeFull) && entry.SKUCodeFull.Length > 3)
                    {
                        newProduct.SKUCode = entry.SKUCodeFull.Substring(0, entry.SKUCodeFull.Length - 3);
                    }


                    // ��Ǩ�ͺ��С�˹���� PrintingType
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

                // Insert into product table

                if (existingOrder.QuoStatus == 1)
                {


                    foreach (var entry in updateModel.Entries)
                    {
                        var newProductm = new ProductModel
                        {
                            OrderNumber = genNewOrdernumber,
                            ProductName = entry.ProductName,
                            Qty = entry.Qty,
                            SKUCode = "",
                            Size = entry.Size,
                            Color = entry.Color,
                            Price = entry.Price,
                            PrintingType = 0,
                            CreateBy = "ADMIN",
                            CreateDate = DateTime.Now
                               //SKUCodeFull = entry.SKUCodeFull,
                        };

                        // �Ѵ��������ѧ�͡�ҡ SKUCode
                        if (!string.IsNullOrEmpty(entry.SKUCodeFull) && entry.SKUCodeFull.Length > 3)
                        {
                            newProductm.SKUCode = entry.SKUCodeFull.Substring(0, entry.SKUCodeFull.Length - 3);
                        }


                        // ��Ǩ�ͺ��С�˹���� PrintingType
                        if (!string.IsNullOrEmpty(entry.Sku) && entry.Sku.Length > 3)
                        {
                            string lastThreeChars = entry.Sku.Substring(entry.Sku.Length - 3);
                            if (int.TryParse(lastThreeChars, out int printingType))
                            {
                                newProductm.PrintingType = printingType;
                            }
                        }

                        _context.ProductModel.Add(newProductm);
                    }

                    _context.SaveChanges();



                }
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

        //Load ������ Order
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
            // ���ҧ����������Ѻ Quotation Number
            var quotationFolderPath = Path.Combine(_uploadPath, quotationNumber);
            if (!Directory.Exists(quotationFolderPath))
            {
                Directory.CreateDirectory(quotationFolderPath);
            }

            // �ѹ�֡���ŧ�������
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(quotationFolderPath, fileName);


            //Check ���������
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

            // �ѹ�֡������ŧ�ҹ������
            var newFile = new QuotationFile
            {
                QuotationNumber = quotationNumber,
                FileName = fileName,
                FilePath = Path.Combine("Uploads", quotationNumber, fileName), // �� Path Ẻ Relative
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
            // ź����� "Uploads" �͡�ҡ filePath �����
            if (filePath.StartsWith("Uploads", StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring("Uploads".Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            // �����鹷ҧ���
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

            // ź����� "Uploads" �͡�ҡ filePath �����
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
            // ź�����Ũҡ�ҹ������
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

            // �֧�����Ũҡ�ҹ�����ŷ������Ǣ�ͧ�Ѻ QuotationNumber
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


        [HttpGet]
        public JsonResult GetOrderInfos()
        {
            var orderData = _context.OrderModel
                .Select(o => new
                {
                    o.OrderNumber,
                    o.OrderDate,
                    o.ShipDate,
                    o.TotalQty,
                    o.OrderStatus,
                    o.CustomerName
                })
                .OrderByDescending(o => o.OrderNumber)
                .ToList();
            //return Ok(orderData);
            return Json(new { data = orderData });
        }

        public IActionResult ViewOrder()
        {
            //˹�Ҵ���������´ Order
            return View();
        }

        public IActionResult ViewAttachments()
        {
            // ˹�� Ṻ���
            return View();
        }

        [HttpGet]
        public JsonResult GetOrderDetails(string orderNumber)
        {
            var order = _context.OrderModel
                .Where(o => o.OrderNumber == orderNumber)
                .Select(o => new
                {
                    o.OrderNumber,
                    OrderDate = o.OrderDate.ToString("dd/MM/yyyy"), // �Ѵ�ٻẺ�ѹ���
                    ShipDate = o.ShipDate.HasValue ? o.ShipDate.Value.ToString("dd/MM/yyyy") : null, // �� null ��͹�Ѵ�ٻẺ
                    o.TotalQty,
                    o.OrderStatus
                })
                .FirstOrDefault();

            return Json(order);
        }

        [HttpGet]
        public JsonResult GetProductOrders(string orderNumber)
        {
            var productOrder = _context.ProductModel
                .Where(o => o.OrderNumber == orderNumber)
                .Select(o => new
                {

                    o.OrderNumber,
                    o.ProductName,
                    o.Size,
                    o.Color,
                    o.Qty
                   
                   
                })
                .ToList();

            return Json(productOrder);
        }

       //Get Quotation file by order num
        [HttpGet]
        public IActionResult GetDataQuoFileTables(string orderNumber)

        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return BadRequest("Order number is required.");
            }

            //�� file Quotation

            var QuoNum = _context.YmtgOrders.Where(z => z.OrderNumber == orderNumber).FirstOrDefault();

            var fileQuos = _context.QuotationFiles
                .Where(f => f.QuotationNumber == QuoNum.QuotationNumber)
                .Select(f => new
                {
                    f.QuotationNumber,
                    f.Id,
                    f.FileName,
                    f.FilePath,
                    f.FileDescription,
                    CreatedAt = f.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")
                })
                .ToList();

            return Ok(fileQuos);

        }



        [HttpPost]
        public IActionResult UploadFileAboutOrders(IFormFile file, [FromForm] string fileDescription, [FromForm] string orderNumber)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            if (string.IsNullOrEmpty(orderNumber))
            {
                return BadRequest("Order number is required.");
            }

            if (string.IsNullOrEmpty(fileDescription))
            {
                fileDescription = "";
            }

            // ���ҧ������ Otherfile �������������Ѻ Order Number
            var otherfileFolderPath = Path.Combine(_uploadPath, "Otherfile", orderNumber);
            if (!Directory.Exists(otherfileFolderPath))
            {
                Directory.CreateDirectory(otherfileFolderPath);
            }

            // �Ѵ��áóժ��������
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(otherfileFolderPath, fileName);

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileExtension = Path.GetExtension(fileName);
                fileName = $"{fileNameWithoutExtension}_{counter++}{fileExtension}";
                filePath = Path.Combine(otherfileFolderPath, fileName);
            }

            // �ѹ�֡���ŧ�������
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            //"dd/MM/yyyy HH:mm:ss"
            // �ѹ�֡���������ŧ�ҹ������
            var newFile = new AttachmentsModel
            {
                OrderNumber = orderNumber,
                FileName = fileName,
                FilePath = Path.Combine("Uploads", "Otherfile", orderNumber, fileName), // �� Path Ẻ Relative
                FileDescription = fileDescription,
                CreatedAt = DateTime.Now,
                AddFileBy = "ADMIN"
            };
            _context.AttachmentsModel.Add(newFile);
            _context.SaveChanges();

            return Json(new { sendFile = newFile });

           
        }

        //GetDataOtherFileTable


        [HttpGet]
        public IActionResult GetDataOtherFileTable(string orderNumber)

        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return BadRequest("Order number is required.");
            }

            //�� file Quotation

            

            var fileOther = _context.AttachmentsModel
                .Where(f => f.OrderNumber == orderNumber)
                .Select(f => new
                {
                 
                    f.Id,
                    f.FileName,
                    f.FilePath,
                    f.FileDescription,
                    CreatedAt = f.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")
                })
                .ToList();

            return Ok(fileOther);

        }

        //DeleteFileAboutOrders

        [HttpDelete]
        public IActionResult DeleteFileAboutOrders(string filePath, string orderNumber)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(orderNumber))
            {
                return BadRequest("File path and Quotation number are required.");
            }

            // ź����� "Uploads" �͡�ҡ filePath �����
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
            // ź�����Ũҡ�ҹ������
            var fileRecord = _context.AttachmentsModel
                .FirstOrDefault(f => f.FilePath == formattedFilePath && f.OrderNumber == orderNumber);
            if (fileRecord != null)
            {
                _context.AttachmentsModel.Remove(fileRecord);
                _context.SaveChanges();
            }

            return Ok("File deleted successfully.");
        }



    }

}
