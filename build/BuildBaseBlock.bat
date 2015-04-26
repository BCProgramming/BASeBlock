Set useVersion=8.1.0.0

if %1!==! goto noparameters
set useVersion=%1
:noparameters
REM look for the appropriate VS Tools folder.
echo Searching for VS Tools...
if exist "%VS140COMNTOOLS%" set usetools=%VS140COMNTOOLS%&goto FOUNDTOOLS
if exist "%VS130COMNTOOLS%" set usetools=%VS130COMNTOOLS%&goto FOUNDTOOLS
if exist "%VS120COMNTOOLS%" set usetools=%VS120COMNTOOLS%&goto FOUNDTOOLS
if exist "%VS110COMNTOOLS%" set usetools=%VS110COMNTOOLS%&goto FOUNDTOOLS
echo Unable to locate VS140COMNTOOLS, VS130COMNTOOLS, VS120COMNTOOLS, or VS110COMNTOOLS environment variable. 
echo Cannot set up Visual Studio Tools environment. Aborting.
GOTO :EOF
:FOUNDTOOLS

if exist "%usetools%vcvars32.bat" call "%usetools%vcvars32.bat" &goto RANTOOLS
if exist "%usetools%vsdevcmd.bat" call "%usetools%vsdevcmd.bat" &goto RANTOOLS
:RANTOOLS
echo building SQLSetup Tool.
msbuild baseblockbuildscript.xml /filelogger /fileloggerparameters:Append /p:Platform=Any CPU;Configuration=Debug; /fl /t:"BASeBlock"

echo build completed. 
