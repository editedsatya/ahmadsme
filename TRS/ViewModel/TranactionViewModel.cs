using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TRS.Models;

namespace TRS.ViewModel
{

    public class ReportsViewModel
    {
        public List<Transaction> _All { get; set; }
        public List<Transaction> _Success { get; set; }
        public List<Transaction> _Failed { get; set; }
        public List<Point> _Point { get; set; }

        public string MADA { get; set; }
        public string VISA { get; set; }
        public string VISACredit { get; set; }
        public string VISADebit { get; set; }


        public int AllTranactionCount { get; set; }
        public int SuccessTranactionCount { get; set; }
        public int FailedTranactionCount { get; set; }
        public int SelttedTranactionCount { get; set; }

        public string TotalSaleAMount { get; set; }
        public int Branch { get; set; }
        public string currentDate { get; set; }
        //public string datetimes { get; set; }
    }


    public  class Point
    {

        public string label  { get; set; }
        public double y  { get; set; }

    }
}
