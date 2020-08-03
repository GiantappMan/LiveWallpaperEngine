## 打包命令
```
dotnet pack LiveWallpaperEngineAPI -o ../LocalNuget/Packages --configuration release
```
## 生成license
```
thirdlicense.exe --project=LiveWallpaperEngineAPI/LiveWallpaperEngineAPI.csproj --output=Thirdparty-LiveWallpaperEngineAPI.TXT
thirdlicense.exe --project=LiveWallpaperEngineWebRender/LiveWallpaperEngineWebRender.csproj --output=Thirdparty-LiveWallpaperEngineWebRender.TXT
```
## 打包LiveWallpaperEngineWebRender
- 执行 build.ps1

