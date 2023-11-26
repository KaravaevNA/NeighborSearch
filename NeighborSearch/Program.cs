

namespace NeighborSearch
{
    class Program
    {
        static void LaunchOptions()
        {
            PointSet set = new PointSet();
            Console.WriteLine("По умолчанию радиус соседства 3.7, чтение точек из points.db, вывод соседей точек в result.txt");
            Console.WriteLine("Введите Y для запуска со стандартными параметрами или N для изменения параметров");
            set.FindNeighbors();
            //set.WriteNeighborsInTxt();
            //set.ConsolePrintPoints();
            set.SavePointsToDB();
        }

        static void Main()
        {
            LaunchOptions();
        }
    }
}