

namespace NeighborSearch
{
    class Program
    {
        static void Main()
        {
            PointSet set = new PointSet();
            set.FindNeighbors();
            set.WriteNeighborsInFile();
            set.ConsolePrintNeighbors();
            //set.ConsolePrintPoints();

            
        }
    }
}