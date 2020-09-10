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
            grid.Width = this.Width;
            grid.Height = this.Height;



            this.Top = -this.Height ;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth/2 - this.Width / 2;

            DrawSchedule();
        }

        private void DrawSchedule()
        {

            grid.Width = this.Height;
            grid.Height = this.Width;
            double minutesinaday = 60 * 24;
            double gridHeight = this.Width;
            double gridWidth = this.Height;
            for (int i = 0; i < parent.csv.Count; i++) //i is the day of the task
            {
                for (int j = -1; j < parent.csv[i].Task.Length; j++) 
                {
                    double LeftRightMargins = 7;
                    double TopBottomMargins = 5;
                    Border border = new Border();
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    border.BorderThickness = new Thickness(1);
                    border.CornerRadius = new CornerRadius(6);
                    border.Padding = new Thickness(0);
                    border.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    TextBlock tBlock = new TextBlock();
                    tBlock.Padding = new Thickness(0);
                    tBlock.Margin = new Thickness(0);
                    border.Width = gridWidth / 7 - 2 * LeftRightMargins;

                    //Gotta find height and y-pos of each task
                    double task_height, task_ypos;

                    if (j == -1) //if it's the first task of the day try to get the previous days final task.
                    {
                        //Get the text from the previous day's final task
                        try { tBlock.Text = parent.csv[i - 1].Task.Last(); }
                        catch { tBlock.Text = ""; }

                        //Get the Time of the first task today and fill the gap
                        task_height = (parent.csv[i].Time.First().Hour) * 60 + (parent.csv[i].Time.First().Minute);

                        //Since it's the first task of the day it has no offset
                        task_ypos = 0;
                    }

                    else if (j == parent.csv[i].Task.Length - 1) //if it's the last task then we need to make sure it spans the remainder of the day
                    {
                        //Set the task name
                        tBlock.Text = parent.csv[i].Task[j];

                        //Get the time until midnight and fill the gap
                        task_height = (24 - parent.csv[i].Time[j].Hour) * 60 - (parent.csv[i].Time[j].Minute) - 2 * TopBottomMargins;

                        //Set the ypos
                        task_ypos = (parent.csv[i].Time[j].Hour * 60) + parent.csv[i].Time[j].Minute;
                    }
                    else //if not last or first we can simply do it as we would expect
                    {
                        //set text
                        tBlock.Text = parent.csv[i].Task[j];

                        //set height
                        task_height = (parent.csv[i].Time[j + 1].Hour - parent.csv[i].Time[j].Hour) * 60
                                    + (parent.csv[i].Time[j + 1].Minute - parent.csv[i].Time[j].Minute);

                        //set ypos
                        task_ypos = parent.csv[i].Time[j].Hour * 60 + parent.csv[i].Time[j].Minute;
                    }
                    task_ypos *= gridHeight / minutesinaday;
                    task_height *= gridHeight / minutesinaday;
                    task_height -= TopBottomMargins;
                    border.Height = task_height;
                    border.Margin = new Thickness(- ( (i+1) * (border.Width) + (2*i)*LeftRightMargins)
                        ,task_ypos,0,0);
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    tBlock.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    tBlock.TextAlignment = TextAlignment.Center;
                    border.BorderBrush = border.Background;

                    double TCentreX, TCentreY;
                    TCentreX = -border.Margin.Left;
                    TCentreY = -border.Margin.Top;
                    TransformGroup TextTransform = new TransformGroup();
                    TextTransform.Children.Add(new RotateTransform(-90, TCentreX, TCentreY));
                    //TextTransform.Children.Add( new TranslateTransform(0,0) );
                    border.RenderTransform = TextTransform;

                    border.Child = tBlock;
                    grid.Children.Add(border);
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid.Children.Clear();
            DrawSchedule();
            this.Top = -this.Height;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2;
            grid.Refresh();
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
