# 概述

本次实现了将鼠标消息发送给任意进程，不管这个进程是否有窗口，只要这个进程使用了MouseEventReciver类。

实现鼠标事件转发是由三个部分组成：
+ LiveWallpaperEngineAPI.Model下的MouseEventReciver类，这个类通过读取共享内存来获得HOOK程序截取到的鼠标事件，并把截取到的事件转发给指定窗口；

+ 项目Injector（虽然算不上注入，但是想不到啥顺眼名字了，涉及HOOK一般就想到了Injector，感觉这个比较顺眼而已），通过”自己注入自己（手动狗头）“的方式启动HOOK；

+ 项目MouseHook，生成一个DLL，这个DLL中包含了Hook过程，Hook掉桌面上所有的鼠标事件并通过共享内存向目标进程发送鼠标事件。

# 过程

1. 由LiveWallpaperEngineAPI.Model下的MouseEventReciver类启动Injector；

2. Injector加载MouseHook.dll，启动Hook；

3. MouseEventReciver类通过读取共享内存获得鼠标消息并转发给指定窗口。

# 注意

+ 请保证需要接受鼠标事件的进程的exe和injector.exe和MouseHook.dll在同一目录下，否则是接收不到鼠标消息的。

+ 由于实现方式和恶意程序相似，杀毒软件可能会拦截。

+ 由于共享内存会被多个进程访问，同步代码已经写好了，不会导致错乱。

+ *吐槽：起初HOOK会导致任务栏假死，修复方法为把Injector.exe和MouseHook.dll编译成32位就可以了？啥？32位能HOOK64位？你在逗我？是啊，我也以为我在逗我，但是就是这么扯。估计是某些细节问题没有注意到吧，32位不能HOOK64位的。*