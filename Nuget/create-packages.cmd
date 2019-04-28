@ECHO OFF
del *.nupkg
.\nuget.exe pack .\LiveWallpaperEngineRender.nuspec -OutputDirectory ..\..\LocalNuget\Packages -symbols
