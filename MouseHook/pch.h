// pch.h: 这是预编译标头文件。
// 下方列出的文件仅编译一次，提高了将来生成的生成性能。
// 这还将影响 IntelliSense 性能，包括代码完成和许多代码浏览功能。
// 但是，如果此处列出的文件中的任何一个在生成之间有更新，它们全部都将被重新编译。
// 请勿在此处添加要频繁更新的文件，这将使得性能优势无效。

#ifndef PCH_H
#define PCH_H

// 添加要在此处预编译的标头
#include "framework.h"
#include <Windows.h>

// 接收转发消息的窗口标题
#define TARGET_NAME L"Form1"

/*
	WM：Windows Message
	CUSTOM：自定义
	MOUSE：鼠标
	L：Left
	BTN：Button
	CLICK：点击
*/

// 传递HOOK句柄的消息
#define WM_CUSTOM_HHOOK	WM_USER
// 鼠标移动消息
#define WM_CUSTOM_MOUSE_MOVE WM_USER + 1
// 鼠标左键单击消息
#define WM_CUSTOM_MOUSE_LBTN_CLICK WM_CUSTOM_MOUSE_MOVE + 1
// 鼠标左键双击消息
#define WM_CUSTOM_MOUSE_LBTN_DOUBLE_CLICK WM_CUSTOM_MOUSE_LBTN_CLICK + 1


#endif //PCH_H
