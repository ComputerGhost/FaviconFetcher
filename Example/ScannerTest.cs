using FaviconFetcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    public partial class ScannerTest : Form
    {
        public ScannerTest()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _ExpandLocationColumn();
        }

        private void lstResults_Resize(object sender, EventArgs e)
        {
            _ExpandLocationColumn();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                var uri = new Uri(txtUri.Text);

                lstResults.Items.Clear();

                foreach (var result in new Scanner().Scan(uri))
                {
                    lstResults.Items.Add(new ListViewItem(new[]{
                        result.ExpectedSize.ToString(),
                        result.Location.ToString()
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void _ExpandLocationColumn()
        {
            colLocation.Width = lstResults.ClientSize.Width - colSize.Width;
        }
    }
}
