using DZY.WinAPI;
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
        List<VideoRender> _videoRenders = new List<VideoRender>();
        HandleRender _handleRender = new HandleRender();

        public MainWindow()
        {
            InitializeComponent();
            LiveWallpaperEngineManager.AllScreens.ToList().ForEach(item =>
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
                case 1:
                    LoadMonitors();
                    break;
            }
        }

        private void LoadMonitors()
        {
            int i = 0;
            if (monitors.Items.Count == 0)
                LiveWallpaperEngineManager.AllScreens.ForEach(m =>
                {
                    monitors.Items.Add(new CheckBox()
                    {
                        Content = m.DeviceName,
                        Tag = m,
                        IsChecked = true
                    });

                    //var render = new VideoRender();
                    //var screen = m;
                    //render.Init(screen);
                    //bool ok = LiveWallpaperEngineManager.Show(render, screen);

                    //_videoRenders.Add(render);
                    _videoRenders.Add(null);
                    i++;
                });
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
            LiveWallpaperEngineManager.Close(_handleRender);
        }

        private void btnShowProcess_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var process = btn.DataContext as Process;

            _handleRender.SetHandle(process.MainWindowHandle);
            LiveWallpaperEngineManager.Show(_handleRender, LiveWallpaperEngineManager.AllScreens[cbDisplay.SelectedIndex]);
        }

        private void btnShowCustomHandle_Click(object sender, RoutedEventArgs e)
        {
            var handle = new IntPtr(long.Parse(txtCustomHandle.Text, System.Globalization.NumberStyles.HexNumber));
            _handleRender.SetHandle(handle);
            LiveWallpaperEngineManager.Show(_handleRender, LiveWallpaperEngineManager.AllScreens[cbDisplay.SelectedIndex]);
        }

        private void btnCloseCustomHandle_Click(object sender, RoutedEventArgs e)
        {
            LiveWallpaperEngineManager.Close(_handleRender);
        }

        #endregion

        private void btnRestoreAllHandles_Click(object sender, RoutedEventArgs e)
        {
            LiveWallpaperEngineCore.RestoreAllHandles();
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            LiveWallpaperEngineManager.UIDispatcher = Dispatcher;
            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ForeachVideoRenders((renderItem, screen) =>
                    {
                        bool returnNew = false;
                        if (renderItem == null || renderItem.RenderDisposed)
                        {
                            returnNew = true;
                            renderItem = new VideoRender();
                            renderItem.Init(screen);
                            bool ok = LiveWallpaperEngineManager.Show(renderItem, screen);
                            if (!ok)
                            {
                                renderItem.CloseRender();
                                MessageBox.Show(ok.ToString());
                            }
                        }

                        string filePath = openFileDialog.FileName;
                        renderItem.Play(filePath);
                        if (returnNew)
                            return renderItem;
                        return null;
                    });
                }
            }
        }

        private void btnStopVideo_Click(object sender, RoutedEventArgs e)
        {
            ForeachVideoRenders((_videoRender, screen) =>
            {
                _videoRender.Stop();
                return null;
            });
        }

        private void ForeachVideoRenders(Func<VideoRender, System.Windows.Forms.Screen, VideoRender> func)
        {
            var tmpRenders = new List<VideoRender>(_videoRenders);
            for (int i = 0; i < tmpRenders.Count; i++)
            {
                var renderItem = _videoRenders[i];
                if (monitors.Items[i] is CheckBox chk)
                {
                    if (chk.IsChecked != true)
                        continue;
                }

                var newRender = func(renderItem, LiveWallpaperEngineManager.AllScreens[i]);
                if (newRender != null)
                    _videoRenders[i] = newRender;
            }
        }

        private void btnPauseVideo_Click(object sender, RoutedEventArgs e)
        {
            ForeachVideoRenders((_videoRender, screen) =>
            {
                if (_videoRender.Paused)
                    _videoRender.Resume();
                else
                    _videoRender.Pause();
                return null;
            });
        }

        private void btnDisposeVideo_Click(object sender, RoutedEventArgs e)
        {
            ForeachVideoRenders((_videoRender, screen) =>
            {
                _videoRender.CloseRender();
                return null;
            });
        }
    }
}
