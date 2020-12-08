using System;
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
            new Polyline().Algorithm(trimBorder, "Green");
        }

        public void LineTrim(object sender, RoutedEventArgs e)
        {
            var rect = new RectangleF(5, 5, 20, 20);
            var newLine = CohenSutherland.ClipSegment(rect, coordinates[0], coordinates[1]);
            new Bresenham().Algorithm(newLine.Item1, newLine.Item2, "Red");
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

    public static class CohenSutherland
    {

        [Flags]
        enum OutCode
        {
            Inside = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

        private static OutCode ComputeOutCode(float x, float y, RectangleF r)
        {
            var code = OutCode.Inside;

            if (x < r.Left) code |= OutCode.Left;
            if (x > r.Right) code |= OutCode.Right;
            if (y < r.Top) code |= OutCode.Top;
            if (y > r.Bottom) code |= OutCode.Bottom;

            return code;
        }
        private static OutCode ComputeOutCode(Point p, RectangleF r) { return ComputeOutCode(p.X, p.Y, r); }

        public static Tuple<Point, Point> ClipSegment(RectangleF r, Point p1, Point p2)
        {
            // classify the endpoints of the line
            var outCodeP1 = ComputeOutCode(p1, r);
            var outCodeP2 = ComputeOutCode(p2, r);
            var accept = false;

            while (true)
            { // should only iterate twice, at most
                // Case 1:
                // both endpoints are within the clipping region
                if ((outCodeP1 | outCodeP2) == OutCode.Inside)
                {
                    accept = true;
                    break;
                }

                // Case 2:
                // both endpoints share an excluded region, impossible for a line between them to be within the clipping region
                if ((outCodeP1 & outCodeP2) != 0)
                {
                    break;
                }

                // Case 3:
                // The endpoints are in different regions, and the segment is partially within the clipping rectangle

                // Select one of the endpoints outside the clipping rectangle
                var outCode = outCodeP1 != OutCode.Inside ? outCodeP1 : outCodeP2;

                // calculate the intersection of the line with the clipping rectangle using parametric line equations
                var p = CalculateIntersection(r, p1, p2, outCode);

                // update the point after clipping and recaluculate outcode
                if (outCode == outCodeP1)
                {
                    p1 = p;
                    outCodeP1 = ComputeOutCode(p1, r);
                }
                else
                {
                    p2 = p;
                    outCodeP2 = ComputeOutCode(p2, r);
                }
            }
            // if clipping area contained a portion of the line
            if (accept)
            {
                return new Tuple<Point, Point>(p1, p2);
            }

            // the line did not intersect the clipping area
            return null;
        }

        private static Point CalculateIntersection(RectangleF r, Point p1, Point p2, OutCode clipTo)
        {
            var dx = (p2.X - p1.X);
            var dy = (p2.Y - p1.Y);

            var slopeY = dx / dy; // slope to use for possibly-vertical lines
            var slopeX = dy / dx; // slope to use for possibly-horizontal lines

            if (clipTo.HasFlag(OutCode.Top))
            {
                return new Point(
                    (int)(p1.X + slopeY * (r.Top - p1.Y)),
                    (int)(r.Top)
                    );
            }
            if (clipTo.HasFlag(OutCode.Bottom))
            {
                return new Point(
                    (int)(p1.X + slopeY * (r.Bottom - p1.Y)),
                    (int)(r.Bottom)
                    );
            }
            if (clipTo.HasFlag(OutCode.Right))
            {
                return new Point(
                    (int)(r.Right),
                    (int)(p1.Y + slopeX * (r.Right - p1.X))
                    );
            }
            if (clipTo.HasFlag(OutCode.Left))
            {
                return new Point(
                    (int)(r.Left),
                    (int)(p1.Y + slopeX * (r.Left - p1.X))
                    );
            }
            throw new ArgumentOutOfRangeException("clipTo = " + clipTo);
        }
    }

    public class CriticalP
    {
        public int index { get; set; }
        public int dir { get; set; }
        public double x_intersection { get; set; }
        public double inv_slope { get; set; }

        public CriticalP(int index, int dir, double x_intersection, double inv_slope)
        {
            this.index = index;
            this.dir = dir;
            this.x_intersection = x_intersection;
            this.inv_slope = inv_slope;
        }

    }

    public class CriticalPIntersectionFirst : Comparer<CriticalP>
    {
        public override int Compare(CriticalP x, CriticalP y)
        {

            if (x.x_intersection.CompareTo(y.x_intersection) != 0)
            {
                return x.x_intersection.CompareTo(y.x_intersection);
            }
            else
            {
                return 0;
            }
        }
    }

    public class Scanline
    {
        public void Sweep(List<Point> points)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            //Encontra o bounding box para y e os pontos críticos (mínimos em y).
            int y_min = int.MaxValue;
            int y_max = int.MinValue;
            List<CriticalP> criticals = new List<CriticalP>();
            for (int i = 0; i < points.Count(); i++)
            {
                if (points[i].Y < y_min)
                {
                    y_min = points[i].Y;
                }
                else if (points[i].Y > y_max)
                {
                    y_max = points[i].Y;
                }

                //Encontrar os pontos críticos.
                Point p_aux = points[(i + 1) % points.Count()];
                if (points[i].Y < p_aux.Y)
                {
                    criticals.Add(new CriticalP(
                        i,
                        1,
                        points[i].X,
                        (p_aux.X - points[i].X * 1.0f) / (p_aux.Y - points[i].Y * 1.0f)
                    ));
                }
                p_aux = points[(i - 1 + points.Count()) % points.Count()];
                if (points[i].Y < p_aux.Y)
                {
                    criticals.Add(new CriticalP(
                        i,
                        -1,
                        points[i].X,
                        (p_aux.X - points[i].X * 1.0f) / (p_aux.Y - points[i].Y * 1.0f)
                    ));
                }
            }

            //Início do algoritmo de varredura - Loop de varredura
            List<CriticalP> active_criticalPs = new List<CriticalP>();
            //Java: CriticalPComparator comparator = new CriticalPComparator();

            for (int y = y_min; y <= y_max; y++)
            {
                //Atualiza o valor de cada intersecção nos pontos ativos.
                foreach (CriticalP e in active_criticalPs)
                {
                    e.x_intersection += e.inv_slope;
                }

                //Adiciona as arestas com pontos criíticos para o y corrente.
                foreach (CriticalP e in criticals)
                {
                    if (points[e.index].Y == y)
                    {
                        active_criticalPs.Add(e);
                    }
                }

                //Remove os pontos com y_max = y_var.
                for (int i = active_criticalPs.Count() - 1; i >= 0; i--)
                {
                    CriticalP e = active_criticalPs[i];
                    Point p_max = points[(e.index + e.dir + points.Count()) % points.Count()];
                    if (p_max.Y == y)
                    {
                        active_criticalPs.RemoveAt(i);
                    }
                }

                //Ordena os pontos ativos conforme o valor de x_intersection para o y corrente.
                active_criticalPs.Sort(new CriticalPIntersectionFirst());

                //Pinta entre cada par de pontos ativos.
                for (int i = 0; i < active_criticalPs.Count(); i += 2)
                {
                    int x_start = Convert.ToInt32(Math.Round(active_criticalPs[i].x_intersection));
                    int x_end = Convert.ToInt32(Math.Round(active_criticalPs[i + 1].x_intersection));

                    for (int x = x_start; x < x_end; x++)
                    {
                        if (windows._board[windows.GetIndex(x, y)].Color == "Red")
                            continue;
                        windows.PutPixel(x, y, "Blue");
                    }
                }
            }
        }
    }
    public class Rotation
    {
        public double[,] matrix;
        public List<Point> originalCoordinates;
        public List<Point> rotatedCoordinates;
        public Point pivot;

        public Rotation(List<Point> coordinates, Point p, int angle)
        {
            double d = ExtensionMethods.GetAngle(angle);
            matrix = new double[,] { { Math.Cos(d), -Math.Sin(d), 0 }, { Math.Sin(d), Math.Cos(d), 0 }, { 0, 0, 1 } };
            originalCoordinates = new List<Point>(coordinates);
            rotatedCoordinates = new List<Point>();

            pivot = new Point { X = p.X, Y = p.Y };

            //Do translation
            for (int i = 0; i < originalCoordinates.Count(); i++)
            {
                originalCoordinates[i].X -= pivot.X;
                originalCoordinates[i].Y -= pivot.Y;
            }
        }
        public Point ApplyRotationMatrix(Point point, double[,] matrix)
        {
            List<double> result = new List<double>() { 0, 0, 1 };
            List<int> vector = new List<int> { point.X, point.Y, point.H };
            List<int> _pivot = new List<int> { pivot.X, pivot.Y, pivot.H };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
                // Undo translation.
                result[i] += _pivot[i];
            }

            Point RotatedPoint = new Point
            {
                X = Convert.ToInt32(result[0]),
                Y = Convert.ToInt32(result[1])
            };

            return RotatedPoint;
        }

        public void Draw()
        {
            foreach (Point p in originalCoordinates)
            {
                rotatedCoordinates.Add(ApplyRotationMatrix(p, matrix));
            }
            var figure = new Polyline();
            figure.Algorithm(rotatedCoordinates, "Red");
        }

        ~Rotation() { }
    }

    public class Translation
    {
        public int[,] matrix;
        public List<Point> originalCoordinates;
        public List<Point> finalPoints;
        public Translation(List<Point> p, int tx, int ty)
        {
            finalPoints = new List<Point>();
            originalCoordinates = p;
            matrix = new int[,] { { 1, 0, tx }, { 0, 1, ty }, { 0, 0, 1 } };
        }

        public Point ApplyTranslationMatrix(Point point, int[,] matrix)
        {
            List<int> result = new List<int>() { 0, 0, 1 };
            List<int> vector = new List<int>
            {
                point.X,
                point.Y,
                point.H
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }

            Point translatedPoint = new Point
            {
                X = result[0],
                Y = result[1]
            };

            return translatedPoint;
        }

        public List<Point> GetTranslatedPoints()
        {
            foreach (Point p in originalCoordinates)
            {
                finalPoints.Add(ApplyTranslationMatrix(p, matrix));
            }
            return finalPoints;
        }

        public void Draw()
        {
            var figure = new Polyline();
            figure.Algorithm(GetTranslatedPoints(), "Red");
        }
    }

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

            for (double t = 0; t <= 1; t += 0.5)
            {
                Point finalPoint = Algorithm(t);
                line.Algorithm(initialPoint, finalPoint, "Red");
                initialPoint = finalPoint;
            }
        }
    }

    public class FloodFill
    {
        public void Algorithm(int x, int y, string color, string edgeColor)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            var current = windows.GetPoint(x, y);
            if (current.Color != edgeColor && current.Color != color)
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
        public void Algorithm(List<Point> points, string color)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                bresenham.Algorithm(points[i % points.Count()], points[(i + 1) % points.Count()], color);
            }
        }
    }

    public class Bresenham
    {
        public void Algorithm(Point p1, Point p2, string color)
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
                Draw(x1, y1, color);
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

        public void Draw(int x, int y, string color)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            windows.PutPixel(x, y, color);
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

    public static class ExtensionMethods
    {
        public static void Swap(Point a)
        {
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

        public static double GetAngle(int degree)
        {
            return (Math.PI * degree / 180.0);
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
            H = 1;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int H { get; set; }

        public Point Dot(double t, Point p)
        {
            var result = new Point
            {
                X = Convert.ToInt32(p.X * t),
                Y = Convert.ToInt32(p.Y * t)
            };
            return result;
        }

        public Point Sum(Point p1, Point p2)
        {
            var result = new Point
            {
                X = p1.X + p2.X,
                Y = p1.Y + p2.Y
            };

            return result;
        }

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
