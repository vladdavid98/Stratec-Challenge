using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Stratec_Challenge.Functions;
using Stratec_Challenge.Model;

namespace Stratec_Challenge
{
    public partial class GridForm : Form
    {
        private Grid grid;

        private List<Button> BtnList;

        public GridForm(Grid grid)
        {
            InitializeComponent();
            this.grid = grid;
        }

        private void SetGridUi()
        {
            int possibleButtonHeight = (this.ClientSize.Height - (5 * grid.NumberOfRows)) / grid.NumberOfColumns;
            int possibleButtonWidth = (this.ClientSize.Width - (5 * grid.NumberOfColumns)) / grid.NumberOfRows;

            for (int i = 0; i < grid.NumberOfRows; i++)
            {
                Point newLoc = new Point(5, 5 * (i + 1) + i * possibleButtonHeight);
                for (int j = 0; j < grid.NumberOfColumns; j++)
                {
                    Button b = new Button
                        {Size = new Size(possibleButtonWidth, possibleButtonHeight), Location = newLoc};
                    newLoc.Offset(possibleButtonWidth + 5, 0);

                    b.Text = grid.Values[i][j].Value.ToString();
                    b.ForeColor = Color.White;
                    b.Font = new Font("Arial", possibleButtonHeight / 4, FontStyle.Bold);


                    if (grid.Values[i][j].Value == -1)
                        b.BackColor = (Color) Color.FromArgb(255, 255, 255, 255);
                    else if (grid.Values[i][j].Value != 0)
                    {
                        int intensity = (255 / (grid.NumberOfNonzeroPins / 2)) * grid.Values[i][j].Value;
                        b.BackColor = (Color) Color.FromArgb(255, 255 - intensity, 0, 0);
                    }

                    Controls.Add(b);
                }
            }

            BtnList = Controls.OfType<Button>().ToList();
        }

        private void UpdateGridButtons()
        {
            int btnRow = 0;
            int btnCol = 0;
            foreach (var btn in BtnList)
            {
                if (btnRow == grid.NumberOfColumns)
                {
                    btnCol++;
                    btnRow = 0;
                }

                btn.Text = grid.Values[btnCol][btnRow].Value.ToString();

                if (grid.Values[btnCol][btnRow].Value != 0)
                {
                    int intensity = (255 / (grid.NumberOfNonzeroPins / 2)) * grid.Values[btnCol][btnRow].Value;
                    if (intensity < 0) intensity = 0;
                    btn.BackColor = (Color) Color.FromArgb(255, 255 - intensity, 0, 0);
                }

                btnRow++;
            }
        }

        private bool GetPaths(Grid grid, int forPinValue)
        {
            Grid auxGrid = grid.MakeGridDirty(forPinValue);

            // get path coordinates forPinValue
            var listOfPoints = auxGrid.UsablePointsList[forPinValue - 1];

            for (int i = 0; i < listOfPoints.Count-1; i++)
            {
                var res = auxGrid.GetPath(listOfPoints[i], listOfPoints[i+1]);

                if (res.Count == 0) return false;

                // then update the main grid: put new points on it
                foreach (GridPoint point in res)
                {
                    grid.PutNewPoint(point, forPinValue);
                }

//                UpdateGridButtons();
                grid.FindUsablePoints();
            }

            
            return true;
        }


        private void GridForm_Load(object sender, EventArgs e)
        {
            List<List<int>> permList = new List<List<int>>();


            if (grid.InitialPointsList.Count <= 5)
            {
                IEnumerable<IEnumerable<int>> result =
                    Permutations.GetPermutations(Enumerable.Range(1, grid.InitialPointsList.Count), grid.InitialPointsList.Count);

                foreach (var list in result)
                {
                    var row = new List<int>();
                    foreach (var elem in list)
                    {
                        row.Add(elem);
                    }
                    permList.Add(row);
                }
            }
            
            SetGridUi();

            var gridCpy = new Grid(grid.CopyList());

            if (grid.InitialPointsList.Count > 5)
            {
                for (int i = 1; i < grid.InitialPointsList.Count; i++)
                {
                    if (i == 5)
                    {
                        Console.WriteLine(gridCpy.MakeGridDirty(5));
                    }
                    GetPaths(gridCpy, i);


                }
                this.grid = gridCpy;
                this.UpdateGridButtons();
                
            }
            else
            {
                int permListIndex = 0;


                while (!gridCpy.isCorrect() && permListIndex < permList.Count - 1)
                {
                    Console.WriteLine($"index {permListIndex} out of {permList.Count}");

                    gridCpy = new Grid(grid.CopyList());
                    foreach (var nr in permList[permListIndex])
                    {
                        bool hasPath = GetPaths(gridCpy, nr);
                        if (!hasPath) break;
                    }
                    permListIndex++;
                }
                if (gridCpy.isCorrect())
                {
                    this.grid = gridCpy;
                    this.UpdateGridButtons();
                }
            }



            
        }
    }
}