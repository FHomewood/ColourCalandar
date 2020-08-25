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
        public PopupWindow()
        {
            InitializeComponent();
            double cm = (double)(new System.Windows.LengthConverter().ConvertFrom("1cm"));
            this.Width = 10 * cm;
            this.Height = 3 * cm;
        }
    }
}
