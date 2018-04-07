psMgmt = new ActiveXObject("PowerStateManagement.PowerStateManagement");
var res = psMgmt.GetLastSleepTime();

WScript.Echo(res);