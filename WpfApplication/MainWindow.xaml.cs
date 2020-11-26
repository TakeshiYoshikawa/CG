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
        private int rows = 25;
        private int columns = 25;
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
            point.Color = "Red";

            coordinates.Add(point);
        }

        public int GetIndex(int x, int y)
        {
            return y + (x * rows);
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
            //var polylines = new Polyline();
            //polylines.Algorithm();
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

    /*
    public class Polilyne
    {
        public void Algorithm(IEnumerable<Point> points)
        {
            var endpoints = points.Skip(1).Concat(new[] { points.First() });
            var pairs = points.Zip(endpoints, Tuple.Create);
        
            foreach(var pair in pairs)
            {
                DrawLine(pair.Item1, pair.Item2);
            }
        }

        public void DrawLine(Point p1, Point p2)
        {
            MessageBox.Show("teste", "teste");
        }
    }
    */

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
            int index = windows.GetIndex(x, y);
            windows._board[index].Color = color;
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
        List<int> xn, yn;
        Point _p1, _p2;
        double m;
        bool tradeX, tradeY, tradeXY;

        public Bresenham()
        {
            _p1 = new Point();
            _p2 = new Point();
            xn = new List<int>();
            yn = new List<int>();

            m = 0;
            tradeX = false;
            tradeY = false;
            tradeXY = false;
        }

        public void Algorithm(Point p1, Point p2)
        {
            this._p1 = p1;
            this._p2 = p2;

            Reflect();

            int x = p1.X;
            int y = p1.Y;

            double dx = Math.Abs(p2.X - p1.X);
            double dy = Math.Abs(p2.Y - p1.Y);

            m = dy / dx;

            double e = m - 1.0 / 2.0;

            SetPixels(x, y);

            while(x < p2.X)
            {
                if(e > 0)
                {
                    y++;
                    e--;
                }
                x++;
                e += m;
                SetPixels(x, y);
            }
            Deflect();
            DrawPoints();
        }
        public void Reflect()
        {
            float dx = (_p2.X - _p1.X);
            float dy = (_p2.Y - _p1.Y);

            float m = dy / dx;


            if (m > 1 || m < -1)
            {
                ExtensionMethods.Swap(_p1);
                ExtensionMethods.Swap(_p2);
                tradeXY = true;
            }
            if(_p1.X > _p2.X)
            {
                _p1.X = -_p1.X;
                _p2.X = -_p2.X;
                tradeX = true;
            }
            if(_p1.Y > _p2.Y)
            {
                _p1.Y = -_p1.Y;
                _p2.Y = -_p2.Y;
                tradeY = true;
            }
        }
        public void Deflect()
        {
            if (tradeY == true)
            {
                for (int i = 0; i < yn.Count(); i++)
                {
                    yn[i] = -yn[i];
                }
            }
            if (tradeX == true)
            {
                for (int i = 0; i < xn.Count(); i++)
                {                    
                    xn[i] = -xn[i];
                }
            }
            if (tradeXY == true)
            {
                for (int i = 0; i < xn.Count(); i++)
                {
                    ExtensionMethods.Swap(xn, yn, i);
                }
            }
        }
        public void SetPixels(int x, int y)
        {
            xn.Add(x);
            yn.Add(y);
        }

        public void DrawPoints()
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            for(int i = 0; i < xn.Count(); i++)
            {
                int index = windows.GetIndex(xn[i], yn[i]);
                windows._board[index].Color = "Red";
            }
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
