@echo off

call %~dp0"SetNameServices.bat

set port="5006"
set fileName=TradesReader.Api.exe
set serviceName=%TradesReaderBinance%

echo Installing service %serviceName%...
echo File  %~dp0%fileName%
sc create %serviceName% binPath= "%~dp0%fileName% http://*:%port% --WindowsService" start= auto
sc description %serviceName% "Service Trade"
echo Done

echo Starting service %serviceName% ...
sc start %serviceName%
echo Done

rem @pause