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
using System.Xml.Linq;

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

        public override bool Equals(object? obj)
        {
            if (obj is Point point)
                return X == point.X && Y == point.Y && Z == point.Z;
            return false;
        }

        public override int GetHashCode()
        {
            return Index;
        }
    }

    internal partial class PointSet
    {
        private List<Point> Points { get; }
        private float Radius { get; set; }
        private Task ReadingPoints { get; set; }

        private delegate void PointSetHandler(string message);
        private event PointSetHandler? Notify;

        public PointSet(string readFileName, bool isFromDB = true)
        {
            Notify += DisplayMessage;
            Points = new List<Point>();
            if (isFromDB)
            {
                Notify?.Invoke($"Чтение точек из БД {readFileName}");
                ReadingPoints = Task.Run(() => ReadPointsFromDBAsync($"{readFileName}.db"));
            }
            else
            {
                Notify?.Invoke($"Чтение точек из файла {readFileName}");
                ReadingPoints = Task.Run(() => ReadPointsFromTxtAsync($"{readFileName}.txt"));
            }
        }
        public PointSet() : this("points") { }

        private void DisplayMessage(string message) => Console.WriteLine(message);

        public void FindNeighbors(float radius = 3.7f)
        {
            Radius = radius;
            Notify?.Invoke("Не рекомендуется использовать глупый поиск на входных данных большого размера из-за долгой обработки");
            Notify?.Invoke("Выберите поиск соседей прямым перебором (1), последовательный поиск по ячейкам (2) или параллельный поиск по ячейкам (3):");
            var sw = new Stopwatch();

            ReadingPoints.Wait();

            bool inputError = false;
            do {
                string? userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        inputError = false;
                        Notify?.Invoke($"Начато выполнение поиска соседей последовательным методом прямого перебора");
                        if (Points.Count > 10000)
                            Notify?.Invoke($"На текущем объеме входных данных ({Points.Count} точек) поиск этим методом может занять крайне продолжительное время");

                        sw.Start();
                        NeighborFinder.NaiveSearch(Points, Radius);
                        sw.Stop();

                        break;
                    case "2":
                        inputError = false;
                        Notify?.Invoke("Начато выполнение поиска соседей последовательным методом ячеек");

                        sw.Start();
                        NeighborFinder.CellSearch(Points, Radius, isParallel: false);
                        sw.Stop();

                        break;
                    case "3":
                        inputError = false;
                        Notify?.Invoke("Начато выполнение поиска соседей параллельным методом ячеек");

                        sw.Start();
                        NeighborFinder.CellSearch(Points, Radius, isParallel: true);
                        sw.Stop();

                        break;
                    default:
                        inputError = true;
                        Notify?.Invoke("Ошибка ввода при выборе алгоритма. Допустимые значения: 1, 2, 3");
                        break;
                }
            } while (inputError);

            Notify?.Invoke($"Количество точек: {Points.Count}");
            Notify?.Invoke($"Затрачено времени на поиск: {sw.ElapsedMilliseconds} мс");
        }

        private async Task ReadPointsFromTxtAsync(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Notify?.Invoke($"Файл {fileName} не найден");
                return;
            }

            using StreamReader reader = new StreamReader(fileName);
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Match match = Regex.Match(line, @"\(i = (\d+) :\s*([\d.]+)\s*,\s*([\d.]+)\s*,\s*([\d.]+)\)"); //индекс и три координаты

                    if (match.Success)
                    {
                        Points.Add(new Point(int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                                             float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                                             float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                                             float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture)));
                    }
                }
            }

            Notify?.Invoke($"Прочитано {Points.Count} точек из файла {fileName}");
        }

        public void WriteNeighborsInTxt(string fileName = "result.txt")
        {
            using StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine($"Radius is {Radius}\n\n");
            Points.ForEach(point => writer.WriteLine($"For {point.Index} is {point.NeighborIndex.Count} : {string.Join(" ", point.NeighborIndex)}"));
        }
        public void ConsolePrintPoints()
        {
            Points.ForEach(point => Notify?.Invoke($"(i = {point.Index} :\t{point.X},\t{point.Y},\t{point.Z})"));
        }

        public void ConsolePrintNeighbors()
        {
            Console.WriteLine($"Radius is {Radius}\n\n");
            Points.ForEach(point => Notify?.Invoke($"For {point.Index} is {point.NeighborIndex.Count} : {string.Join(" ", point.NeighborIndex)}"));
        }
    }
}
