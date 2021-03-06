﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace WpfApplication
{
    public class SutherlandHodgman
    {
        public void TrimPolygon(Rectangle rect, List<Point> subjectPolygon)
        {
            var clipPolygon = new List<Tuple<Point, Point, string>>() {
                new Tuple<Point, Point, string> (new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top), "left"),
                new Tuple<Point, Point, string> (new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom), "right"),
                new Tuple<Point, Point, string> (new Point(rect.Right, rect.Bottom), new Point(rect.Left, rect.Bottom), "bottom"),
                new Tuple<Point, Point, string> (new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top), "top"),
            };
            var outputList = subjectPolygon.ToList();

            foreach (var clipEdge in clipPolygon)
            {
                var inputList = outputList.ToList();
                outputList.Clear();

                for (int i = 0; i < inputList.Count(); i++)
                {
                    Point currentPoint = inputList[i];
                    Point previousPoint = inputList[(i + inputList.Count() - 1) % inputList.Count()];
                    Point intersectingPoint = ComputeIntersection(previousPoint, currentPoint, clipEdge);

                    if (IsInside(clipEdge, currentPoint))
                    {
                        if (!IsInside(clipEdge, previousPoint))
                        {
                            outputList.Add(intersectingPoint);
                        }
                        outputList.Add(currentPoint);
                    }
                    else if (IsInside(clipEdge, previousPoint))
                    {
                        outputList.Add(intersectingPoint);
                    }
                }
            }

            new Polyline().Algorithm(subjectPolygon, "LightGray");
            new Polyline().Algorithm(outputList, "Green");
        }

        public bool IsInside(Tuple<Point, Point, string> edge, Point p)
        {
            if (edge.Item3.Equals("left"))
            {
                return p.X >= edge.Item1.X;
            } 
            else if (edge.Item3.Equals("right"))
            {
                return p.X <= edge.Item1.X;
            } 
            else if (edge.Item3.Equals("bottom"))
            {
                return p.Y <= edge.Item1.Y;
            } 
            else if (edge.Item3.Equals("top"))
            {
                return p.Y >= edge.Item1.Y;
            }
            else
            {
                return false;
            }
        }

        public Point ComputeIntersection(Point p1, Point p2, Tuple<Point, Point, string> clipEdge)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;

            var slopeY = dx / (dy + 0.00001);
            var slopeX = dy / (dx + 0.00001);

            var xi = 0;
            var yi = 0;

            if (clipEdge.Item1.X == clipEdge.Item2.X)
            {
                xi = clipEdge.Item1.X;
                yi = Convert.ToInt32((xi - p1.X) * slopeX + p1.Y);
            }
            else if (clipEdge.Item1.Y == clipEdge.Item2.Y)
            {
                yi = clipEdge.Item1.Y;
                xi = Convert.ToInt32((yi - p1.Y) * slopeY + p1.X);
            }

            return new Point(xi, yi);
        }
    }
}
