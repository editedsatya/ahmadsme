using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Xml;
using System.Web.Script.Services;

namespace TRS_Web_Service
{
    /// <summary>
    /// Summary description for WebServicePOS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]


    public class WebServicePOS : System.Web.Services.WebService
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
        public XmlElement WS_TransactionBatch(string TrxType, string TrxDetails)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xmlNodeResponce = xmlDoc.CreateElement("TRS");
            xmlNodeResponce.InnerText = "";

            XmlAttribute xmlAttributeResponce = xmlDoc.CreateAttribute("ResponseCode");
            xmlNodeResponce.Attributes.Append(xmlAttributeResponce);
            xmlDoc.AppendChild(xmlNodeResponce);


            string Result = "000";

            try
            {
                log.Info("Start");

                log.Info("TrxType=" + TrxType + ",TrxDetails=" + TrxDetails + "");


                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;










                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //string query = "insert into TransactionHistory(TrxType,TrxDetails)values(@TrxType,@TrxDetails)";
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SaveTranasction", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TrxType", TrxType);
                        command.Parameters.AddWithValue("@TrxDetails", TrxDetails);

                        var returnParameter = command.Parameters.Add("@ReturnVal", System.Data.SqlDbType.Int);
                        returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();

                        // returnParameter.Value;
                        if ((int)returnParameter.Value == 0)
                        {

                            Result = "000";
                        }
                        else
                        {
                            Result = "002";
                            log.Error("Check ErrorsLog_Tbl Table");
                        }

                    }

                    connection.Close();
                }

                log.Info("End , Return =" + Result);


                xmlAttributeResponce.Value = Result;

                return xmlDoc.DocumentElement;



            }
            catch (Exception ex)
            {

                log.Error(ex.Message, ex);

                Result = "0001";
                xmlAttributeResponce.Value = Result;

                return xmlDoc.DocumentElement;
            }




        }


    }


}
