using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngineWebRender
{
    public class AppContext : ApplicationContext
    {
        private string[] args;

        public AppContext()
        {
        }

        public AppContext(string[] args)
        {
            this.args = args;
            foreach (var item in args)
            {
                BrowserForm f = new BrowserForm(item);
                f.Show();
            }
        }
    }
}
