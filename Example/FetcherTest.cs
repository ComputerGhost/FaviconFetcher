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
    public partial class FetcherTest : Form
    {
        public FetcherTest()
        {
            InitializeComponent();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            try
            {
                var uri = new Uri(txtUri.Text);
                var minSize = (int)numMinSize.Value;
                var maxSize = (int)numMaxSize.Value;
                var perfectSize = (int)numPerfectSize.Value;

                picIcon.Size = new Size(16, 16);
                picIcon.Image = null;

                var image = new Fetcher().Fetch(uri, new FetchOptions
                {
                    MinimumSize = new Size(minSize, minSize),
                    MaximumSize = new Size(maxSize, maxSize),
                    PerfectSize = new Size(perfectSize, perfectSize)
                });
                if (image != null)
                {
                    picIcon.Size = image.Size;
                    picIcon.Image = image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
