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


            string remarkText = "";
            string accountInfo = "ธนาคารไทยพาณิชย์ จำกัด (มหาชน) ชื่อบัญชี NOBLE DESTRIBUTION S เลขที่บัญชี 278-227978-1";
            string preparedBy = "";

            //var LogoLink = @"wwwroot/images/logo-nds.png";
            string logoLink = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo-nds.png");

            var GetProductTable = new List<YmtgProductNds>();
            var GetOrderTable = new YmtgOrderNds();
            var GetMasterStyle = new MasterStyle();

            if (quotationNumber != null && quotationNumber != "")
            {
                GetOrderTable = _context.YmtgOrders.Where(z => z.QuotationNumber == quotationNumber).FirstOrDefault();

                GetProductTable = _context.YmtgProducts.Where(z => z.QuotationNumber == quotationNumber).ToList();


                remarkText = GetOrderTable.QuoRemark;               
                preparedBy = GetOrderTable?.CreateBy ?? "Unknown";
                //GetMasterStyle = _context.MasterStyles
              
            }


            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // Create a new PDF document
                    Document document = new Document(PageSize.A4, 25, 25, 5, 55); //เดิม 25, 25, 30, 30
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);

                    //เพิ่มสำหรับ footer 
                    writer.PageEvent = new FooterEvent(remarkText, accountInfo, preparedBy, FontLinkBold, FontLinkNormal);
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
                    logo.ScaleToFit(70f, 100f);
                    logo.Alignment = Element.ALIGN_RIGHT;

                    PdfPCell imageCell = new PdfPCell(logo);
                    imageCell.Border = Rectangle.NO_BORDER;
                    imageCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    imageCell.PaddingTop = 25f; // รูปกับด้านบน
                    headerTable.AddCell(imageCell);

                    // Adding Company Information Text without Table
                    // Company Name with larger font
                    Paragraph companyName = new Paragraph(
                        "บริษัท โนเบิล ดิสทริบิวชั่นเซอรวิ์สเซส(ไทย) จํากัด\n",
                        new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 22, Font.NORMAL));

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
                    headerTable.SpacingBefore = 0f;

                    document.Add(headerTable);

                    document.Add(new Paragraph("\n")); // เว้นบรรทัด 

                    // Text : ใบเสนอราคา (Quotation)
                    // สร้างฟอนต์สำหรับข้อความ
                    BaseFont baseFont = BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font fonthead = new Font(baseFont, 20, Font.NORMAL);
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
                    phrase.Add(new Chunk("ใบเสนอราคา (Quotation)", fonthead));
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
                    tableHeadTextQuotation.SpacingBefore = 0f; //กำหนดระยะห่างจากด้านบน
                    // เพิ่มตารางลงในเอกสาร
                    document.Add(tableHeadTextQuotation);

                    // สร้างตาราง 2 คอลัมน์
                    PdfPTable tableHeadDetail = new PdfPTable(2);
                    tableHeadDetail.WidthPercentage = 100; // ความกว้างตาราง 100%
                    float[] DetailWidths = { 1.5f, 1f }; // ค่าความกว้างของคอลัมน์
                    tableHeadDetail.SetWidths(DetailWidths);
                   
                    // กำหนดค่าตัวแปร
                    var Customer = GetOrderTable.CustomerName + " " + GetOrderTable.QuoLastname;
                    var CompanyCust = GetOrderTable.QuoCompanyName;

                    if (string.IsNullOrEmpty(GetOrderTable.CustomerName) && string.IsNullOrEmpty(GetOrderTable.QuoLastname))
                    {
                        Customer = "-";
                    }

                    if (string.IsNullOrEmpty(CompanyCust))
                    {
                        CompanyCust = "-";
                    }

                    //----ชื่อลูกค้าและชื่อบริษัท (รวมใน Cell ด้านซ้าย)
                    Paragraph LableCustCompany = new Paragraph();
                    LableCustCompany.Add(new Chunk("ชื่อลูกค้า :   ", new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableCustCompany.Add(new Chunk(Customer, new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableCustCompany.Add(Chunk.Newline);
                    LableCustCompany.Add(new Chunk("ชื่อบริษัท :  ", new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableCustCompany.Add(new Chunk(CompanyCust, new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableCustCompany.Leading = 15f;
                    LableCustCompany.Alignment = Element.ALIGN_LEFT;


                    PdfPCell cellLeft1 = new PdfPCell();
                    cellLeft1.Border = PdfPCell.NO_BORDER;
                    cellLeft1.AddElement(LableCustCompany);

                    //----เลขที่ใบเสนอราคา (ด้านขวา)
                    Paragraph LabelQuotationNumber = new Paragraph();
                    LabelQuotationNumber.Add(new Chunk("เลขที่ใบเสนอราคา : ", new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LabelQuotationNumber.Add(new Chunk(GetOrderTable.QuotationNumber, new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LabelQuotationNumber.Alignment = Element.ALIGN_RIGHT;


                    PdfPCell cellRight1 = new PdfPCell();
                    cellRight1.Border = PdfPCell.NO_BORDER;
                    cellRight1.AddElement(LabelQuotationNumber);

                    // เพิ่มแถวแรกในตาราง
                    tableHeadDetail.AddCell(cellLeft1);
                    tableHeadDetail.AddCell(cellRight1);

                    //----ที่อยู่ลูกค้า (เต็มความกว้างของตาราง)
                    //Paragraph LableAddress = new Paragraph();
                    //LableAddress.Add(new Chunk("ที่อยู่ : ", new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    //LableAddress.Add(new Chunk(GetOrderTable.CustomerAddress + "\n", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    //LableAddress.Add(new Chunk("ตำบล " + GetOrderTable.QuoSubDistricts + "  อำเภอ " + GetOrderTable.QuoDistricts + "\n", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    //LableAddress.Add(new Chunk("จังหวัด " + GetOrderTable.QuoProvince + " " + GetOrderTable.QuoZipCode + "\n", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    //LableAddress.Leading = 15f;
                    //LableAddress.Alignment = Element.ALIGN_LEFT;

                    Paragraph LableAddress = new Paragraph();
                    LableAddress.Add(new Chunk("ที่อยู่ :       ", new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableAddress.Add(new Chunk(GetOrderTable.CustomerAddress + " ตำบล " + GetOrderTable.QuoSubDistricts + "\n", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableAddress.Add(new Chunk("               อำเภอ " + GetOrderTable.QuoDistricts + " จังหวัด " + GetOrderTable.QuoProvince + " " + GetOrderTable.QuoZipCode + "\n", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    LableAddress.Leading = 15f;
                    LableAddress.Alignment = Element.ALIGN_LEFT;


                    PdfPCell cellFullWidth = new PdfPCell();
                    cellFullWidth.Colspan = 2; // กำหนดให้ Cell นี้ครอบคลุมทั้ง 2 คอลัมน์
                    cellFullWidth.Border = PdfPCell.NO_BORDER;
                    cellFullWidth.AddElement(LableAddress);

                    // เพิ่มแถวที่สอง (ที่อยู่)
                    tableHeadDetail.AddCell(cellFullWidth);

                    // เพิ่มตารางใน Document
                    document.Add(tableHeadDetail);


                    //document.Add(new Paragraph("\n"));

                    Paragraph TaxID = new Paragraph();
                    TaxID.Add(new Chunk("เลขที่ประจําตัวผู้เสียภาษีอากร : ", new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    TaxID.Add(new Chunk(GetOrderTable.QuoTaxID, new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 15)));
                    TaxID.Alignment = Element.ALIGN_LEFT;
                    document.Add(TaxID);

                    document.Add(new Paragraph("\n"));

                    //document.Add(tableHeadDetail);

                    // Table of items (Full Width Alignment) -- ตารางที่มีรายการสินค้า
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1f, 5f, 1.5f, 1.5f, 1.5f });

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
                        // ดึงข้อมูลจาก MasterStyle
                        var masterStyle = _context.MasterStyles
                            .Where(ms => ms.StyleCode == product.ProductName)
                            .FirstOrDefault();

                        string productDescription = product.ProductName;

                        // รวมข้อมูล FabricDesc และ PatternDetail
                        if (masterStyle != null)
                        {
                            if (product.Size == "NO SIZE" && product.Color != "NO COLOR")
                            {
                                productDescription = productDescription + ", Fabric " + masterStyle.FabricDesc + ", Pattern " + masterStyle.PatternDetail +
                                ", Color " + product.Color;
                            }
                            else if (product.Size != "NO SIZE" && product.Color == "NO COLOR")
                            {
                                productDescription = productDescription + ", Fabric " + masterStyle.FabricDesc + ", Pattern " + masterStyle.PatternDetail +
                                    ", Size " + product.Size;
                            }
                            else if (product.Size == "NO SIZE" && product.Color == "NO COLOR")
                            {
                                productDescription = productDescription + ", Fabric " + masterStyle.FabricDesc + ", Pattern " + masterStyle.PatternDetail;
                            }
                            else
                            {
                             
                                productDescription = productDescription + ", Fabric " + masterStyle.FabricDesc + ", Pattern " + masterStyle.PatternDetail +
                                    ", Size " + product.Size + ", Color " + product.Color;
                            }

                        }


                        // ลำดับ
                        table.AddCell(new PdfPCell(new Phrase(index.ToString(), new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                            //PaddingBottom = 10f 
                        });

                        // รายการสินค้า
                        table.AddCell(new PdfPCell(new Phrase(productDescription, new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                             PaddingBottom = 10f
                        });

                        // จำนวน
                        table.AddCell(new PdfPCell(new Phrase($"{product.Qty}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                             //PaddingBottom = 10f
                        });

                        // ราคา/หน่วย
                        decimal pricePerUnit = product.Qty > 0 ? product.Price / product.Qty : 0;
                        table.AddCell(new PdfPCell(new Phrase($"{pricePerUnit.ToString("N2")}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                             //PaddingBottom = 10f
                        });

                        // จำนวนเงิน
                        table.AddCell(new PdfPCell(new Phrase($"{product.Price.ToString("N2")}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                            //PaddingBottom = 10f
                        });

                        index++; // Increment the index
                    }

                    // Add shipping fee row if QuoShippingPrice > 0
                    if (GetOrderTable.QuoShippingPrice > 0)
                    {
                        // ลำดับ (ใช้ลำดับที่มากที่สุด)
                        table.AddCell(new PdfPCell(new Phrase(index.ToString(), new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });

                        // รายการสินค้า (ค่าขนส่ง)
                        table.AddCell(new PdfPCell(new Phrase("ค่าขนส่ง (Shipping Fee)", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            PaddingBottom = 10f
                        });

                        // จำนวน (เว้นว่าง)
                        table.AddCell(new PdfPCell(new Phrase("-", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });

                        // ราคา/หน่วย (เว้นว่าง)
                        table.AddCell(new PdfPCell(new Phrase("-", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        });

                        // จำนวนเงิน (แสดงค่าขนส่ง)
                        table.AddCell(new PdfPCell(new Phrase($"{GetOrderTable.QuoShippingPrice.ToString("N2")}", new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        });

                       
                    
                    }

                    document.Add(table);

                    document.Add(new Paragraph("\n"));

                    //คำนวนราคารวม-----------------------------------

                    decimal totalAmountValue = 0;

                    // คำนวณยอดรวมจากข้อมูลใน GetProductTable
                    foreach (var product in GetProductTable)
                    {
                        totalAmountValue += product.Price;
                    }

                    // รวมค่าขนส่งเข้าใน totalAmountValue
                    totalAmountValue = totalAmountValue + GetOrderTable.QuoShippingPrice;
                    // คำนวณ VAT 7%
                    //decimal vat = totalAmountValue * 0.07m;
                    // ราคาก่อน VAT 
                    decimal BeforeVat = (totalAmountValue * 100)/107;

                    decimal vat = (totalAmountValue * 7)/107;

                    // คำนวณรวมราคาทั้งสิ้น
                    //decimal grandTotal = totalAmountValue + vat;



                    // Total amount (Right Alignment)
                    //Paragraph totalAmount = new Paragraph("Vat 7% : " + vat.ToString("N2") + "\n" +
                    //                                     "ราคาก่อน Vat : " + BeforeVat.ToString("N2") + "\n" +
                    //                                     "รวมราคาทั้งสิ้น : " + totalAmountValue.ToString("N2") + "",
                    //                                      new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16));
                    //totalAmount.Alignment = Element.ALIGN_RIGHT;
                    //document.Add(totalAmount);

                    // สร้างตารางหลักสำหรับรูปภาพและ totalAmountTable
                    PdfPTable mainTable = new PdfPTable(1); // สร้างตาราง 2 คอลัมน์
                    mainTable.WidthPercentage = 25;
                    mainTable.HorizontalAlignment = Element.ALIGN_RIGHT;// กำหนดความกว้างตารางเป็น 100% ของหน้ากระดาษ
                    mainTable.SetWidths(new float[] { 1f }); // สัดส่วนคอลัมน์: รูปภาพ 2 ส่วน, ตารางตัวเลข 1 ส่วน

                 



                    // สร้างตาราง 2 คอลัมน์เพื่อแสดงข้อความและตัวเลข
                    PdfPTable totalAmountTable = new PdfPTable(2);
                    totalAmountTable.WidthPercentage = 25; // กำหนดความกว้างของตาราง (50% ของหน้ากระดาษ)
                    totalAmountTable.HorizontalAlignment = Element.ALIGN_RIGHT; // จัดตารางชิดขวา
                    totalAmountTable.SetWidths(new float[] { 2f, 1f }); // กำหนดสัดส่วนคอลัมน์ (ข้อความ:ตัวเลข)



                    // ฟอนต์สำหรับข้อความ
                    Font boldFont = new Font(BaseFont.CreateFont(FontLinkBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16);
                    Font normalFont = new Font(BaseFont.CreateFont(FontLinkNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16);

                    // เพิ่มข้อความและตัวเลขในแต่ละแถว

                    // Row 3: รวมราคาทั้งสิ้น
                    totalAmountTable.AddCell(CreateAlignedCell("รวมราคาทั้งสิ้น : ", boldFont, Element.ALIGN_LEFT));
                    totalAmountTable.AddCell(CreateAlignedCell(totalAmountValue.ToString("N2"), normalFont, Element.ALIGN_RIGHT));


                     // Row 2: ราคาก่อน Vat
                    totalAmountTable.AddCell(CreateAlignedCell("ราคาก่อน Vat : ", boldFont, Element.ALIGN_LEFT));
                    totalAmountTable.AddCell(CreateAlignedCell(BeforeVat.ToString("N2"), normalFont, Element.ALIGN_RIGHT));


                    // Row 1: Vat 7%
                    totalAmountTable.AddCell(CreateAlignedCell("Vat 7% : ", boldFont, Element.ALIGN_LEFT));
                    totalAmountTable.AddCell(CreateAlignedCell(vat.ToString("N2"), normalFont, Element.ALIGN_RIGHT));

                    //TableMain
                    // เซลล์ที่สอง: ใส่ totalAmountTable
                    PdfPCell totalAmountCell = new PdfPCell(totalAmountTable); // ใส่ตารางตัวเลขที่สร้างไว้
                    totalAmountCell.Border = Rectangle.NO_BORDER; // ไม่แสดงเส้นขอบ
                    totalAmountCell.HorizontalAlignment = Element.ALIGN_RIGHT; // จัดแนวตารางในเซลล์
                    mainTable.AddCell(totalAmountCell);

                    // เพิ่มตารางลงในเอกสาร
                    //document.Add(totalAmountTable);



                    // เซลล์แรก: ใส่รูปภาพ
                    string pmLink = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "NdsImages", "payment.png");


                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pmLink); // ใส่ path ของรูปภาพ
                    image.ScaleToFit(100f, 100f); // ปรับขนาดรูปภาพ
                    image.Alignment = Element.ALIGN_CENTER; // จัดรูปภาพให้อยู่ตรงกลาง

                    PdfPCell imageCellpayment = new PdfPCell(image);
                    imageCellpayment.Border = Rectangle.NO_BORDER; // ไม่แสดงเส้นขอบ
                    imageCellpayment.HorizontalAlignment = Element.ALIGN_RIGHT; // จัดแนวรูปภาพในเซลล์
                    imageCellpayment.PaddingTop = 10f;
                    mainTable.AddCell(imageCellpayment);



                    // เพิ่ม mainTable ลงในเอกสาร PDF
                    document.Add(mainTable);






                    // ฟังก์ชันช่วยสร้างเซลล์ที่จัดตำแหน่ง
                    PdfPCell CreateAlignedCell(string text, Font font, int alignment)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(text, font));
                        cell.Border = Rectangle.NO_BORDER; // ไม่มีเส้นขอบ
                        cell.HorizontalAlignment = alignment; // จัดตำแหน่งข้อความในเซลล์
                        return cell;
                    }


                    document.Add(new Paragraph("\n"));


                    document.Close();

                    byte[] bytes = stream.ToArray();
                    return File(bytes, "application/pdf", GetOrderTable.QuotationNumber + ".pdf");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error generating PDF: " + ex.Message);
            }
        }
    }
}
