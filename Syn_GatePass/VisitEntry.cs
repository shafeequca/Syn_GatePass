using CrystalDecisions.CrystalReports.Engine;
using Syn_GatePass.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Imaging;

namespace Syn_GatePass
{
    public partial class VisitEntry : Form
    {
        
        public VisitEntry()
        {
            InitializeComponent();
        }

        private void VisitEntry_Load(object sender, EventArgs e)
        {
            btnSave.Tag = "";
            Color clr = this.BackColor;
            GetVisitorData();
            GetPersonToVisit();
            GetPurposeToVisit();
            //   PrintReport(14);


        }
        private void PrintReport(Int32 i)
        {
            DataSet1  ds = new DataSet1();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Get_Visit";
            cmd.Parameters.Add("@Passid", SqlDbType.Int).Value = i;

            Connections.Instance.OpenConection();
            cmd.Connection = Connections.Instance.con;

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            string ReportName = System.Configuration.ConfigurationSettings.AppSettings["ReceiptSlip"].ToString();

            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath).ToString() + ReportName);
            cryRpt.DataSourceConnections.Clear();
            cryRpt.Refresh();
            ds.Tables["Visit"].Clear();
            ds.Tables["Visit"].Merge(dt);

            cryRpt.SetDataSource(ds);
            cryRpt.Refresh();
            cryRpt.VerifyDatabase();
            cryRpt.ReadRecords();
            cryRpt.Refresh();
            cryRpt.PrintToPrinter(1, true, 0, 0);
  
            cryRpt.Close();
            cryRpt.Dispose();
       
