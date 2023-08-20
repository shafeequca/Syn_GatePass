using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Syn_GatePass
{
    public sealed class Connections
    {
        private static Connections instance = null;


        string ConnectionString = @"Data Source=" + System.Configuration.ConfigurationSettings.AppSettings["Server"] + ";Initial Catalog=" + System.Configuration.ConfigurationSettings.AppSettings["Catalog"] + ";Integrated Security=True;";
        string App_Type = System.Configuration.ConfigurationSettings.AppSettings["App_Type"];

        //string ConnectionString = @"Data Source=tcp:192.168.100.12;Initial Catalog=Inventory_GST;User ID=sa;Password=INS";

        //string ConnectionString = @"Data Source=APPU-PC;Initial Catalog=Synthite_Canteen;Integrated Security=True;";

        public SqlConnection con;
        public string user_type;
        public string user_id = System.Configuration.ConfigurationSettings.AppSettings["default_user_id"];
        public string user_name;
        public string Yard_Id = System.Configuration.ConfigurationSettings.AppSettings["Yard"];
        public static Connections Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new Connections();
                }
                return instance;
            }
        }
        public void Set_Connenction_String()
        {
            if (System.Configuration.ConfigurationSettings.AppSettings["Server_Mode"] == "Windows")
            {
                ConnectionString = @"Data Source=" + System.Configuration.ConfigurationSettings.AppSettings["Server"] + ";Initial Catalog=" + System.Configuration.ConfigurationSettings.AppSettings["Catalog"] + ";Integrated Security=True;";
            }
            else
            {
                ConnectionString = @"Data Source=" + System.Configuration.ConfigurationSettings.AppSettings["Server"] + ";Initial Catalog=" + System.Configuration.ConfigurationSettings.AppSettings["Catalog"] + ";User ID=" + System.Configuration.ConfigurationSettings.AppSettings["uid"] + ";Password=" + System.Configuration.ConfigurationSettings.AppSettings["psd"] + "";
            }
        }
        public void OpenConection()
        {
            Set_Connenction_String();
            con = new SqlConnection(ConnectionString);
            con.Open();
        }


        public void CloseConnection()
        {
            con.Close();
        }

        public void ExecuteProcedure(SqlCommand SqlCmd)
        {
            Set_Connenction_String();
            SqlCmd.Connection = con;
            SqlCmd.ExecuteNonQuery();
        }


        public void ExecuteQueries(string Query_)
        {
            Set_Connenction_String();
            SqlCommand cmd = new SqlCommand(Query_, con);
            cmd.ExecuteNonQuery();
        }

        public Int32 ExuecuteQueryWithReturn(string Query_)
        {
            Set_Connenction_String();
            using (SqlCommand cmd = new SqlCommand(Query_, con))
            {
                int modified = (int)cmd.ExecuteScalar();
                return modified;
            }

        }

        public SqlDataReader DataReader(string Query_)
        {
            Set_Connenction_String();
            SqlCommand cmd = new SqlCommand(Query_, con);
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        public object ShowDataInGridView(string Query_)
        {
            Set_Connenction_String();
            SqlDataAdapter dr = new SqlDataAdapter(Query_, ConnectionString);
            System.Data.DataSet ds = new System.Data.DataSet();
            dr.Fill(ds);
            object dataum = ds.Tables[0];
            return dataum;
        }
    }
}
