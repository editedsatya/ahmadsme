using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using TRS.Extensions;
using TRS.Models;
using System.Linq;
using TRS.ViewModel;
using Newtonsoft.Json;
using System.Globalization;

namespace TRS.Controllers
{
    public class HomeController : BaseController
    {
        Models.DataAccessLayer db = new Models.DataAccessLayer();
        DataTable _dt;

        public ActionResult DashboardV1(string Daterange)
        {
            ReportsViewModel reportsViewModel = new ReportsViewModel();
            try
            {

           
            var data = GetRecords(Daterange);

            var settledTransaction = data.Tables[1];
            var SettledAmount = Convert.ToString(data.Tables[2].Rows[0][0]);
            var donuteData = data.Tables[3];
            var Branch = data.Tables[4];
            var dataPoints= data.Tables[5];

            List<Transaction> tranactionList = new List<Transaction>();
            tranactionList = (from DataRow row in data.Tables[0].Rows

                              select new Transaction()
                              {
                                  id = Convert.ToInt64(row["ID"]),
                                  TID = Convert.ToInt64(row["TID"]),
                                  RRN = Convert.ToInt64(row["RRN"]),
                                  Amount = Convert.ToDecimal(row["Amount"]),
                                  Status = Convert.ToInt16(row["Status"]),

                              }).ToList();


            List<Point> _pointList = new List<Point>();
            _pointList = (from DataRow row in data.Tables[5].Rows

                              select new Point()
                              {
                                  label = Convert.ToString(row["label"]),
                                  y = Convert.ToDouble(row["y"]),

                              }).ToList();

            // here records categorised by Total & success & failed and pass to view according!!
            var SuccessData = tranactionList.Where(x => x.Status == 0).ToList();
            var FailedData = tranactionList.Where(x => x.Status != 0).ToList();

            ModelState.Clear();
          
            var pointsDate = JsonConvert.SerializeObject(_pointList, _jsonSetting);
            ViewBag.Points= pointsDate;
          
            reportsViewModel.AllTranactionCount = tranactionList.Count;
            reportsViewModel.SuccessTranactionCount = SuccessData.Count;
            reportsViewModel.FailedTranactionCount = FailedData.Count;
            reportsViewModel.SelttedTranactionCount = settledTransaction.Rows.Count;

            // Transaction Data list
            reportsViewModel._All = tranactionList;
            reportsViewModel._Success = SuccessData;
            reportsViewModel._Failed = FailedData;

            //Schema Name
            reportsViewModel.MADA = "0";
            reportsViewModel.VISA = "0";
            reportsViewModel.VISACredit = "0";
            reportsViewModel.VISADebit = "0";


            if (donuteData.Rows.Count > 0)
                reportsViewModel.MADA = Convert.ToString(donuteData.Rows[0][0] == null ? "0" : donuteData.Rows[0][0]);

            if (donuteData.Rows.Count > 1)
                reportsViewModel.VISA = Convert.ToString(donuteData.Rows[1][0] == null ? "0" : donuteData.Rows[1][0]);

            if (donuteData.Rows.Count > 2)
                reportsViewModel.VISACredit = Convert.ToString(donuteData.Rows[2][0] == null ? "0" : donuteData.Rows[2][0]);

            if (donuteData.Rows.Count > 3)
                reportsViewModel.VISADebit = Convert.ToString(donuteData.Rows[3][0] == null ? "0" : donuteData.Rows[3][0]);

            //Branch
            reportsViewModel.Branch = Branch.Rows.Count;

            //Current Date
            reportsViewModel.currentDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");
            

            //Total sale amount
            reportsViewModel.TotalSaleAMount = SettledAmount;
            }
            catch (Exception){
            }
            return View(reportsViewModel);
        }

        public ActionResult DashboardV2()
        {
            return View();
        }

