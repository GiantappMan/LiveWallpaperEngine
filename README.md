# LiveWallpaperEngine 巨应壁纸引擎

## 功能：
Windows10 动态壁纸极简API

## 示例：
```csharp
//初始化
WallpaperApi.Initlize(Dispatcher);

//显示视频壁纸
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.mp4"},WallpaperManager.Screens[0])
//显示exe壁纸
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.exe"},WallpaperManager.Screens[0])
//显示html壁纸
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.html"},WallpaperManager.Screens[0])
//显示图片壁纸
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.png"},WallpaperManager.Screens[0])
```

## 功能目标：
- [x] 无UI壁纸引擎
- [x] 多屏幕
- [x] EXE壁纸 
	- [x] 鼠标事件转发 (感谢[ADD-SP](https://github.com/ADD-SP)提供的思路)  
- [x] 视频壁纸
- [x] Web壁纸
- [x] 系统图片壁纸
- [x] 音量设置

## 对于开源的期望:
- 本项目是为了回赠开源社区对我的提升，欢迎所有开源开发者白piao本仓库~。
- 不建议商业项目使用。

## 运行效果：
* 本仓库自带demo，运行即可。  
```
测试壁纸路径
LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples
```

## 其他注意事项：
* 本项目在win10环境开发，win7没有兼容可以自己提交PR。
* 和桌面整理软件冲突，我不用该类软件，所以也没有想去解决，可以自己提交PR修复。
* 开启360、腾讯管家等杀毒全家桶软件，有可能无法嵌入桌面。
---

## 如果对你有帮助请star支持一下
