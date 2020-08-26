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
using System.Windows.Threading;

namespace Scheduler
{
    public partial class PopupWindow : Window
    {
        MainWindow parent;
        int transitionIndex = 0;
        double transitionTime;

        //Initialize timer
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public PopupWindow(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
            double cm = (double)(new System.Windows.LengthConverter().ConvertFrom("1cm"));
            this.Width = 7 * cm;
            this.Height = 1 * cm;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Background = new SolidColorBrush(Color.FromRgb(0, 20, 30));
            lblTitle.Margin = new Thickness(0.7*cm, 0, 0, 0);
            lblTitle.FontSize = 15;
            lblTitle.FontFamily = new FontFamily("Century Gothic");

            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Tick += timer;
        }

        public void Update(double secDuration)
        {
            transitionTime = secDuration;
            dispatcherTimer.Start();
        }

        void timer(object sender, EventArgs e)
        {
            transitionIndex++;
            double transitionPercentage = transitionIndex * dispatcherTimer.Interval.TotalSeconds / transitionTime;
            double valHeight = this.Height * (1- Math.Pow(Math.Cos(transitionPercentage * Math.PI),6));
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - valHeight;
            if (transitionPercentage >= 1)
            {
                transitionIndex = 0;
                dispatcherTimer.Stop();
            }
                
        }
    }
}
