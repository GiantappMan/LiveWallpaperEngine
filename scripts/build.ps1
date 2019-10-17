function build_webrender($target) {
    Write-Host 'Build-WebRender'
    Set-Location -Path "../LiveWallpaperWebRender"
    $Null = @(               
        Invoke-Expression "npm install"
        Invoke-Expression "npm run package"      
    )

    $dir = '.\out\livewallpaperwebrender-win32-ia32'
    $absolutePath = (Get-Item $dir).FullName
    Copy-Item $absolutePath -Destination $target -Recurse
    Remove-Item -Recurse -Force $absolutePath
    Set-Location -Path "../scripts"
}

function build_liveWallpaperEngine ($target) {
    Set-Location -Path "../"
    $cmd = "dotnet publish LiveWallpaperEngine -c Release -o " + $target
    Invoke-Expression $cmd    
    Set-Location -Path ".\scripts"
}

Remove-Item -Recurse -Force  "../dist"
build_liveWallpaperEngine ([IO.Path]::GetFullPath( "../dist/LiveWallpaperEngine"))
build_webrender ([IO.Path]::GetFullPath("../dist/LiveWallpaperEngine/WebRender"))