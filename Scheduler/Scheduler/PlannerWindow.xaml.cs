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
    public partial class PlannerWindow : Window
    {
        MainWindow parent;
        DispatcherTimer _timer = new DispatcherTimer();
        bool showing = false;
        DateTime begin, end;

        public PlannerWindow(MainWindow parent)
        {
            InitializeComponent();
            _timer.Interval = new TimeSpan((long)(10000000 / parent.winFramerate));
            _timer.Tick += tick;
            this.parent = parent;

            this.Top = -this.Height ;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth/2 - this.Width / 2;
        }

        public void Transition()
        {
            showing = !showing;

            begin = DateTime.Now;
            end = begin.AddSeconds(1);

            _timer.Start();
        }



        private void ChangeHeight()
        {
            double transitionPercentage = (begin - DateTime.Now).TotalSeconds / (begin - end).TotalSeconds;
            double hFactor;
            double showingloc = System.Windows.SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2;
            double hiddenloc = -this.Height;
            hFactor = (1 - Math.Pow(Math.Cos(Math.PI * transitionPercentage / 2), 2));

            if (transitionPercentage < 1)
            {
                if (showing) this.Top = hiddenloc + hFactor * (showingloc - hiddenloc);
                else this.Top = showingloc + hFactor * (hiddenloc - showingloc);
            }
            else
            {
                if (showing) this.Top = showingloc;
                else this.Top = hiddenloc;
            }



            ////calculate how far through the transition we are
            //double transitionPercentage = (begin - DateTime.Now).TotalSeconds / (begin - end).TotalSeconds;
            ////compute how far above the lower border the popup should be
            //double valHeight = this.Height * (1 - Math.Pow(Math.Cos(transitionPercentage * Math.PI), 8));
            ////assign this to the window's y coordinate
            //this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - valHeight;
            ////if the transition is over
            //if (transitionPercentage >= 1)
            //{
            //    //stop the timer
            //    this.Top = System.Windows.SystemParameters.PrimaryScreenHeight;
            //    _timer.Stop();
            //}


        }

        private void tick(object sender, EventArgs e)
        {
            bool endTick = false;
            ChangeHeight();


            if (endTick) _timer.Stop();

            this.Refresh();

        }
    }
}
