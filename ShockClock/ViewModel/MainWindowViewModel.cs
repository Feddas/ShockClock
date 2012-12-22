using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace ShockClock.ViewModel
{
    public class MainWindowViewModel : DependencyObject // : INotifyPropertyChanged
    {
        //INotifyPropertyChanged
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //    {
        //        this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        //    }
        //}

        public string ClockText
        {
            get { return (string)GetValue(ClockTextProperty); }
            set { SetValue(ClockTextProperty, value); }
        }
        public static readonly DependencyProperty ClockTextProperty =
            DependencyProperty.Register("ClockText", typeof(string), typeof(MainWindowViewModel), new UIPropertyMetadata("00:00"));

        public Visibility VisibilityOfSettings
        {
            get { return (Visibility)GetValue(VisibilityOfSettingsProperty); }
            set { SetValue(VisibilityOfSettingsProperty, value); }
        }
        public static readonly DependencyProperty VisibilityOfSettingsProperty =
            DependencyProperty.Register("VisibilityOfSettings", typeof(Visibility), typeof(MainWindowViewModel), new UIPropertyMetadata(Visibility.Visible));

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }
        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("Minutes", typeof(int), typeof(MainWindowViewModel), new UIPropertyMetadata(0));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(MainWindowViewModel), new UIPropertyMetadata(DateTime.Now.AddMinutes(30)));

        private DispatcherTimer countdownTimer;

        public MainWindowViewModel()
        {
        }

        /// <summary> </summary>
        /// <returns>true if window should be topmost</returns>
        public bool StartClock(MainWindow thisHwnd)
        {
            if (VisibilityOfSettings == System.Windows.Visibility.Visible)
            {
                VisibilityOfSettings = System.Windows.Visibility.Collapsed;
                var hwnd = new WindowInteropHelper(thisHwnd).Handle;
                WindowsServices.SetWindowExTransparent(hwnd);

                if (Minutes != 0)
                    EndTime = DateTime.Now.AddMinutes(Minutes);
                countdownTimer = new DispatcherTimer();
                countdownTimer.Interval = TimeSpan.FromSeconds(1);
                countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
                countdownTimer.Start();
                return true;
            }
            else
            {
                VisibilityOfSettings = System.Windows.Visibility.Visible;
                return false;
            }
        }

        public WindowState Maximize(WindowState winState, out string btnText)
        {
            if (winState == System.Windows.WindowState.Normal)
            {
                winState = System.Windows.WindowState.Maximized;
                btnText = "Un-Maximize";
            }
            else
            {
                winState = System.Windows.WindowState.Normal;
                btnText = "Maximize";
            }
            return winState;
        }

        void countdownTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeLeft = EndTime - DateTime.Now;
            string minutesRemaining = ((int)timeLeft.TotalMinutes).ToString("00"); //show 2 digits for minutes
            ClockText = minutesRemaining + timeLeft.ToString(@"\:ss");
            //txtClock.Text = timeLeft.ToString(@"mm\:ss"); //http://msdn.microsoft.com/en-us/library/ee372287.aspx

            if (timeLeft.TotalSeconds <= 0)
            {
                ClockText = "00:00";
                countdownTimer.Stop();
            }
        }
    }

    //http://stackoverflow.com/questions/2842667/how-to-create-a-semi-transparent-window-in-wpf-that-allows-mouse-events-to-pass
    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}