using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        private int rows = 50;
        private int columns = 50;
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

        public void KeyInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "[F1] - Clear Board\n" + 
                "[F2] - Clip Window", 
                "Instructions"
            );
        }

        public void Utilities(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                coordinates.Clear();

                foreach (var i in _board)
                    i.Color = "White";
            }
            else if (e.Key == Key.F2)
            {
                List<Point> trimBorder = new List<Point>() {
                    new Point(5,5),
                    new Point(5,25),
                    new Point(25,25),
                    new Point(25,5)
                };
                new Polyline().Algorithm(trimBorder, "Red");
            }
        }

        public void PutPixel(int x, int y, string color)
        {
            // Out of range coordinates are ignored.
            if ((x < 0 || y < 0) || (x >= rows || y >= rows))
            {
                ;
            }
            else { 
                if (_board[GetIndex(x,y)].Color != "Red")
                    _board[GetIndex(x, y)].Color = color;
            }
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
            new Bresenham().Algorithm(coordinates[0], coordinates[1], "Red");
        }

        public void Circle(object sender, RoutedEventArgs e)
        {
            new Circle().Algorithm(coordinates[0], coordinates[1]);
        }

        public void Curve(object sender, RoutedEventArgs e)
        {
            new Curve(coordinates).DrawCurve();
        }

        public void Polyline(object sender, RoutedEventArgs e)
        {
            new Polyline().Algorithm(coordinates, "Red");
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            new FloodFill().Algorithm(coordinates.Last().X, coordinates.Last().Y, "Blue", "Red");
        }
        public void Scanline(object sender, RoutedEventArgs e)
        {
            new Scanline().Sweep(coordinates);
        }

        public void LineTrim(object sender, RoutedEventArgs e)
        {
            var rect = new RectangleF(6, 6, 18, 18);
            new CohenSutherland().ClipSegment(rect, coordinates[0], coordinates[1]);
        }

        public void PolygonTrim(object sender, RoutedEventArgs e)
        {
            var rect = new Rectangle(6, 6, 18, 18);
            new SutherlandHodgman().TrimPolygon(rect, coordinates);
        }

        public void Translation(object sender, RoutedEventArgs e)
        {
            ClearBoard();
            new Translation(coordinates, -3, -3).Draw();
        }

        public void Rotation(object sender, RoutedEventArgs e)
        {
            ClearBoard();
            coordinates = new Rotation(coordinates, coordinates[0], 90).Rotate();
            new Polyline().Algorithm(coordinates, "Red");
        }

        public void Scale(object sender, RoutedEventArgs e)
        {
            ClearBoard();
            coordinates = new Scale(coordinates, coordinates[0], 2, 2)._Resize();
            new Polyline().Algorithm(coordinates, "Red");
        }

        public void ClearBoard()
        {
            foreach (var i in _board)
                i.Color = "White";
        }
    }
}
