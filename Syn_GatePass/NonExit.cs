using Syn_GatePass.DataSet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Syn_GatePass
{
    public partial class NonExit : Form
    {
        public NonExit()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void NonExit_Load(object sender, EventArgs e)
        {

            dataGridView1.Visible = true;
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Get_Non_Exit_Report";

            Connections.Instance.OpenConection();
            cmd.Connection = Connections.Instance.con;
            //Connections.Instance.ExecuteProcedure(cmd);

            dt.Load(cmd.ExecuteReader());
            dataGridView1.DataSource = dt;

            dt.Dispose();

           // DataSet1 ds = new DataSet1();
           // DataTable dt = new DataTable();
           // SqlCommand cmd = new SqlCommand();
           // cmd.CommandType = CommandType.StoredProcedure;
           // cmd.CommandText = "Get_Non_Exit_Report";

           // Connections.Instance.OpenConection();
           // cmd.Connection = Connections.Instance.con;
           // //Connections.Instance.ExecuteProcedure(cmd);

           // dt.Load(cmd.ExecuteReader());

           // ds.Tables["Visit"].Clear();
           // ds.Tables["Visit"].Merge(dt);

           // CrystalDecisions.CrystalReports.Engine.ReportDocument cryRpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

           // string ReportName = System.Configuration.ConfigurationSettings.AppSettings["NonExitReport"].ToString();

           // cryRpt.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath).ToString() + ReportName);

           // cryRpt.SetDataSource(ds);
           // cryRpt.Refresh();

           // //cryRpt.PrintToPrinter(1, true, 0, 0);

           // CrystalDecisions.CrystalReports.Engine.ReportDocument rpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
           //// crystalReportViewer1.ReportSource = cryRpt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
