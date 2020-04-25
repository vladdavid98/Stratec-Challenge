using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratec_Challenge.Model
{
    public class Grid
    {
        public List<List<Pin>> Values { get; }

        public int NumberOfRows { get; }


        public int NumberOfColumns { get; }

        public int NumberOfNonzeroPins { get; }

        public List<List<GridPoint>> InitialPointsList;

        public List<List<GridPoint>> UsablePointsList;

        public List<List<Pin>> PutNewPoint(GridPoint gp, int val)
        {
            Values[gp.X][gp.Y].Value = val;
            return Values;
        }

        public Grid(List<List<int>> values)
        {
            List<List<Pin>> pinList = new List<List<Pin>>();

            foreach (var column in values)
            {
                List<Pin> pinRow = new List<Pin>();
                foreach (var row in column)
                {
                    Pin newPin = new Pin(row);
                    pinRow.Add(newPin);
                }

                pinList.Add(pinRow);
            }

            this.Values = pinList;
            NumberOfRows = Values.Count;
            NumberOfColumns = Values[0].Count;
            NumberOfNonzeroPins = GetNumberOfNonzeroPins();
            UsablePointsList = FindUsablePoints();
            InitialPointsList = UsablePointsList;
        }

        public Grid(List<List<Pin>> values)
        {
            this.Values = values;
            NumberOfRows = Values.Count;
            NumberOfColumns = Values[0].Count;
            NumberOfNonzeroPins = GetNumberOfNonzeroPins();
            UsablePointsList = FindUsablePoints();
            InitialPointsList = FindUsablePoints();
        }

        public List<List<Pin>> CopyList()
        {
            List<List<Pin>> listCopy = new List<List<Pin>>();
            for (int i = 0; i < NumberOfRows; i++)
            {
                List<Pin> row = new List<Pin>();
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    Pin pin = new Pin(Values[i][j].Value);
                    row.Add(pin);
                }

                listCopy.Add(row);
            }

            return listCopy;
        }

        public List<List<GridPoint>> FindUsablePoints()
        {
            var listCopy = CopyList();

            List<List<GridPoint>> res = new List<List<GridPoint>>();

            for (int currentNr = 1; currentNr <= NumberOfNonzeroPins / 2; currentNr++)
            {
                List<GridPoint> row = new List<GridPoint>();
                for (int i = 0; i < NumberOfRows; i++)
                {
                    for (int j = 0; j < NumberOfColumns; j++)
                    {
                        if (listCopy[i][j].Value == currentNr)
                        {
                            row.Add(new GridPoint(i, j));
                            listCopy[i][j].Value = 0;
                        }
                    }
                }

                if (row.Count > 0) res.Add(row);
            }

            return res;
        }

        private int GetNumberOfNonzeroPins()
        {
            return Values.SelectMany(line => line).Count(elem => elem.Value != 0);
        }

        public bool isCorrect()
        {
            for (int i = 1; i < NumberOfRows - 1; i++)
            {
                for (int j = 1; j < NumberOfColumns - 1; j++)
                {
                    int currentVal = Values[i][j].Value;
                    if (currentVal != 0)
                    {
                        int numberOfSameVal = 0;
                        if (Values[i - 1][j - 1].Value == currentVal) numberOfSameVal++;
                        if (Values[i][j - 1].Value == currentVal) numberOfSameVal++;
                        if (Values[i + 1][j - 1].Value == currentVal) numberOfSameVal++;
                        if (Values[i - 1][j].Value == currentVal) numberOfSameVal++;
                        if (Values[i + 1][j].Value == currentVal) numberOfSameVal++;
                        if (Values[i - 1][j + 1].Value == currentVal) numberOfSameVal++;
                        if (Values[i][j + 1].Value == currentVal) numberOfSameVal++;
                        if (Values[i + 1][j + 1].Value == currentVal) numberOfSameVal++;
                        if (numberOfSameVal == 0) return false;
                    }
                }
            }

            return true;
        }

        public bool IsDefective()
        {
            for (int i = 1; i < NumberOfRows - 1; i++)
            {
                for (int j = 1; j < NumberOfColumns - 1; j++)
                {
                    int currentVal = Values[i][j].Value;
                    if (Values[i - 1][j - 1].Value != 0 && Values[i - 1][j - 1].Value != currentVal) return false;
                    if (Values[i][j - 1].Value != 0 && Values[i][j - 1].Value != currentVal) return false;
                    if (Values[i + 1][j - 1].Value != 0 && Values[i + 1][j - 1].Value != currentVal) return false;
                    if (Values[i - 1][j].Value != 0 && Values[i - 1][j].Value != currentVal) return false;
                    if (Values[i + 1][j].Value != 0 && Values[i + 1][j].Value != currentVal) return false;
                    if (Values[i - 1][j + 1].Value != 0 && Values[i - 1][j + 1].Value != currentVal) return false;
                    if (Values[i][j + 1].Value != 0 && Values[i][j + 1].Value != currentVal) return false;
                    if (Values[i + 1][j + 1].Value != 0 && Values[i + 1][j + 1].Value != currentVal) return false;
                }
            }

            return true;
        }

        public List<GridPoint> GetPath(GridPoint start, GridPoint end)
        {
            int currentVal = Values[start.X][start.Y].Value;

            var arr = new List<List<bool>>();
            for (int i = 0; i < NumberOfRows; i++)
            {
                var col = new List<bool>();
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    if (Values[i][j].Value == 0 || Values[i][j].Value == currentVal)
                        col.Add(true);
                    else
                        col.Add(false);
                }

                arr.Add(col);
            }


            RoomPathFinder rpf = new RoomPathFinder();
            List<GridPoint> res = rpf.FindPath(arr, start, end);

//            for (int j = 0; j < res.Count; j++)
//            {
//                Console.WriteLine(res[j].ToString() + " ");
//            }
            return res;
        }

