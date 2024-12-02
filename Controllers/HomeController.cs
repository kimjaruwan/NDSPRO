using Microsoft.AspNetCore.Mvc;
using NDSPRO.Models;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using NDSPRO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json.Linq;
using Azure.Core;
using System.Drawing.Drawing2D;

namespace NDSPRO.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
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
            var GetOrderTypeLists = _context.TypeOrder
          .Select(p => new
          {
              TypeRecapFrom = p.TypeRecapFrom,
              CodeChar = p.CodeChar
          })
          .Distinct()
          .ToList();
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
                                // จัดการกรณีที่การแปลงล้มเหลว
                                // เช่น กำหนดค่าเริ่มต้นหรือแจ้งเตือนผู้ใช้
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
            var dataquo = _context.YmtgOrders.Where(z=>z.QuotationNumber == "QUONDS24110033").ToList();
            return Ok(dataquo);
        }

        

    }

}
