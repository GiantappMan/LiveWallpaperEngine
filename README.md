# LiveWallpaperEngine 

[中文文档](https://github.com/giant-app/LiveWallpaperEngine/blob/master/Docs/README_zh.md)

## Features：
Windows10 Live Wallpaper Minimalist API

## App:
[LiveWallpaper](https://livewallpaper.giantapp.cn/)

## Example：
```csharp
WallpaperApi.Initlize(Dispatcher);

//Display video wallpaper
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.mp4"},WallpaperManager.Screens[0])
//Display exe wallpaper
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.exe"},WallpaperManager.Screens[0])
//Display HTML wallpaper
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.html"},WallpaperManager.Screens[0])
//Display image wallpaper
WallpaperApi.ShowWallpaper(new WallpaperModel() { Path = "/xxx.png"},WallpaperManager.Screens[0])
```

## Goals：
- [x] No UI wallpaper engine
- [x] Support for multiple screens
- [x] Supports EXE wallpaper 
	- [x] Mouse event forwarding (Thanks [ADD-SP](https://github.com/ADD-SP) for his advice)  
- [x] Video wallpaper
- [x] Web wallpaper
- [x] Image wallpaper
- [x] Audio control

## Expectations for open source:
- Welcom PR,Suggest
- Not recommended for commercial projects

## Run demo：
```
//Select files in this directory for testing
LiveWallpaperEngine\LiveWallpaperEngine.Samples.NetCore.Test\WallpaperSamples
```

## Note：
* This project is developed in Win10 environment, Win7 is not compatible,if you want you can submit PR by yourself.
* Sometimes it conflicts with desktop organization software, such as Fences.
* Open the antivirus family bucket software, it may not be embedded in the desktop.

## Branch management
- master The version under development may have various errors
- 1.x Current online stable version

## Author
- [DaZiYuan](https://github.com/DaZiYuan)

## If it helps you please give me a star

This document is translated by Google. If you find any grammatical problems, please don’t be stingy with your PR.
