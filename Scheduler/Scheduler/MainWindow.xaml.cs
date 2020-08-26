using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scheduler
{
    public partial class MainWindow : Window
    {
        // Constants used for the window style
        public const int WS_EX_TRANSPARENT = 0x0000020;
        public const int GWL_EXSTYLE = (-20);

        // Import necessary DLLs for the click-through semi-transparent window
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        //Initialize timer
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        //Colour variables
        public double hue = 0;
        int transition = 1;

        //Create popup window object
        PopupWindow popup;

        public MainWindow()
        {
            InitializeComponent();

            popup = new PopupWindow(this);
            //Set Screen to the primary monitor
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            //Set time interval
            dispatcherTimer.Interval = new TimeSpan(10 * 1000 * 10);
            //Add the tick event
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            //Begin the timer
            dispatcherTimer.Start();

            //temporary show popup
            popup.Show();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            // Change the extended window style to include WS_EX_TRANSPARENT
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            base.OnSourceInitialized(e);

            popup.Update("Go to lunch");
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            int mSecondsInTransition = 50;
            int framesInTransition = 50;
            double hueChange = 1f * Math.PI / 3f;

            //If we're mid-transition between scenes
            if (transition > 0)
            {
                transition--;
                //set the timer according to the specified framerate
                dispatcherTimer.Interval = new TimeSpan(10 * 1000 * mSecondsInTransition / framesInTransition);

                //Change the hue
                hue += hueChange / (double)framesInTransition;
                //apply the new colour according to the hue formulae
                this.Background = new SolidColorBrush(Color.FromArgb(
                    10, //transparency 24 is a solid option
                    (byte)(128 + 127 * Math.Cos(hue)),
                    (byte)(128 + 127 * Math.Cos(hue + 2 * Math.PI / 3)),
                    (byte)(128 + 127 * Math.Cos(hue + 4 * Math.PI / 3))
                ));

                //if this is the end of the transition we set the next check to the next next minute (on the dot)
                if (transition == 0)
                {
                    dispatcherTimer.Interval = new TimeSpan(10 * 1000 * (1000 - DateTime.Now.Millisecond) );//+ 10 * 1000 * 1000 * (59 - DateTime.Now.Second));
                }
            }
            //If we're not mid-transition between scenes
            //Check if we're due to transition
            else if (DateTime.Now.Second % 10 != 0)
            {
                //not due? check again in 1 minute
                dispatcherTimer.Interval = new TimeSpan(10 * 1000 * 1000 );
            }
            //If we're due a transition
            else
            {
                //set the total number of transition frames
                transition = framesInTransition;
                //reset quickly so the transition can be initiated
                dispatcherTimer.Interval = new TimeSpan(1);

                //popup show location at beginning of transition
                popup.Update("10 Second reminder");
            }
        }
    }
}