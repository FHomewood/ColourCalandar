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
        public const int WS_EX_TRANSPARENT = 0x0000020; public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);


        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        double hue = 0;
        int transition = 1;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Interval = new TimeSpan(10 * 1000 * 10);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            // Change the extended window style to include WS_EX_TRANSPARENT
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            base.OnSourceInitialized(e);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            int mSecondsInTransition = 100;
            int framesInTransition = 50;
            double hueChange = 1f * Math.PI / 3f;

            if (transition > 0)
            {
                transition--;
                dispatcherTimer.Interval = new TimeSpan(10 * 1000 * mSecondsInTransition / framesInTransition);
                hue += hueChange / (double)framesInTransition;
                this.Background = new SolidColorBrush(Color.FromArgb(
                    24,
                    (byte)(128 + 127 * Math.Cos(hue)),
                    (byte)(128 + 127 * Math.Cos(hue + 2 * Math.PI / 3)),
                    (byte)(128 + 127 * Math.Cos(hue + 4 * Math.PI / 3))
                ));
                if (transition == 0)
                    dispatcherTimer.Interval = new TimeSpan(10 * 1000 * (1000 - DateTime.Now.Millisecond) + 10 * 1000 * 1000 * (59-DateTime.Now.Second));
            }
            else if (DateTime.Now.Minute % 10 != 0)
                dispatcherTimer.Interval = new TimeSpan(10 * 1000 * 1000 * 60);
            else
            {
                transition = framesInTransition;
                dispatcherTimer.Interval = new TimeSpan(1);
            }
        }
    }
}
