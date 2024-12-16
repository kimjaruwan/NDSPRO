namespace NDSPRO.Models
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    public class FooterEvent : PdfPageEventHelper
    {
        private readonly string _remarkText;
        private readonly string _accountInfo;
        private readonly string _preparedBy;
        private readonly string _preparedByy;
        private readonly string _fontPathBold;
        private readonly string _fontPathNormal;
        private readonly Font _font;

        public FooterEvent(string remarkText, string accountInfo, string preparedBy, string fontPathBold, string fontPathNormal)
        {
            _remarkText = remarkText;
            _accountInfo = accountInfo;
            _preparedBy = "Phitsukan Phiriyawitthaya";
            _preparedByy = "Phitsukan";
            //_preparedBy = preparedBy;
            _fontPathBold = fontPathBold;
            _fontPathNormal = fontPathNormal;

            // Default font for Footer
            _font = new Font(BaseFont.CreateFont(_fontPathNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL);

        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {

            // สร้าง Paragraph โดยใช้ Chunk
            Paragraph remarkTextParagraph = new Paragraph();
            Font normalFont = new Font(BaseFont.CreateFont(_fontPathNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL);
            Font boldFont = new Font(BaseFont.CreateFont(_fontPathBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL);

            // เพิ่มข้อความ "หมายเหตุ :" ด้วยฟอนต์ตัวหนา
            remarkTextParagraph.Add(new Chunk("หมายเหตุ : ", boldFont));

            // เพิ่มข้อความ _remarkText และ _accountInfo ด้วยฟอนต์ปกติ
            remarkTextParagraph.Add(new Chunk(_remarkText, normalFont));
            remarkTextParagraph.Add(Chunk.Newline);
            remarkTextParagraph.Add(new Chunk("" + _accountInfo, normalFont));

            // คำนวณตำแหน่ง Y สำหรับ Paragraphs
            float x = document.LeftMargin; // จุดเริ่มต้นจากซ้าย
            float yRemarkText = document.BottomMargin + 70; // ตำแหน่ง Y ของ Remark Text

            ColumnText remarkTextColumn = new ColumnText(writer.DirectContent);
            remarkTextColumn.SetSimpleColumn(
                x, yRemarkText, // เริ่มต้นที่ตำแหน่ง X, Y
                document.PageSize.Width - document.RightMargin, // ความกว้างถึงขอบขวา
                yRemarkText + 40, // ความสูงของ Column
                15, // ระยะห่างระหว่างบรรทัด
                Element.ALIGN_LEFT
            );

            remarkTextColumn.AddElement(remarkTextParagraph);
            remarkTextColumn.Go();





            // Set up Footer Table
            PdfPTable footerTable = new PdfPTable(3);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.SetWidths(new float[] { 1f, 1f, 1f });

            // Add cells
            footerTable.AddCell(CreateFooterCell("สั่งซื้อโดย/Order By\n_______________\n(_______________)\nวันที่___/___/___", Element.ALIGN_CENTER));
            footerTable.AddCell(CreateFooterCell($"ออกโดย/Prepared By\n{_preparedByy}\n({_preparedBy})\nวันที่ {DateTime.Now:dd/MM/yyyy}", Element.ALIGN_CENTER));
            footerTable.AddCell(CreateFooterCell("ผู้มีอำนาจอนุมัติ/Authorized\n_______________\n(_______________)\nวันที่___/___/___", Element.ALIGN_CENTER));

            // Write footer table
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin + 50, writer.DirectContent);

           


        }

        private PdfPCell CreateFooterCell(string text, int alignment)
        {
            Font normalFont = new Font(BaseFont.CreateFont(_fontPathNormal, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL);
            Font boldFont = new Font(BaseFont.CreateFont(_fontPathBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 16, Font.NORMAL);

            // ตรวจสอบข้อความที่ต้องการทำตัวหนา
            Phrase cellPhrase = new Phrase();
            if (text.StartsWith("สั่งซื้อโดย/Order By"))
            {
                cellPhrase.Add(new Chunk("สั่งซื้อโดย/Order By", boldFont));
                cellPhrase.Add(new Chunk("\n_______________\n(_______________)\nวันที่___/___/___", normalFont));
            }
            else if (text.StartsWith("ออกโดย/Prepared By"))
            {
                cellPhrase.Add(new Chunk("ออกโดย/Prepared By", boldFont));
                cellPhrase.Add(new Chunk($"\n{_preparedByy}\n({_preparedBy})\nวันที่ {DateTime.Now:dd/MM/yyyy}", normalFont));
            }
            else if (text.StartsWith("ผู้มีอำนาจอนุมัติ/Authorized"))
            {
                cellPhrase.Add(new Chunk("ผู้มีอำนาจอนุมัติ/Authorized", boldFont));
                cellPhrase.Add(new Chunk("\n_______________\n(_______________)\nวันที่___/___/___", normalFont));
            }
            else
            {
                // สำหรับข้อความอื่น
                cellPhrase.Add(new Chunk(text, normalFont));
            }

            PdfPCell cell = new PdfPCell(cellPhrase)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = alignment
            };
            return cell;
        }
    }
}
