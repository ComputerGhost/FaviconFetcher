using FaviconFetcher;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        private bool _isFetching = false;
        private CancellationTokenSource _cancellationTokenSource = null;

        private async void btnFetch_Click(object sender, EventArgs e)
        {
            if (_isFetching)
            {
                _cancellationTokenSource?.Cancel();
                return;
            }

            _isFetching = true;
            ((Button)sender).Text = "Cancel";

            try
            {
                var uri = new Uri(txtUri.Text);
                var minSize = (int)numMinSize.Value;
                var maxSize = (int)numMaxSize.Value;
                var perfectSize = (int)numPerfectSize.Value;

                _cancellationTokenSource = new CancellationTokenSource();

                picIcon.Size = new Size(16, 16);
                picIcon.Image = null;

                var image = await new Fetcher().Fetch(
                    uri,
                    new FetchOptions
                    {
                        MinimumSize = new IconSize(minSize, minSize),
                        MaximumSize = new IconSize(maxSize, maxSize),
                        PerfectSize = new IconSize(perfectSize, perfectSize)
                    },
                    _cancellationTokenSource);

                if (image != null)
                {
                    picIcon.Size = new Size(image.Size.Width, image.Size.Height);
                    picIcon.Image = image.ToSKBitmap().ToBitmap();
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
                _isFetching = false;
                ((Button)sender).Text = "Fetch";
            }
        }
    }
}
