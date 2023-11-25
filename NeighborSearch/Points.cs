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
        public int Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public List<int> NeighborIndex { get; }

        public Point(int index, float x, float y, float z)
        {
            Index = index;

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
        private Task ReadingPoints { get; set; }

        public PointSet(string readFileName)
        {
            Points = new List<Point>();
            ReadingPoints = ReadPointsAsync(readFileName);
        }
        public PointSet() : this("Nat_3kk.txt") { }

        private async Task ReadPointsAsync(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        Match match = Regex.Match(line, @"\(i = (\d+) :\s*([\d.]+)\s*,\s*([\d.]+)\s*,\s*([\d.]+)\)"); //три координаты

                        if (match.Success)
                        {
                            Points.Add(new Point(int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                                                 float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                                                 float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                                                 float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture)));
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения файла: " + e.Message);
            }
        }

        public void WriteNeighborsInFile(string fileName = "result.txt")
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Radius is {Radius}\n\n");
                Points.ForEach(point => writer.WriteLine($"For {point.Index} is {point.NeighborIndex.Count} : {string.Join(" ", point.NeighborIndex)}"));
            }
        }

        public void FindNeighbors(float radius = 3.7f)
        {
            Radius = radius;
            Console.WriteLine("Глупый поиск соседей (1), последовательный поиск по ячейкам (2) или параллельный поиск по ячейкам (3)?");
            string? userInput = Console.ReadLine();
            var sw = new Stopwatch();

            ReadingPoints.Wait();

            switch (userInput)
            {
                case "1":
                    sw.Start();
                    NeighborFinder.NaiveSearch(Points, Radius);
                    sw.Stop();
                    break;
                case "2":
                    sw.Start();
                    NeighborFinder.CellSearch(Points, Radius, isParallel: false);
                    sw.Stop();
                    break;
                case "3":
                    sw.Start();
                    NeighborFinder.CellSearch(Points, Radius, isParallel: true);
                    sw.Stop();
                    break;
                default:
                    Console.WriteLine("Ошибка ввода при выборе алгоритма");
                    return;
            }

            Console.WriteLine($"Затрачено времени: {sw.ElapsedMilliseconds} мс");
        }

        public void ConsolePrintPoints()
        {
            Points.ForEach(point => Console.WriteLine($"(i = {point.Index} :\t{point.X},\t{point.Y},\t{point.Z})"));
        }

        public void ConsolePrintNeighbors()
        {
            Console.WriteLine($"Radius is {Radius}\n\n");
            Points.ForEach(point => Console.WriteLine($"For {point.Index} is {point.NeighborIndex.Count} : {string.Join(" ", point.NeighborIndex)}"));
        }
    }
}
