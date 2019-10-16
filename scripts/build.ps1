function build_webrender {
    Write-Host 'Build-WebRender'
    Set-Location -Path "../LiveWallpaperWebRender"

    $Null = @(
        Write-Host 'Delete out directory'
        Remove-Item -Recurse -Force "out"
        Invoke-Expression "npm install"
        Invoke-Expression "npm run package"
    )

    $dir = '.\out\livewallpaperwebrender-win32-ia32'
    $absolutePath = (Get-Item $dir).FullName

    return $absolutePath    
}

function build_liveWallpaperEngine {
    
}

$webRenderDir = build_webrender
Write-Host "webrender:"+$webRenderDir

$lvEngine=build_liveWallpaperEngine
Write-Host "webrender:"+$lvEngine

Copy-Item $webRenderDir -Destination "../dist/webrender" -Recurse
Copy-Item $lvEngine -Destination "../dist/" -Recurse