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
    public partial class VisitDetails : Form
    {
        public VisitDetails()
        {
            InitializeComponent();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            crystalReportViewer1.Visible = false;
            dataGridView1.Visible = true;
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Get_Visit_report";

            cmd.Parameters.Add("@Dt_From", SqlDbType.DateTime).Value = dtFrom.Value.ToString("yyyy-MM-dd HH:mm:00");
            cmd.Parameters.Add("@Dt_To", SqlDbType.DateTime).Value = dtTo.Value.ToString("yyyy-MM-dd HH:mm:00");

            var t = dtTo.Value.ToString("yyyy-MM-dd HH:mm:00");

            Connections.Instance.OpenConection();
            cmd.Connection = Connections.Instance.con;
            //Connections.Instance.ExecuteProcedure(cmd);

            dt.Load(cmd.ExecuteReader());
            dataGridView1.DataSource = dt;

            dt.Dispose();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
            crystalReportViewer1.Visible = false;
            dataGridView1.Visible = true;

        }

        private void VisitDetails_Load(object sender, EventArgs e)
        {
            btnShow_Click(null, null);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            crystalReportViewer1.Visible = true;
            dataGridView1.Visible = false;

            DataSet1 ds = new DataSet1();
            DataTable dt = new DataTable();
            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "Get_Visit_report";

            //Connections.Instance.OpenConection();
            //cmd.Connection = Connections.Instance.con;
            //Connections.Instance.ExecuteProcedure(cmd);

            dt.Merge((DataTable)dataGridView1.DataSource);

            ds.Tables["Visit"].Clear();
            ds.Tables["Visit"].Merge(dt);

            CrystalDecisions.CrystalReports.Engine.ReportDocument cryRpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

            string ReportName = System.Configuration.ConfigurationSettings.AppSettings["VisitReport"].ToString();
            cryRpt.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath).ToString() + ReportName);

            cryRpt.SetDataSource(ds);
            cryRpt.DataDefinition.FormulaFields[0].Text = "'" + dtFrom.Value.ToString("dd-MM-yy hh:mmtt") + "'";
            cryRpt.DataDefinition.FormulaFields[1].Text = "'" + dtTo.Value.ToString("dd-MM-yy hh:mmtt") + "'";

            cryRpt.Refresh();

            //cryRpt.PrintToPrinter(1, true, 0, 0);

            CrystalDecisions.CrystalReports.Engine.ReportDocument rpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            crystalReportViewer1.ReportSource = cryRpt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
