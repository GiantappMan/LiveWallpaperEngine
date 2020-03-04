# LiveWallpaperEngine 巨应壁纸引擎

## 愿景：
希望成为开源社区的第一壁纸引擎。
保持单一只做壁纸，不搞其他附加需求。

## 项目结构：
.net core3 + winform + electron

## 功能目标：
- [x] 嵌入桌面
- [x] exe壁纸鼠标事件转发 (现已支持，感谢[ADD-SP]的PR(https://github.com/ADD-SP))  
具体描述:窗口在桌面下面，不能响应鼠标事件。 如何在桌面移动鼠标时，让桌面下的窗口能收到同样的windows消息。

- [x] 支持多屏幕
- [x] 内置视频渲染
- [ ] 内置web渲染 （已初步完成，多屏还有bug）
- [x] 无UI壁纸引擎

## 对于开源的期望:
- 为了更容易吸引社区力量，我反复重写了N次让代码保持少量简洁。
希望大家多多提出有价值的issue。
- 作为业余开发项目，我私下也花了不少时间研究wallpaper engine的一些功能以及实现，甚至解决了一些upupon都没有解决的问题。
我不希望一些公司或者闭源开发者打包抄袭本仓库盈利，如果你希望使用可以考虑私下捐赠我。
- 本项目是为了回赠开源社区对我的提升，欢迎所有开源开发者白piao本仓库~。

## 严禁闭源项目抄袭使用本仓库代码，保留法律权利。

## 运行效果：
* 本仓库自带demo，运行即可。  
* 也可以查看我利用本仓库实现的动态壁纸小软件：   
https://www.mscoder.cn/product/livewallpaper/

## 开发环境
vs2019
.net core 3.0

## 其他注意事项：
* 开启360、腾讯管家等全家桶软件，有可能无法嵌入桌面

## 感谢这些作者的开源精神：
https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows  
https://github.com/Francesco149/weebp/blob/master/src/weebp.c  

---

## 如果对你有帮助请star支持一下。你们的支持是我搞下去的动力~~
