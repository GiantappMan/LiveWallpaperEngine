using LiveWallpaperEngine.Common;
using System;
using System.Collections.Generic;
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
using wf = System.Windows.Forms;

namespace Test.WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Form1 f = new Form1();
        WallpaperHelper w = new WallpaperHelper(wf.Screen.AllScreens[0].Bounds);
        public MainWindow()
        {
            InitializeComponent();
            f.Show();
            WallpaperHelper.DoSomeMagic();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            f.Controls.Add(new wf.Button() { Text = DateTime.Now.ToString(), Dock = wf.DockStyle.Fill });
            f.Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            f.Controls.Clear();
            f.Refresh();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //f.Opacity = 1;
            w.SendToBackground(f.Handle);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //f.Opacity = 0;
            w.RestoreParent();
        }
    }
}
