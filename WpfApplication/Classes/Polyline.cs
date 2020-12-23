using System.Collections.Generic;
using System.Linq;

namespace WpfApplication
{
    public class Polyline
    {
        Bresenham bresenham;

        public Polyline()
        {
            bresenham = new Bresenham();
        }
        public void Algorithm(List<Point> points, string color)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                bresenham.Algorithm(points[i % points.Count()], points[(i + 1) % points.Count()], color);
            }
        }
    }
}
