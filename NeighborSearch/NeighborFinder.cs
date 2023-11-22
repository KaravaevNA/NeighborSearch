using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborSearch
{
    internal static class NeighborFinder
    {
        public static void ParallelCell(List<Point> points, float radius)
        {

        }

        public static void SequentialCell(List<Point> points, float radius)
        {

        }

        public static void SequentialSearch(List<Point> points, float radius)
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (Math.Sqrt(Math.Pow(points[i].X - points[j].X, 2) + 
                                  Math.Pow(points[i].Y - points[j].Y, 2) +
                                  Math.Pow(points[i].Z - points[j].Z, 2)) <= radius)
                    {
                        points[i].NeighborIndex.Add(j);
                        points[j].NeighborIndex.Add(i);
                    }
                }
            }
        }
    }
}
