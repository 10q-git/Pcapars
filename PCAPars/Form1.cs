using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketsPars;

namespace PCAPars
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string fileName = openFileDialog1.FileName;

            FileOpen File = new FileOpen(fileName);
            File.Open(fileName);

       
            Dictionary<int, Dictionary<string, string>> returnsItems = new Dictionary<int, Dictionary<string, string>>();

            returnsItems = FileOpen.returnsItems;

            for (int i = 1; i <= FileOpen.count-1; i++)
            {
                try
                {
                    Dictionary<string, string> thisItem = returnsItems[i];

                    Label Number = new Label();
                    Number.Text = i.ToString();

                    Label Time = new Label();
                    Time.Text = returnsItems[i]["Date"];

                    Label Source = new Label();
                    Source.Text = returnsItems[i]["Source"];

                    Label Destination = new Label();
                    Destination.Text = returnsItems[i]["Destination"];

                    Label Protocol = new Label();
                    Protocol.Text = returnsItems[i]["Protocol"];

                    Label Length = new Label();
                    Length.Text = returnsItems[i]["Length"];

                    Label Info = new Label();
                    Info.Text = returnsItems[i]["Info"];

                    tableLayoutPanel1.RowCount++;
                    tableLayoutPanel1.Controls.Add(Number);   
                    tableLayoutPanel1.Controls.Add(Time);
                    tableLayoutPanel1.Controls.Add(Source);
                    tableLayoutPanel1.Controls.Add(Destination);
                    tableLayoutPanel1.Controls.Add(Protocol);
                    tableLayoutPanel1.Controls.Add(Length);
                    tableLayoutPanel1.Controls.Add(Info);
                }
                catch
                {

                }
            }

        }
    }
}