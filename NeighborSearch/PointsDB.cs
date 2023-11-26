using Microsoft.EntityFrameworkCore;
using NeighborSearch;
using System;
using System.Collections.Generic;
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
        }

        public void SavePointsToDB(string dbName = "points.db")
        {
            ReadingPoints.Wait();

            using PointSetContext context = new PointSetContext(dbName);

            List<int>? pointIndexes = Points.Select(p => p.Index).ToList();
            List<Point>? pointsToRemove = context.Points.Where(p => !pointIndexes.Contains(p.Index)).ToList();
            context.Points.RemoveRange(pointsToRemove);

            foreach (var point in Points)
            {
                Point? existingPoint = context.Points.Find(point.Index);

                if (existingPoint == null)
                    context.Points.Add(point);
                else if (!existingPoint.Equals(point))
                    context.Entry(existingPoint).CurrentValues.SetValues(point);
            }

            context.SaveChanges();
        }
    }
}