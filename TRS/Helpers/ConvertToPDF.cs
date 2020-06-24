using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TRS.Helpers
{
    public static class ConvertToPDF
    {


        public static MemoryStream pdfConvert(string url)
        {
            var MemoryStreamdata = new MemoryStream();
            try
            {
                var htmlPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfbyte = htmlPdf.GeneratePdfFromFile(url, null);
                MemoryStream ms = new MemoryStream(pdfbyte);
                MemoryStreamdata = ms;
            }
            catch (Exception ex){

            }
            
            return MemoryStreamdata;
        }

    }
}