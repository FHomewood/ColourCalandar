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

            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight*0.75;
            this.Width  = System.Windows.SystemParameters.PrimaryScreenWidth*0.75;
            grid.Height = this.Height - 4 * parent.cm;
            grid.Width = this.Width-7*parent.cm;
            grid.Margin = new Thickness(6 * parent.cm, 3 *parent.cm, 1 * parent.cm, 0);




            this.Top = -this.Height ;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth/2 - this.Width / 2;

            DrawSchedule();
        }

        private void DrawSchedule()
        {
            double minutesinaday = 60 * 24;

            for (int i = 0; i < parent.csv.Count; i++)
            {
                for (int j = -1; j < parent.csv[i].Task.Length; j++)
                {
                    double rectmargins = 10;
                    Rectangle rect = new Rectangle();
                    Label lbl = new Label();
                    rect.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    rect.Height = grid.Height / 7 - 2 * rectmargins;


                    double taskoffset;
                    double taskwidth;
                    if (j == -1)
                    {
                        taskwidth = (parent.csv[i].Time.First().Hour - 6) * 60 + (parent.csv[i].Time.First().Minute);
                        taskwidth *= grid.Width / minutesinaday;
                        taskoffset = 0;
                        try { lbl.Content = parent.csv[i - 1].Task.Last(); }
                        catch { lbl.Content = ""; }
                    }
                    else if (parent.csv[i].Time[j] == parent.csv[i].Time.Last())
                    {
                        taskwidth = (30 - parent.csv[i].Time[j].Hour) * 60 + (60 - parent.csv[i].Time[j].Minute);
                        taskwidth *= grid.Width / minutesinaday;
                        lbl.Content = parent.csv[i].Task[j];
                        taskoffset = (parent.csv[i].Time[j].Hour * 60
                                      - 360
                                      + parent.csv[i].Time[j].Minute  )
                            * grid.Width / minutesinaday;
                    }
                    else
                    {
                        taskwidth = (parent.csv[i].Time[j + 1].Hour - parent.csv[i].Time[j].Hour) * 60
                                    + (parent.csv[i].Time[j + 1].Minute - parent.csv[i].Time[j].Minute);
                        taskwidth *= grid.Width / minutesinaday;
                        lbl.Content = parent.csv[i].Task[j];
                        taskoffset = (parent.csv[i].Time[j].Hour * 60
                                      - 360
                                      + parent.csv[i].Time[j].Minute  )
                            * grid.Width / minutesinaday;
                    }

                    rect.Margin = new Thickness(taskoffset, rectmargins + (rect.Height + 2 * rectmargins - 2) * i,0,0);
                    lbl.Margin = rect.Margin;
                    rect.Width = taskwidth - 2;
                    rect.VerticalAlignment = VerticalAlignment.Top;
                    rect.HorizontalAlignment = HorizontalAlignment.Left;
                    TransformGroup labelTransform = new TransformGroup();
                    labelTransform.Children.Add(new RotateTransform(-90));
                    labelTransform.Children.Add(new TranslateTransform(0, -2 * rectmargins));
                    lbl.RenderTransform = labelTransform;
                    
                    grid.Children.Add(rect);
                    grid.Children.Add(lbl);
                }
            }
        }

        public void Transition()
        {
            showing = !showing;
            if (this.Visibility == Visibility.Hidden) this.Visibility = Visibility.Visible;
            begin = DateTime.Now;
            end = begin.AddSeconds(0.75);
            _timer.Start();
        }



        private void ChangeHeight()
        {
            double transitionPercentage = (begin - DateTime.Now).TotalSeconds / (begin - end).TotalSeconds;
            double showingloc = System.Windows.SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2;
            double hiddenloc = -this.Height;

            if (transitionPercentage < 1)
            {
                double hFactor = (1 - Math.Pow(Math.Cos(Math.PI * transitionPercentage / 2), 2));
                if (showing) this.Top = hiddenloc + hFactor * (showingloc - hiddenloc);
                else this.Top = showingloc + hFactor * (hiddenloc - showingloc);
            }
            else
            {
                if (showing) this.Top = showingloc;
                else
                {
                    this.Top = hiddenloc;
                    this.Visibility = Visibility.Hidden;
                }
            }
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
