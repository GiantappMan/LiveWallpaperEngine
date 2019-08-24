using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveWallpaperEngine
{
    /// <summary>
    /// 壁纸管理
    /// </summary>
    public static class WallpaperManager
    {
        public static Settings Settings { get; private set; }

        public static Task SetSettings(Settings settings)
        {
            return Task.CompletedTask;
        }

        #region public methods

        /// <summary>
        /// 显示壁纸                
        /// </summary>
        /// <remarks>       
        /// 视频
        /// todo 网页
        /// todo exe
        /// todo 图片地址
        /// </remarks>
        /// <param name="filepath">壁纸路经，内部自动按后缀处理。</param>
        /// <param name="screenIndexs">显示壁纸的屏幕索引，从0开始</param>
        public static void ShowWallpaper(string filepath, int[] screenIndexs)
        {
            //todo
        }

        #endregion

        #region private methods

        /// <summary>
        /// 暂停壁纸
        /// </summary>
        /// <param name="screendIndexs"></param>
        private static void Pause(int[] screendIndexs)
        {

        }

        /// <summary>
        /// 恢复壁纸
        /// </summary>
        /// <param name="screendIndexs"></param>
        private static void Resume(int[] screendIndexs)
        {

        }

        #endregion

    }
}
