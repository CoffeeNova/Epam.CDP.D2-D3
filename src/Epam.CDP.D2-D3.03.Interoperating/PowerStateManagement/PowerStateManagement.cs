using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("7F115E86-D1F0-49CD-B3ED-BF8B9940D06B")]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class PowerStateManagement : IPowerStateManagement
    {
        public DateTime GetLastSleepTime()
        {
            IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));
            var retval = GetPowerInformation(InformationLevel.LastSleepTime, (IntPtr)null, 0, status, (UInt32)Marshal.SizeOf(typeof(ulong)));
            if (retval != 0)
            {
                Marshal.FreeCoTaskMem(status);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            var sleepTime = Marshal.ReadInt64(status);
            Marshal.FreeCoTaskMem(status);

            return DateTime.FromFileTime(GetLastBootupTime().ToFileTime() + sleepTime);
        }

        public DateTime GetLastWakeTime()
        {
            IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));
            var retval = GetPowerInformation(InformationLevel.LastWakeTime, (IntPtr)null, 0, status, (UInt32)Marshal.SizeOf(typeof(ulong)));
            if (retval != 0)
            {
                Marshal.FreeCoTaskMem(status);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            var wakeTime = Marshal.ReadInt64(status);
            Marshal.FreeCoTaskMem(status);

            return DateTime.FromFileTime(GetLastBootupTime().ToFileTime() + wakeTime);
        }

        public BatteryState GetSystemBatteryState()
        {
            IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SystemBatteryState)));
            var retval = GetPowerInformation(InformationLevel.SystemBatteryState, (IntPtr)null, 0, status, (UInt32)Marshal.SizeOf(typeof(SystemBatteryState)));
            if (retval != 0)
            {
                Marshal.FreeCoTaskMem(status);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            var batteryState = (SystemBatteryState)Marshal.PtrToStructure(status, typeof(SystemBatteryState));
            Marshal.FreeCoTaskMem(status);

            return new BatteryState(batteryState);
        }

        public PowerInformation GetSystemPowerInformation()
        {
            IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SystemPowerInformation)));
            var retval = GetPowerInformation(InformationLevel.SystemPowerInformation, (IntPtr)null, 0, status, (UInt32)Marshal.SizeOf(typeof(SystemPowerInformation)));
            if (retval != 0)
            {
                Marshal.FreeCoTaskMem(status);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            var powerInformation = (SystemPowerInformation)Marshal.PtrToStructure(status, typeof(SystemPowerInformation));
            Marshal.FreeCoTaskMem(status);

            return new PowerInformation(powerInformation);
        }

        public void SetSuspendState(PowerState powerState)
        {
            if (powerState != PowerState.Hibernate && powerState != PowerState.Sleep)
                throw new ArgumentException($"{nameof(powerState)} should be {Enum.GetName(typeof(PowerState), PowerState.Hibernate)} or {Enum.GetName(typeof(PowerState), PowerState.Sleep)}");

            var result = SetSuspendState(powerState == PowerState.Hibernate, false, false);
            if (!result)
            {
                var lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError);
            }
        }

        private DateTime GetLastBootupTime()
        {
            return DateTime.Now.AddMilliseconds(-Environment.TickCount);
        }
    }
}
