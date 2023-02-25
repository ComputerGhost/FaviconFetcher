using FaviconFetcher;
using SkiaSharp.Views.Desktop;
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
                    MinimumSize = new IconSize(minSize, minSize),
                    MaximumSize = new IconSize(maxSize, maxSize),
                    PerfectSize = new IconSize(perfectSize, perfectSize)
                });
                if (image != null)
                {
                    picIcon.Size = new Size(image.Size.Width, image.Size.Height);
                    picIcon.Image = image.ToSKBitmap().ToBitmap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
