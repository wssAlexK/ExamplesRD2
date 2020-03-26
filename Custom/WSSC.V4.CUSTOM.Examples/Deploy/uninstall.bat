@ECHO OFF
@SET WspInstaller="c:\program files\common files\microsoft shared\web server extensions\12\TEMPLATE\LAYOUTS\WSS\WspInstaller.exe"
IF NOT EXIST %WspInstaller% @SET WspInstaller="c:\program files\common files\microsoft shared\web server extensions\14\TEMPLATE\LAYOUTS\WSS\WspInstaller.exe"
IF NOT EXIST %WspInstaller% @SET WspInstaller="..\WspInstaller.exe"
IF NOT EXIST %WspInstaller% @SET WspInstaller="WspInstaller.exe"
IF EXIST %WspInstaller% (%WspInstaller% -uninstall -single -recycle) ELSE (ECHO. & ECHO Could not find WspInstaller.exe. Copy WspInstaller.exe in the current executable folder, or in the folder level above, or in the folder LAYOUTS\WSS)
PAUSE