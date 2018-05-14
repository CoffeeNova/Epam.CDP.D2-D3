@echo off
set currentDir=%~dp0
FileFormatterCentralService\bin\Release\FileFormatterCentralService.exe install -output "%currentDir%\Output" -timeout "10000" -systemfiles "%currentDir%\SystemFiles" 
