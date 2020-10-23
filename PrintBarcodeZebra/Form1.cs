using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintBarcodeZebra
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = true;
            zplTBox.Text = @"CT~~CD,~CC^~CT~
^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR2,2~SD30^JUS^LRN^CI0^XZ
^XA
^MMT
^PW244
^LL0071
^LS0
^BY20,20^FT213,10^BXI,2,200,0,0,1,~
^FH\^FDSN^FS
^FT166,36^A0I,8,14^FH\^FDPAR0^FS
^FT166,26^A0I,8,14^FH\^FDPAR1^FS
^PQ1,0,1,Y^XZ
";
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (snBox.Text.Length > 15)
            {
                string dataToSend = GetZplData(snBox.Text);
                // Allow the user to select a printer.
                if (tboxPrinter.Text == "")
                {
                    PrintDialog pd = new PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    if (DialogResult.OK == pd.ShowDialog(this))
                    {
                        tboxPrinter.Text = pd.PrinterSettings.PrinterName;
                    }
                }
                RawPrinterHelper.SendStringToPrinter(tboxPrinter.Text, dataToSend);
                snBox.Text = "";
                snBox.Focus();
                //PrintDialog pd = new PrintDialog();
                //pd.PrinterSettings = new PrinterSettings();
                //if (DialogResult.OK == pd.ShowDialog(this))
                //{
                //    // Send a printer-specific to the printer.
                //    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, dataToSend);
                    
                //    //MessageBox.Show("Data sent to printer.");
                //    snBox.Clear();
                //}
                //else
                //{
                //    MessageBox.Show("Data not sent to printer.");
                //}
            }
            
        }

        private string GetZplData(string serialNumber)
        {
            StringBuilder zpl = new StringBuilder();
            //zpl.Append(@"CT~~CD,~CC^~CT~
            //            ^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR2,2~SD30^JUS^LRN^CI0^XZ
            //            ^XA
            //            ^MMT
            //            ^PW243
            //            ^LL0070
            //            ^LS0
            //            ^FT169,58^BQN,2,2
            //            ^FH\^FDMA,<PARAM>^FS
            //            ^FT165,26^A0I,8,14^FH\^FD<PARAM1>^FS
            //            ^FT165,16^A0I,8,14^FH\^FD<PARAM2>^FS
            //            ^PQ1,0,1,Y^XZ
            //            ");
            zpl.Append(zplTBox.Text);

            string param1 = serialNumber.Substring(0, 10);
            string param2 = serialNumber.Substring(10);

            zpl.Replace("SN", serialNumber);
            zpl.Replace("PAR0", param1);
            zpl.Replace("PAR1", param2);

            return zpl.ToString();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            HideControls();
            lblClose.Show();
        }
        
        private void HideControls()
        {
            foreach (Control ctrl in panelMain.Controls)
            {
                ctrl.Hide();
            }
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void snBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void snBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnPrint.PerformClick();
                
            }
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelMain_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelSettings_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panelSettings_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                zplTBox.Enabled = false;
            }
            else
            {
                zplTBox.Enabled = true;
            }
        }

        private void zplTBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
