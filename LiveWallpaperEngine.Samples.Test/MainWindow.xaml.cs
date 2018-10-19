using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiveWallpaperEngine.Samples.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int lastIndex = -1;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tb = sender as TabControl;
            if (lastIndex == tb.SelectedIndex)
                return;
            lastIndex = tb.SelectedIndex;
            switch (lastIndex)
            {
                case 0:
                    LoadProcess();
                    break;
                case 1: break;
            }
        }
        #region tabitem1
        private void LoadProcess()
        {
            var allProcesses = Process.GetProcesses().Where(m => !string.IsNullOrEmpty(m.MainWindowTitle) || m.MainWindowHandle != IntPtr.Zero).ToList();
            lstBoxProcess.ItemsSource = allProcesses;
        }

        private void btnCloseProcess_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            LWECore.RestoreParent();
        }

        private void btnShowProcess_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var process = btn.DataContext as Process;
            LWECore.SendToBackground(process.MainWindowHandle, chkDisableOSWallpaper.IsChecked.Value);
        }
        #endregion

    }
}
