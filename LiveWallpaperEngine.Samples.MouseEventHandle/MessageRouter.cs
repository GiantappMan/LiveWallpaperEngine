using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace LiveWallpaperEngine.Samples.MouseEventHandle
{
    /// <summary>
    /// 鼠标事件回调
    /// </summary>
    /// <param name="messageId">事件ID</param>
    /// <param name="x">鼠标坐标</param>
    /// <param name="y">鼠标坐标</param>
    public delegate void OnMouseEvent(Int32 messageId, Int64 x, Int64 y);
    public class MouseEventReciver : System.Windows.Forms.IMessageFilter
    {
        /// <summary>
        /// 包含了少数预置的窗口消息（这里消息的数值不是MSDN的数值，
        /// 而是HOOK过程接收到消息后发送的新的数值，数值详情见MouseHook->头文件->pch.h
        /// </summary>
        public enum WindwosMessageIds
        {
            /// <summary>
            /// 鼠标移动消息
            /// </summary>
            WM_MOUSEMOVE = 0x0401,
            /// <summary>
            /// 鼠标左键摁下消息
            /// </summary>
            WM_LBUTTONDOWN = 0x0402,
            /// <summary>
            /// 鼠标左键双击消息
            /// </summary>
            WM_LBUTTONDBLCLK = 0x0403
        }

        /// <summary>
        /// 消息截获进程，当需要停止HOOK的时候结束即可
        /// </summary>
        private Process injector;

        /// <summary>
        /// HOOK句柄
        /// </summary>
        private IntPtr hhook;

        /// <summary>
        /// 用于快速查询需要处理的消息
        /// </summary>
        private HashSet<Int32> messageIds = new HashSet<int>();

        /// <summary>
        /// 事件回调
        /// </summary>
        public OnMouseEvent onMouseEvent;

        /// <summary>
        /// 不会启动消息捕获
        /// </summary>
        public MouseEventReciver()
        {
            Application.AddMessageFilter(this);
        }

        ~MouseEventReciver()
        {
            EndRoute();
        }

        /// <summary>
        /// 启动消息捕获
        /// </summary>
        public void StartRoute()
        {
            Application.AddMessageFilter(this);

            // 结束之前所有的进程
            Process[] processes = Process.GetProcessesByName("Injeector");
            while (processes.Length != 0)
            {
                processes[0].Kill();
            }

            // 重新创建一个进程进行HOOK
            var processInfo = new ProcessStartInfo("Injector.exe");
            injector = Process.Start(processInfo);
        }

        /// <summary>
        /// 关闭消息捕获
        /// </summary>
        public void EndRoute()
        {
            Application.RemoveMessageFilter(this);
            UnhookWindowsHookEx(hhook);
            if (injector != null)
            {
                injector.Kill();
            }
        }

        /// <summary>
        /// 消息转发过程
        /// </summary>
        /// <param name="m">消息信息</param>
        /// <returns></returns>
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (messageIds.Contains(m.Msg))
            {
                onMouseEvent(m.Msg, (Int64)m.WParam, (Int64)m.LParam);
                return true;
            }
            else if (m.Msg == 0x400)
            {
                // WM_USER 消息，这里用来传递Hook句柄，便于后期解除HOOK
                hhook = m.WParam;
            }
            return false;
        }

        /// <summary>
        /// 添加需要被处理的消息
        /// </summary>
        /// <param name="windwosMessageIds">消息ID</param>
        public void AddMeaageToBeHandled(WindwosMessageIds windwosMessageIds)
        {
            messageIds.Add((int)windwosMessageIds);
        }
        
        /// <summary>
        /// 添加需要被处理的消息
        /// </summary>
        /// <param name="windwosMessageIds">消息ID</param>
        public void RemoveMeaageToBeHandled(Int32 windwosMessageIds)
        {
            messageIds.Add(windwosMessageIds);
        }

        /// <summary>
        /// 使得一个消息不再被捕获
        /// </summary>
        /// <param name="windwosMessageIds">消息ID</param>
        public void RemoveMeaageToBeHandled(WindwosMessageIds windwosMessageIds)
        {
            messageIds.Remove((int)windwosMessageIds);
        }

        [DllImport("User32.dll", EntryPoint = "UnhookWindowsHookEx", CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 UnhookWindowsHookEx(IntPtr hhook);
    }
}
