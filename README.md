01. Multi-Threading in .NET.

I've finished first CDP theme, choosed V1 version of tasks. In some tasks interfaces are very ugly, pls don't be very strict.
There are 2 different solutions in different folders. The task #4 from Async-Await part is absent because
i had a similar task few weeks ago and I want to attach the link (the project from this repo also):
https://github.com/isalzh/TravixOpp.TestProjectCore
This project has examples of async CRUD operations.

02. Expressions and IQueryable

about Task #2 Expression Tree part: Realized mapper has contains very primitive implementation, this approach described by the lector
in Q&A video part. Imho, it should be enough. Or not?

about Task #1 IQueryable part: It seems that the task is outdated, requests are not executed as expected, but I tried to do my best
and completed this part. 

03. Interoperating with Unmanaged Code

u can find reg.cmd and unreg.cmd files to (un)register PowerStateManagement.dll library with RegAsm.exe .
also there is "Tests" folder with some js and vbs tests files for Task#3.

ReserveHibernationFile.js and RemoveHibernationFile.js test files should run as administrator if your cuurent user have no privileges to work with hiberfil.sys file.

04. Windows Services

Solution folder contains install.cmd and uninstall.cmd which are intended for installation/deinstallation FileFormatterService.
Please build solution in Release mode before installation service.
we can install service with several parameters:

-path: contains paths to monitoring folders which will receive new pictures. Should be delimited by ';' symbol;

-output: directory path for formatted files;

-damaged: if there will be a broken image file, all batch of images should moved to this folder;

-timeout: timeout in ms, signal to start formate a new file;

-filetype: type of output file, only .pdf supported for this moment;

-attempt: attempt count to read image files before throw exception.

05. Message Queues

I've used MS Azure ServiceBus library to implement this module.
Also this module has changes from module#9 AOP in the FileFormatterService project. 

06. Advanced Xml

I've decided to realize this module as class library with single class for each task.

07. Debugging

Solution contains a simple code-generator which replicate the behavior of the CrackMe.exe program.

08. Profiling and Optimization

About task#4:
Unhandled exception was thrown from result() method. I've used 'DebugDiag2 Analysis' tool analyze dump file. Then I manually fixed the problem by
wrapping this method in try..catch.

09. Aspect-oriented programming

I've decided change project FileFormatterService from solution in module 5 (Epam.CDP.D2-D3.05.MessageQueues) as most completed.
I used Microsoft.Unity library as IoC for "dynamic proxy" logging variant, and trial version licence of PostSharp library for "code rewriting".
Pay attention, that service doesn't work correctly, because the trial licence of Azure ServiceBus is expired, but this situation doesn't affect
logging.

You can find folder with log file in the base FileFormatterService directory, also you can change loggin realization in FileFormatter.Common.Bootstrapper file.