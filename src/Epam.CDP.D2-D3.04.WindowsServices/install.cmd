@echo off
set currentDir=%~dp0
FileFormatterService\bin\Release\FileFormatterService.exe install -path "%currentDir%\ImagesReceiver" -output "%currentDir%\Output" -damaged "%currentDir%\Damaged" -timeout "4000" -filetype "pdf"
