using System;
using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("A8BE3DE4-F35F-4EE2-9EB9-648F96F24D59")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    interface IPowerStateManagement
    {
        DateTime GetLastSleepTime();
        DateTime GetLastWakeTime();
        BatteryState GetSystemBatteryState();
        PowerInformation GetSystemPowerInformation();
        void SetSuspendState(PowerState powerState);
    }
}
