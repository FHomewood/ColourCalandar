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
using System.IO;

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

        //Initialize keyboard hook
        LowLevelKeyboardListener keyboardHook;

        //Colour variables
        public Color currentTaskcol = Color.FromRgb(255,255,255);
        int transition = 1;
        string currentTask = "";
        List<DaySchedule> csv;

        Key previous_keypressed;


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

            //Link the current keyboard to the hook and define the key input function
            keyboardHook = new LowLevelKeyboardListener();
            keyboardHook.OnKeyPressed += OnKeyPressed;
            keyboardHook.HookKeyboard();

            //pull calendar from the UserSchedule.csv resource
            csv = ReadCSV();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            int mSecondsInTransition = 50;
            int framesInTransition = 50;
            Color targetColour = GetNextTaskColour();

            //If we're mid-transition between scenes
            if (transition > 0)
            {
                transition--;
                //set the timer according to the specified framerate
                dispatcherTimer.Interval = new TimeSpan(10 * 1000 * mSecondsInTransition / framesInTransition);

                //Change the colour


                //apply the new colour according to the hue formulae


                //if this is the end of the transition we set the next check to the next next minute (on the dot)
                if (transition == 0)
                {
                    dispatcherTimer.Interval = TimeToNextTask();
                    popup.Update(GetCurrentTaskTitle());
                }
            }
            //If we're not mid-transition between scenes
            //Check if we're due to transition
            else if (currentTask != GetCurrentTaskTitle())
            {
                transition = framesInTransition;
                dispatcherTimer.Interval = new TimeSpan(1);
            }
            else
            {
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            }
            currentTask = GetCurrentTaskTitle();
        }

        private string GetCurrentTaskTitle()
        {
            //This linear search can likely be improved as the dates are ordered
            for (int i = 0; i < csv.Count; i++)
            {
                if (DateTime.Now > csv[i].Day &&
                    DateTime.Now < csv[i].Day.AddDays(1))
                {
                    for (int j = 0; j < csv[i].Time.Length - 1; j++)
                    {
                        if (DateTime.Now > csv[i].Time[j] &&
                            DateTime.Now < csv[i].Time[j + 1])
                        {
                            return csv[i].Task[j];
                        }
                    }
                    return csv[i].Task.Last();
                }
            }
            return "";
        }

        private Color GetNextTaskColour()
        {
            //This linear search can likely be improved as the dates are ordered
            for (int i = 0; i < csv.Count; i++)
            {
                if (DateTime.Now > csv[i].Day &&
                    DateTime.Now < csv[i].Day.AddDays(1))
                {
                    for (int j = 0; j < csv[i].Time.Length - 1; j++)
                    {
                        if (DateTime.Now > csv[i].Time[j] &&
                            DateTime.Now < csv[i].Time[j + 1])
                        {
                            return csv[i].Colour[j];
                        }
                    }
                    return csv[i].Colour.Last();
                }
            }
            return Color.FromRgb(255,255,255);
        }

        private TimeSpan TimeToNextTask()
        {
            //This linear search can likely be improved as the dates are ordered
            for (int i = 0; i < csv.Count; i++)
            {
                if (DateTime.Now > csv[i].Day &&
                    DateTime.Now < csv[i].Day.AddDays(1))
                {
                    for (int j = 0; j < csv[i].Time.Length - 1; j++)
                    {
                        if (DateTime.Now > csv[i].Time[j] &&
                            DateTime.Now < csv[i].Time[j + 1])
                        {
                            return csv[i].Time[j + 1] - DateTime.Now;
                        }
                    }
                }
            }
            return DateTime.Today.AddDays(1) - DateTime.Now;
        }

        private List<DaySchedule> ReadCSV()
        {
            List<DaySchedule> csv = new List<DaySchedule>();

            // We change file extension here to make sure it's a .csv file.
            // TODO: Error checking.
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension("Resources//UserSchedule", ".csv"));
            for (int i = 0; i < lines.Length; i++)
            {
                csv.Add(new DaySchedule(lines[i]));
            }
            return csv;
        }

        void OnKeyPressed(object sender, KeyPressedArgs e)
        {
            Key previousKeyReplacement = e.KeyPressed;
            if (previous_keypressed == Key.RightAlt) {
                previousKeyReplacement = Key.None;
                switch (e.KeyPressed)
                {
                    case Key.T:         {   //hotkey for reshowing the popup menu
                                            popup.Update();
                                            break;                                          }
                    case Key.Space:     {   //hotkey for showing/hiding the planner menu
                                            break;                                          }
                    case Key.Escape:    {   //hotkey for exiting program
                                            popup.Close();
                                            this.Close();
                                            break;                                          }
                }
            }
            previous_keypressed = previousKeyReplacement;
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keyboardHook.UnHookKeyboard();
        }
    }
}