using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication.ViewModels;

namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        List<MatrixElement> _board;
        public MainWindow()
        {
            InitializeComponent();
            int rows = 20;
            int columns = 20;

            _board = new List<MatrixElement>();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    _board.Add(new MatrixElement(r, c) { Color = "White" });
                }
            }
            Board.ItemsSource = _board;
        }

        private void CellClick(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var point = (MatrixElement)border.Tag;
            point.Color = "#00BFFF";
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test", "Test");
        }
    }

    public class MatrixElement
    {
        private string _color;

        public MatrixElement(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (ColorChanged != null)
                {
                    ColorChanged(this, EventArgs.Empty);
                    MessageBox.Show("[" + this.X.ToString() + "]" + "[" + this.Y.ToString() + "]");
                }
            }
        }
        public event EventHandler ColorChanged;
    }
}
