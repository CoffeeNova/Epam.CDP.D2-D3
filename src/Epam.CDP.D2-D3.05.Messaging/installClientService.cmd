@echo off
set currentDir=%~dp0
FileFormatterService\bin\Release\FileFormatterService.exe install -path "%currentDir%\ImagesReceiver" -damaged "%currentDir%\Damaged" -timeout "4000" -filetype "pdf" -nodename "node1" -maxmsgsize "65536"
