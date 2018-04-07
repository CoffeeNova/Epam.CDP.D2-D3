set sMgmt = CreateObject("PowerStateManagement.PowerStateManagement")
res = sMgmt.GetLastSleepTime()

WScript.Echo(res)