Write-Host 'Build-WebRender'
Set-Location -Path "../LiveWallpaperWebRender"

Write-Host 'Delete out directory'
Remove-Item -Recurse -Force "out"
Invoke-Expression "npm install"
Invoke-Expression "npm run package"

$dir = '.\out\livewallpaperwebrender-win32-ia32'
$absolutePath= (Get-Item $dir).FullName

return $absolutePath