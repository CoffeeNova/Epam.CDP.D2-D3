psMgmt = new ActiveXObject("PowerStateManagement.PowerStateManagement");
var res = psMgmt.GetSystemPowerInformation();

WScript.Echo(
"MaxIdlenessAllowed =" + res.MaxIdlenessAllowed + "\n" 
+ "Idleness =" + res.Idleness + "\n"
+ "Charging =" + res.Charging + "\n"
+ "TimeRemaining =" + res.TimeRemaining + "\n"
+ "CoolingMode =" + res.CoolingMode);