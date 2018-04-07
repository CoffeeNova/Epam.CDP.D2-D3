using System;
using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    public struct SystemBatteryState : IBatteryState
    {
        public byte AcOnLine { get; set; }
        public byte BatteryPresent { get; set; }
        public byte Charging { get; set; }
        public byte Discharging { get; set; }
        public byte Spare1 { get; set; }
        public byte Spare2 { get; set; }
        public byte Spare3 { get; set; }
        public byte Spare4 { get; set; }
        public UInt32 MaxCapacity { get; set; }
        public UInt32 RemainingCapacity { get; set; }
        public Int32 Rate { get; set; }
        public UInt32 EstimatedTime { get; set; }
        public UInt32 DefaultAlert1 { get; set; }
        public UInt32 DefaultAlert2 { get; set; }
    }

    [ComVisible(true)]
    [Guid("A96F11B8-A22C-4B18-A165-5FAE3C1CB6FA")]
    [ClassInterface(ClassInterfaceType.None)]
    public class BatteryState : IBatteryState
    {
        public BatteryState(SystemBatteryState state)
        {
            AcOnLine = state.AcOnLine;
            BatteryPresent = state.BatteryPresent;
            Charging = state.Charging;
            Discharging = state.Discharging;
            Spare1 = state.Spare1;
            Spare2 = state.Spare2;
            Spare3 = state.Spare3;
            Spare4 = state.Spare4;
            MaxCapacity = state.MaxCapacity;
            RemainingCapacity = state.RemainingCapacity;
            Rate = state.Rate;
            EstimatedTime = state.EstimatedTime;
            DefaultAlert1 = state.DefaultAlert1;
            DefaultAlert2 = state.DefaultAlert2;
        }

        public byte AcOnLine { get; set; }
        public byte BatteryPresent { get; set; }
        public byte Charging { get; set; }
        public byte Discharging { get; set; }
        public byte Spare1 { get; set; }
        public byte Spare2 { get; set; }
        public byte Spare3 { get; set; }
        public byte Spare4 { get; set; }
        public UInt32 MaxCapacity { get; set; }
        public UInt32 RemainingCapacity { get; set; }
        public Int32 Rate { get; set; }
        public UInt32 EstimatedTime { get; set; }
        public UInt32 DefaultAlert1 { get; set; }
        public UInt32 DefaultAlert2 { get; set; }
    }
}
