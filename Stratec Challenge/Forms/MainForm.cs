using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Stratec_Challenge.Functions;
using Stratec_Challenge.Functions.CSVFunctions;
using Stratec_Challenge.Model;

namespace Stratec_Challenge
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();





        }

        private void openNewFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string csvContent = CsvWorker.ReadCsv();

            Grid csvGrid = new Grid(CsvWorker.parseCSV(csvContent));

            GridForm m = new GridForm(csvGrid);
            m.Show();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}