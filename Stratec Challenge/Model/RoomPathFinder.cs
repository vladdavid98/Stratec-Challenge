using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratec_Challenge.Model
{
    public class GridPoint
    {
        public int X, Y;

        public GridPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public GridPoint(float x, float y)
        {
            X = (int) x;
            Y = (int) y;
        }

        public override string ToString()
        {
            return "(" + X + " " + Y + ")";
        }
    }

    class RoomPathFinder
    {
        private int CostToMoveStraight = 1;
        private int CostToMoveDiagonally = 1;

        Dictionary<GridPoint, bool> closedSet = new Dictionary<GridPoint, bool>();
        Dictionary<GridPoint, bool> openSet = new Dictionary<GridPoint, bool>();

        //cost of start to this key node
        Dictionary<GridPoint, double> gScore = new Dictionary<GridPoint, double>();

        //cost of start to goal, passing through key node
        Dictionary<GridPoint, double> fScore = new Dictionary<GridPoint, double>();

        Dictionary<GridPoint, GridPoint> nodeLinks = new Dictionary<GridPoint, GridPoint>();

        public List<GridPoint> FindPath(List<List<bool>> graph, GridPoint start, GridPoint goal)
        {
            openSet[start] = true;
            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);

            while (openSet.Count > 0 && openSet.Count<graph.Count*graph.Count)
            {
                var current = nextBest();
                if (current.X == goal.X && current.Y == goal.Y)
                {
                    return Reconstruct(current);
                }


                openSet.Remove(current);
                closedSet[current] = true;

                foreach (var neighbor in Neighbors(graph, current))
                {
                    if (closedSet.ContainsKey(neighbor))
                        continue;

                    var projectedG = getGScore(current) + 1;

                    if (!openSet.ContainsKey(neighbor))
                        openSet[neighbor] = true;
                    else if (projectedG >= getGScore(neighbor))
                        continue;

                    //record it
                    nodeLinks[neighbor] = current;
                    gScore[neighbor] = projectedG;
                    fScore[neighbor] = projectedG + Heuristic(neighbor, goal);
                }
            }


            return new List<GridPoint>();
        }

        private double Heuristic(GridPoint start, GridPoint goal)
        {
            var dx = Math.Abs(start.X - goal.X);
            var dy = Math.Abs(start.Y - goal.Y);
            return Math.Sqrt(dx * dx + dy * dy);
//            return CostToMoveStraight * (dx + dy) + (CostToMoveDiagonally - 2 * CostToMoveStraight) * Math.Min(dx, dy);
        }

        private double getGScore(GridPoint pt)
        {
            double score = double.MaxValue;
            gScore.TryGetValue(pt, out score);
            return score;
        }


        private double getFScore(GridPoint pt)
        {
            double score = double.MaxValue;
            fScore.TryGetValue(pt, out score);
            return score;
        }

        public static IEnumerable<GridPoint> Neighbors(List<List<bool>> graph, GridPoint center)
        {
            GridPoint pt = new GridPoint(center.X - 1, center.Y - 1);
            if (IsValidNeighbor(graph, pt))
                yield return pt;

            pt = new GridPoint(center.X, center.Y - 1);
            if (IsValidNeighbor(graph, pt))
                yield return pt;

            pt = new GridPoint(center.X + 1, center.Y - 1);
            if (IsValidNeighbor(graph, pt))
                yield return pt;

            //middle row
            pt = new GridPoint(center.X - 1, center.Y);
            if (IsValidNeighbor(graph, pt))
                yield return pt;

            pt = new GridPoint(center.X + 1, center.Y);
            if (IsValidNeighbor(graph, pt))
                yield return pt;


            //bottom row
            pt = new GridPoint(center.X - 1, center.Y + 1);
            if (IsValidNeighbor(graph, pt))
                yield return pt;

            pt = new GridPoint(center.X, center.Y + 1);
            if (IsValidNeighbor(graph, pt))
                yield return pt;

            pt = new GridPoint(center.X + 1, center.Y + 1);
            if (IsValidNeighbor(graph, pt))
                yield return pt;
        }

        public static bool IsValidNeighbor(List<List<bool>> matrix, GridPoint pt)
        {
            int x = pt.X;
            int y = pt.Y;
            if (x < 0 || x >= matrix.Count)
                return false;

            if (y < 0 || y >= matrix[x].Count)
                return false;

            return matrix[x][y];
        }

        private List<GridPoint> Reconstruct(GridPoint current)
        {
            List<GridPoint> path = new List<GridPoint>();
            while (nodeLinks.ContainsKey(current))
            {
                path.Add(current);
                current = nodeLinks[current];
            }

            path.Reverse();
            return path;
        }

        private GridPoint nextBest()
        {
            double best = int.MaxValue;
            GridPoint bestPt = null;
            foreach (var node in openSet.Keys)
            {
                var score = getFScore(node);
                if (score < best)
                {
                    bestPt = node;
                    best = score;
                }
            }


            return bestPt;
        }
    }
}