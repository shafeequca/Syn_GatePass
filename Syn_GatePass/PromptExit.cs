using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Syn_GatePass
{
    public static class PromptExit
    {
        public static void ShowDialog(ref Int32 PassNo,ref DateTime ExitTime)
        {
            Form PromptExit = new Form()
            {
                Width = 300,
                Height = 300,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = System.Drawing.Color.MidnightBlue,
                Text = "Exit Entry",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = "Pass Number" };
            textLabel.ForeColor = System.Drawing.Color.White;

            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };

            Label textLabel2 = new Label() { Left = 50, Top = 70, Text = "Exit Time" };
            textLabel2.ForeColor = System.Drawing.Color.White;

            DateTimePicker DtPicker = new DateTimePicker() { Left = 50, Top = 100, Width = 200 };
            DtPicker.CustomFormat = "dd-MM-yyyy HH:mm tt";
            DtPicker.Format = DateTimePickerFormat.Custom;
            DtPicker.Value = DateTime.Now;

            Button confirmation = new Button() { Text = "Ok", Left = 50, Width = 100, Top = 130, DialogResult = DialogResult.OK };
            confirmation.ForeColor = System.Drawing.Color.White;
            confirmation.Click += (sender, e) => { PromptExit.Close(); };
            PromptExit.Controls.Add(textBox);
            PromptExit.Controls.Add(confirmation);
            PromptExit.Controls.Add(textLabel);
            PromptExit.Controls.Add(textLabel2);
            PromptExit.Controls.Add(DtPicker);
            PromptExit.AcceptButton = confirmation;
            int i = PromptExit.ShowDialog() == DialogResult.OK ? 1 : 0;
            if (i == 1)
            {
                PassNo = Convert.ToInt32(textBox.Text);
                ExitTime = DtPicker.Value;
            }
        }
    }
}
