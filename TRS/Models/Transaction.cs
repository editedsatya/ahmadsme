using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRS.Models
{
    public class Transaction
    {
        public Int64 id { get; set; }
        public Int64 TID { get; set; }
        public int Status { get; set; }
        public string PAN { get; set; }
        public string SchemaID { get; set; }
        public string SchemaName { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int AuthCode { get; set; }
        public int RespCode { get; set; }
        public Int64 RRN { get; set; }
        public string AID { get; set; }
        public string STAN { get; set; }
        public DateTime? CardExpireDate { get; set; }
        public decimal Amount { get; set; }
        public int EntryMode { get; set; }
        public string LeapInfo { get; set; }
        public int MID { get; set; }
        public string RelVersion { get; set; }
        public int RefNumber { get; set; }
    }
}