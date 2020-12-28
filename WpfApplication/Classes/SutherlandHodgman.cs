using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace WpfApplication
{
    public class SutherlandHodgman
    {
        public List<Point> TrimPolygon(Rectangle rect, List<Point> subjectPolygon)
        {
            var clipPolygon = new List<Tuple<Point, Point>>() { 
                new Tuple<Point, Point> (new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top)),
                new Tuple<Point, Point> (new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top)),
                new Tuple<Point, Point> (new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom)),
                new Tuple<Point, Point> (new Point(rect.Right, rect.Bottom), new Point(rect.Left, rect.Bottom))
            };
            var outputList = subjectPolygon.ToList();

            foreach (var clipEdge in clipPolygon)
            {
                var inputList = outputList.ToList();
                outputList.Clear();

                for(int i = 0; i < inputList.Count(); i++)
                {
                    Point currentPoint = inputList[i];
                    Point previousPoint = inputList[(i + inputList.Count() - 1) % inputList.Count()];
                    Point intersectingPoint = ComputeIntersection(previousPoint, currentPoint, clipEdge);

                    if(isInside(rect, currentPoint))
                    {
                        if(!isInside(rect, previousPoint))
                        {
                            outputList.Add(intersectingPoint);
                        }
                        outputList.Add(currentPoint);
                    }
                    else if(isInside(rect, previousPoint))
                    {
                        outputList.Add(intersectingPoint);
                    }
                }
            }
            return outputList;
        }


        public bool isInside(RectangleF rect, Point p)
        {
            return rect.Contains(p.X, p.Y);
        }
        public Point ComputeIntersection(Point p1, Point p2, Tuple<Point, Point> clipEdge)
        {
            var xi = 0;
            var yi = 0;
            int v = 0;
            
            if (clipEdge.Item1.X == clipEdge.Item2.X)
            {
                xi = clipEdge.Item1.X;
                try { yi = (xi - p1.X) * (p2.Y - p1.Y) / (p2.X - p1.X) + p1.Y; }
                catch (DivideByZeroException) { }
            } else if(clipEdge.Item1.Y == clipEdge.Item2.Y)
            {
                yi = clipEdge.Item1.Y;
                try { xi = (yi - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X; }
                catch (DivideByZeroException) { }
            }

            return new Point(
                Convert.ToInt32(xi),
                Convert.ToInt32(yi)
            );
        }
    }
}