            dt.Dispose();
            Connections.Instance.CloseConnection();
            cmd.Dispose();
            ds.Dispose();
        }
        private void GetVisitorData()
        {
            GridView.DataSource = null;
            string query = "SELECT TOP 500 PassID,VisitorName,Address,ContactNumber,Purpose,PersonToVisit,VisitType,Remarks,ImagePath FROM tblVisit WHERE VisitorName LIKE '" + txtSearch.Text.Trim() + "%' OR PassID like '%" + txtSearch.Text.Trim() + "%' ORDER BY 1 DESC";
            GridView.DataSource = Connections.Instance.ShowDataInGridView(query);
            GridView.Columns[0].Visible = false;
            GridView.Columns[4].Visible = false;
            GridView.Columns[5].Visible = false;
            GridView.Columns[6].Visible = false;
            GridView.Columns[7].Visible = false;
            GridView.Columns[8].Visible = false;

        }
        private void GetPersonToVisit()
        {
            cboPerson.DataSource = null;
            string query = "SELECT Id,PersonToVisit From tblPersonToVisit ORDER BY 2";
            cboPerson.DataSource = Connections.Instance.ShowDataInGridView(query);
            cboPerson.DisplayMember = "PersonToVisit";
            cboPerson.ValueMember = "Id";
            cboPerson.SelectedIndex = -1;
            cboPerson.Text = "";

        }
        private void GetPurposeToVisit()
        {
            cboPurpose.DataSource = null;
            string query = "SELECT Id,Purpose From tblPurposeToVisit ORDER BY 2";
            cboPurpose.DataSource = Connections.Instance.ShowDataInGridView(query);
            cboPurpose.DisplayMember = "Purpose";
            cboPurpose.ValueMember = "Id";
            cboPurpose.SelectedIndex = -1;
            cboPurpose.Text = "";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (txtName.Text == "")
            {
                MessageBox.Show("Please enter the name of visitor");
                txtName.Focus();
                return;
            }
            else if (txtPath.Text == "")
            {
                MessageBox.Show("Please select visitor's photo");
                btnBrowse_Click(null, null);
                return;
            }
            else if (txtAddress.Text == "")
            {
                MessageBox.Show("Please enter the address of visitor");
                txtAddress.Focus();
                return;
            }
            else if (txtContactNo.Text == "")
            {
                MessageBox.Show("Please enter the contact number of visitor");
                txtContactNo.Focus();
                return;
            }
            else if (cboPurpose.Text=="")
            {
                MessageBox.Show("Please select purpose of visit");
                cboPurpose.Focus();
                return;
            }
            else if (cboPerson.Text == "")
            {
                MessageBox.Show("Please select person to visit");
                cboPerson.Focus();
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Do you want to save the entry?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                btnSave.Tag = "SAVE";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_Visit";


                cmd.Parameters.Add("@VisitDate", SqlDbType.Date).Value = DtDateTime.Value.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@VisitTime", SqlDbType.Time).Value = DtDateTime.Value.ToString("HH:mm:00");
                cmd.Parameters.Add("@VisitorName", SqlDbType.VarChar).Value = txtName.Text.Trim();
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = txtAddress.Text.Trim();
                cmd.Parameters.Add("@ContactNumber", SqlDbType.VarChar).Value = txtContactNo.Text.Trim();

                MemoryStream ms1 = new MemoryStream();
                Image img = Image.FromFile(txtPath.Text);
                img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] img_arr1 = new byte[ms1.Length];
                ms1.Read(img_arr1, 0, img_arr1.Length);
                cmd.Parameters.Add("@VisitorPhoto", SqlDbType.Image).Value = img_arr1;
                cmd.Parameters.Add("@Purpose", SqlDbType.VarChar).Value = cboPurpose.Text.Trim();
                cmd.Parameters.Add("@PersonToVisit", SqlDbType.VarChar).Value = cboPerson.Text.Trim();
                cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value =txtRemarks.Text.Trim();
                cmd.Parameters.Add("ImagePath", SqlDbType.VarChar).Value = txtPath.Text.Trim();

                if (rbtSingle.Checked == true)
                {
                    cmd.Parameters.Add("@VisitType", SqlDbType.VarChar).Value = "Single";
                }
                else
                {
                    cmd.Parameters.Add("@VisitType", SqlDbType.VarChar).Value = "Group";
                }

                SqlParameter PassId = new SqlParameter("@PassId", SqlDbType.Int);

                PassId.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(PassId);

                Connections.Instance.OpenConection();
                Connections.Instance.ExecuteProcedure(cmd);
                Int32 i = Convert.ToInt32(PassId.Value);
                Connections.Instance.CloseConnection();
                cmd.Dispose();
                ms1.Dispose();
                img.Dispose();

                btnClear_Click(null, null);

                PrintReport(i);

            }
        }
        
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select Photo";
            openFileDialog1.FileName = "Select Photo";
            openFileDialog1.DefaultExt = "JPG";
            openFileDialog1.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                //foreach (String file in openFileDialog1.FileNames)
                //{
                try
                {
                    Picturebox.Controls.Clear();
                    txtPath.Text = openFileDialog1.FileName;

                    //Picturebox.Image = Image.FromFile(openFileDialog1.FileName);

                    PictureBox imageControl = new PictureBox();
                    imageControl.Height = Picturebox.Height;
                    imageControl.Width = Picturebox.Width;


                    Image.GetThumbnailImageAbort myCallback =
                            new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    Bitmap myBitmap = new Bitmap(openFileDialog1.FileName);
                    Image myThumbnail = myBitmap.GetThumbnailImage(Picturebox.Width, Picturebox.Height,
                        myCallback, IntPtr.Zero);
                    imageControl.Image = myThumbnail;

                    Picturebox.Controls.Add(imageControl);
                    myBitmap.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                //}

            }
            openFileDialog1.Dispose();


        }
        public bool ThumbnailCallback()
        {
            return false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

            Picturebox.Controls.Clear();
            // PrintReport(Convert.ToInt32(txtPath.Text));
            txtName.Text = "";
            txtSearch.Text = "";
            txtPath.Text = "";
            if (rbtSingle.Checked == true || btnSave.Tag.ToString()=="")
            {
                rbtSingle.Checked = true;
                DtDateTime.Value = DateTime.Now;
                Picturebox.Image = null;
                txtAddress.Text = "";
                txtContactNo.Text = "";
                txtRemarks.Text = "";
                cboPerson.Text = "";
                cboPerson.SelectedIndex = -1;
                cboPurpose.Text = "";
                cboPurpose.SelectedIndex = -1;
                GetVisitorData();
                GetPersonToVisit();
                GetPurposeToVisit();
            }
            btnSave.Tag = "";
            txtName.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetVisitorData();
        }

        private void btnreprint_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Gate Pass Number","Print");
            if (promptValue != "")
            {
                try
                {
                    PrintReport(Convert.ToInt32(promptValue));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid Number!");
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 PassNo = 0;
                DateTime dtExitTime = DateTime.Now;
                PromptExit.ShowDialog(ref PassNo, ref dtExitTime);
                if (PassNo != 0)
                {

                    DialogResult dialogResult = MessageBox.Show("Do you want to update the exit?", "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {

                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Visit_Exit";

                        cmd.Parameters.Add("@Passid", SqlDbType.Int).Value = PassNo;
                        cmd.Parameters.Add("@ExitTime", SqlDbType.DateTime).Value = dtExitTime;

                        SqlParameter Status_Message = new SqlParameter("@Status_Message", SqlDbType.VarChar);
                        Status_Message.Size = 100;
                        Status_Message.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(Status_Message);

                        Connections.Instance.OpenConection();
                        Connections.Instance.ExecuteProcedure(cmd);
                        
                        MessageBox.Show(Status_Message.Value.ToString());

                        Connections.Instance.CloseConnection();

                        cmd.Dispose();

                        btnClear_Click(null, null);


                    }
                }
                else
                {
                    MessageBox.Show("Please enter proper values");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Number!");
            }
   
        }
        
        private void GridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int rowno = e.RowIndex;
            DtDateTime.Value = DateTime.Now;
            txtName.Tag = GridView.Rows[rowno].Cells[0].Value.ToString();
            txtName.Text = GridView.Rows[rowno].Cells[1].Value.ToString();
            txtAddress.Text = GridView.Rows[rowno].Cells[2].Value.ToString();
            txtContactNo.Text = GridView.Rows[rowno].Cells[3].Value.ToString();
            cboPurpose.Text = GridView.Rows[rowno].Cells[4].Value.ToString();
            cboPerson.Text = GridView.Rows[rowno].Cells[5].Value.ToString();
            txtRemarks.Text= GridView.Rows[rowno].Cells[7].Value.ToString();
            txtPath.Text = GridView.Rows[rowno].Cells[8].Value.ToString();

            if (GridView.Rows[rowno].Cells[6].Value.ToString() == "Single")
            {
                rbtSingle.Checked = true;
            }
            else
            {
                rbtGroup.Checked = true;
            }

            try
            {
                Picturebox.Controls.Clear();
                

                //Picturebox.Image = Image.FromFile(openFileDialog1.FileName);

                PictureBox imageControl = new PictureBox();
                imageControl.Height = Picturebox.Height;
                imageControl.Width = Picturebox.Width;


                Image.GetThumbnailImageAbort myCallback =
                        new Image.GetThumbnailImageAbort(ThumbnailCallback);
                Bitmap myBitmap = new Bitmap(txtPath.Text);
                Image myThumbnail = myBitmap.GetThumbnailImage(Picturebox.Width, Picturebox.Height,
                    myCallback, IntPtr.Zero);
                imageControl.Image = myThumbnail;

                Picturebox.Controls.Add(imageControl);
                myBitmap.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void rbtSingle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void NonExitReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NonExit frm = new NonExit();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void VisitReportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            VisitDetails frm = new VisitDetails();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void masterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
