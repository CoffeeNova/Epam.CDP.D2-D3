using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            var salt = Encoding.ASCII.GetBytes("asdasd12313adavc342gszvswgft 'm;4;jk124");
            var totalMethodCalls = 1000;
            Console.WriteLine($"Optimization results with method class count: {totalMethodCalls}:");

            for (var i = 0; i < 4; i++)
            {
                string resultNonOptimazed = null;
                string resultOptimized = null;
                long timeNonOptimized = 0;
                long timeOptimized = 0;
                var pass = GetRandomString(8);

                var sp = new Stopwatch();
                sp.Start();

                for (var j = 0; j < totalMethodCalls; j++)
                {
                    resultNonOptimazed = GeneratePasswordHashUsingSalt(pass, salt);
                    timeNonOptimized += sp.ElapsedMilliseconds;
                }

                sp.Restart();

                var iterate = 10000;
                var pbkdf2 = new Rfc2898DeriveBytes(pass, salt, iterate);
                for (var j = 0; j < totalMethodCalls; j++)
                {
                    resultOptimized = GeneratePasswordHashUsingSalt_Optimized(ref pbkdf2, salt);
                    timeOptimized += sp.ElapsedMilliseconds;
                }

                pbkdf2.Dispose();

                if (!string.Equals(resultNonOptimazed, resultOptimized))
                    throw new Exception("different result.");

                var timeInMs = timeNonOptimized - timeOptimized;
                var timeInPct = (double)timeInMs / timeNonOptimized * 100;
                Console.WriteLine($"Time benefit from optimization: {timeInMs} ms or {timeInPct:0.0} %");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }

        private static string GeneratePasswordHashUsingSalt_Optimized(ref Rfc2898DeriveBytes pbkdf2, byte[] salt)
        {
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var passwordHash = Convert.ToBase64String(hashBytes);
            pbkdf2.Reset();

            return passwordHash;
        }

        private static IEnumerable<byte> GetBytes(int cb, Rfc2898DeriveBytes pbkdf2)
        {
            for (var i = 0; i < cb; i++)
                yield return pbkdf2.GetBytes(1)[0];
        }

        private static readonly Random Random = new Random();

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
