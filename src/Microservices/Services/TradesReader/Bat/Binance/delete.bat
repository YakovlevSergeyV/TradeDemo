@echo off

call %~dp0"SetNameServices.bat
set serviceName=%TradesReaderBinance%

echo Stopping service %serviceName%...
sc stop %serviceName%
echo Done
echo Deleting service %serviceName%...
sc delete %serviceName%
echo Done