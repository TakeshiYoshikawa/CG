using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApplication
{
    public class Scale
    {
        public double[,] matrix;
        public List<Point> originalCoordinates;
        public Point setPoint;
        
        public Scale(List<Point> coordinates, Point p, int scaleX, int scaleY)
        {
            matrix = new double[,] { { scaleX, 0, 0 }, { 0, scaleY, 0 }, { 0, 0, 1 } };
            originalCoordinates = new List<Point>(coordinates);
            
            setPoint = new Point { 
                X = p.X, 
                Y = p.Y 
            };

            //Do translation
            for (int i = 0; i < originalCoordinates.Count(); i++)
            {
                originalCoordinates[i].X -= setPoint.X;
                originalCoordinates[i].Y -= setPoint.Y;
            }
        }
        
        public Point ApplyScalationMatrix(Point point, double[,] matrix)
        {
            List<double> result = new List<double>() { 0, 0, 1 };
            List<int> vector = new List<int> { point.X, point.Y, point.H };
            List<int> _setPoint = new List<int> { setPoint.X, setPoint.Y, setPoint.H };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
                // Undo translation.
                result[i] += _setPoint[i];
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
            List<Point> scaledCoordinates = new List<Point>();
            foreach (Point p in originalCoordinates)
            {
                scaledCoordinates.Add(ApplyScalationMatrix(p, matrix));
            }
            var figure = new Polyline();
            figure.Algorithm(scaledCoordinates, "Red");
        }
    }
}
