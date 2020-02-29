#include "pch.h"

// DLL句柄
HINSTANCE dllIns;

// HOOK句柄
HHOOK hhook;

// 桌面句柄
HWND hWndDesktop = NULL;

// 需要接收转发消息的窗口句柄
HWND targetWindow = NULL;

// 获取需要接收转发消息的句柄
HWND GetTargetWindow();

// 获取桌面句柄
HWND GetDesktop();

// HOOK过程
LRESULT CALLBACK MouseProc(int code, WPARAM wParam, LPARAM lParam);

// 安装HOOK
extern "C" _declspec(dllexport) HHOOK __cdecl InstallHook();

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    dllIns = hModule;
    targetWindow = GetTargetWindow();
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
        MOUSEHOOKSTRUCT* pMouseHookStuct = (MOUSEHOOKSTRUCT*)lParam;

        // 发送鼠标消息类型和坐标给目标窗口
        switch (wParam)
        {
        case WM_MOUSEMOVE:
            PostMessage(targetWindow, WM_CUSTOM_MOUSE_MOVE, pMouseHookStuct->pt.x, pMouseHookStuct->pt.y);
            break;
        case WM_LBUTTONDOWN:
            PostMessage(targetWindow, WM_CUSTOM_MOUSE_LBTN_CLICK, pMouseHookStuct->pt.x, pMouseHookStuct->pt.y);
            break;
        case WM_LBUTTONDBLCLK:
            PostMessage(targetWindow, WM_CUSTOM_MOUSE_LBTN_DOUBLE_CLICK, pMouseHookStuct->pt.x, pMouseHookStuct->pt.y);
            break;
        default:
            break;
        }

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

BOOL CALLBACK CallBackFindTargetWindowEnum(HWND hwnd, LPARAM lParam)
{
    WCHAR szName[MAX_PATH] = { 0 };
    GetWindowText(hwnd, szName, ARRAYSIZE(szName) - 1);
    if (wcsstr(szName, TARGET_NAME) != NULL)
    {
        *((HWND*)lParam) = hwnd;
        return FALSE;
    }
    return TRUE;
}

HWND GetTargetWindow()
{
    HWND hTarget = NULL;
    // 暴力查找窗口
    EnumWindows(CallBackFindTargetWindowEnum, (LPARAM)&hTarget);
    return hTarget;
}

