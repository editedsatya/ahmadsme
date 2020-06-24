using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TRS.Models;
using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using TRS.Extensions;
using TRS.Helpers;
using System.Net;
using System.Configuration;

namespace TRS.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,User")]

    public class ReportsController : Controller
    {
        string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
        Models.DataAccessLayer db = new Models.DataAccessLayer();

        private ApplicationDbContext dbContext;

        public ReportsController()
        {

        

           dbContext = new ApplicationDbContext();

        }

        // GET: Transaction
        public ActionResult TransactionsDetails()
        {
            
            return View();
          
        }


        [AllowAnonymous]
        public ActionResult PrintRecipt(Int64 RRN)
        {

            foreach (DataRow item in db.populate("Select * from Transactions_tbl where RRN=N'" + RRN + "'").Rows)
            {
                ViewBag.TerminalID = item["TID"].ToString();


                foreach (DataRow itemTerminalInfo in db.populate("Select * from TerminalsInfo_Tbl where TID=N'" + item["TID"].ToString() + "'").Rows)
                {

                    ViewBag.MID = itemTerminalInfo["MID"].ToString();
                    ViewBag.MerchantName = itemTerminalInfo["MerchantName"].ToString();
                    ViewBag.MerchantAddress = itemTerminalInfo["MerchantAddress"].ToString();
                    ViewBag.MerchantPhone = itemTerminalInfo["MerchantPhone"].ToString();
                    ViewBag.AcqBankName = itemTerminalInfo["AcqBankName"].ToString();
                    ViewBag.AcqBankID = itemTerminalInfo["AcqBankID"].ToString();
                }


                ViewBag.Amount = item.Field<System.Decimal>("AMOUNT").ToString("0.00");
                ViewBag.AmountAr = TRS.Helpers.Helper.ToIndicDigits(item.Field<System.Decimal>("AMOUNT").ToString("0.00")); 



                ViewBag.StartDate = item.Field<System.DateTime>("StartDateTime").ToString("MM/dd/yyyy");
                ViewBag.StartTime = item.Field<System.DateTime>("StartDateTime").ToString("HH:mm:ss");

                ViewBag.EndDate = item.Field<System.DateTime>("EndDateTime").ToString("MM/dd/yyyy");
                ViewBag.EndTime = item.Field<System.DateTime>("EndDateTime").ToString("HH:mm:ss");


                ViewBag.RespCode = item.Field<Int32>("RespCode").ToString("D3");
                ViewBag.MerchantID = item.Field<System.Int64>("MID");
                ViewBag.RRN = item.Field<System.Int64>("RRN");
                //ViewBag.STATUS = item.Field<System.Int32>("STATUS") == 0 ? "APPROVE" : "DICLINE";

                ViewBag.STAN = item["STAN"].ToString();
                ViewBag.Version = item["RelVersion"].ToString();
                ViewBag.RRN = item["RRN"].ToString();


                ViewBag.AuthCode = item["AuthCode"].ToString();
                ViewBag.AuthCodeAr = TRS.Helpers.Helper.ToIndicDigits(item["AuthCode"].ToString());


                switch (item["SchemaID"].ToString().ToUpper())
                {
                    case "P1":
                        ViewBag.SchemaNameAr = "مدى";
                        break;
                    case "VC":
                        ViewBag.SchemaNameAr = "فيزا";
                        break;
                    case "MC":
                        ViewBag.SchemaNameAr = "ماستر كارد";
                        break;
                    case "DM":
                        ViewBag.SchemaNameAr = "مايسترو";
                        break;
                    case "AE":
                        ViewBag.SchemaNameAr = "أمريكان إكسبريس";
                        break;
                    case "UP":
                        ViewBag.SchemaNameAr = "يونيون باي";
                        break;
                    default:
                        ViewBag.SchemaNameAr = "";
                        break;
                }

                ViewBag.SchemaNameEn = item["SchemaName"].ToString();
                ViewBag.CardExpireDate = item["CardExpireDate"].ToString().Insert(2, "/");

                ViewBag.PAN = item["PAN"].ToString();


                switch ("PUR"/*item["TrxType"].ToString().ToUpper()*/)
                {
                    case "PUR":
                        ViewBag.TransactionTypeAr = "شراء";
                        ViewBag.TransactionTypeEn = "PURCHASE";
                        ViewBag.AmountLableAr = "مبلغ الشراء";
                        ViewBag.AmountLableEn = "PURCHASE AMOUNT";
                        ViewBag.CashbackAmountLableAr = "";
                        ViewBag.CashbackAmountLableEn = "";
                        if (item.Field<System.Int32>("STATUS") == 0)
                        {
                            ViewBag.TransactionOutcomeAr = "مقبولة";
                            ViewBag.TransactionOutcomeEn = "APPROVED";

                            ViewBag.CardholderVerification_ReasonforDeclineAr = "لا يتطلب التحقق";
                            ViewBag.CardholderVerification_ReasonforDeclineEn = "NO VERIFICATION REQUIRED";
                        }
                        else
                        {
                            ViewBag.TransactionOutcomeAr = "مرفوضة";
                            ViewBag.TransactionOutcomeEn = "DECLINED";

                            ViewBag.CardholderVerification_ReasonforDeclineAr = "مرفوضة";
                            ViewBag.CardholderVerification_ReasonforDeclineEn = "DECLINED";
                        }


                        break;
                    case "PWN":
                        ViewBag.TransactionTypeAr = "شراء مع نقد";
                        ViewBag.TransactionTypeEn = "PURCHASE WITH NAQD";
                        break;
                    case "REF":
                        ViewBag.TransactionTypeAr = "إسترداد";
                        ViewBag.TransactionTypeEn = "REFUND";
                        break;
                    case "REV":
                        ViewBag.TransactionTypeAr = "عملية معكوسة";
                        ViewBag.TransactionTypeEn = "REVERSAL";
                        break;
                    case "CA":
                        ViewBag.TransactionTypeAr = "سلفة نقدية";
                        ViewBag.TransactionTypeEn = "CASH ADVANCE";
                        break;
                    case "AUTH":
                        ViewBag.TransactionTypeAr = "تفويض";
                        ViewBag.TransactionTypeEn = "AUTHORIZATION";
                        break;
                    case "PA":
                        ViewBag.TransactionTypeAr = "إشعار بالشراء";
                        ViewBag.TransactionTypeEn = "PURCHASE ADVICE";
                        break;
                    default:
                        ViewBag.SchemaNameAr = "";
                        break;
                }



                ViewBag.LeapInfo = item["LeapInfo"].ToString();


                switch (item["EntryMode"])
                {
                    case 1:
                        ViewBag.EntryMode = "KEYED";
                        break;
                    case 2:
                        ViewBag.EntryMode = "SWIPED";
                        break;
                    case 5:
                        ViewBag.EntryMode = "DIPPED";
                        break;
                    case 7:
                        ViewBag.EntryMode = "CONTACTLESS";
                        break;
                    default:
                        ViewBag.EntryMode = "";
                        break;
                }
       


            }
           // TempData["RRN"] = RRN;
            return View();

        }


        public ActionResult TransactionsDetailsByDateAndTID()
        {
       
            return View();

        }
         

        [HttpPost]
        public ActionResult LoadData()
        {


            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            //SORT
            string Sort = "";
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                Sort = "Order By " + sortColumn + " " + sortColumnDir ;
            }





            //Global search field
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();


            //find search columns info
            var dateRange = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var gSearch = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            var TIDs = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

           

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;



            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();


            var Where = " WHERE ( TID       LIKE '%" + gSearch.ToString() + "%' or " +
                              " AMOUNT    LIKE '%" + gSearch.ToString() + "%' or " +
                              " RespCode LIKE '%" + gSearch.ToString() + "%' )  ";
            

        

            if (!string.IsNullOrEmpty(dateRange))
            {
               
                String[] substrings = dateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1=DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));

                Where += " and StartDateTime between @date1 and @date2 "; 
            }
             

            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));

           
            


            recordsTotal = (int)db.populate("select count(*) from Transactions_tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where , sqlParams.ToArray()).Rows[0][0];

             DataTable objDt = db.populate(" select A.*,(select max(EndDateTime) from Reconciliations_Tbl where Reconciliations_Tbl.TID=A.TID and Reconciliations_Tbl.EndDateTime>A.EndDateTime)  as [ReconciliationDate] from Transactions_tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where +
                                           Sort  +
                                          " OFFSET " + skip + " ROWS" +
                                          " FETCH NEXT " + pageSize + " ROWS ONLY ", sqlParams.ToArray());
            objDt.TableName = "myTable";


            var data = (from p in objDt.AsEnumerable()
                        select new
                        {
                            TID = p.Field<System.Int64>("TID"),
                            AMOUNT = p.Field<System.Decimal>("AMOUNT").ToString("G"),
                            StartDateTime = p.Field<System.DateTime>("StartDateTime").ToString("MM/dd/yyyy HH:mm "),
                            RespCode = p.Field<Int32>("RespCode"),
                            MID = p.Field<System.Int64>("MID"),
                            RRN = p.Field<System.Int64>("RRN"),
                            STATUS = p.Field<System.Int32>("STATUS")==0?"APPROVE":"DICLINE",
                            ReconciliationDate =p["ReconciliationDate"]==DBNull.Value ? "-" : p.Field<System.DateTime>("ReconciliationDate").ToString("MM/dd/yyyy HH:mm "),
                        }).ToList();

            
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            
        }
        

        public JsonResult GetHierarchy()
        {

            List<IData> records;
            List<SqlParameter> sqlParams = new List<SqlParameter>();




            sqlParams.Add(new SqlParameter("@CustomerId", User.Identity.GetCustomerId()));
            sqlParams.Add(new SqlParameter("@UserId", User.Identity.GetUserId()));

            DataTable objDt = db.populate("Select A.* from CustomerStructures A inner join AssignCustomerStructureToUser B on  A.Id=b.nodeid and a.CustomerId=b.CustomerId Where B.CustomerId=@CustomerId and B.UserId=@UserId", sqlParams.ToArray());
            objDt.TableName = "myTable";


            records = objDt.AsEnumerable().Where(l => l["PerentId"] == System.DBNull.Value)
                  .Select(l => new IData
                  {
                      //code = l.Field<System.Int32>("Id").ToString(),
                      name = l.Field<System.String>("Name"),
                      //perentId = l.Field<System.Int32?>("PerentId"),
                      attributes=new IDataAttributes() { key= l.Field<System.Int32>("Id").ToString() },
                      nodeSelected =l.Field<System.Boolean>("IsTID") ,
                      //nodeDisabled =!l.Field<System.Boolean>("IsTID"),
                      children = GetChildren(objDt, l.Field<System.Int32>("Id"))
                  }).ToList();



            return this.Json(records, JsonRequestBehavior.AllowGet);
            // return View();
        }


        private List<IData> GetChildren(DataTable objDt, int parentId)
        {
            return objDt.AsEnumerable().Where(l => l.Field<System.Int32?>("PerentId") == parentId)
                .Select(l => new IData
                {
                    //code = l.Field<System.Int32>("Id").ToString(),
                    name = l.Field<System.String>("Name"),
                    //perentId = l.Field<System.Int32>("PerentId"),
                    attributes = new IDataAttributes() { key = l.Field<System.Int32>("Id").ToString() },
                    nodeSelected = l.Field<System.Boolean>("IsTID"),
                   
                    children = GetChildren(objDt, l.Field<System.Int32>("Id"))
                }).ToList();

        }
    

        [HttpPost]
        public ActionResult TrxDetailsReport(string DateRange, string TIDS)
        {

            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            var Where = " WHERE TID is not null";

            if (!string.IsNullOrEmpty(DateRange))
            {

                String[] substrings = DateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));

                Where += " and StartDateTime between @date1 and @date2 ";
            }
            else
            {
                return null;

            }
            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));


            DataTable Final = db.populate(" select A.TID,case A.Status when 0 then 'APPROVE' else 'DICLINE' END AS Status,A.PAN,A.SchemaID,A.SchemaName,A.StartDateTime,A.EndDateTime" +
                ",A.AuthCode,A.RespCode,A.RRN,A.AID	,A.STAN	,A.CardExpireDate	,A.Amount	,A.EntryMode	,A.LeapInfo	,A.MID	,A.RelVersion" +
                ",(select max(EndDateTime) from Reconciliations_Tbl where Reconciliations_Tbl.TID=A.TID and Reconciliations_Tbl.EndDateTime>A.EndDateTime)  as [Reconciliation Date] from Transactions_tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId and B.ID in ('" + TIDS.Replace(",", "','") + "') " + Where +
                                         " ", sqlParams.ToArray());

            
            Final.TableName = "Final_Report";


            var dataExcel = CreateExcelBook.ConvertDataExcel(Final, "Trx.Details Report", "_ITS_Trx_Details_REPORT",null);
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = dataExcel.Item2;

            // Note we are returning a filename as well as the handle
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = dataExcel.Item1 }
            };

        }
        

        [HttpPost]
        public ActionResult TrxDetailsByDateAndTIDReport(string DateRange, string TIDS)
        {

            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            sqlParams.Add(new SqlParameter("@CustomerId", User.Identity.GetCustomerId()));
            sqlParams.Add(new SqlParameter("@UserID", User.Identity.GetUserId()));

            var Where = " WHERE CustomerId=@CustomerId and UserID=@UserID and NodeId in ('" + TIDS.Replace(",", "','") + "') ";

            if (!string.IsNullOrEmpty(DateRange))
            {

                String[] substrings = DateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1.Date));
                sqlParams.Add(new SqlParameter("@date2", dDate2.Date));

                Where += " and cast(StartDateTime as date) between @date1 and @date2 ";
            }
            else
            {
                return null;

            }
        

            DataTable Final = db.populate(" SELECT  Branch,cast(pvt.StartDateTime as date) as TRANS_Date ,TID, sum(isnull([P1],0)) as [MADA],sum(isnull([VC],0)) as [VISA],sum(isnull([MC],0)) as [MASTER CARD],sum(isnull([AE],0)) as [American Express], sum(isnull([UP],0)) as [UNION PAY] FROM " +
                "(SELECT * from TransactionView  " + Where + ") AS MAIN" +
                                         " PIVOT ( SUM(amount) FOR SchemaID IN ([P1],[VC], [MC],[AE], [UP]) ) AS pvt group by Branch,cast(pvt.StartDateTime as date),pvt.TID ", sqlParams.ToArray());



            Final.TableName = "Final_Report";

            int[] FieldRange ={ 3, 4, 5, 6, 7 };
            var dataExcel = CreateExcelBook.ConvertDataExcel(Final, "Report", "_ITS_Trx_Details_REPORT1", FieldRange);
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = dataExcel.Item2;
            

            // Note we are returning a filename as well as the handle
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = dataExcel.Item1 }
            };

        }


        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            { 
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }


        public ActionResult ReconciliationsDetails()
        {

            return View();

        }


        [HttpPost]
        public ActionResult LoadReconciliations()
        {


            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            //SORT
            string Sort = "";
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                Sort = "Order By " + sortColumn + " " + sortColumnDir;
            }

            //Global search field
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();


            //find search columns info
            var dateRange = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var gSearch = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            var TIDs = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;



            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();


            var Where = " WHERE ( TID  LIKE '%" + gSearch.ToString() + "%' or " +
                                " STAN    LIKE '%" + gSearch.ToString() + "%' )  ";




            if (!string.IsNullOrEmpty(dateRange))
            {

                String[] substrings = dateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));

                Where += " and EndDateTime between @date1 and @date2 ";
            }
            
            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));





            recordsTotal = (int)db.populate("select count(*) from Reconciliations_Tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where, sqlParams.ToArray()).Rows[0][0];

            DataTable objDt = db.populate(" select A.* from Reconciliations_Tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where + 
                                           Sort+
                                          " OFFSET " + skip + " ROWS" +
                                          " FETCH NEXT " + pageSize + " ROWS ONLY ", sqlParams.ToArray());
            objDt.TableName = "myTable";


            var data = (from p in objDt.AsEnumerable()
                        select new
                        {
                            Id= p.Field<System.Int64>("Id"),
                            TID = p.Field<System.Int64>("TID"),
                            DateTime = p.Field<System.DateTime>("EndDateTime").ToString("MM/dd/yyyy HH:mm "),
                            STAN = p.Field<string>("STAN"),
                            STATUS = p.Field<System.String>("ReconType"),
                         
                        }).ToList();


    

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult ReconciliationsReport(string DateRange, string TIDS)
        {

            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            var Where = " WHERE TID is not null   ";

            if (!string.IsNullOrEmpty(DateRange))
            {

                String[] substrings = DateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));

                Where += " and EndDateTime between @date1 and @date2 ";
            }
            else
            {
                return null;

            }
            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));

            DataTable Final = db.populate(" select A.TID,A.EndDateTime as [Date Time],A.STAN,ReconType as Status from Reconciliations_Tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDS.Replace(",", "','") + "') */" + Where +
                                            " ", sqlParams.ToArray());


            Final.TableName = "Final_Report";

            var dataExcel = CreateExcelBook.ConvertDataExcel(Final, "Reconciliations Report", "_ITS_Reconciliations_Report",null);
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = dataExcel.Item2;
            
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = dataExcel.Item1 }
            };

        }


        public ActionResult ReconciliationsTotalReport()
        {

            return View();
        }

        [HttpPost]
        public ActionResult LoadReconciliationsTotalReport()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            //SORT
            string Sort = "";
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                Sort = "Order By " + sortColumn + " " + sortColumnDir;
            }

            //Global search field
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();


            //find search columns info
            var dateRange = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var gSearch = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            var TIDs = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var ColumnName = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            ColumnName = ColumnName == ""?"TID": ColumnName;


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;



            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();


            var Where = "WHERE ( "+ColumnName+"  LIKE '%" + gSearch.ToString() + "%')";




            if (!string.IsNullOrEmpty(dateRange))
            {

                String[] substrings = dateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));

                Where += " and StartDateTime between @date1 and @date2 ";
            }

            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));





            recordsTotal = (int)db.populate("with dataCount as(select sum(cast(AuthorisationCount as numeric)) AuthorisationCount from Reconciliations_Tbl A inner join Reconciliations_Totals_Tbl B on A.Id=b.RefId  /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where+ "group by A.TID,a.StartDateTime,a.ReconType,SchemaID,TotalType) select COUNT(*) from dataCount", sqlParams.ToArray()).Rows[0][0];

            DataTable Final = db.populate("select A.TID, a.StartDateTime,a.ReconType,SchemaID,TotalType" +
                                    ", sum(CONVERT(DECIMAL(13, 2) ,STUFF(DebitAmount, 14, 0, '.'))) DebitAmount,sum(cast(DebitCount as numeric)) DebitCount" +
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CreditAmount, 14, 0, '.'))) CreditAmount, sum(cast(CreditCount as numeric)) CreditCount" +
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CashBackAmount, 14, 0, '.'))) CashBackAmount" +
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CashAdvanceAmount, 14, 0, '.'))) CashAdvanceAmount" +
                                    ", sum(cast(AuthorisationCount as numeric)) AuthorisationCount" +
                                    " from Reconciliations_Tbl A inner join Reconciliations_Totals_Tbl B on A.Id=b.RefId " + Where +
                                    " group by A.TID,a.StartDateTime,a.ReconType,SchemaID,TotalType "  
                                    + Sort + " OFFSET " + skip + " ROWS" + " FETCH NEXT " + pageSize + " ROWS ONLY ", sqlParams.ToArray());



            Final.TableName = "Final_Report";


            var data = (from p in Final.AsEnumerable()
                        select new
                        {
                            //Id = p.Field<System.Int64>("TID"),
                            TID = p.Field<System.Int64>("TID"),
                            StartDateTime = p.Field<System.DateTime>("StartDateTime").ToString("MM/dd/yyyy HH:mm"),
                            ReconType = p.Field<string>("ReconType"),
                            SchemaID = p.Field<string>("SchemaID"),
                            TotalType = p.Field<string>("TotalType"),
                            DebitAmount = p.Field<System.Decimal> ("DebitAmount").ToString("G"),
                            DebitCount = p.Field<System.Decimal>("DebitCount").ToString("G"),
                            CreditAmount = p.Field<System.Decimal>("CreditAmount").ToString("G"),
                            CreditCount = p.Field<Decimal> ("CreditCount").ToString("G"),
                            CashBackAmount = p.Field<System.Decimal>("CashBackAmount").ToString("G"),
                            CashAdvanceAmount = p.Field<System.Decimal>("CashAdvanceAmount").ToString("G"),
                            AuthorisationCount = p.Field<Decimal>("AuthorisationCount").ToString("G"),

                        }).ToList();




            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult ReconciliationsTotalReport(string DateRange, string TIDS)
        {

            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            var Where = " WHERE TID is not null   ";

            if (!string.IsNullOrEmpty(DateRange))
            {

                String[] substrings = DateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));

                Where += " and StartDateTime between @date1 and @date2 ";
            }
            else
            {
                return null;

            }
            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));

            DataTable Final = db.populate("select A.TID, a.StartDateTime, a.ReconType, SchemaID, TotalType" +
                                    ", sum(CONVERT(DECIMAL(13, 2) ,STUFF(DebitAmount, 14, 0, '.'))) DebitAmount,sum(cast(DebitCount as numeric)) DebitCount" +
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CreditAmount, 14, 0, '.'))) CreditAmount, sum(cast(CreditCount as numeric)) CreditCount" +
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CashBackAmount, 14, 0, '.'))) CashBackAmount" +
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CashAdvanceAmount, 14, 0, '.'))) CashAdvanceAmount" +
                                    ", sum(cast(AuthorisationCount as numeric)) AuthorisationCount" +
                                    " from Reconciliations_Tbl A inner join Reconciliations_Totals_Tbl B on A.Id=b.RefId " + Where +
                                    " group by A.TID,a.StartDateTime,a.ReconType,SchemaID,TotalType ", sqlParams.ToArray());


            Final.TableName = "Final_Report";

            var dataExcel = CreateExcelBook.ConvertDataExcel(Final, "ReconciliationsTotal Report", "_ITS_ReconciliationsTotal_Report", null);
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = dataExcel.Item2;

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = dataExcel.Item1 }
            };

        }


        public ActionResult TerminalsDetails()
        {

            return View();

        }


        [HttpPost]
        public ActionResult LoadTerminalsDetails()
        {


            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            //Global search field
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();


            //find search columns info
           
            var gSearch = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            var TIDs = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;



            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Clear();

            var Where = " WHERE ( TID  LIKE '%" + gSearch.ToString() + "%')  ";




            

            //get customer ID
            sqlParams.Add(new SqlParameter("@CustomerId", User.Identity.GetCustomerId()));
            


            recordsTotal = (int)db.populate("select count(*) from TerminalsInfo_Tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where, sqlParams.ToArray()).Rows[0][0];

            DataTable objDt = db.populate(" select A.*,(select max(StartDateTime) from Transactions_tbl where TID=A.TID) as LastTrx,(select max(EndDateTime) from Reconciliations_Tbl where TID=A.TID) as LastReconciliation " +
                                          " from TerminalsInfo_Tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId /*and B.ID in ('" + TIDs.Replace(",", "','") + "')*/ " + Where +
                                          " order by TID" +
                                          " OFFSET " + skip + " ROWS" +
                                          " FETCH NEXT " + pageSize + " ROWS ONLY ", sqlParams.ToArray());
            objDt.TableName = "myTable";


            var data = (from p in objDt.AsEnumerable()
                        select new
                        {
                            TID = p.Field<System.Int64>("TID"),
                            MID = p.Field<System.Int64>("MID"),
                            MerchantName = p.Field<string>("MerchantName"),
                            MerchantAddress = p.Field<string>("MerchantAddress"),
                            MerchantPhone = p.Field<string>("MerchantPhone"),
                            BankName = p.Field<string>("acqBankName"),
                            LastTrx = p["LastReconciliation"] == DBNull.Value ? "-" : p.Field<System.DateTime>("LastTrx").ToString("MM/dd/yyyy HH:mm "),
                            LastReconciliation = p["LastReconciliation"] == DBNull.Value ? "-" : p.Field<System.DateTime>("LastReconciliation").ToString("MM/dd/yyyy HH:mm "),

                        }).ToList();




            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult TerminalsDetailsReport( string TIDS)
        {

            //SEARCHING...
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Clear();
            var Where = " WHERE TID is not null   ";

     
            //get customer ID
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));


            DataTable Final = db.populate(" select  A.TID as [Terminal ID] , A.MID as [Merchent ID] ,A.MerchantName,A.MerchantAddress,A.MerchantPhone,A.acqBankName "+
                " ,(select max(StartDateTime) from Transactions_tbl where TID=A.TID) as LastTrx,(select max(EndDateTime) from Reconciliations_Tbl where TID=A.TID) as LastReconciliation " +
                " from TerminalsInfo_Tbl A inner join  CustomerStructures B on A.TID=B.[Name] and B.IsTID=1 and B.CustomerId=@CustomerId and B.ID in ('" + TIDS.Replace(",", "','") + "') " + Where +
                                         " ", sqlParams.ToArray(),true);


            Final.TableName = "Final_Report";

            var dataExcel = CreateExcelBook.ConvertDataExcel(Final, "Terminals Info. Report", "_ITS_TerminalsInfo_Report",null);
            // Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = dataExcel.Item2;

            //// Note we are returning a filename as well as the handle
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = dataExcel.Item1 }
            };
        }


        public ActionResult PrintReconcilationRecipt(Int64 Id)
        {
            DataTable Final = db.populate("select a.StartDateTime,a.ReconType,SchemaID,TotalType" +
                                    ", sum(CONVERT(DECIMAL(13, 2) ,STUFF(DebitAmount, 14, 0, '.'))) DebitAmount,sum(cast(DebitCount as numeric)) DebitCount"+
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CreditAmount, 14, 0, '.'))) CreditAmount, sum(cast(CreditCount as numeric)) CreditCount"+
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CashBackAmount, 14, 0, '.'))) CashBackAmount"+
                                    ", sum(CONVERT(DECIMAL(13, 2), STUFF(CashAdvanceAmount, 14, 0, '.'))) CashAdvanceAmount"+
                                    ", sum(cast(AuthorisationCount as numeric)) AuthorisationCount"+
                " from Reconciliations_Tbl A inner join Reconciliations_Totals_Tbl B on A.Id=b.RefId where A.TID='" + Id + "' group by a.StartDateTime,a.ReconType,SchemaID,TotalType", null,true);



            Final.TableName = "Final_Report";



            return View();

        }


        public FileStreamResult  pdfGenerator(FormCollection formCollection)
        {
            ///this action convert html file to pdf through Nreco.dll free 
            

            var ms = new MemoryStream();
            try
            {
                //var RRN = TempData["RRN"];
                var RRN = formCollection["hdn_rrnID"];
                var url = BaseURL + "Reports/PrintRecipt?RRN=" + RRN;
                ms = ConvertToPDF.pdfConvert(url);
            }
            catch (Exception){
            }

            //TempData["RRN"] = null;
            return  new FileStreamResult(ms, "application/pdf");
        }
    }
}