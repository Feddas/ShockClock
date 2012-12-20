using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ShockClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer countdownTimer;

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }
        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("Minutes", typeof(int), typeof(MainWindow), new UIPropertyMetadata(0));

        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(MainWindow), new UIPropertyMetadata(DateTime.Now.AddMinutes(30)));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Visibility == System.Windows.Visibility.Visible)
            {
                Settings.Visibility = System.Windows.Visibility.Collapsed;
                this.Topmost = true;
                var hwnd = new WindowInteropHelper(this).Handle;
                WindowsServices.SetWindowExTransparent(hwnd);

                if (Minutes != 0)
                    EndTime = DateTime.Now.AddMinutes(Minutes);
                countdownTimer = new DispatcherTimer();
                countdownTimer.Interval = TimeSpan.FromSeconds(1);
                countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
                countdownTimer.Start();
            }
            else
            {
                Settings.Visibility = System.Windows.Visibility.Visible;
                this.Topmost = false;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.Shutdown();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Button btnMax = sender as Button;
            if (WindowState == System.Windows.WindowState.Normal)
            {
                WindowState = System.Windows.WindowState.Maximized;
                if (btnMax != null) btnMax.Content = "Un-Maximize";
            }
            else
            {
                WindowState = System.Windows.WindowState.Normal;
                if (btnMax != null) btnMax.Content = "Maximize";
            }
        }

        private void DragMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void countdownTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeLeft = EndTime - DateTime.Now;
            string minutesRemaining = ((int)timeLeft.TotalMinutes).ToString("00"); //show 2 digits for minutes
            txtClock.Text = minutesRemaining + timeLeft.ToString(@"\:ss");
            //txtClock.Text = timeLeft.ToString(@"mm\:ss"); //http://msdn.microsoft.com/en-us/library/ee372287.aspx

            if (timeLeft.TotalSeconds <= 0)
            {
                txtClock.Text = "00:00";
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

    //public void ClipIt()
    //{
    //    ///http://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=4&cad=rja&ved=0CD8QFjAD&url=http%3A%2F%2Fstackoverflow.com%2Fquestions%2F1491115%2Ftemporarily-see-through-wpf-application-a-la-visual-studio-ctrl-for-tooltips&ei=dfRIUKaGJcinqwGYhoHQDg&usg=AFQjCNGh9jpXQdLgcvpxfJ6m0FZCT_SDiw
    //    IntPtr windowRegion = Control.Windows.CreateRectRgn(0, 0, parkingWindowRect.Width, parkingWindowRect.Height);
    //            Mubox.Win32.Windows.RECT clipRect = new Mubox.Win32.Windows.RECT();

    //                Mubox.Win32.Windows.GetClientRect(this.Handle, out clipRect);
    //                clipRect.Left = (parkingWindowRect.Width - clipRect.Width) / 2;
    //                clipRect.Right += clipRect.Left;
    //                clipRect.Top = (parkingWindowRect.Height - clipRect.Height) - clipRect.Left;
    //                clipRect.Bottom = parkingWindowRect.Height - clipRect.Left;


    //            IntPtr clipRegion = Control.Windows.CreateRectRgnIndirect(ref clipRect);
    //            Control.Windows.CombineRgn(windowRegion, windowRegion, clipRegion, Windows.CombineRgnStyles.RGN_XOR);
    //            Control.Windows.DeleteObject(clipRegion);
    //            Control.Windows.SetWindowRgn(this.Handle, windowRegion, true);
    //}
}
