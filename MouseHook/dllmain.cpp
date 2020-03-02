#include "pch.h"

// DLL句柄
HINSTANCE dllIns;

// HOOK句柄
HHOOK hhook;

// 桌面句柄
HWND hWndDesktop = NULL;

// 获取桌面句柄
HWND GetDesktop();

// HOOK过程
LRESULT CALLBACK MouseProc(int code, WPARAM wParam, LPARAM lParam);

// 安装HOOK
extern "C" _declspec(dllexport) HHOOK __cdecl InstallHook();

// 鼠标事件结构体
struct MouseEvent
{
    UINT32 messageId;
    UINT32 x, y;
};

// 粗糙的鼠标事件队列
struct MouseEventQueue
{
    UINT32 count;
    UINT32 currentIndex;
    MouseEvent* queue;
};


BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    dllIns = hModule;
    return TRUE;
}

extern "C" _declspec(dllexport) HHOOK __cdecl InstallHook()
{
    HWND hWndDesktop = GetDesktop();
    if (!hWndDesktop)
    {
        MessageBox(NULL, L"获取桌面句柄失败！", L"ERROR", MB_OK);
        return NULL;
    }

    DWORD dwThreadId = GetWindowThreadProcessId(hWndDesktop, NULL);

    // 安装HOOK
    hhook = SetWindowsHookExW(WH_MOUSE, MouseProc, dllIns, dwThreadId);

    return hhook;
}

WCHAR* debugToStr(long number)
{
    WCHAR* str = new WCHAR[100];
    WCHAR* p = str;
    while (number != 0)
    {
        *p = (number % 10) + '0';
        number /= 10;
        ++p;
    }
    *p = NULL;
    return str;
}

LRESULT CALLBACK MouseProc(int code, WPARAM wParam, LPARAM lParam)
{
    if (code >= 0)
    {
        // 解析消息
        MOUSEHOOKSTRUCT* pMouseHookStruct = (MOUSEHOOKSTRUCT*)lParam;
        // 获取全局互斥锁
        HANDLE hMutex = CreateSemaphore(NULL, 1, 1, MUTEX_NAME);
        // 上锁
        WaitForSingleObject(hMutex, INFINITE);
        
        // 获取共享内存句柄
        HANDLE hShareMem = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE | SEC_COMMIT, 0, 4096, SHARE_MEM_NAME);
        if (!hShareMem)
        {
            return CallNextHookEx(NULL, code, wParam, lParam);
        }

        // 获取共享内存首地址并赋值给队列
        MouseEventQueue* eventQueue = (MouseEventQueue*)MapViewOfFile(hShareMem, FILE_MAP_ALL_ACCESS, NULL, NULL, NULL);
        if (!eventQueue)
        {
            return CallNextHookEx(NULL, code, wParam, lParam);
        }

        // 计算下一个鼠标事件存放的位置
        MouseEvent* mouseEvent = NULL;
        // 队列中最终50个事件，满了之后覆盖最后一个事件
        if (eventQueue->count >= 50)
        {
            mouseEvent = (MouseEvent*)((int*)eventQueue + 2 + 50);
        }
        else
        {
            mouseEvent = (MouseEvent*)((int*)eventQueue + 2) + eventQueue->count;
            ++(eventQueue->count);
        }

        // 写入鼠标坐标
        mouseEvent->x = pMouseHookStruct->pt.x;
        mouseEvent->y = pMouseHookStruct->pt.y;

        // 写入事件ID
        switch (wParam)
        {
        case WM_MOUSEMOVE:
            mouseEvent->messageId = WM_MOUSEMOVE;
            break;
        case WM_LBUTTONDOWN:
            mouseEvent->messageId = WM_LBUTTONDOWN;
            break;
        case WM_LBUTTONDBLCLK:
            mouseEvent->messageId = WM_LBUTTONDBLCLK;
            break;
        default:
            break;
        }
        //开锁
        ReleaseSemaphore(hMutex, 1, NULL);
    }
    return CallNextHookEx(NULL, code, wParam, lParam);
}

HWND GetDesktop()
{
    HWND hWndProgMan = FindWindow(L"ProgMan", NULL);
    HWND hWndWorkerW = NULL;
    HWND hWndShellDefView = NULL;
    HWND hWndDesktop = NULL;

    // 以常规方式查找桌面句柄
    if (hWndProgMan)
    {
        hWndShellDefView = FindWindowEx(hWndProgMan, NULL, L"SHELLDLL_DefView", NULL);
        if (hWndShellDefView)
        {
            hWndDesktop = FindWindowEx(hWndShellDefView, NULL, L"SysListView32", NULL);
        }
    }

    if (hWndDesktop)
    {
        return hWndDesktop;
    }

    // 获取最顶端窗口，用于搜索其它的子窗口
    HWND absoluteTop = GetDesktopWindow();

    // 桌面窗口所在层次已经改变，这是规避措施
    do
    {
        hWndWorkerW = FindWindowExW(absoluteTop, hWndWorkerW, L"WorkerW", NULL);
        if (hWndWorkerW)
        {
            hWndShellDefView = FindWindowEx(hWndWorkerW, NULL, L"SHELLDLL_DefView", NULL);
            if (hWndShellDefView)
            {
                hWndDesktop = FindWindowEx(hWndShellDefView, NULL, L"SysListView32", NULL);
            }
        }
    } while (!hWndDesktop && hWndWorkerW);

    return hWndDesktop;
}

