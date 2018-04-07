using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("15C38A26-A2B9-4481-8755-E08668B07522")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IPowerInformation
    {
        byte CoolingMode { get; set; }
        uint Idleness { get; set; }
        uint MaxIdlenessAllowed { get; set; }
        uint TimeRemaining { get; set; }
    }
}