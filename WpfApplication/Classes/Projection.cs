using System.Collections.Generic;

namespace WpfApplication
{
    public class Projection
    {
        public List<Point> coordinates;
        public Projection(List<Point> coordinates)
        {
            this.coordinates = new List<Point>(coordinates);
        }

        public void Draw()
        {
            var a = coordinates[0];
            var b = coordinates[1];
            var c = coordinates[2];
            var d = coordinates[3];
            var e = coordinates[4];
            var f = coordinates[5];
            var g = coordinates[6];
            var h = coordinates[7];


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
