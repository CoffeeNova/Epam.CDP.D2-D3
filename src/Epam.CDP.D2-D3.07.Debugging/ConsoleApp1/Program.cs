using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CrackMe;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            var form1 = new Form1();
            form1.eval_d.Text = "123";
            form1.eval_a(null, null);

            var networkInterface = ((IEnumerable<NetworkInterface>)NetworkInterface.GetAllNetworkInterfaces()).FirstOrDefault<NetworkInterface>();
            var addressBytes = networkInterface?.GetPhysicalAddress().GetAddressBytes();
            var data = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());

            var source = addressBytes.Select((x, i) => x ^ data[i]).Select(x =>
            {
                if (x <= 999)
                    return x * 10;
                return x;
            });
            var entry = new[] {123};
            var res = source.Select((x, i) => x - entry[i]);

        }
    }
}
