using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    public struct SystemPowerInformation : IPowerInformation
    {
        public uint MaxIdlenessAllowed { get; set; }
        public uint Idleness { get; set; }
        public uint TimeRemaining { get; set; }
        public byte CoolingMode { get; set; }
    }

    [ComVisible(true)]
    [Guid("BFA7887B-3E6C-46C9-B3C5-820DD8B153C0")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerInformation : IPowerInformation
    {
        public PowerInformation(SystemPowerInformation powerInfo)
        {
            MaxIdlenessAllowed = powerInfo.MaxIdlenessAllowed;
            Idleness = powerInfo.Idleness;
            TimeRemaining = powerInfo.TimeRemaining;
            CoolingMode = powerInfo.CoolingMode;
        }

        public uint MaxIdlenessAllowed { get; set; }
        public uint Idleness { get; set; }
        public uint TimeRemaining { get; set; }
        public byte CoolingMode { get; set; }
    }
}
