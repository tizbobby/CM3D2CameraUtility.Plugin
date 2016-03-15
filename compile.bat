@echo off
setlocal
pushd "%~pd0"

if "%CM3D2_PLATFORM%" == "" set CM3D2_PLATFORM=x64
if "%CM3D2_MOD_DIR%" == ""  set CM3D2_MOD_DIR=C:\Games\KISS\CM3D2_MOD
set REIPATCHER_DIR=%CM3D2_MOD_DIR%\ReiPatcher
set UNITY_INJECTOR_DIR=%CM3D2_MOD_DIR%\UnityInjector
set CM3D2_MOD_MANAGED_DIR=%CM3D2_MOD_DIR%\CM3D2%CM3D2_PLATFORM%_Data\Managed


set "CSC_REG_KEY=HKLM\SoftWare\Microsoft\NET Framework Setup\NDP\v3.5"
set "CSC_REG_VALUE=InstallPath"
for /F "usebackq skip=2 tokens=1-2*" %%A in (`REG QUERY "%CSC_REG_KEY%" /v "%CSC_REG_VALUE%" 2^>nul`) do (
    set "CSC_PATH=%%C"
)
set "CSC=%CSC_PATH%\csc.exe"


set TYPE=/t:library
set TARGET=CM3D2CameraUtility.Plugin.dll
set SRCS=^
  CM3D2CameraUtility.Plugin\Properties\AssemblyInfo.cs ^
  CM3D2CameraUtility.Plugin\CM3D2CameraUtility.Plugin.cs
set CSOPT=/optimize+
set CSLIB=^
  /lib:"%REIPATCHER_DIR%" ^
  /r:ReiPatcher.exe ^
  /r:mono.cecil.dll ^
  /r:mono.cecil.rocks.dll ^
  /lib:"%CM3D2_MOD_MANAGED_DIR%" ^
  /r:UnityInjector.dll ^
  /r:UnityEngine.dll ^
  /r:Assembly-CSharp.dll ^
  /r:Assembly-CSharp-firstpass.dll ^
  /r:ExIni.dll
set OUTDIR=%UNITY_INJECTOR_DIR%

%CSC% %CSOPT% %TYPE% %CSLIB% /out:%OUTDIR%\%TARGET% %SRCS% || exit /b 1


popd
endlocal
