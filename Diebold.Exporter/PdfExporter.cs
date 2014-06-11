using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Diebold.Exporter
{
    public enum OrientationPageType
    {
        Portrait,
        LandScape
    }
    
    public class PdfExporter : Exporter
    {
        public override byte[] Export<T>(string title, OrientationPageType orientationPage, IList<T> list, 
                                       DateTime dateFrom, DateTime dateTo, float[] columnsWidths)
        {
            var memStream = new MemoryStream();
            PdfPTable table = null;
            Phrase phrase = null;
            Font font = null;

            var document = new Document((orientationPage == OrientationPageType.Portrait) ? PageSize.A4 : PageSize.A4.Rotate(), 20, 20, 150, 40);

            try
            {
                if (list != null)
                {
                    var colWithsList = new List<float>();
                    float[] colWidths;

                    var visibleProperties = GetVisibleProperties<T>();
                    var headers = GetHeaders(visibleProperties);

                    if (columnsWidths == null)
                    {
                        var totalLength = headers.Sum(x => x.Length);

                        foreach (var header in headers)
                        {
                            colWithsList.Add(header.Length * 100 / totalLength);
                        }

                        colWidths = colWithsList.ToArray();
                    }
                    else
                    {
                        colWidths = columnsWidths;
                    }

                    table = CreateTablePDF(list, visibleProperties, headers, colWidths);
                }
            }
            catch (Exception e)
            {
                throw new ExporterException(e.Message);
            }

            var pdfWriter = PdfWriter.GetInstance(document, memStream);

            // Our custom Header and Footer is done using Event Handler
            var pageEventHandler = new PdfPageEvent();
            pdfWriter.PageEvent = pageEventHandler;

            #region Central Header

            font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 15, Font.BOLD, new BaseColor(12, 102, 190));

            phrase = new Phrase(8, (title.Replace("<h2>", "")).Replace("</h2>", ""), font);
            pageEventHandler.HeaderCenter.Add(PdfPageEvent.FormatCell(new PdfPCell(phrase), Element.ALIGN_CENTER, Element.ALIGN_CENTER, 29));

            #endregion

            #region Left Header

            font = FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.NORMAL);

            var assembly = Assembly.GetExecutingAssembly();
            var myStream = assembly.GetManifestResourceStream("Diebold.Exporter.Content.Images.Logo.gif");
            var image = Image.GetInstance(myStream);
            image.ScaleToFit(130, 130);

            pageEventHandler.HeaderLeft.Add(PdfPageEvent.FormatCell(new PdfPCell(image), Element.ALIGN_LEFT,
                                                                    Element.ALIGN_CENTER, 0));

            //phrase = new Phrase(8, "190 Front Street, Suite 201", font);
            //pageEventHandler.HeaderLeft.Add(PdfPageEvent.FormatCell(new PdfPCell(phrase), Element.ALIGN_LEFT, Element.ALIGN_CENTER, 0));

            //phrase = new Phrase(8, "Ashland, MA 01721", font);
            //pageEventHandler.HeaderLeft.Add(PdfPageEvent.FormatCell(new PdfPCell(phrase), Element.ALIGN_LEFT, Element.ALIGN_CENTER, 0));

            //phrase = new Phrase(8, "Tel: 888-881-0838", font);
            //pageEventHandler.HeaderLeft.Add(PdfPageEvent.FormatCell(new PdfPCell(phrase), Element.ALIGN_LEFT, Element.ALIGN_CENTER, 0));

            #endregion

            #region Right Header

            phrase = new Phrase(8, "From: " + dateFrom.ToString("MM/dd/yyyy"), font);
            pageEventHandler.HeaderRight.Add(PdfPageEvent.FormatCell(new PdfPCell(phrase), Element.ALIGN_RIGHT,
                                                                     Element.ALIGN_CENTER, 50));

            phrase = new Phrase(8, "To: " + dateTo.ToString("MM/dd/yyyy"), font);
            pageEventHandler.HeaderRight.Add(PdfPageEvent.FormatCell(new PdfPCell(phrase), Element.ALIGN_RIGHT,
                                                                     Element.ALIGN_CENTER, 0));

            #endregion

            document.Open();

            if (table != null)
                document.Add(table);

            document.Close();

            return memStream.ToArray();
        }

        #region Private Methods

        private static PdfPTable CreateTablePDF<T>(IEnumerable<T> list, IEnumerable<PropertyInfo> visibleProperties, IList<string> headers, float[] columnsWiths) where T : new()
        {
            Font font = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK);

            if (visibleProperties == null)
                return new PdfPTable(0);

            var table = new PdfPTable(visibleProperties.Count());
            table.DefaultCell.BorderColor = new BaseColor(0, 0, 255);
            table.DefaultCell.BorderWidth = 1;
            table.WidthPercentage = 95;

            //Define the column withs
            if (visibleProperties.Count() != 0)
                table.SetWidths(columnsWiths);

            int i = 0;

            // Add the header row to the table
            
            foreach (var header in headers)
            {
                var cell = new PdfPCell(new Phrase(header, font))
                               {
                                   HorizontalAlignment = Element.ALIGN_CENTER,
                                   VerticalAlignment = Element.ALIGN_CENTER,
                                   BackgroundColor = new BaseColor(0, 144, 210)
                               };

                table.AddCell(cell);
            }

            //Add each item to the table
            foreach (var item in list)
            {
                foreach (var prop in visibleProperties)
                {
                    PdfPCell cell = null;

                    var value = prop.GetValue(item, null);

                    cell = new PdfPCell(new Phrase(value != null ? value.ToString() : string.Empty, font))
                               {
                                   HorizontalAlignment = Element.ALIGN_CENTER,
                                   VerticalAlignment = Element.ALIGN_CENTER
                               };


                    if ((i % 2) == 0)
                        cell.BackgroundColor = new BaseColor(245, 250, 253);
                    else
                        cell.BackgroundColor = new BaseColor(229, 242, 248);

                    table.AddCell(cell);
                }

                i++;
            }

            return table;
        }

        #endregion
    }

    public class PdfPageEvent : PdfPageEventHelper
    {
        #region Properties

        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // We will put the final number of pages in a template
        PdfTemplate template;

        // This is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime printTime = DateTime.Now;

        private List<PdfPCell> _headerCenter = new List<PdfPCell>();
        public List<PdfPCell> HeaderCenter
        {
            get { return _headerCenter; }
            set { _headerCenter = value; }
        }

        private List<PdfPCell> _headerLeft = new List<PdfPCell>();
        public List<PdfPCell> HeaderLeft
        {
            get { return _headerLeft; }
            set { _headerLeft = value; }
        }

        private List<PdfPCell> _headerRight = new List<PdfPCell>();
        public List<PdfPCell> HeaderRight
        {
            get { return _headerRight; }
            set { _headerRight = value; }
        }

        #endregion

        #region Public Events

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                printTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException)
            { }
            catch (System.IO.IOException)
            { }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;

            //Create a table with 3 columns       
            PdfPTable HeaderTable = new PdfPTable(3);
            HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
            HeaderTable.TotalWidth = pageSize.Width - 80; HeaderTable.SetWidths(new float[] { 28, 44, 28 });

            PdfPTable headerTableCell = null;
            PdfPCell headerCell = null;

            #region Left Table

            headerTableCell = new PdfPTable(1);
            foreach (PdfPCell cell in HeaderLeft)
                headerTableCell.AddCell(cell);

            headerCell = FormatCell(new PdfPCell(headerTableCell), Element.ALIGN_CENTER, Element.ALIGN_CENTER, 0);
            headerCell.BorderWidthBottom = 1.3f; headerCell.Padding = 0; headerCell.PaddingBottom = 5;
            HeaderTable.AddCell(headerCell);

            #endregion

            #region Center Table

            headerTableCell = new PdfPTable(1);
            foreach (PdfPCell cell in HeaderCenter)
                headerTableCell.AddCell(cell);

            headerCell = FormatCell(new PdfPCell(headerTableCell), Element.ALIGN_CENTER, Element.ALIGN_CENTER, 0);
            headerCell.BorderWidthBottom = 1.3f; headerCell.Padding = 0; headerCell.PaddingBottom = 5;
            HeaderTable.AddCell(headerCell);

            #endregion

            #region Right Table

            headerTableCell = new PdfPTable(1);
            foreach (PdfPCell cell in HeaderRight)
                headerTableCell.AddCell(cell);

            headerCell = FormatCell(new PdfPCell(headerTableCell), Element.ALIGN_CENTER, Element.ALIGN_CENTER, 0);
            headerCell.BorderWidthBottom = 1.3f; headerCell.Padding = 0; headerCell.PaddingBottom = 5;
            HeaderTable.AddCell(headerCell);

            #endregion

            cb.SetRGBColorFill(0, 0, 0);
            HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Printed On " + printTime.ToString(), pageSize.GetRight(40), pageSize.GetBottom(30), 0);
            cb.EndText();
        }

        public static PdfPCell FormatCell(PdfPCell cell, int hAlignment, int vAlignment, float paddingTop)
        {
            cell = PreFormat(cell, hAlignment, vAlignment);

            cell.PaddingLeft = 3;
            cell.PaddingRight = 3;
            cell.PaddingBottom = 3;
            cell.PaddingTop = paddingTop;
            cell.HorizontalAlignment = hAlignment;
            cell.VerticalAlignment = vAlignment;

            return cell;
        }

        public static PdfPCell PreFormat(PdfPCell cell, int hAlignment, int vAlignment)
        {
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 0;
            cell.PaddingLeft = 0;
            cell.PaddingRight = 0;
            cell.PaddingBottom = 0;
            cell.PaddingTop = 0;
            cell.HorizontalAlignment = hAlignment;
            cell.VerticalAlignment = vAlignment;

            return cell;
        }

        #endregion
    }


}
