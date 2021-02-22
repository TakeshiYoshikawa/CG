using System;

namespace WpfApplication
{
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
        public Point(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            H = 1;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
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
