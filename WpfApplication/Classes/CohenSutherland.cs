using System;
using System.Drawing;

namespace WpfApplication
{
    public class CohenSutherland
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

            if (x < r.Left)
                code |= OutCode.Left;
            if (x > r.Right)
                code |= OutCode.Right;
            if (y < r.Top)
                code |= OutCode.Top;
            if (y > r.Bottom)
                code |= OutCode.Bottom;

            return code;
        }
        private static OutCode ComputeOutCode(Point p, RectangleF r)
        {
            return ComputeOutCode(p.X, p.Y, r);
        }

        public void ClipSegment(RectangleF r, Point p1, Point p2)
        {
            new Bresenham().Algorithm(p1, p2, "LightGray");
            OutCode outCodeP1 = ComputeOutCode(p1, r);
            OutCode outCodeP2 = ComputeOutCode(p2, r);
            bool accept = false;

            while (true)
            {
                if ((outCodeP1 | outCodeP2) == OutCode.Inside)
                {
                    accept = true;
                    break;
                }
                else if ((outCodeP1 & outCodeP2) != 0)
                {
                    break;
                }
                else
                {
                    OutCode outcodeOut = outCodeP1 != OutCode.Inside ? outCodeP1 : outCodeP2;
                    Point p = ComputeIntersection(r, p1, p2, outcodeOut);

                    if (outcodeOut == outCodeP1)
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
            }
            if (accept)
                new Bresenham().Algorithm(p1, p2, "Green");
            else
               return;
        }

        private static Point ComputeIntersection(RectangleF r, Point p1, Point p2, OutCode clipTo)
        {
            var dx = (p2.X - p1.X);
            var dy = (p2.Y - p1.Y);

            var slopeY = dx / (dy + 0.000001); // slope to use for possibly-vertical lines
            var slopeX = dy / (dx + 0.000001); // slope to use for possibly-horizontal lines

            if (clipTo.HasFlag(OutCode.Top))
            {
                return new Point(
                    Convert.ToInt32(p1.X + slopeY * (r.Top - p1.Y)),
                    Convert.ToInt32((r.Top))
                    );
            }
            if (clipTo.HasFlag(OutCode.Bottom))
            {
                return new Point(
                    Convert.ToInt32(p1.X + slopeY * (r.Bottom - p1.Y)),
                    Convert.ToInt32(r.Bottom)
                    );
            }
            if (clipTo.HasFlag(OutCode.Right))
            {
                return new Point(
                    Convert.ToInt32(r.Right),
                    Convert.ToInt32(p1.Y + slopeX * (r.Right - p1.X))
                    );
            }
            if (clipTo.HasFlag(OutCode.Left))
            {
                return new Point(
                    Convert.ToInt32(r.Left),
                    Convert.ToInt32(p1.Y + slopeX * (r.Left - p1.X))
                    );
            }
            throw new ArgumentOutOfRangeException("clipTo = " + clipTo);
        }
    }
}
