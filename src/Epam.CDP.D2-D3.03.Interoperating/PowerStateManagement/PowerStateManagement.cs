using System;
using Common;
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
            var sleepTime = GetPowerInformation<ulong>(InformationLevel.LastSleepTime);
            return DateTime.FromFileTime(GetLastBootupTime().ToFileTime() + (long)sleepTime);
        }

        public DateTime GetLastWakeTime()
        {
            var wakeTime = GetPowerInformation<ulong>(InformationLevel.LastWakeTime);
            return DateTime.FromFileTime(GetLastBootupTime().ToFileTime() + (long)wakeTime);
        }

        public BatteryState GetSystemBatteryState()
        {
            var batteryState = GetPowerInformation<SystemBatteryState>(InformationLevel.SystemBatteryState);
            return new BatteryState(batteryState);
        }

        public PowerInformation GetSystemPowerInformation()
        {
            var powerInformation = GetPowerInformation<SystemPowerInformation>(InformationLevel.SystemPowerInformation);
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

        public void ReserveHibernationFile(HiberFileAction action)
        {
            var inp = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(inp, 0, (int)action);
            var retval = GetPowerInformation(InformationLevel.SystemReserveHiberFile, inp, (UInt32)Marshal.SizeOf(typeof(int)), (IntPtr)null, 0);
            if (retval != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Marshal.FreeCoTaskMem(inp);
        }

        private T GetPowerInformation<T>(InformationLevel level)
        {
            var type = typeof(T);
            var status = Marshal.AllocCoTaskMem(Marshal.SizeOf(type));
            var retval = GetPowerInformation(level, (IntPtr)null, 0, status, (UInt32)Marshal.SizeOf(type));
            if (retval != 0)
            {
                Marshal.FreeCoTaskMem(status);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            T returnedValue = default(T);
            if (type == typeof(ulong))
                returnedValue = (T)Convert.ChangeType(Marshal.ReadInt64(status), type);

            else if (type.IsStruct())
                returnedValue = (T)Marshal.PtrToStructure(status, type);

            Marshal.FreeCoTaskMem(status);

            return returnedValue;
        }

        private DateTime GetLastBootupTime()
        {
            return DateTime.Now.AddMilliseconds(-Environment.TickCount);
        }
    }
}
