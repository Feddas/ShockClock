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
using ShockClock.ViewModel;

namespace ShockClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary> ViewModel defined in the XAML won't be instantiated </summary>
        public MainWindowViewModel ViewModelBehind
        {
            get
            {
                if (_viewModel == null && this.LayoutRoot != null)
                {
                    _viewModel = (MainWindowViewModel)this.LayoutRoot.DataContext;
                }
                return _viewModel;
            }
        }
        private MainWindowViewModel _viewModel;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModelBehind != null)
                this.Topmost = ViewModelBehind.StartClock(this);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.Shutdown();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Button btnMax = sender as Button;

            if (ViewModelBehind != null && btnMax != null)
            {
                string btnText;
                WindowState = ViewModelBehind.Maximize(WindowState, out btnText);
                btnMax.Content = btnText;
            }
        }

        private void DragMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}