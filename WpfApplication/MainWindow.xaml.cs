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
        public List<Pixel> _board;
        public List<int> coordinates;

        public MainWindow()
        {
            InitializeComponent();

            _board = new List<Pixel>();
            coordinates = new List<int>();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    _board.Add(new Pixel(r, c) { Color = "White" });
                }
            }
            Board.ItemsSource = _board;
        }

        private void CellClick(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var point = (Pixel)border.Tag;
            point.Color = "Red";

            coordinates.Add(point.X);
            coordinates.Add(point.Y);
            //MessageBox.Show("[" + point.X.ToString() + "][" + point.Y.ToString() + "]");
        }

        public int GetIndex(int x, int y)
        {
            return y + (x * rows);
        }
        
        private void Line(object sender, RoutedEventArgs e)
        {
            var bresenham = new Bresenham();
            bresenham.Algorithm(coordinates[0], coordinates[1], coordinates[2], coordinates[3]);
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

    public class Pixel
    {
        private string _color;

        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

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

    public class Point
    {
        public int X;
        public int Y;

        public void SetCoordinates(int a, int b)
        {
            X = a;
            Y = b;
        }
    }
    public class Bresenham
    {
        List<int> xn, yn;
        Point p1, p2;
        double m;
        bool tradeX, tradeY, tradeXY;

        public Bresenham()
        {
            p1 = new Point();
            p2 = new Point();
            xn = new List<int>();
            yn = new List<int>();

            m = 0;
            tradeX = false;
            tradeY = false;
            tradeXY = false;
        }

        public void Algorithm(int x1, int y1, int x2, int y2)
        {
            p1.SetCoordinates(x1, y1);
            p2.SetCoordinates(x2, y2);

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
            float dx = (p2.X - p1.X);
            float dy = (p2.Y - p1.Y);

            float m = dy / dx;

            if (m > 1 || m < -1)
            {
                ExtensionMethods.Swap(ref p1.X, ref p1.Y);
                ExtensionMethods.Swap(ref p2.X, ref p2.Y);
                tradeXY = true;
            }
            if(p1.X > p2.X)
            {
                p1.X = -p1.X;
                p2.X = -p2.X;
                tradeX = true;
            }
            if(p1.Y > p2.Y)
            {
                p1.Y = -p1.Y;
                p2.Y = -p2.Y;
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
                //MessageBox.Show("[" + xn[i].ToString() + "][" + yn[i].ToString() + "] ; Index = " + windows.GetIndex(xn[i], yn[i]).ToString());
                windows._board[index].Color = "Red";
            }
        }
    }

    public static class ExtensionMethods
    {
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
}
