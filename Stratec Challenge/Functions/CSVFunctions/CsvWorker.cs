using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stratec_Challenge.Functions.CSVFunctions
{
    public static class CsvWorker
    {
        public static string ReadCsv()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            return fileContent;
        }

        public static List<List<int>> parseCSV(string input)
        {
            string[] lines = input.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            );

            List<List<int>> intArrList = new List<List<int>>();

            foreach (string line in lines)
            {
                string[] elems = line.Split(',');
                List<int> intLine = new List<int>();
                foreach (var s in elems)
                {
                    if(s=="Z")
                        intLine.Add(-1);
                    else if (s != "")
                        intLine.Add(Int32.Parse(s));
                }

                if (intLine.Count > 0)
                    intArrList.Add(intLine);
            }

            return intArrList;
        }
    }
}