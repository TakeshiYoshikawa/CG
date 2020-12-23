using System;
using System.Collections.Generic;

namespace WpfApplication
{
    public static class ExtensionMethods
    {
        public static void Swap(Point a)
        {
            var temp = a.X;
            a.X = a.Y;
            a.Y = temp;
        }
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        public static void Swap<T>(this List<T> list1, List<T> list2, int index)
        {
            T temp = list1[index];
            list1[index] = list2[index];
            list2[index] = temp;
        }

        public static double GetAngle(int degree)
        {
            return (Math.PI * degree / 180.0);
        }
    }
}
