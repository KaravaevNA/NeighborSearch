using Microsoft.EntityFrameworkCore;
using NeighborSearch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborSearch
{
    public class PointSetContext : DbContext
    {
        public DbSet<Point> Points { get; set; }
        private string DBName { get; set; }

        public PointSetContext(string dbName)
        {
            DBName = dbName;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DBName}");
        }
    }

    internal partial class PointSet
    {
        private async Task ReadPointsFromDBAsync(string dbName)
        {
            
            using PointSetContext context = new PointSetContext(dbName);
            Points.AddRange(await context.Points.ToListAsync());

            Notify?.Invoke($"Прочитано {Points.Count} точек из файла {dbName}");
        }

        public void SavePointsToDB(string dbName = "points.db")
        {
            ReadingPoints.Wait();
            Notify?.Invoke($"Запись точек в БД {dbName}");

            using PointSetContext context = new PointSetContext(dbName);

            if (context.Points.Count() != Points.Count)
            {
                context.Database.ExecuteSqlRaw("DELETE FROM Points");
                context.AddRange(Points);
            }
            else
            {
                List<Point> dbPoints = context.Points.ToList();
                for (int i = 0; i < dbPoints.Count; i++)
                    if (!dbPoints[i].Equals(Points[i]))
                    {
                        dbPoints[i].X = Points[i].X;
                        dbPoints[i].Y = Points[i].Y;
                        dbPoints[i].Z = Points[i].Z;
                    }
            }

            context.SaveChanges();
        }
    }
}