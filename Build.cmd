@echo off

echo LinkTime - build task
echo.

echo Restoring nuget packages
nuget restore LinkTime.sln
if errorlevel 1 goto error

echo Setting version number
Build\Packages\Veronique.1.2.0\tools\Veronique
if errorlevel 1 goto error

echo Building solution (release)
msbuild.exe /consoleloggerparameters:ErrorsOnly /maxcpucount /nologo ^
  /property:Configuration=Release /property:Platform="Any CPU" ^
  /verbosity:quiet ^
  LinkTime.sln
if errorlevel 1 goto error

echo Cleaning up
git checkout -- LinkTime/LinkTime/Properties/AssemblyInfo.cs
if errorlevel 1 goto error
git checkout -- LinkTime/LinkTime.Test/Properties/AssemblyInfo.cs
if errorlevel 1 goto error

:success
echo.
echo Build successful
exit /b 0

:error
echo.
echo Build failed
exit /b 1