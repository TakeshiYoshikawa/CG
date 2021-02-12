using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApplication
{
    public class Scale
    {
        public int[,] matrix;
        public List<Point> originalCoordinates;
        public List<Point> scaledCoordinates;
        public Point setPoint;

        public Scale(List<Point> coordinates, Point p, int scaleX, int scaleY)
        {
            matrix = new int[,] { { scaleY, 0, 0 }, 
                                     { 0, scaleX, 0 }, 
                                     { 0, 0, 1 } };
            originalCoordinates = new List<Point>(coordinates);
            scaledCoordinates = new List<Point>();


            setPoint = new Point
            {
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

        public Point ApplyScalationMatrix(Point point, int[,] matrix)
        {
            List<int> result = new List<int>() { 0, 0, 1 };
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

            Point scaledPoint = new Point
            {
                X = result[0],
                Y = result[1]
            };

            return scaledPoint;
        }
        
        public List<Point> _Resize()
        {
            foreach (Point p in originalCoordinates)
            {
                scaledCoordinates.Add(ApplyScalationMatrix(p, matrix));
            }
            return scaledCoordinates;
        }
    }
}
