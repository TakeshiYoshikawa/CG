namespace WpfApplication
{
    public class CriticalP
    {
        public int index { get; set; }
        public int dir { get; set; }
        public double x_intersection { get; set; }
        public double inv_slope { get; set; }

        public CriticalP(int index, int dir, double x_intersection, double inv_slope)
        {
            this.index = index;
            this.dir = dir;
            this.x_intersection = x_intersection;
            this.inv_slope = inv_slope;
        }

    }
}
