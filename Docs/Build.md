## 打包命令
```
dotnet pack LiveWallpaperEngineAPI -o ../LocalNuget/Packages --configuration release
```
## 生成license
```
thirdlicense.exe --project=LiveWallpaperEngineAPI/LiveWallpaperEngineAPI.csproj --output=Thirdparty-LiveWallpaperEngineAPI.TXT
thirdlicense.exe --project=LiveWallpaperEngineWebRender/LiveWallpaperEngineWebRender.csproj --output=Thirdparty-LiveWallpaperEngineWebRender.TXT
```
## LiveWallpaperEngineWebRender
直接右键发布，配置文件已上传