using GiantappConfiger;
using GiantappConfiger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            InitializeComponent();
            monitors.ItemsSource = monitorsVM = Screen.AllScreens.Select(m => new Monitor()
            {
                DeviceName = m.DeviceName,
                Checked = true
            }).ToList();

            var audioOption = Screen.AllScreens.Select(m => new DescriptorInfo()
            {
                Text = m.DeviceName,
                DefaultValue = Screen.AllScreens.ToList().IndexOf(m)
            }).ToList();
            audioOption.Insert(0, new DescriptorInfo() { Text = "禁用", DefaultValue = -1 });

            var screenSetting = Screen.AllScreens.Select(m => new ScreenSetting()
            {
                ScreenIndex = Screen.AllScreens.ToList().IndexOf(m),
                WhenCurrentScreenMaximized = ActionWhenMaximized.Pause
            }).ToList();

            var screenSettingOptions = new List<DescriptorInfo>()
            {
                new DescriptorInfo(){Text="播放",DefaultValue=ActionWhenMaximized.Play},
                new DescriptorInfo(){Text="暂停",DefaultValue=ActionWhenMaximized.Pause},
                new DescriptorInfo(){Text="停止",DefaultValue=ActionWhenMaximized.Stop},
            };

            var descInfo = new DescriptorInfoDict()
            {
                { "WallpaperManagerSetting",
                    new DescriptorInfo(){
                        Text="壁纸设置",
                        PropertyDescriptors=new DescriptorInfoDict(){
                            {"AudioScreenIndex",
                                new DescriptorInfo(){
                                    Text="音源",
                                    Type=PropertyType.Combobox,Options=new ObservableCollection<DescriptorInfo>(audioOption),
                                    DefaultValue=-1,
                                }
                            },
                    {"ScreenSettings",
                        new DescriptorInfo(){
                            Text ="显示器设置",
                            Type =PropertyType.List,
                            CanAddItem =false,
                            CanRemoveItem=false,
                            DefaultValue=screenSetting,
                            PropertyDescriptors=new DescriptorInfoDict()
                            {
                                {"ScreenIndex",new DescriptorInfo(){ Text="屏幕",Type=PropertyType.Label } },
                                {"WhenCurrentScreenMaximized",new DescriptorInfo(){ Text="桌面被挡住时",Options=new ObservableCollection<DescriptorInfo>(screenSettingOptions)} }
                            }
                        }
                    },
                }}}
            };
            var vm = ConfigerService.GetVM(WallpaperManager.GlobalSetting, descInfo);
            configer.DataContext = vm;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "WallpaperSamples";
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var displayIds = monitorsVM.Where(m => m.Checked).Select(m => monitorsVM.IndexOf(m)).ToArray();
                    WallpaperManager.ShowWallpaper(new Wallpaper() { Path = openFileDialog.FileName }, displayIds);
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            var displayIds = monitorsVM.Where(m => m.Checked).Select(m => monitorsVM.IndexOf(m)).ToArray();
            WallpaperManager.CloseWallpaper(displayIds);
        }
    }
}
