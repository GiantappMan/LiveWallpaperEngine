// Injector.cpp : 定义应用程序的入口点。
//

#include "framework.h"
#include "Injector.h"
#include <vector>

#define MAX_LOADSTRING 100


// dll中安装hook过程的入口
FARPROC pInstallHook = NULL;

// 获取桌面句柄
HWND GetDesktop();

// 获取要接收转发消息的窗口的句柄
HWND GetTargetWindow();

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    //GetDesktop();
    HWND targetWindow = GetTargetWindow();

    // 加载DLL
    HMODULE hmod = LoadLibrary(DLL_PATH);
    if (hmod)
    {
        pInstallHook = GetProcAddress(hmod, "InstallHook");
        if (pInstallHook)
        {
            HHOOK hhook = (HHOOK)pInstallHook();
            if (hhook)
            {
                // 将HOOK句柄传出，便于后期摘掉HOOK
                PostMessage(targetWindow, WM_USER, (WPARAM)hhook, NULL);
            }
            else
            {
                MessageBox(NULL, L"可尝试在DLL中调用GetLastError()获取详细信息", L"安装HOOK失败", MB_OK);
            }
        }
        else
        {
            MessageBox(NULL, L"InstallHook", L"获取DLL函数入口失败", MB_OK);
        }
    }
    else
    {
        MessageBox(NULL, DLL_PATH, L"加载下列DLL失败", MB_OK);
    }

    MSG msg;
    // 瞎等就对了，反正注入自己
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        
    }
    return TRUE;
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

BOOL CALLBACK CallBackFindWindowEnum(HWND hwnd, LPARAM lParam)
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
    EnumWindows(CallBackFindWindowEnum, (LPARAM)&hTarget);
    return hTarget;
}
