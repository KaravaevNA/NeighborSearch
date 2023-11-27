using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NeighborSearch
{
    class Program
    {
        static void LaunchOptions()
        {
            PointSet set;
            Console.WriteLine("По умолчанию радиус соседства 3.7, чтение точек из points.db, вывод соседей точек в result.txt");
            Console.WriteLine("Введите Y для запуска со стандартными параметрами или N для изменения параметров");

            bool inputError = false;
            do
            {
                string userInput = Console.ReadLine()?.ToUpper() ?? string.Empty;
                switch (userInput)
                {
                    case "Y":
                        inputError = false;
                        set = new PointSet();
                        set.FindNeighbors();
                        set.SavePointsToDB();
                        set.WriteNeighborsInTxt();
                        break;

                    case "N":
                        inputError = false;
                        float radius = 3.7f;
                        Console.WriteLine("Y для изменения радиуса:");
                        userInput = Console.ReadLine()?.ToUpper() ?? string.Empty;
                        if (userInput == "Y")
                        {
                            Console.WriteLine("Введите радиус поиска соседей:");
                            userInput = Console.ReadLine() ?? string.Empty;
                            radius = float.Parse(userInput, CultureInfo.InvariantCulture);
                        }

                        bool isFromDB = true;
                        string fileName = "points";
                        Console.WriteLine("Y для изменения считываемого файла:");
                        userInput = Console.ReadLine()?.ToUpper() ?? string.Empty;
                        if (userInput == "Y")
                        {
                            Console.WriteLine("Выберите - чтение точек из БД (1) или txt (2):");
                            userInput = Console.ReadLine() ?? string.Empty;
                            if (userInput == "2")
                                isFromDB = false;
                            else
                                isFromDB = true;

                            Console.WriteLine("Введите имя файла, из которого будет производиться чтение:");
                            fileName = Console.ReadLine() ?? "points";
                        }

                        set = new PointSet(fileName, isFromDB);
                        set.FindNeighbors(radius);
                        set.SavePointsToDB();
                        set.WriteNeighborsInTxt();

                        Console.WriteLine("Введите Y для вывода точек в консоль. Используйте для тестирования на малых наборах:");
                        userInput = Console.ReadLine()?.ToUpper() ?? string.Empty;
                        if (userInput == "Y")
                            set.ConsolePrintPoints();

                        Console.WriteLine("Введите Y для вывода соседей в консоль. Используйте для тестирования на малых наборах:");
                        userInput = Console.ReadLine()?.ToUpper() ?? string.Empty;
                        if (userInput == "Y")
                            set.ConsolePrintNeighbors();

                        break;
                    default:
                        Console.WriteLine("Ошибка ввода при параметров. Допустимые значения: Y, N");
                        inputError = true;
                        break;
                }
            } while (inputError);
        }

        static void Main()
        {
            LaunchOptions();
        }
    }
}