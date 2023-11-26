using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NeighborSearch
{
    internal static class NeighborFinder
    {
        public static void CellSearch(List<Point> points, float radius, bool isParallel = false)
        {
            int numberOfCells = (int)Math.Ceiling(300 / radius);
            List<Point>[,,] cells = new List<Point>[numberOfCells, numberOfCells, numberOfCells];

            for (int xi = 0; xi < numberOfCells; xi++)
                for (int yi = 0; yi < numberOfCells; yi++)
                    for (int zi = 0; zi < numberOfCells; zi++)
                        cells[xi, yi, zi] = new List<Point>();

            foreach (Point point in points)
            {
                int cellXi = (int)(point.X / radius);
                int cellYi = (int)(point.Y / radius);
                int cellZi = (int)(point.Z / radius);

                cells[cellXi, cellYi, cellZi].Add(point);
            }

            if (isParallel)
            {
                var partitioner = Partitioner.Create(points, EnumerablePartitionerOptions.NoBuffering);

                Parallel.ForEach(partitioner, pointI =>
                {
                    int cellXi = (int)(pointI.X / radius);
                    int cellYi = (int)(pointI.Y / radius);
                    int cellZi = (int)(pointI.Z / radius);

                    for (int cellXj = Math.Max(cellXi - 1, 0); cellXj <= Math.Min(cellXi + 1, numberOfCells - 1); cellXj++)
                        for (int cellYj = Math.Max(cellYi - 1, 0); cellYj <= Math.Min(cellYi + 1, numberOfCells - 1); cellYj++)
                            for (int cellZj = Math.Max(cellZi - 1, 0); cellZj <= Math.Min(cellZi + 1, numberOfCells - 1); cellZj++)

                                foreach (Point pointJ in cells[cellXj, cellYj, cellZj])
                                {
                                    if (pointI.Index == pointJ.Index)
                                        continue;

                                    if (Math.Sqrt(Math.Pow(pointI.X - pointJ.X, 2) +
                                                  Math.Pow(pointI.Y - pointJ.Y, 2) +
                                                  Math.Pow(pointI.Z - pointJ.Z, 2)) <= radius)
                                        pointI.NeighborIndex.Add(pointJ.Index);
                                }
                });
            }
            else
            {
                foreach (Point pointI in points)
                {
                    int cellXi = (int)(pointI.X / radius);
                    int cellYi = (int)(pointI.Y / radius);
                    int cellZi = (int)(pointI.Z / radius);

                    for (int cellXj = Math.Max(cellXi - 1, 0); cellXj <= Math.Min(cellXi + 1, numberOfCells - 1); cellXj++)
                        for (int cellYj = Math.Max(cellYi - 1, 0); cellYj <= Math.Min(cellYi + 1, numberOfCells - 1); cellYj++)
                            for (int cellZj = Math.Max(cellZi - 1, 0); cellZj <= Math.Min(cellZi + 1, numberOfCells - 1); cellZj++)

                                foreach (Point pointJ in cells[cellXj, cellYj, cellZj])
                                {
                                    if (pointI.Index == pointJ.Index)
                                        continue;

                                    if (Math.Sqrt(Math.Pow(pointI.X - pointJ.X, 2) +
                                                  Math.Pow(pointI.Y - pointJ.Y, 2) +
                                                  Math.Pow(pointI.Z - pointJ.Z, 2)) <= radius)
                                        pointI.NeighborIndex.Add(pointJ.Index);
                                }
                }
            }
        }

        public static void NaiveSearch(List<Point> points, float radius)
        {
            for (int i = 0; i < points.Count; i++)
                for (int j = i + 1; j < points.Count; j++)
                    if (Math.Sqrt(Math.Pow(points[i].X - points[j].X, 2) + 
                                  Math.Pow(points[i].Y - points[j].Y, 2) +
                                  Math.Pow(points[i].Z - points[j].Z, 2)) <= radius)
                    {
                        points[i].NeighborIndex.Add(points[j].Index);
                        points[j].NeighborIndex.Add(points[i].Index);
                    }
        }
    }
}
