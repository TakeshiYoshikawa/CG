using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication
{
    class Perspective
    {
        public List<Point> coordinates;

        double[,] PPer, transposedCoordinates, result;

        public Perspective(List<Point> coordinates)
        {
            this.coordinates = new List<Point>(coordinates);

            int d = -30;

            PPer = new double[4, 4]{
                {d,0,0,0},
                {0,d,0,0},
                {0,0,d,0},
                {0,0,1,0}
            };

            transposedCoordinates = new double[4, coordinates.Count()];

            for (int i = 0; i < coordinates.Count(); i++)
                transposedCoordinates[0, i] = coordinates[i].X;
            for (int i = 0; i < coordinates.Count(); i++)
                transposedCoordinates[1, i] = coordinates[i].Y;
            for (int i = 0; i < coordinates.Count(); i++)
                transposedCoordinates[2, i] = coordinates[i].Z;
            for (int i = 0; i < coordinates.Count(); i++)
                transposedCoordinates[3, i] = coordinates[i].H;
        }

        public static double[,] Multiply(double[,] matrix1, double[,] matrix2)
        {
            var result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];

            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    for (int k = 0; k < matrix1.GetLength(0); k++)
                    {
                        result[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }

            return result;
        }

        public static double[,] ToHomogenous(double[,] coordinates)
        {
            double[,] homoResult = (double[,])coordinates.Clone();

            for (int i = 0; i < homoResult.GetLength(1); i++)
                for (int j = 0; j <= 3; j++)
                    homoResult[j, i] /= coordinates[3, i];

            return homoResult;
        }

        public void CalculatePerspective()
        {
            result = Multiply(PPer, transposedCoordinates);
            result = ToHomogenous(result);
        }

        public void Draw()
        {
            var a = new Point(Convert.ToInt32(result[1, 0]), Convert.ToInt32(result[0, 0]));
            var b = new Point(Convert.ToInt32(result[1, 1]), Convert.ToInt32(result[0, 1]));
            var c = new Point(Convert.ToInt32(result[1, 2]), Convert.ToInt32(result[0, 2]));
            var d = new Point(Convert.ToInt32(result[1, 3]), Convert.ToInt32(result[0, 3]));
            var e = new Point(Convert.ToInt32(result[1, 4]), Convert.ToInt32(result[0, 4]));
            var f = new Point(Convert.ToInt32(result[1, 5]), Convert.ToInt32(result[0, 5]));
            var g = new Point(Convert.ToInt32(result[1, 6]), Convert.ToInt32(result[0, 6]));
            var h = new Point(Convert.ToInt32(result[1, 7]), Convert.ToInt32(result[0, 7]));

            new Bresenham().Algorithm(a, b, "Red"); //AB
            new Bresenham().Algorithm(b, c, "Red"); //BC
            new Bresenham().Algorithm(c, d, "Red"); //CD
            new Bresenham().Algorithm(d, a, "Red"); //DA
            new Bresenham().Algorithm(e, f, "Red"); //EF
            new Bresenham().Algorithm(f, g, "Red"); //FG
            new Bresenham().Algorithm(g, h, "Red"); //GH
            new Bresenham().Algorithm(h, e, "Red"); //HE
            new Bresenham().Algorithm(c, g, "Red"); //CG
            new Bresenham().Algorithm(b, f, "Red"); //BF
            new Bresenham().Algorithm(d, h, "Red"); //DH
            new Bresenham().Algorithm(a, e, "Red"); //AE
        }
    }
}
