# LiveWallpaperEngine 巨应壁纸引擎

## 功能：
Windows10 动态壁纸极简API

## 示例：
如不适用web壁纸可忽略这一步
```
Install-Package Selenium.WebDriver.ChromeDriver
```

```csharp
//初始化
WallpaperManager.Initlize(Dispatcher);

//显示视频壁纸
WallpaperManager.ShowWallpaper(new WallpaperModel() { Path = "/xxx.mp4"},WallpaperManager.Screens[0])
//显示exe壁纸
WallpaperManager.ShowWallpaper(new WallpaperModel() { Path = "/xxx.exe"},WallpaperManager.Screens[0])
```

## 功能目标：
- [x] 无UI壁纸引擎
- [x] 多屏幕
- [x] EXE壁纸 
	- [x] 鼠标事件转发 (感谢[ADD-SP](https://github.com/ADD-SP)提供的思路)  
- [x] 视频壁纸
- [ ] Web壁纸
- [ ] 系统图片壁纸
- [x] 音量设置

## 当前一些没有解决方案的问题:
- [ ] 启动壁纸会出现短暂闪屏，因为在修改parent之前有一小短时间窗口已经渲染，没有找到解决方案。

## 对于开源的期望:
- 本项目是为了回赠开源社区对我的提升，欢迎所有开源开发者白piao本仓库~。
- 严禁闭源项目抄袭使用本仓库代码，保留法律权利。

## 运行效果：
* 本仓库自带demo，运行即可。  

## 其他注意事项：
* 开启360、腾讯管家等全家桶软件，有可能无法嵌入桌面
* 和桌面整理软件冲突，我不用整理软件，所以暂时也没有想去解决

---

## 如果对你有帮助请star支持一下
