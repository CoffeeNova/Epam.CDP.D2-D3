psMgmt = new ActiveXObject("PowerStateManagement.PowerStateManagement");
var res = psMgmt.GetLastWakeTime();

WScript.Echo(res);