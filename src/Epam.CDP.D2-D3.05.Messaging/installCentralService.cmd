@echo off
set currentDir=%~dp0
FileFormatterCentralService\bin\Release\FileFormatterCentralService.exe install -output "%currentDir%\Output" -systemfiles "%currentDir%" -timeout "10000"
