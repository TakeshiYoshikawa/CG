using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApplication
{
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
}
