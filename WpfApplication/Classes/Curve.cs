using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfApplication
{
    public class Curve
    {
        public List<Point> pts;
        public Bresenham line;

        public Curve(List<Point> ControlPts)
        {
            pts = new List<Point>();
            pts = ControlPts;
        }

        public Point Algorithm(double t)
        {
            int n = pts.Count() - 1;

            for (int r = 1; r <= n; r++)
            {
                for (int i = 0; i <= n - r; i++)
                {
                    pts[i] = pts[i].Sum(
                        pts[i].Dot((1 - t), pts[i]),
                        pts[i].Dot(t, pts[i + 1])
                    );
                }
            }
            return pts[0];
        }

        public void DrawCurve()
        {
            _ = (MainWindow)Application.Current.MainWindow;
            line = new Bresenham();
            Point initialPoint = pts[0];

            for (double t = 0; t <= 1; t += 0.1)
            {
                Point finalPoint = Algorithm(t);
                line.Algorithm(initialPoint, finalPoint, "Red");
                initialPoint = finalPoint;
            }
        }
    }
}
