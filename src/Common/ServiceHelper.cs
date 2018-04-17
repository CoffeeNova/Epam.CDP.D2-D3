using System;
using System.Management;

namespace Common
{
    public static class ServiceHelper
    {
        public static string GetServiceName()
        {
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var query = "SELECT * FROM Win32_Service where ProcessId = " + processId;
            var searcher = new ManagementObjectSearcher(query);

            foreach (var o in searcher.Get())
            {
                var queryObj = (ManagementObject) o;
                return queryObj["Name"].ToString();
            }

            throw new InvalidOperationException("Can not get the ServiceName");
        }
    }
}
