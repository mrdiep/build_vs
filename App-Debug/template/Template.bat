call ColorText 02 "Building __{0}"

SET logfile="__{2}"
set solutionPath="__{1}"

:: Set build environment
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"

:: Publish project
call msbuild %solutionPath% /m /l:FileLogger,Microsoft.Build.Engine;logfile=%logfile%

call ColorText 02 "Build __{0}"
