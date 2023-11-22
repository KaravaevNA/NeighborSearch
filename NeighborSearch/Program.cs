

namespace NeighborSearch
{
    class Program
    {
        static void Main()
        {
            PointSet set = new PointSet();
            //set.FindNeighbors();
            //set.ConsolePrintPoints();

            List<Point>[,,] cells = new List<Point>[2, 2, 2];
            cells[0, 0, 0] = new List<Point>();
            cells[0, 0, 0].Add(new Point(1, 2, 3));
            Console.WriteLine(cells[0, 0, 0][0].X);
            Console.WriteLine(cells[0, 0, 0][0].Y);
            Console.WriteLine(cells[0, 0, 0][0].Z);
        }
    }
}