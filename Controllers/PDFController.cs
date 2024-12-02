using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.EntityFrameworkCore;
using NDSPRO.Models;
using NDSPRO.Data;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server;
using iText.Layout.Borders;

namespace NDSPRO.Controllers
{   

    public class PDFController : Controller
    {

        private readonly ILogger<PDFController> _logger;
        private readonly ApplicationDbContext _context;

        public PDFController(ILogger<PDFController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PrintPDF(string quotationNumber)
        {
           
            var FontLinkBold = @"wwwroot/fonts/THSarabunPSK/THSarabun Bold.ttf";
            var FontLinkNormal = @"wwwroot/fonts/THSarabunPSK/THSarabun.ttf";
            //var LogoLink = @"wwwroot/images/logo-nds.png";
            string logoLink = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo-nds.png");
 
            var GetProductTable = new List<YmtgProductNds>();
            var GetOrderTable = new YmtgOrderNds();

            if (quotationNumber != null && quotationNumber != "")
            {
                GetOrderTable = _context.YmtgOrders.Where(z => z.QuotationNumber == quotationNumber).FirstOrDefault();

                GetProductTable = _context.YmtgProducts.Where(z => z.QuotationNumber == quotationNumber).ToList();
            }


            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // Create a new PDF document
                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter.GetInstance(document, stream);
                    document.Open();


                    // Header with Image and Text
                    PdfPTable headerTable = new PdfPTable(2);
                    headerTable.WidthPercentage = 100;
                    float[] columnWidths = { 1f, 3f };
                    headerTable.SetWidths(columnWidths);

                    // Adding Image
                    //string imagePath = LogoLink; // Adjust the path as needed
                    //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imagePath);
                    // เพิ่มรูปใน PDF
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoLink);
                    logo.ScaleToFit(80f, 110f);
                    logo.Alignment = Element.ALIGN_LEFT;

                    PdfPCell imageCell = new PdfPCell(logo);
                    imageCell.Border = Rectangle.NO_BORDER;
                    imageCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerTable.AddCell(imageCell);

                    // Adding Company Information Text without Table
                    // Company Name with larger font
                    Paragraph companyName = new Paragraph(
                        "บริษัท โนเบิล ดิสทริบิวชั่นเซอรวิ์สเซส(ไทย) จํากัด\n",
                        new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 25, Font.NORMAL));
                  
                    companyName.Alignment = Element.ALIGN_LEFT;

                    document.Add(new Paragraph("\n")); // เว้นบรรทัด 

                    // Company Address and Contact Information with smaller font
                    Paragraph companyDetails = new Paragraph(
                        "47/403 ถนนป๊อปปูล่า ตําบลบ้านใหม่ อําเภอปากเกร็ด จ.นนทบุรี 11120\n" +
                        "โทร : 024291302    อีเมล์ : Phitsukan@yehpattana.com\n" +
                        "เลขที่ประจําตัวผู้เสียภาษีอากร : 0125561013962",
                        new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL));
                    companyDetails.Alignment = Element.ALIGN_LEFT;
                    companyDetails.Leading = 15f; // ระยะห่างระหว่างบรรทัด


                    PdfPCell companyInfoCell = new PdfPCell();
                    companyInfoCell.AddElement(companyName);
                    companyInfoCell.AddElement(companyDetails);
                    companyInfoCell.Border = Rectangle.NO_BORDER;
                    companyInfoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerTable.AddCell(companyInfoCell);

                    document.Add(headerTable);

                    document.Add(new Paragraph("\n")); // เว้นบรรทัด 

                    // Text : ใบเสนอราคา (Quotation)
                    // สร้างฟอนต์สำหรับข้อความ
                    BaseFont baseFont = BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font fonthead = new Font(baseFont, 25, Font.NORMAL);
                    BaseFont baseFontDe = BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font fontDe = new Font(baseFontDe, 16, Font.NORMAL);

