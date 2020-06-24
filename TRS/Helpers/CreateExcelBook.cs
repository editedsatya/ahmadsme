using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace TRS.Helpers
{
    public static class CreateExcelBook
    {
       
    

        public static Tuple<string,byte[]> ConvertDataExcel(DataTable dt, string sheetName, string FileFormatName,int[] FieldNo)
        {

            XLWorkbook workbook = new XLWorkbook();

            workbook.Worksheets.Add(dt, sheetName);
            workbook.Worksheet(sheetName).Table(0).Column(1).Style.NumberFormat.Format = "####";

            // To check rows  lenth and sum of total coulumn amount
            if (FieldNo !=null && FieldNo.Length > 0)
            {
                workbook.Worksheet(sheetName).Table(0).SetShowTotalsRow(true);
                for (int i = 0; i < FieldNo.Length; i++)
                {
                    var no = FieldNo[i];
                   workbook.Worksheet(sheetName).Table(0).Field(no).TotalsRowFunction = XLTotalsRowFunction.Sum;
                }
                workbook.Worksheet(sheetName).Table(0).LastRowUsed().Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                //TempData[handle] = memoryStream.ToArray();
            var outputFileName = string.Format("{0}" + FileFormatName + ".xlsx", System.DateTime.Now.Ticks);
            return Tuple.Create(outputFileName,memoryStream.ToArray());
            }
        }

    }
}