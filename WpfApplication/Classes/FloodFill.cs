using System.Windows;

namespace WpfApplication
{
    public class FloodFill
    {
        public void Algorithm(int x, int y, string color, string edgeColor)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            var current = windows.GetPoint(x, y);
            if (current.Color != edgeColor && current.Color != color)
            {
                windows.PutPixel(x, y, color);
                Algorithm(x + 1, y, color, edgeColor);
                Algorithm(x, y + 1, color, edgeColor);
                Algorithm(x - 1, y, color, edgeColor);
                Algorithm(x, y - 1, color, edgeColor);
            }
        }
    }
}