                    //สร้างตารางที่มี 3 Column
                    PdfPTable tableHeadTextQuotation = new PdfPTable(3);
                    tableHeadTextQuotation.WidthPercentage = 100; // กำหนดความกว้างของตารางเป็น 100%
                    float[] TextQuotationWidths = { 1f, 3f, 1f };
                    tableHeadTextQuotation.SetWidths(TextQuotationWidths);


                    //เพิ่มข้อมูล
                    PdfPCell cell1 = new PdfPCell(new Phrase(""));
                    cell1.Border = PdfPCell.NO_BORDER;

                    tableHeadTextQuotation.AddCell(cell1);


                    //// สร้างเซลล์และเพิ่มข้อความลงในเซลล์
                    //PdfPCell cell2 = new PdfPCell(new Phrase("ใบเสนอราคา (Quotation)\n", font))
                    //{
                    //    HorizontalAlignment = Element.ALIGN_CENTER, // จัดข้อความกึ่งกลาง
                    //    Border = PdfPCell.BOX, // กำหนดเส้นขอบรอบเซลล์
                    //    BorderWidth = 1f, //กำหนดความหนาของเส้นขอบ
                    //    Padding = 5f // กำหนดระยะห่างระหว่างข้อความกับเส้นขอบ
                    //};

                    Phrase phrase = new Phrase();
                    phrase.Add(new Chunk("ใบเสนอราคา (Quotation)",fonthead));
                    phrase.Add(Chunk.Newline);
                    phrase.Add(Chunk.Newline);
                   