        #region GetRecords
        // Method gives record daily basis
        public DataSet GetRecords(string dateRange)
        {
            DataSet _dts = new DataSet();
            try
            {
           
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            var monthchar = DateTime.Now.Date.Month;
            var CustomerId = User.Identity.GetCustomerId();
            sqlParams.Add(new SqlParameter("@CustomerId", CustomerId));
            sqlParams.Add(new SqlParameter("@monthChar", monthchar));

            if (!string.IsNullOrEmpty(dateRange))
            {

                String[] substrings = dateRange.Split('-');
                var strDate1 = substrings[0].Trim();
                var strDate2 = substrings[1].Trim();

                DateTime dDate1 = DateTime.ParseExact(strDate1, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                DateTime dDate2 = DateTime.ParseExact(strDate2, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                sqlParams.Add(new SqlParameter("@date1", dDate1));
                sqlParams.Add(new SqlParameter("@date2", dDate2));
                

                // Where += " and StartDateTime between @date1 and @date2 ";
            }
            else
            {
                sqlParams.Add(new SqlParameter("@date1", DateTime.Now.Date));
                sqlParams.Add(new SqlParameter("@date2", DateTime.Now.Date.AddDays(1)));
            }

            _dts = db.Pro_GetAlLTransaction(sqlParams.ToArray());

            }
            catch (Exception){
            }

            return _dts;

        }
        #endregion

        [HttpPost]
        public ActionResult GetDataByDateRange(string Daterange)
        {
            ReportsViewModel reportsViewModel = new ReportsViewModel();

            try
            {

           

            var data = GetRecords(Daterange);

            var settledTransaction = data.Tables[1];
            var SettledAmount = Convert.ToString(data.Tables[2].Rows[0][0]);
            var donuteData = data.Tables[3];
            var Branch = data.Tables[4];
            var dataPoints = data.Tables[5];

            List<Transaction> tranactionList = new List<Transaction>();
            tranactionList = (from DataRow row in data.Tables[0].Rows

                              select new Transaction()
                              {
                                  id = Convert.ToInt64(row["ID"]),
                                  TID = Convert.ToInt64(row["TID"]),
                                  RRN = Convert.ToInt64(row["RRN"]),
                                  Amount = Convert.ToDecimal(row["Amount"]),
                                  Status = Convert.ToInt16(row["Status"]),

                              }).ToList();


            List<Point> _pointList = new List<Point>();
            _pointList = (from DataRow row in data.Tables[5].Rows

                          select new Point()
                          {
                              label = Convert.ToString(row["label"]),
                              y = Convert.ToDouble(row["y"]),

                          }).ToList();

            // here records categorised by Total & success & failed and pass to view according!!
            var SuccessData = tranactionList.Where(x => x.Status == 0).ToList();
            var FailedData = tranactionList.Where(x => x.Status != 0).ToList();

            ModelState.Clear();


            var pointsDate = JsonConvert.SerializeObject(_pointList, _jsonSetting);
            ViewBag.Points = pointsDate;
            reportsViewModel.AllTranactionCount = tranactionList.Count;
            reportsViewModel.SuccessTranactionCount = SuccessData.Count;
            reportsViewModel.FailedTranactionCount = FailedData.Count;
            reportsViewModel.SelttedTranactionCount = settledTransaction.Rows.Count;

            // Transaction Data list
            reportsViewModel._All = tranactionList;
            reportsViewModel._Success = SuccessData;
            reportsViewModel._Failed = FailedData;

            //Schema Name
            reportsViewModel.MADA = "0";
            reportsViewModel.VISA = "0";
            reportsViewModel.VISACredit = "0";
            reportsViewModel.VISADebit = "0";


            if (donuteData.Rows.Count > 0)
                reportsViewModel.MADA = Convert.ToString(donuteData.Rows[0][0] == null ? "0" : donuteData.Rows[0][0]);

            if (donuteData.Rows.Count > 1)
                reportsViewModel.VISA = Convert.ToString(donuteData.Rows[1][0] == null ? "0" : donuteData.Rows[1][0]);

            if (donuteData.Rows.Count > 2)
                reportsViewModel.VISACredit = Convert.ToString(donuteData.Rows[2][0] == null ? "0" : donuteData.Rows[2][0]);

            if (donuteData.Rows.Count > 3)
                reportsViewModel.VISADebit = Convert.ToString(donuteData.Rows[3][0] == null ? "0" : donuteData.Rows[3][0]);

            //Branch
            reportsViewModel.Branch = Branch.Rows.Count;

            //Current Date
            reportsViewModel.currentDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");


            //Total sale amount
            reportsViewModel.TotalSaleAMount = SettledAmount;

            }
            catch (Exception){
            }
            return PartialView(reportsViewModel);
        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }

}