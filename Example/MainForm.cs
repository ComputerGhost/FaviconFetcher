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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // File menu

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Tests menu

        private void scannerTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ScannerTest();
            form.MdiParent = this;
            form.Show();
        }

        private void fetcherTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FetcherTest();
            form.MdiParent = this;
            form.Show();
        }

        // Help menu

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Update github repo url
            MessageBox.Show(
                "This program tests and showcases the FaviconFetcher library. For more information, visit our github repo at <tbd>.", 
                "About FaviconFetcher Example", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
