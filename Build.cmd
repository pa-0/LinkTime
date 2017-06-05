@echo off

echo LinkTime - build task
echo.

echo Preparing environment
call "%VS140COMNTOOLS%\vsvars32.bat"
if errorlevel 1 goto error

echo Restoring nuget packages
nuget restore LinkTime.sln
if errorlevel 1 goto error

echo Building solution (release)
msbuild.exe /consoleloggerparameters:ErrorsOnly /maxcpucount /nologo ^
  /property:Configuration=Release /property:Platform="Any CPU" ^
  /verbosity:quiet ^
  LinkTime.sln
if errorlevel 1 goto error

:success
echo.
echo Build successful
exit /b 0

:error
echo.
echo Build failed
exit /b 1