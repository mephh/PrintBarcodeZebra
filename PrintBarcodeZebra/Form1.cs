using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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
            firstLineTBox.Text = FileOperations.ReadSetting("firstLine");
            zplTBox.Text = FileOperations.ReadSetting("zplCode");
            if (zplTBox.Text == "Not Found")
            {
                zplTBox.Text = @"CT~~CD,~CC^~CT~
^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR2,2~SD30^JUS^LRN^CI0^XZ
^XA
^MMT
^PW233
^LL0203
^LS0
^FT69,11^BQN,2,3
^FH\^FDMA,SN^FS
^PQ1,0,1,Y^XZ
";
                firstLineTBox.Text = "5";
                btnSave.PerformClick();
            }
        }

        private string GetIpr(string hid, string textFile)
        {
            try
            {
                StreamReader file = new StreamReader(textFile);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(hid))
                    {
                        return line.Split('\t')[1];
                    }
                }
                return "";
            }
            catch
            {
                MessageBox.Show("brak pliku z ta nazwa");
                return "";
            }
        }

        private string GetZplData(string serialNumber)
        {
            StringBuilder zpl = new StringBuilder();
            zpl.Append(zplTBox.Text);

            string ipr = GetIpr(serialNumber, filePathTBox.Text);
            string hidipr = serialNumber + ";" + ipr;
            hidipr = AddPrefix(hidipr);
            zpl.Replace("SN", hidipr);

            if (chBoxForText.Checked)
            {
                int x = Int32.Parse(firstLineTBox.Text);

                string param1 = serialNumber.Substring(0, x);
                string param2 = serialNumber.Substring(x);
                zpl.Replace("PAR0", param1); //prawdopodobnie nie dziala :)
                zpl.Replace("PAR1", param2);
            }
            return zpl.ToString();
        }

        private string AddPrefix(string qrData)
        {
            //HTTPS://HQR.CODES?HID=590568002D7P2020422000011&IPR=aae61069-34a2-406c-a738-a320078731ef
            qrData = qrData.Insert(0, @"HTTPS://HQR.CODES?HID=");
            qrData = qrData.Insert(qrData.IndexOf(';')+1, @"IPR=").Replace(';','&'); //insert,replace doesnt update current instance
            return qrData;
        }

        //EVENTS
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelMain_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panelSettings_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void snBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnPrint.PerformClick();

            }
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            FileOperations.AddUpdateAppSetting("zplCode", zplTBox.Text);
            FileOperations.AddUpdateAppSetting("firstLine", firstLineTBox.Text);
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
            }
        }
    }
}
