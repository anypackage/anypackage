Output "demo.gif"

Set Shell powershell 
Set Width 1400
Set Height 600

# Start PowerShell 7 due to Shell pwsh bug
Hide
Type "pwsh -noprofile -nologo"
Enter

# Set prompt to match what VHS does
Type "Set-PSReadLineOption -HistorySaveStyle SaveNothing -PredictionSource None; Function prompt { Write-Host -ForegroundColor Blue -NoNewLine '>'; return ' '}"
Enter
Type "clear"
Enter
Show

Type "# AnyPackage standardized commands"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Get-Command -Module AnyPackage | Select-Object -Property Name"
Enter
Sleep 10s
Hide
Type "clear"
Enter
Show

Type "# Import package provider"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Import-Module AnyPackage.Scoop"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "# Get imported package providers"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Get-PackageProvider"
Enter
Sleep 5s
Hide
Type "clear"
Enter
Show

Type "# Get installed packages"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Get-Package"
Enter
Sleep 5s
Hide
Type "clear"
Enter
Show

Type "# Find packages"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Find-Package -Name 7zip"
Enter
Sleep 5s
Hide
Type "clear"
Enter
Show

Type "# Install packages"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Install-Package -Name 7zip -PassThru"
Enter
Sleep 5s
Hide
Enter
Type "clear"
Enter
Show

Type "# Uninstall packages"
Sleep 2s
Hide
Enter
Type "clear"
Enter
Show

Type "Uninstall-Package -Name 7zip -PassThru"
Enter
Sleep 5s
Hide
Enter
Type "clear"
Enter
Show

Type "# Find and install packages with command not found"
Sleep 2s
Hide
Enter
Type "Set-PSReadLineOption -PredictionSource Plugin; Set-PSReadLineOption -PredictionViewStyle ListView"
Enter
Type "clear"
Enter
Show

Type "vim"
Enter
Sleep 5s
Type@500ms "Install"
Sleep 1s
Down@5s 2
Sleep 5s
