using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;

namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        private int rows = 30;
        private int columns = 30;
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
            int index = GetIndex(x, y);
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
            bresenham.Algorithm(coordinates[0], coordinates[1], "Red");
        }

        public void Circle(object sender, RoutedEventArgs e)
        {
            var circle = new Circle();
            circle.Algorithm(coordinates[0], coordinates[1]);
        }

        public void Curve(object sender, RoutedEventArgs e)
        {
            var curve = new Curve(coordinates);
            curve.DrawCurve();
        }

        public void Polyline(object sender, RoutedEventArgs e)
        {
            var polylines = new Polyline();
            polylines.Algorithm(coordinates, "Red");
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            var flood = new FloodFill();
            flood.Algorithm(coordinates.Last().X, coordinates.Last().Y, "Blue", "Red");
        }
        public void Scanline(object sender, RoutedEventArgs e)
        {
            new Scanline().Sweep(coordinates);
        }

        public void TrimSubwindows(object sender, RoutedEventArgs e)
        {
            List<Point> trimBorder = new List<Point>() {
                new Point(5,5),
                new Point(5,25),
                new Point(25,25),
                new Point(25,5)
            };
            new Polyline().Algorithm(trimBorder, "Red");
        }

        public void LineTrim(object sender, RoutedEventArgs e)
        {
            var rect = new RectangleF(5, 5, 20, 20);
            var newLine = CohenSutherland.ClipSegment(rect, coordinates[0], coordinates[1]);
            ClearBoard();
            new Bresenham().Algorithm(newLine.Item1, newLine.Item2, "Red");
        }

        public void PolygonTrim(object sender, RoutedEventArgs e)
        {
            var rect = new Rectangle(6, 6, 19, 19);
            new SutherlandHodgman().TrimPolygon(rect, coordinates);
        }

        public void Translation(object sender, RoutedEventArgs e)
        {
            ClearBoard();
            var trans = new Translation(coordinates, -3, -3);
            trans.Draw();
        }

        public void Rotation(object sender, RoutedEventArgs e)
        {
            ClearBoard();
            new Rotation(coordinates, coordinates[0], 90).Draw();
        }

        public void Scale(object sender, RoutedEventArgs e)
        {
            ClearBoard();
            new Scale(coordinates, coordinates[0], 2, 2).Draw();
        }

        public void ClearBoard()
        {
            foreach (var i in _board)
                i.Color = "White";
        }
        public void RefreshUI(object sender, RoutedEventArgs e)
        {
            coordinates.Clear();
            foreach (var i in _board)
                i.Color = "White";
        }
    }
}
