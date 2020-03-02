using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveWallpaperEngineAPI.Models
{
    /// <summary>
    /// 鼠标事件
    /// </summary>
    internal struct MouseEvent
    {
        public UInt32 messageId;
        public UInt32 x, y;

        public MouseEvent(uint messageId, uint x, uint y)
        {
            this.messageId = messageId;
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// 鼠标事件处理程序
    /// </summary>
    /// <param name="messageId">鼠标消息ID，详见MSDN</param>
    /// <param name="x">鼠标坐标</param>
    /// <param name="y">鼠标坐标</param>
    public delegate void MouseEventHandler(UInt32 messageId, UInt32 x, UInt32 y);

    unsafe public class MouseEventReciver
    {
        public enum WindowMessage
        {
            /// <summary>
            /// 鼠标移动消息
            /// </summary>
            WM_MOUSEMOVE = 0x200,
            /// <summary>
            /// 鼠标左键单击消息
            /// </summary>
            WM_LBUTTONDOWN = 0x0201,
            /// <summary>
            /// 鼠标左键双击消息
            /// </summary>
            WM_LBUTTONDBLCLK = 0x0203
        }

        /// <summary>
        /// 共享内存名称
        /// </summary>
        private static string SharMemoryName { get => "LiveWallpaperEngineShareMemory"; }
        /// <summary>
        /// 共享内存访问时的互斥锁的名称
        /// </summary>
        private static string SharMemoryMutexName { get => "LiveWallpaperEngineShareMemoryMutex"; }

        /// <summary>
        /// HOOK程序路径，如果无法获取到鼠标事件可能是路径问题。
        /// 务必保证Injector.exe和MouseHook.dll在同一目录下。
        /// </summary>
        private static string InjectorPath { get => "Injector.exe"; }

        /// <summary>
        /// 当桌面发生鼠标事件的时候会被调用
        /// </summary>
        public MouseEventHandler OnMouseEvent;

        /// <summary>
        /// 共享内存句柄
        /// </summary>
        private IntPtr HShareMemory { get; set; }


        /// <summary>
        /// 共享内存所用互斥锁句柄
        /// </summary>
        private IntPtr HSharMemoryMutex { get; set; }

        /// <summary>
        /// 共享内存首地址
        /// </summary>
        private IntPtr StartAddress { get; set; }

        /// <summary>
        /// 鼠标事件接收线程
        /// </summary>
        private Thread reviveThread;

        /// <summary>
        /// hook进程
        /// </summary>
        private Process hooker;

        public MouseEventReciver() 
        {
            HShareMemory = CreateFileMapping(SharMemoryName);
            HSharMemoryMutex = CreateOrOpenMutex(SharMemoryMutexName);
            StartAddress = MapViewOfFile(HShareMemory);
        }

        /// <summary>
        /// 获得下一个鼠标事件
        /// </summary>
        /// <returns>包含鼠标事件详情的结构体</returns>
        private MouseEvent GetNextMouseEvent()
        {
            MouseEvent ret = new MouseEvent();
            ret.messageId = 0;
            do
            {
                /*
                 * 共享内存中的数据结构可见MouseHook项目中的结构体
                 */
                WaitForMutex(HSharMemoryMutex);
                int* pCount = (int*)StartAddress;
                int* curEventIndex = (int*)(StartAddress + 4);
                if (*pCount != 0)
                {
                    ret = *(MouseEvent*)(StartAddress + 8 + *curEventIndex * sizeof(MouseEvent));
                    --(*pCount);
                    ++(*curEventIndex);
                }

                if (*pCount == 0 || *curEventIndex == 50)
                {
                    *pCount = 0;
                    *curEventIndex = 0;
                }
                ReleaseMutex(HSharMemoryMutex);

            } while (ret.messageId == 0);
            return ret;
        }

        /// <summary>
        /// 不断获取鼠标事件
        /// </summary>
        private void Revive()
        {
            while (true)
            {
                MouseEvent mouseEvent = GetNextMouseEvent();
                OnMouseEvent(mouseEvent.messageId, mouseEvent.x, mouseEvent.y);
            }
        }

        /// <summary>
        /// 开始接收鼠标事件
        /// </summary>
        public void StartRecive()
        {
            Process[] processes = Process.GetProcessesByName("Injector");
            while (processes.Length != 0)
            {
                // 这里可能会抛出异常显示拒绝访问，但是进程确实是结束了
                try
                {
                    processes[0].Kill();
                }
                catch (Exception) { }
                ;
            }
            hooker = Process.Start(InjectorPath);
            reviveThread = new Thread(new ThreadStart(Revive));
            reviveThread.Start();
        }

        /// <summary>
        /// 停止接收鼠标事件
        /// </summary>
        public void StopRecive()
        {
            hooker.Kill();
            reviveThread.Abort();
        }

        #region 封装WIN32 API

        /// <summary>
        /// 创建互斥锁，如果已经存在则不做任何操作
        /// </summary>
        /// <param name="mutexName">互斥锁名称</param>
        /// <returns>互斥锁句柄</returns>
        public IntPtr CreateOrOpenMutex(string mutexName)
        {
            return CreateSemaphoreW(0, 1, 1, Encoding.Unicode.GetBytes(mutexName));
        }

        /// <summary>
        /// 上锁
        /// </summary>
        /// <param name="hMutex">互斥锁句柄</param>
        public void WaitForMutex(IntPtr hMutex)
        {
            WaitForSingleObject(hMutex, 0xFFFFFFFF);
        }

        /// <summary>
        /// 开锁
        /// </summary>
        /// <param name="hMutex">互斥锁句柄</param>
        public void ReleaseMutex(IntPtr hMutex)
        {
            ReleaseSemaphore(hMutex, 1, 0);
        }

        /// <summary>
        /// 创建共享内存，如果已经存在则不做任何操作
        /// </summary>
        /// <param name="shareMemoryName">共享内存名称</param>
        /// <returns>共享内存句柄</returns>
        public IntPtr CreateFileMapping(string shareMemoryName)
        {
            return CreateFileMappingW((UInt32)0xFFFFFFFF, 0, 134217732, 0, 4096,
                Encoding.Unicode.GetBytes(shareMemoryName));
        }

        /// <summary>
        /// 获取共享内存首地址
        /// </summary>
        /// <param name="hShareMemory">共享内存句柄</param>
        /// <returns>共享内存首地址</returns>
        public IntPtr MapViewOfFile(IntPtr hShareMemory)
        {
            return MapViewOfFile(hShareMemory, 983071, 0, 0, 0);
        }

        #endregion

        #region WIN32 API
        [DllImport("Kernel32.dll", EntryPoint = "OpenFileMappingW", CallingConvention = CallingConvention.Winapi
           , CharSet = CharSet.Unicode)]
        private extern static IntPtr OpenFileMappingW(UInt32 dwDesiredAccess, UInt32 bInheritHandle, byte[] lpName);


        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMappingW", CallingConvention = CallingConvention.Winapi
            , CharSet = CharSet.Unicode)]
        private extern static IntPtr CreateFileMappingW(UInt32 hFile, UInt32 lpFileMappingAttributes, UInt32 flProtect,
            UInt32 dwMaximumSizeHigh, UInt32 dwMaximumSizeLow, byte[] lpName);

        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile", CallingConvention = CallingConvention.Winapi
            , CharSet = CharSet.Unicode)]
        private extern static IntPtr MapViewOfFile(IntPtr hFileMappingObject, UInt32 dwDesiredAccess,
            UInt32 dwFileOffsetHigh, UInt32 dwFileOffsetLow, UInt32 dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", EntryPoint = "CreateSemaphoreW", CallingConvention = CallingConvention.Winapi
            , CharSet = CharSet.Unicode)]
        private extern static IntPtr CreateSemaphoreW(UInt32 lpSemaphoreAttributes, UInt32 lInitialCount,
            UInt32 lMaximumCount, byte[] lpName);

        [DllImport("Kernel32.dll", EntryPoint = "OpenSemaphoreW", CallingConvention = CallingConvention.Winapi
            , CharSet = CharSet.Unicode)]
        private extern static IntPtr OpenSemaphoreW(UInt32 dwDesiredAccess, UInt32 bInheritHandle, byte[] lpName);

        [DllImport("Kernel32.dll", EntryPoint = "WaitForSingleObject", CallingConvention = CallingConvention.Winapi
            , CharSet = CharSet.Unicode)]
        private extern static UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("Kernel32.dll", EntryPoint = "ReleaseSemaphore", CallingConvention = CallingConvention.Winapi
            , CharSet = CharSet.Unicode)]
        private extern static UInt32 ReleaseSemaphore(IntPtr hSemaphore, UInt32 lReleaseCount, UInt32 lpPreviousCount);
        #endregion
    }
}
