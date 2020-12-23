using System.Collections.Generic;

namespace WpfApplication
{
    public class CriticalPIntersectionFirst : Comparer<CriticalP>
    {
        public override int Compare(CriticalP x, CriticalP y)
        {

            if (x.x_intersection.CompareTo(y.x_intersection) != 0)
            {
                return x.x_intersection.CompareTo(y.x_intersection);
            }
            else
            {
                return 0;
            }
        }
    }
}
