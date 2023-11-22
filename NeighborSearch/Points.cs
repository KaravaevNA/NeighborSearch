using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeighborSearch
{
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public List<int> NeighborIndex { get; }

        public Point(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;

            NeighborIndex = new List<int>();
        }
    }

    internal class PointSet
    {
        private List<Point> Points { get; }
        private float Radius { get; set; }

        public PointSet(float radius, string readFileName)
        {
            Points = new List<Point>();
            Radius = radius;
            ReadPoints(readFileName);
        }
        public PointSet() : this(3.7f, "points.txt") { }

        private void ReadPoints(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Match match = Regex.Match(line, @"\(i = \d+ :\s*([\d.]+),\s*([\d.]+),\s*([\d.]+)\)"); //три координаты

                        if (match.Success)
                        {
                            Points.Add(new Point(float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                                                 float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                                                 float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture)));
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения файла: " + e.Message);
            }
        }

        private void WriteNeighbors(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Radius is {Radius}\n\n");
                for (int i = 0; i < Points.Count; i++)
                    writer.WriteLine($"For {i} is {Points[i].NeighborIndex.Count} : {string.Join(" ", Points[i].NeighborIndex)}");
            }
        }

        public void FindNeighbors()
        {
            Console.Write("Глупый поиск соседей (1), последовательный поиск по ячейкам (2) или параллельный поиск по ячейкам (3)?");
            string? userInput = Console.ReadLine();
            var sw = new Stopwatch();
            switch (userInput)
            {
                case "1":
                    sw.Start();
                    NeighborFinder.SequentialSearch(Points);
                    sw.Stop();
                    break;
                case "2":
                    sw.Start();
                    NeighborFinder.SequentialCell(Points);
                    sw.Stop();
                    break;
                case "3":
                    sw.Start();
                    NeighborFinder.ParallelCell(Points);
                    sw.Stop();
                    break;
                default:
                    Console.WriteLine("Ошибка ввода при выборе алгоритма");
                    return;
            }
            Console.WriteLine($"Затрачено времени: {sw.ElapsedMilliseconds}");
        }

        //public void ConsolePrintPoints()
        //{
        //    for (int i = 0; i < Points.Count; i++)
        //        Console.WriteLine($"(i = {i} :\t{Points[i].X},\t{Points[i].Y},\t{Points[i].Z})");
        //}
    }
}
