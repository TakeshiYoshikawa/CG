using System;
using System.Windows;

namespace WpfApplication
{
    public class Circle
    {
        public void Algorithm(Point p1, Point p2)
        {
            int x = 0;
            int radius = Radius(p1.X, p1.Y, p2.X, p2.Y);
            int y = radius;
            int p = 3 - (2 * radius);

            DrawCircle(p1.X, p1.Y, x, y);

            while (x < y)
            {
                x++;
                if (p < 0)
                {
                    p += (4 * x) + 6;
                }
                else
                {
                    y--;
                    p += 4 * (x - y) + 10;
                }
                DrawCircle(p1.X, p1.Y, x, y);
            }
        }

        public void DrawCircle(int xc, int yc, int x, int y)
        {
            PutPixel(xc + x, yc + y, "Red");
            PutPixel(xc - x, yc + y, "Red");
            PutPixel(xc + x, yc - y, "Red");
            PutPixel(xc - x, yc - y, "Red");

            PutPixel(xc + y, yc + x, "Red");
            PutPixel(xc - y, yc + x, "Red");
            PutPixel(xc + y, yc - x, "Red");
            PutPixel(xc - y, yc - x, "Red");
        }

        public void PutPixel(int x, int y, string color)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            windows.PutPixel(x, y, "Red");
        }

        public int Radius(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);

            return Math.Max(dx, dy);
        }
    }
}
