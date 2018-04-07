set sMgmt = CreateObject("PowerStateManagement.PowerStateManagement")
res = sMgmt.GetLastWakeTime()

WScript.Echo(res)