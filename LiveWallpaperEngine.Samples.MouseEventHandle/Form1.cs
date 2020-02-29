using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveWallpaperEngine.Samples.MouseEventHandle
{
    public partial class Form1 : Form
    {
        static MouseEventReciver reciver = new MouseEventReciver();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 添加事件回调
            reciver.onMouseEvent += onMouseEvent;

            // 分别设置接收三种鼠标消息
            reciver.AddMeaageToBeHandled(MouseEventReciver.WindwosMessageIds.WM_MOUSEMOVE);
            reciver.AddMeaageToBeHandled(MouseEventReciver.WindwosMessageIds.WM_LBUTTONDOWN);
            reciver.AddMeaageToBeHandled(MouseEventReciver.WindwosMessageIds.WM_LBUTTONDBLCLK);

            // 开始消息接收
            reciver.StartRoute();
        }

        private void onMouseEvent(Int32 messageId, Int64 x, Int64 y)
        {
            // 单击和双击只能同时启用一个，因为这里的测试方法是弹窗，弹窗后双击会被打断，但是不影响消息截获
            switch (messageId)
            {
                case (int)MouseEventReciver.WindwosMessageIds.WM_MOUSEMOVE:
                    label1.Text = "X：" + x.ToString();
                    label2.Text = "Y：" + y.ToString();
                    break;
                case (int)MouseEventReciver.WindwosMessageIds.WM_LBUTTONDOWN:
                    MessageBox.Show("在桌面单击了鼠标左键");
                    break;
                case (int)MouseEventReciver.WindwosMessageIds.WM_LBUTTONDBLCLK:
                    //MessageBox.Show("在桌面双击了鼠标左键");
                    break;
            }
        }
    }
}
