using FaviconFetcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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

        private bool _isScanning = false;
        private CancellationTokenSource _cancellationTokenSource = null;

        private async void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isScanning)
                {
                    _cancellationTokenSource?.Cancel();
                    return;
                }

                _isScanning = true;
                ((Button)sender).Text = "Cancel";

                var uri = new Uri(txtUri.Text);

                lstResults.Items.Clear();

                _cancellationTokenSource = new CancellationTokenSource();

                foreach (var result in await new Scanner().Scan(uri, _cancellationTokenSource))
                {
                    lstResults.Items.Add(new ListViewItem(new[]{
                        result.ExpectedSize.ToString(),
                        result.Location.ToString()
                    }));
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _isScanning = false;
                ((Button)sender).Text = "Scan";
                _cancellationTokenSource?.Dispose();
            }
        }

        private void _ExpandLocationColumn()
        {
            colLocation.Width = lstResults.ClientSize.Width - colSize.Width;
        }
    }
}