//        public bool IsSolved()
//        {
//        }

        public override string ToString()
        {
            string s = "";
            foreach (var line in Values)
            {
                foreach (var elem in line)
                {
                    if (elem.Value < 0)
                        s += " ";
                    else
                        s += "  ";
                    s += elem.ToString();
                }

                s += "\n";
            }

            return s;
        }

        public Grid MakeGridDirty(int exceptForValue)
        {
            UsablePointsList = FindUsablePoints();
            var listCpy = CopyList();
//            Console.WriteLine(this);
            foreach (var lst in UsablePointsList)
            {
                // pt fiecare numar: 
                foreach (var point in lst)
                {
                    if (listCpy[point.X][point.Y].Value != exceptForValue)
                    {
                        //pt fiecare punct de pe grid, cele de langa el nu sunt traversabile de altele(pune -2)

                        if (point.X - 1 >= 0 && point.Y - 1 >= 0)
                            if (listCpy[point.X - 1][point.Y - 1].Value == 0)
                                listCpy[point.X - 1][point.Y - 1].Value = -2;

                        if (point.Y - 1 >= 0)
                            if (listCpy[point.X][point.Y - 1].Value == 0)
                                listCpy[point.X][point.Y - 1].Value = -2;

                        if (point.X + 1 < NumberOfRows && point.Y - 1 >= 0)
                            if (listCpy[point.X + 1][point.Y - 1].Value == 0)
                                listCpy[point.X + 1][point.Y - 1].Value = -2;

                        if (point.X - 1 >= 0)
                            if (listCpy[point.X - 1][point.Y].Value == 0)
                                listCpy[point.X - 1][point.Y].Value = -2;

                        if (point.X + 1 < NumberOfRows)
                            if (listCpy[point.X + 1][point.Y].Value == 0)
                                listCpy[point.X + 1][point.Y].Value = -2;

                        if (point.X - 1 >= 0 && point.Y + 1 < NumberOfColumns)
                            if (listCpy[point.X - 1][point.Y + 1].Value == 0)
                                listCpy[point.X - 1][point.Y + 1].Value = -2;

                        if (point.Y + 1 < NumberOfColumns)
                            if (listCpy[point.X][point.Y + 1].Value == 0)
                                listCpy[point.X][point.Y + 1].Value = -2;

                        if (point.X + 1 < NumberOfRows && point.Y + 1 < NumberOfColumns)
                            if (listCpy[point.X + 1][point.Y + 1].Value == 0)
                                listCpy[point.X + 1][point.Y + 1].Value = -2;
                    }
                }
            }


            var toRet = new Grid(listCpy);
//            Console.WriteLine(toRet);
            return toRet;
        }
    }
}