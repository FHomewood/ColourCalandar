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
using System.Windows.Shapes;

namespace Scheduler
{
    public partial class PopupWindow : Window
    {
        MainWindow parent;
        public PopupWindow(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
            double cm = (double)(new System.Windows.LengthConverter().ConvertFrom("1cm"));
            this.Width = 10 * cm;
            this.Height = 3 * cm;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Background = new SolidColorBrush(Color.FromRgb(0, 20, 30));
            lblTitle.Margin = new Thickness(1*cm, 0.3*cm, 0, 0);
            lblTitle.FontSize = 15;
            lblTitle.FontFamily = new FontFamily("Century Gothic");

            Rectangle rect = new Rectangle();
        }
    }
}