                    // สร้าง PdfPCell และเพิ่ม Phrase ลงในเซลล์
                    PdfPCell cell2 = new PdfPCell(phrase)
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = PdfPCell.BOX,
                        BorderWidth = 1f,
                        Padding = 5f
                    };


                    //// เพิ่มเซลล์ลงในตาราง
                    tableHeadTextQuotation.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase(""));
                    cell3.Border = PdfPCell.NO_BORDER;

                    tableHeadTextQuotation.AddCell(cell3);

                    // เพิ่มตารางลงในเอกสาร
                    document.Add(tableHeadTextQuotation);


                    //สร้างตารางที่มี 3 Column - ชื่อลูกค้า ที่อยู่ เลขที่ใบเสนอราคา
                    PdfPTable tableHeadDetail = new PdfPTable(2);
                    tableHeadDetail.WidthPercentage = 100; // กำหนดความกว้างของตารางเป็น 100%
                    float[] DetailWidths = { 1f, 1f};
                    tableHeadDetail.SetWidths(DetailWidths);


                    //---เลขที่ใบเสนอราคา
                    Paragraph LabelNum = new Paragraph("เลขที่ใบเสนอราคา : " + GetOrderTable.QuotationNumber,
            new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL));
                    LabelNum.Alignment = Element.ALIGN_LEFT;

                    PdfPCell cellD1 = new PdfPCell();
                    cellD1.Border = PdfPCell.NO_BORDER;
                    cellD1.AddElement(LabelNum);
                    tableHeadDetail.AddCell(cellD1);
                    //---เลขที่ใบเสนอราคา

                    var Customer = ""; 
                    Customer = GetOrderTable.CustomerName;

                    if (Customer == null || Customer.Length == 0) {
                        Customer = GetOrderTable.QuoCompanyName;

                    } else Customer = GetOrderTable.CustomerName + " " +GetOrderTable.QuoLastname;

                    //----ชื่อลูกค้า
                    Paragraph LableCust = new Paragraph("ชื่อลูกค้า : " + Customer,
           new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL));
                    LableCust.Alignment = Element.ALIGN_LEFT;

                    PdfPCell cellD2 = new PdfPCell();
                    cellD2.Border = PdfPCell.NO_BORDER;
                    cellD2.AddElement(LableCust);
                    tableHeadDetail.AddCell(cellD2);
                 
                    //----ชื่อลูกค้า

                    //document.Add(tableHeadDetail);
                    //document.Add(new Paragraph("\n"));

                    // เพิ่ม Cell  3 เป็นค่าว่าง-----

                    PdfPCell cellD3 = new PdfPCell(new Phrase(""));
                    cellD3.Border = PdfPCell.NO_BORDER;

                    tableHeadDetail.AddCell(cellD3);
                    // เพิ่ม Cell  3 เป็นค่าว่าง-----



                    //----ที่อยู่ลูกค้า

                    Paragraph LableAddress = new Paragraph("ที่อยู่ : " + GetOrderTable.CustomerAddress + "\n" +
                         "ตำบล " + GetOrderTable.QuoSubDistricts + "  อำเภอ " + GetOrderTable.QuoDistricts + "\n" +
                        "จังหวัด " + GetOrderTable.QuoProvince + " " + GetOrderTable.QuoZipCode + "\n", fontDe
           );
                    // กำหนดค่า leading
                    LableAddress.Leading = 15f; // ระยะห่างระหว่างบรรทัด
                    LableAddress.Alignment = Element.ALIGN_LEFT;

                    PdfPCell cellD4 = new PdfPCell();
                    cellD4.Border = PdfPCell.NO_BORDER;
                    cellD4.AddElement(LableAddress);
                    tableHeadDetail.AddCell(cellD4);

                    //----ที่อยู่ลูกค้า

                    document.Add(tableHeadDetail);
                    //document.Add(new Paragraph("\n"));

                    Paragraph TaxID = new Paragraph("เลขที่ประจำตัวผู้เสียภาษีอากร : " + GetOrderTable.QuoTaxID, fontDe
                        );
                    
                    document.Add(TaxID);
                    document.Add(new Paragraph("\n"));

                    //document.Add(tableHeadDetail);

                    // Table of items (Full Width Alignment) -- ตารางที่มีรายการสินค้า
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1, 4, 2, 2, 2 });

                    // Table headers
                    PdfPCell[] headers = new PdfPCell[]
                    {
                        new PdfPCell(new Phrase("ลําดับ \n\n", fontDe)),
                        new PdfPCell(new Phrase("รายการสินค้า \n\n", fontDe)),
                        new PdfPCell(new Phrase("จำนวน (PCS.)\n\n", fontDe)),
                        new PdfPCell(new Phrase("ราคา/หน่วย (บาท)\n\n", fontDe)),
                        new PdfPCell(new Phrase("จำนวนเงิน (บาท)\n\n", fontDe))
                    };
                    foreach (var headerCell in headers)
                    {
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        headerCell.BackgroundColor = BaseColor.LightGray;
                        table.AddCell(headerCell);
                    }


                    // Add rows for each product
                    int index = 1; // Start the index for "ลำดับ"
                    foreach (var product in GetProductTable)
                    {
                        // ลำดับ
                        table.AddCell(new PdfPCell(new Phrase(index.ToString(), new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });

                        // รายการสินค้า
                        table.AddCell(new PdfPCell(new Phrase(product.ProductName, new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT
                        });

                        // จำนวน
                        table.AddCell(new PdfPCell(new Phrase($"{product.Qty}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });

                        // ราคา/หน่วย
                        decimal pricePerUnit = product.Qty > 0 ? product.Price / product.Qty : 0;
                        table.AddCell(new PdfPCell(new Phrase($"{pricePerUnit.ToString("N2")}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        });

                        // จำนวนเงิน
                        table.AddCell(new PdfPCell(new Phrase($"{product.Price.ToString("N2")}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        });

                        index++; // Increment the index
                    }

                    String Test = "";

                    document.Add(table);

                    document.Add(new Paragraph("\n"));

                    //คำนวนราคารวม-----------------------------------
                    
                    decimal totalAmountValue = 0;

                    // คำนวณยอดรวมจากข้อมูลใน GetProductTable
                    foreach (var product in GetProductTable)
                    {
                        totalAmountValue += product.Price;
                    }

                    var rateTest = "TESTTTTTTT";
                    // คำนวณ VAT 7%
                    decimal vat = totalAmountValue * 0.07m;

                    // คำนวณรวมราคาทั้งสิ้น
                    decimal grandTotal = totalAmountValue + vat;



                    // Total amount (Right Alignment)
                    Paragraph totalAmount = new Paragraph("รวมเงิน : "+ totalAmountValue.ToString("N2") + "\n" +
                                                          "Vat 7% : "+ vat.ToString("N2") + "\n" +
                                                          "รวมราคาทั้งสิ้น : " + grandTotal.ToString("N2") + "",
                                                          new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16));
                    totalAmount.Alignment = Element.ALIGN_RIGHT;
                    document.Add(totalAmount);

                    document.Add(new Paragraph("\n"));

                    // หมายเหตุ-------- 
                    Paragraph Remark = new Paragraph("หมายเหตุ \n" +
                                                       "หลังจากยืนยัน PO ลูกค้าชำระเงิน 50% ของราคาสินค้า และชำระอีก 50% ก่อนส่งสินค้า\n" +
                                                       "ธนาคารไทยพาณิชย์ จำกัด (มหาชน) ชื่อบัญชี NOBLE DESTRIBUTION S เลขที่บัญชี 278-227978-1",
                                                       new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16));
                    totalAmount.Alignment = Element.ALIGN_LEFT;
                    document.Add(Remark);

                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("\n"));
                    //สร้างตารางที่มี 3 Column *Footer*
                    PdfPTable tableFooter = new PdfPTable(3);
                    tableFooter.WidthPercentage = 100; // กำหนดความกว้างของตารางเป็น 100%
                    float[] tableFooterW = { 1f, 1f, 1f };
                    tableFooter.SetWidths(tableFooterW);


                    //เพิ่มข้อมูล cellFooter1
                    PdfPCell cellFooter1 = new PdfPCell(new Phrase("สั่งซื้อโดย/Order By \n" +
                        "_______________ \n" +
                        "(_______________) \n" +
                        "วันที่___/___/___",fontDe));
                    cellFooter1.Border = PdfPCell.NO_BORDER;

                    cellFooter1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cellFooter3.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableFooter.AddCell(cellFooter1);

                    //เพิ่มข้อมูล cellFooter2
                    PdfPCell cellFooter2 = new PdfPCell(new Phrase("ออกโดย/Prepared By \n" +
                        "" + GetOrderTable.CreateBy +" \n" +
                        "( "+ GetOrderTable.CreateBy +" ) \n" +
                        "วันที่ " + DateTime.Now.Date.ToString("dd/MM/yyyy"), fontDe));
                    cellFooter2.Border = PdfPCell.NO_BORDER;

                    cellFooter2.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cellFooter3.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableFooter.AddCell(cellFooter2);



                    //เพิ่มข้อมูล cellFooter3
                    PdfPCell cellFooter3 = new PdfPCell(new Phrase("ผู้มีอำนาจอนุมัติ/Authorized \n" +
                      "_______________ \n" +
                        "(_______________) \n" +
                        "วันที่___/___/___", fontDe));
                    cellFooter3.Border = PdfPCell.NO_BORDER;

                    // จัดข้อความให้อยู่กึ่งกลางทั้งแนวตั้งและแนวนอน
                    cellFooter3.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cellFooter3.VerticalAlignment = Element.ALIGN_MIDDLE;

                    tableFooter.AddCell(cellFooter3);

                    // Footer (Left Alignment)
                    //Paragraph footer = new Paragraph("ออกโดย:" +
                    //                                 "วันที่: 11/09/2024",
                    //                                 new Font(BaseFont.CreateFont(@"c:/windows/fonts/TAHOMA.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 12));
                    //footer.Alignment = Element.ALIGN_LEFT;
                    document.Add(tableFooter);
               
                    document.Close();

                    byte[] bytes = stream.ToArray();
                    return File(bytes, "application/pdf", GetOrderTable.QuotationNumber+".pdf");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error generating PDF: " + ex.Message);
            }
        }
    }
}
