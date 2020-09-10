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
        DateTime begin,end;


        //Initialize timer
        DispatcherTimer _timer = new DispatcherTimer();

        public PopupWindow(MainWindow parent)
        {
            InitializeComponent();

            //Gives access to the hue of the overlay window
            //this isn't actually used at any point yet
            this.parent = parent;

            //format popup window
                //size
            this.Width = 7 * parent.cm;
            this.Height = 1 * parent.cm;
                //location
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
                //colour
            this.Background = new SolidColorBrush(Color.FromRgb(0, 20, 30));

            //format popup text
            lblTitle.Margin = new Thickness(0.7 * parent.cm, 0, 0, 0);
            lblTitle.FontSize = 15;
            lblTitle.FontFamily = new FontFamily("Century Gothic");

            //set the timer interval and assign the timer event to a subroutine
            _timer.Interval = new TimeSpan((long)(10000000/parent.winFramerate));
            _timer.Tick += tick;
        }

        //this is the subroutine to call to automatically notify a new task occuring
        public void Update(string task)
        {
            //set the text contents to be the desired task
            lblTitle.Content = task;
            lblTitle.Foreground = new SolidColorBrush(parent.currentTaskcol);
            //update the begin and end times
            begin = DateTime.Now;
            end = DateTime.Now.AddSeconds(6);

            //begin the timer
            _timer.Start();
        }
        public void Update()
        {
            begin = DateTime.Now;
            end = DateTime.Now.AddSeconds(6);
            this.Show();
            //begin the timer
            _timer.Start();
        }

        void tick(object sender, EventArgs e)
        {
            //calculate how far through the transition we are
            double transitionPercentage = (begin - DateTime.Now).TotalSeconds / (begin - end).TotalSeconds;
            //compute how far above the lower border the popup should be
            double valHeight = this.Height * (1 - Math.Pow(Math.Cos(transitionPercentage * Math.PI), 8));
            //assign this to the window's y coordinate
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - valHeight;
            this.Refresh();
            //if the transition is over
            if (transitionPercentage >= 1)
            {
                //stop the timer
                this.Top = System.Windows.SystemParameters.PrimaryScreenHeight;
                _timer.Stop();
            }
                
        }
    }
}
