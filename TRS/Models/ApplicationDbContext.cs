using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TRS.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public System.Data.Entity.DbSet<TRS.Models.Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<TRS.Models.CustomerStructure> CustomerStructures { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class DataAccessLayer
    {




        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public DataTable populate(string query, SqlParameter[] parm = null, bool InitialRowNull = false)
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            try
            {

                if (parm != null)
                {

                    da.SelectCommand.Parameters.AddRange(parm);

                }
                da.Fill(dt);
                if (dt.Rows.Count == 0 && InitialRowNull)
                {
                    dt.Rows.Add(dt.NewRow());
                }
                da.SelectCommand.Parameters.Clear();


            }
            catch (Exception /*ex*/)
            {
                dt = null;
                if (con.State == ConnectionState.Open)
                    con.Close();

            }
            finally
            {
                if (con != null)
                    if (con.State == ConnectionState.Open) con.Close();


                con.Dispose();
                da.Dispose();

                //if (da.SelectCommand != null)
                //    da.SelectCommand.Dispose();
            }

            return dt;


        }



    }

}