using System;
using System.Collections.Generic;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Diebold.Exporter
{
    public class ExcelExporter : Exporter
    {
        private const int X_TABLE = 1;
        private const int Y_TABLE = 9;
        private const string DATE_FORMAT = "mm/dd/yyyy";

        public override byte[] Export<T>(string title, OrientationPageType orientationPage, IList<T> list,
                                       DateTime dateFrom, DateTime dateTo, float[] columnsWidths)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    // add a new worksheet to the empty workbook
                    var worksheet = package.Workbook.Worksheets.Add("Sheet");
                    
                    var visibleProperties = GetVisibleProperties<T>();
                    var headers = GetHeaders(visibleProperties);

                    //Image.
                    worksheet.Cells[3, 1, 3, visibleProperties.Count].Merge = true;
                    worksheet.Cells[5, 1].Value = title;
                    
                    var assembly = Assembly.GetExecutingAssembly();
                    var myStream = assembly.GetManifestResourceStream("Diebold.Exporter.Content.Images.Logo.gif");
                    if (myStream != null)
                    {
                        using (var image = Image.FromStream(myStream))
	                    {
		                    //set row height to accommodate the picture
			                //ws.Row(currentRow).Height = ExcelHelper.Pixel2RowHeight(pictureHeight + 1);

			                //add picture to cell
                            var pic = worksheet.Drawings.AddPicture("DieboldLogo", image);

			                //position picture on desired column
			                pic.From.Column = 0;
			                pic.From.Row = 0;

                            //pic.From.ColumnOff = ExcelHelper.Pixel2MTU(1);
                            //pic.From.RowOff = ExcelHelper.Pixel2MTU(1);
			                //set picture size to fit inside the cell
                            //pic.SetSize(pictureWidth, pictureHeight);
	                    }

                        //image.ScaleToFit(130, 130);
                    } else
                    {
                        throw new Exception("Image could not be found.");
                    }
                    
                    //Title.
                    worksheet.Cells[5, 1, 5, visibleProperties.Count].Merge = true;
                    worksheet.Cells[5, 1].Value = title;

                    //Dates.
                    worksheet.Cells[6, visibleProperties.Count - 1].Value = "From:";
                    worksheet.Cells[6, visibleProperties.Count].Value = dateFrom.ToShortDateString();

                    worksheet.Cells[7, visibleProperties.Count - 1].Value = "To:";
                    worksheet.Cells[7, visibleProperties.Count].Value = dateTo.ToShortDateString();

                    var i = X_TABLE;
                    foreach (var header in headers)
                    {
                        worksheet.Cells[Y_TABLE, i].Value = header;
                        i++;
                    }

                    var row = Y_TABLE + 1;
                    foreach (var item in list)
                    {
                        var column = 1;

                        foreach (var prop in visibleProperties)
                        {
                            worksheet.Cells[row, column].Value = prop.GetValue(item, null);
                            worksheet.Cells[row, column].Style.Numberformat.Format = DATE_FORMAT;
                            column++;
                        }
                        row++;
                    }

                    //Ok now format the values
                    FormatCells(visibleProperties, worksheet);

                    //Create an autofilter for the range
                    worksheet.Cells[Y_TABLE, X_TABLE, Y_TABLE, visibleProperties.Count].AutoFilter = true;

                    //worksheet.Cells["A2:A4"].Style.Numberformat.Format = "@";   //Format as text
                    //worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                    //worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                    //worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                    // Change the sheet view to show it in page layout mode
                    //worksheet.View.PageLayoutView = true;
                    
                    worksheet.PrinterSettings.Orientation = orientationPage == OrientationPageType.Portrait ? eOrientation.Portrait : eOrientation.Landscape;

                    // set some document properties
                    package.Workbook.Properties.Title = title;
                    package.Workbook.Properties.Author = "Diebold";

                    // set some extended property values
                    package.Workbook.Properties.Company = "Diebold";

                    // return bytes stream
                    return package.GetAsByteArray();
                }
            }
            catch (Exception de)
            {
                throw new ExporterException(de.Message);
            }
        }

        private static void FormatCells(IList<PropertyInfo> visibleProperties, ExcelWorksheet worksheet)
        {
            //Title.
            using (var range = worksheet.Cells[5, 1])
            {
                range.Style.Font.Size = 14;
                range.Style.Font.Bold = true;
                range.Style.Font.Color.SetColor(Color.DarkBlue);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            //Filters.
            using (var range = worksheet.Cells[6, visibleProperties.Count - 1, 7, visibleProperties.Count])
            {
                range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            //Headers.
            using (var range = worksheet.Cells[Y_TABLE, X_TABLE, Y_TABLE, visibleProperties.Count])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.White);
            }
        }
    }
}
