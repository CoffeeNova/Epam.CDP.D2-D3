psMgmt = new ActiveXObject("PowerStateManagement.PowerStateManagement");
var res = psMgmt.GetSystemBatteryState();

WScript.Echo(
"AcOnLine =" + res.AcOnLine + "\n" 
+ "BatteryPresent =" + res.BatteryPresent + "\n"
+ "Charging =" + res.Charging + "\n"
+ "Discharging =" + res.BatteryPresent + "\n"
+ "Spare1 =" + res.Spare1 + "\n"
+ "Spare2 =" + res.Spare2 + "\n"
+ "Spare3 =" + res.Spare3 + "\n"
+ "Spare4 =" + res.Spare4 + "\n"
+ "MaxCapacity =" + res.MaxCapacity + "\n"
+ "Rate =" + res.Rate + "\n"
+ "EstimatedTime =" + res.EstimatedTime + "\n"
+ "DefaultAlert1 =" + res.DefaultAlert1 + "\n"
+ "DefaultAlert2 =" + res.DefaultAlert2);