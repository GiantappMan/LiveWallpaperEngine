# 概述

实现鼠标事件转发是由三个部分组成
+ 位于LiveWallpaperEngine.Samples.MouseEventHandle项目下的MouserReciver类，这个类用于接收被转发的消息以及启动注入器，运行在target窗口；

+ 项目Injector（虽然算不上注入，但是想不到啥顺眼名字了，涉及HOOK一般就想到了Injector，感觉这个比较顺眼而已），通过”自己注入自己（手动狗头）“的方式启动HOOK；

+ 项目MouseHook，生成一个DLL，这个DLL中包含了Hook过程，Hook掉桌面上所有的鼠标事件并转发到target窗口（目前只会转发四种事件，后期可以扩展）；

# 过程

1. 更改MouseHook项目下的”pch.h“文件中的下列宏为要接受鼠标消息的窗口的标题，并且要保证窗口标题不和其它窗口重复不重复，否则会出问题；
```c
#define TARGET_NAME L"Form1"
```

2. 更改Injector项目下的”Injector.h“文件中的下列宏为要接受鼠标消息的窗口的标题，并且要保证窗口标题不和其它窗口重复不重复，否则会出问题；
```c
#define TARGET_NAME L"Form1"
```

3. 由LiveWallpaperEngine.Samples.MouseEventHandle启动Injector；

4. Injector加载MouseHook.dll，启动Hook，并向LiveWallpaperEngine.Samples.MouseEventHandle返回Hook句柄用于后期摘掉Hook；

5. 通过MouserReciver类中的OnMouseEvent事件来处理接收到的鼠标消息；

# 注意

+ 由于实现方式和恶意程序相似，杀毒软件可能会拦截。

+ 由于Windows的限制，64位系统需要用64位的Injector和64位的MouseHook，32位系统则要用32位的Injector和32位的MouseHook；

+ **一定要在启动LiveWallpaperEngine.Samples.NetCore.Test项目后启动HOOK，因为该项目改变了桌面所在的层次（当然最重要的是句柄变了，HOOK也就挂了），这个已经做了规避，但是不保证以后不会出问题，因为不知道这个项目会不会继续将桌面窗口更改到其它没见过的层次。**