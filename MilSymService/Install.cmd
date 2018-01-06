set FRAMEWORKDIR=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

%FRAMEWORKDIR%\InstallUtil.exe MilSymService.exe

if not exist .\images mkdir images 
