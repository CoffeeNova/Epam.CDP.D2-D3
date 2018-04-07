using System;
using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    public partial class PowerStateManagement
    {
        [DllImport("powrprof.dll", EntryPoint = "CallNtPowerInformation", SetLastError = true)]
        private static extern UInt32 GetPowerInformation(
            InformationLevel informationLevel,
            IntPtr lpInputBuffer,
            UInt32 nInputBufferSize,
            [Out] IntPtr lpOutputBuffer,
            UInt32 nOutputBufferSize
        );

        [DllImport("Powrprof.dll", SetLastError = true)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
