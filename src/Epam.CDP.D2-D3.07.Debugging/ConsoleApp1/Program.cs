using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            var addressBytes = networkInterface?.GetPhysicalAddress().GetAddressBytes();
            if (addressBytes == null)
                throw new InvalidOperationException("Please setup atleast one network interface.");

            var dateBytes = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());
            var cryptedAddressBytes = addressBytes.Select((x, i) => (x ^ dateBytes[i]) * 10);

            Console.WriteLine($"Correct key: {string.Join("-", cryptedAddressBytes)}");
            Console.ReadLine();
        }
    }
}
