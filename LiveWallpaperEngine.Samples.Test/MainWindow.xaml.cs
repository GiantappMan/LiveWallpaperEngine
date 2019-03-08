using LiveWallpaperEngineRender.Renders;
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
            System.Windows.Forms.Screen.AllScreens.ToList().ForEach(item =>
            {
                cbDisplay.Items.Add(new ComboBoxItem() { Content = item.DeviceName });
            });
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
            Task.Run(() =>
           {
               var allProcesses = Process.GetProcesses().Where(m => !string.IsNullOrEmpty(m.MainWindowTitle) || m.MainWindowHandle != IntPtr.Zero).ToList();
               Dispatcher.Invoke(() =>
               {
                   lstBoxProcess.ItemsSource = allProcesses;
               });
           });
        }

        private void btnCloseProcess_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            LiveWallpaperEngineCore.RestoreParent();
        }

        private void btnShowProcess_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var process = btn.DataContext as Process;
            LiveWallpaperEngineCore.SendToBackground(process.MainWindowHandle, cbDisplay.SelectedIndex);
        }

        private void btnShowCustomHandle_Click(object sender, RoutedEventArgs e)
        {
            var handle = new IntPtr(long.Parse(txtCustomHandle.Text, System.Globalization.NumberStyles.HexNumber));
            LiveWallpaperEngineCore.SendToBackground(handle, cbDisplay.SelectedIndex);
        }

        private void btnCloseCustomHandle_Click(object sender, RoutedEventArgs e)
        {
            LiveWallpaperEngineCore.RestoreParent();
        }
        #endregion

        private void btnRestoreAllHandles_Click(object sender, RoutedEventArgs e)
        {
            LiveWallpaperEngineCore.RestoreAllHandles();
        }

        VideoRender _videoRender = new VideoRender();
        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            if (_videoRender.RenderDisposed)
                _videoRender = new VideoRender();

            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    _videoRender.Play(filePath);
                    LiveWallpaperEngineManager.Show(_videoRender);
                }
            }
        }

        private void btnStopVideo_Click(object sender, RoutedEventArgs e)
        {
            _videoRender.Stop();
        }

        private void btnPauseVideo_Click(object sender, RoutedEventArgs e)
        {
            if (_videoRender.Paused)
                _videoRender.Resume();
            else
                _videoRender.Pause();
        }

        private void btnDisposeVideo_Click(object sender, RoutedEventArgs e)
        {
            _videoRender.CloseRender();
        }
    }
}
