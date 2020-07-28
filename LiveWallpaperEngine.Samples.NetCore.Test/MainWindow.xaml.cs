using GiantappConfiger;
using GiantappConfiger.Models;
using Giantapp.LiveWallpaper.Engine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using static Giantapp.LiveWallpaper.Engine.ScreenOption;
using DZY.WinAPI;
using System;
using System.Text;
using System.Windows.Forms.Design;

namespace LiveWallpaperEngine.Samples.NetCore.Test
{
    class Monitor
    {
        public string DeviceName { get; set; }
        public bool Checked { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Monitor> monitorsVM = new List<Monitor>();
        public MainWindow()
        {
            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;
            WallpaperManager.Initlize(Dispatcher);
            InitializeComponent();
            monitors.ItemsSource = monitorsVM = Screen.AllScreens.Select(m => new Monitor()
            {
                DeviceName = m.DeviceName,
                Checked = true
            }).ToList();

            var audioOption = Screen.AllScreens.Select(m => new DescriptorInfo()
            {
                Text = m.DeviceName,
                DefaultValue = m.DeviceName
            }).ToList();
            audioOption.Insert(0, new DescriptorInfo() { Text = "禁用", DefaultValue = null });

            var screenSetting = Screen.AllScreens.Select(m => new ScreenOption()
            {
                Screen = m.DeviceName,
                WhenAppMaximized = ActionWhenMaximized.Pause,
            }).ToList();

            var screenSettingOptions = new List<DescriptorInfo>()
            {
                new DescriptorInfo(){Text="播放",DefaultValue=ActionWhenMaximized.Play},
                new DescriptorInfo(){Text="暂停",DefaultValue=ActionWhenMaximized.Pause},
                new DescriptorInfo(){Text="停止",DefaultValue=ActionWhenMaximized.Stop},
            };

            var descInfo = new DescriptorInfoDict()
            {
                { nameof(LiveWallpaperOptions),
                    new DescriptorInfo(){
                        Text="壁纸设置",
                        PropertyDescriptors=new DescriptorInfoDict(){
                            {
                                nameof(LiveWallpaperOptions.AudioScreen),
                                new DescriptorInfo(){
                                    Text="音源",
                                    Type=PropertyType.Combobox,Options=new ObservableCollection<DescriptorInfo>(audioOption),
                                    DefaultValue=null,
                                }
                            },
                            {
                                nameof(LiveWallpaperOptions.AutoRestartWhenExplorerCrash),
                                new DescriptorInfo(){
                                    Text="崩溃后自动恢复",
                                    DefaultValue=true,
                            }},
                            {
                                nameof(LiveWallpaperOptions.AppMaximizedEffectAllScreen),
                                new DescriptorInfo(){
                                    Text="全屏检测影响所有屏幕",
                                    DefaultValue=true,
                            }},
                            {
                                nameof(LiveWallpaperOptions.ScreenOptions),
                                new DescriptorInfo(){
                                    Text ="显示器设置",
                                    Type =PropertyType.List,
                                    CanAddItem =false,
                                    CanRemoveItem=false,
                                    DefaultValue=screenSetting,
                                    PropertyDescriptors=new DescriptorInfoDict()
                                    {
                                        {nameof(ScreenOption.Screen),new DescriptorInfo(){ Text="屏幕",Type=PropertyType.Label } },
                                        {nameof(ScreenOption.WhenAppMaximized),new DescriptorInfo(){ Text="桌面被挡住时",Options=new ObservableCollection<DescriptorInfo>(screenSettingOptions)} }
                                    }
                                }
                            },
                }}}
            };
            var setting = new LiveWallpaperOptions()
            {
                ScreenOptions = screenSetting
            };
            var vm = ConfigerService.GetVM(setting, descInfo);
            configer.DataContext = vm;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("MainWindow_Deactivated " + GetActiveWindowTitle());
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("MainWindow_Activated " + GetActiveWindowTitle());
        }

        private async void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("before ShowWallpaper " + GetActiveWindowTitle());
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "WallpaperSamples";
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var displayScreen = monitorsVM.Where(m => m.Checked).Select(m => m.DeviceName).ToArray();
                    btnApply_Click(null, null);
                    await WallpaperManager.ShowWallpaper(new WallpaperModel() { Path = openFileDialog.FileName }, displayScreen);
                }
            }
            //System.Diagnostics.Debug.WriteLine("after ShowWallpaper" + GetActiveWindowTitle());
            //IntPtr progman = User32Wrapper.FindWindow("Progman", null);
            //User32Wrapper.SetForegroundWindow(window); //change focus from the started window//application.
            //User32Wrapper.SetFocus(window);
            Activate();
        }
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = User32Wrapper.GetForegroundWindow();

            if (User32Wrapper.GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            var activeWindowTitle = GetActiveWindowTitle();
            //System.Diagnostics.Debug.WriteLine("btnStop_Click " + activeWindowTitle); 
            var displayIds = monitorsVM.Where(m => m.Checked).Select(m => m.DeviceName).ToArray();
            await WallpaperManager.CloseWallpaper(displayIds);
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            var vm = (ConfigerViewModel)configer.DataContext;
            var setting = ConfigerService.GetData<LiveWallpaperOptions>(vm.Nodes);
            _ = WallpaperManager.SetOptions(setting);
        }
    }
}
