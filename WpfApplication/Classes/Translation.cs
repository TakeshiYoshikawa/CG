using System.Collections.Generic;

namespace WpfApplication
{
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
}
