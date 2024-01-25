using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PersonelVardiyaOtomasyonu.Class.Helper
{
    public static class PrintPDFBuilder
    {
        public static void Print(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            saveFileDialog.FileName = "document.pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

                PdfPTable pdfPTable = new PdfPTable(dataTable.Columns.Count);

                // Header cells
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    PdfPCell pdfPCell = new PdfPCell(new Phrase(dataColumn.ColumnName, font));
                    pdfPCell.HorizontalAlignment = 1;
                    pdfPCell.BackgroundColor = new BaseColor(ColorTranslator.FromHtml("#CED4DA"));
                    pdfPTable.AddCell(pdfPCell);
                }

                // Data cells
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        pdfPTable.AddCell(new Phrase(item.ToString(), font));
                    }
                }

                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 0, 0, 10, 10);
                    PdfWriter.GetInstance(document, fileStream);
                    document.Open();
                    document.Add(pdfPTable);
                    document.Close();
                    fileStream.Close();
                }
            }
        }
    }
}


