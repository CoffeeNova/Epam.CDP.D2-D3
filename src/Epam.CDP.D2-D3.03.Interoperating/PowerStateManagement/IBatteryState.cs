using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("BD384C24-B1EC-4389-9E94-8D23D4AF6A9D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IBatteryState
    {
        byte AcOnLine { get; set; }
        byte BatteryPresent { get; set; }
        byte Charging { get; set; }
        uint DefaultAlert1 { get; set; }
        uint DefaultAlert2 { get; set; }
        byte Discharging { get; set; }
        uint EstimatedTime { get; set; }
        uint MaxCapacity { get; set; }
        int Rate { get; set; }
        uint RemainingCapacity { get; set; }
        byte Spare1 { get; set; }
        byte Spare2 { get; set; }
        byte Spare3 { get; set; }
        byte Spare4 { get; set; }
    }
}