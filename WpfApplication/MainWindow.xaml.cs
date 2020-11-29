using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication.ViewModels;

namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        private int rows = 20;
        private int columns = 20;
        public List<Point> _board;
        public List<Point> coordinates;

        public MainWindow()
        {
            InitializeComponent();

            _board = new List<Point>();
            coordinates = new List<Point>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    _board.Add(new Point(r, c) { Color = "White" });
                }
            }
            Board.ItemsSource = _board;
        }

        public void CellClick(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var point = (Point)border.Tag;
            point.Color = "Black";
            coordinates.Add(point);
        }

        public void PutPixel(int x, int y, string color)
        {
            int index = GetIndex(x, y) ;
            _board[index].Color = color;
        }

        public int GetIndex(int x, int y)
        {
            return y + (x * rows);
        }

        public Point GetPoint(int x, int y)
        {
            return _board[GetIndex(x, y)];
        }
        public void Line(object sender, RoutedEventArgs e)
        {
            var bresenham = new Bresenham();
            bresenham.Algorithm(coordinates[0], coordinates[1]);
        }

        public void Circle(object sender, RoutedEventArgs e)
        {
            var circle = new Circle();
            circle.Algorithm(coordinates[0], coordinates[1]);
        }

        public void Polyline(object sender, RoutedEventArgs e)
        {
            var polylines = new Polyline();
            polylines.Algorithm(coordinates);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            var flood = new FloodFill();
            flood.Algorithm(coordinates.Last().X, coordinates.Last().Y, "Blue", "Red");
        }


        public void RefreshUI(object sender, RoutedEventArgs e)
        {
            coordinates.Clear();
            foreach (var i in _board)
            {
                i.Color = "White";
            }
        }
    }

    public class FloodFill
    {
        public void Algorithm(int x, int y, string color, string edgeColor)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            var current = windows.GetPoint(x,y) ;
            if(current.Color != edgeColor && current.Color != color)
            {
                windows.PutPixel(x, y, color);
                Algorithm(x + 1, y, color, edgeColor);
                Algorithm(x, y + 1, color, edgeColor);
                Algorithm(x - 1, y, color, edgeColor);
                Algorithm(x, y - 1, color, edgeColor);
            }
        }
    }

    public class Polyline
    {
        Bresenham bresenham;

        public Polyline()
        {
            bresenham = new Bresenham();
        }
        public void Algorithm(List<Point> points)
        {
            for(int i = 0; i < points.Count()-1; i++)
            {
                bresenham.Algorithm(points[i], points[i+1]);
            }
        }

        public void DrawLine(Point p1, Point p2)
        {
            bresenham.Algorithm(p1, p2);
        }
    }
    
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
                if(p < 0)
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

    public class Bresenham
    {
        public void Algorithm(Point p1, Point p2)
        {
            int x1 = p1.X; int y1 = p1.Y;
            int x2 = p2.X; int y2 = p2.Y;

            int w = x2 - x1;
            int h = y2 - y1;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);

            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                DrawPoints(x1, y1);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x1 += dx1;
                    y1 += dy1;
                }
                else
                {
                    x1 += dx2;
                    y1 += dy2;
                }
            }
        }

        public void DrawPoints(int x, int y)
        {
            var windows = (MainWindow)Application.Current.MainWindow;       
            windows.PutPixel(x, y, "Red");
        }
    }

    public static class ExtensionMethods
    {
        public static void Swap(Point a){
            var temp = a.X;
            a.X = a.Y;
            a.Y = temp;
        }
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        public static void Swap<T>(this List<T> list1, List<T> list2, int index)
        {
            T temp = list1[index];
            list1[index] = list2[index];
            list2[index] = temp;
        }
    }
    public class Point
    {
        private string _color;

        public Point() { }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (ColorChanged != null)
                {
                    ColorChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler ColorChanged;
    }
}
