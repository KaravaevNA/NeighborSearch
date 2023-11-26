using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeighborSearch
{
    public class Point
    {
        [Key]
        public int Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        [NotMapped]
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

    internal partial class PointSet
    {
        private List<Point> Points { get; }
        private float Radius { get; set; }
        private Task ReadingPoints { get; set; }

        public PointSet(string readFileName, bool isFromDB = false)
        {
            Points = new List<Point>();
            if (isFromDB)
                ReadingPoints = ReadPointsFromDBAsync($"{readFileName}.db");
            else
                ReadingPoints = ReadPointsFromTxtAsync($"{readFileName}.txt");
        }
        public PointSet() : this("points") { }

        public void FindNeighbors(float radius = 3.7f)
        {
            Radius = radius;
            Console.WriteLine("Выберите глупый поиск соседей (1), последовательный поиск по ячейкам (2) или параллельный поиск по ячейкам (3):");
            var sw = new Stopwatch();

            ReadingPoints.Wait();

            bool inputError = false;
            do {
                string? userInput = Console.ReadLine();
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
                        Console.WriteLine("Ошибка ввода при выборе алгоритма. Допустимые значения: 1, 2, 3");
                        inputError = true;
                        break;
                }
            } while (inputError);

            Console.WriteLine($"Затрачено времени на поиск: {sw.ElapsedMilliseconds} мс");
        }

        private async Task ReadPointsFromTxtAsync(string fileName)
        {
            try
            {
                using StreamReader reader = new StreamReader(fileName);
                string? line;
                int id = 1;
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
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения файла: " + e.Message);
            }
        }

        public void WriteNeighborsInTxt(string fileName = "result.txt")
        {
            using StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine($"Radius is {Radius}\n\n");
            Points.ForEach(point => writer.WriteLine($"For {point.Index} is {point.NeighborIndex.Count} : {string.Join(" ", point.NeighborIndex)}"));
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
