using System;
using System.Collections.Generic;
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
        public List<Point> Points { get; }

        public float Radius { get; }

        public PointSet()
        {
            Points = new List<Point>();
            Radius = 3.7f;
        }

        public PointSet(List<Point> points, float radius)
        {
            Points = new List<Point>(points); //хз, работает ли
            Radius = radius;
        }

        public void ReadPoints(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Match match = Regex.Match(line, @"\(([\d\.]+),\s*([\d\.]+),\s*([\d\.]+)\)"); //три координаты

                        if (match.Success)
                        {
                            Points.Add(new Point(float.Parse(match.Groups[1].Value),
                                                 float.Parse(match.Groups[2].Value),
                                                 float.Parse(match.Groups[3].Value)));
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения файла: " + e.Message);
            }
        }

        public void WriteNeighbors(float radius, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Radius is {radius}\n\n");
                for (int i = 0; i < Points.Count; i++)
                    writer.WriteLine($"For {i} is {Points[i].NeighborIndex.Count} : {string.Join(" ", Points[i].NeighborIndex)}");
            }
        }
    }
}
