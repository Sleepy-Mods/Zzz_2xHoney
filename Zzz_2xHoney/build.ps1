param()

$ErrorActionPreference = 'Stop'
Write-Host "Building zzz_honey..."

# Use MSBuild if available, else dotnet build
if (Get-Command msbuild.exe -ErrorAction SilentlyContinue) {
  & msbuild.exe 'zzz_honey.csproj' /t:Restore,Build /p:Configuration=Release /v:m | Out-Host
} else {
  & dotnet build 'zzz_honey.csproj' -c Release -v m | Out-Host
}

Write-Host "Build finished. Ensure EAC is OFF."


