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
        public Int32 messageId;
        public UInt32 x, y;
    }
    
    
    /// <summary>
    /// （此类仅限Windows下有效）
    /// 这个类通过读写和Injector进程共享的一块内存来来得到桌面上的鼠标消息
    /// 并通过PostMessage转发给指定窗口
    /// </summary>
    unsafe public class MouseEventReciver
    {
        /// <summary>
        /// 要接受消息的窗口的句柄
        /// </summary>
        public IntPtr HTargetWindow = IntPtr.Zero;

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
            // 创建并获取共享内存句柄
            HShareMemory = CreateFileMapping(SharMemoryName);
            // 创建并获取互斥锁句柄
            HSharMemoryMutex = CreateOrOpenMutex(SharMemoryMutexName);
            // 得到共享内存首地址
            StartAddress = MapViewOfFile(HShareMemory);
        }

        /// <summary>
        /// 获得下一个鼠标事件
        /// </summary>
        /// <returns>包含鼠标事件详情的结构体</returns>
        private MouseEvent GetNextMouseEvent()
        {
            MouseEvent ret = new MouseEvent
            {
                // 初始化,之所以初始化x,y两个字段是为了消除VS的从未对字段赋值的Warning
                messageId = 0,
                x = 0,
                y = 0
            };
            while (ret.messageId == 0)
            {
                /*
                 * 共享内存中的数据结构可见MouseHook项目中的结构体
                 */
                // 上互斥锁，如果互斥锁被占用则无限期等待直到HOOK进程打开互斥锁
                WaitForMutex(HSharMemoryMutex);
                // 获取当前事件队列中有多少待读取的鼠标事件
                int* pCount = (int*)StartAddress;
                // 获取当前要读取的鼠标事件的下表（鼠标事件被保存在数组内）
                int* curEventIndex = (int*)(StartAddress + 4);
                if (*pCount != 0)
                {
                    // 通过指针运算得到鼠标事件的详细信息，需要搭配MouseHook项目中的结构体才能理解
                    ret = *(MouseEvent*)(StartAddress + 8 + *curEventIndex * sizeof(MouseEvent));
                    --(*pCount);
                    ++(*curEventIndex);
                }

                // 如果队列空或者即将读取第50个事件，则清空队列，因为队列支持同时存在50个事件
                // 队列满后新发生的事件会覆盖掉上一个事件
                if (*pCount == 0 || *curEventIndex == 50)
                {
                    *pCount = 0;
                    *curEventIndex = 0;
                }
                // 开互斥锁
                ReleaseMutex(HSharMemoryMutex);
            }
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
                // 根据官网文档中定义，lParam低16位存储鼠标的x坐标，高16位存储y坐标
                UInt32 lParam = mouseEvent.y;
                lParam <<= 16;
                lParam |= mouseEvent.x;
                // 发送消息给目标窗口
                PostMessageW(HTargetWindow, mouseEvent.messageId, (IntPtr)0x0020, (IntPtr)lParam);
            }
        }

        /// <summary>
        /// 开始接收鼠标事件
        /// </summary>
        public void StartRecive()
        {
            // 获取已经存在的HOOK进程，并全部结束掉
            Process[] processes = Process.GetProcessesByName("Injector");
            while (processes.Length != 0)
            {
                // 这里可能会抛出异常显示拒绝访问，但是进程确实是结束了
                try
                {
                    processes[0].Kill();
                }
                catch (Exception) { }
            }
            // 启动HOOK进程
            hooker = Process.Start(InjectorPath);
            // 创建并运行鼠标消息接收和转发线程
            reviveThread = new Thread(new ThreadStart(Revive));
            reviveThread.Start();
        }

        /// <summary>
        /// 停止接收鼠标事件
        /// </summary>
        public void StopRecive()
        {
            // 结束injector（HOOK）进程
            hooker.Kill();
            // 结束鼠标消息接收和转发线程
            reviveThread.Abort();
        }

        #region 封装WIN32 API

        /// <summary>
        /// 创建互斥锁，如果已经则直接返回互斥锁句柄
        /// </summary>
        /// <param name="mutexName">互斥锁名称</param>
        /// <returns>互斥锁句柄</returns>
        private IntPtr CreateOrOpenMutex(string mutexName)
        {
            return CreateSemaphoreW(0, 1, 1, Encoding.Unicode.GetBytes(mutexName));
        }

        /// <summary>
        /// 上锁
        /// </summary>
        /// <param name="hMutex">互斥锁句柄</param>
        private void WaitForMutex(IntPtr hMutex)
        {
            // 0xFFFFFFFF代表允许无限等待其它进程释放互斥锁
            WaitForSingleObject(hMutex, 0xFFFFFFFF);
        }

        /// <summary>
        /// 开锁
        /// </summary>
        /// <param name="hMutex">互斥锁句柄</param>
        private void ReleaseMutex(IntPtr hMutex)
        {
            ReleaseSemaphore(hMutex, 1, 0);
        }

        /// <summary>
        /// 创建共享内存，如果已经存在则直接返回互斥锁句柄
        /// </summary>
        /// <param name="shareMemoryName">共享内存名称</param>
        /// <returns>共享内存句柄</returns>
        private IntPtr CreateFileMapping(string shareMemoryName)
        {
            return CreateFileMappingW((UInt32)0xFFFFFFFF, 0, 134217732, 0, 4096,
                Encoding.Unicode.GetBytes(shareMemoryName));
        }

        /// <summary>
        /// 获取共享内存首地址
        /// </summary>
        /// <param name="hShareMemory">共享内存句柄</param>
        /// <returns>共享内存首地址</returns>
        private IntPtr MapViewOfFile(IntPtr hShareMemory)
        {
            return MapViewOfFile(hShareMemory, 983071, 0, 0, 0);
        }

        #endregion

        #region WIN32 API

        [DllImport("User32.dll", EntryPoint = "SendMessageW", CallingConvention = CallingConvention.Winapi
           , CharSet = CharSet.Unicode)]
        private extern static IntPtr SendMessageW(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessageW", CallingConvention = CallingConvention.Winapi
           , CharSet = CharSet.Unicode)]
        private extern static IntPtr PostMessageW(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

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
