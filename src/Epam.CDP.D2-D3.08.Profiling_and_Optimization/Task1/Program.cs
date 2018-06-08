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
            var pass = "asdFg@#5!123Sd";
            var salt = Encoding.ASCII.GetBytes("asdasd12313adavc342gszvswgft 'm;4;jk124");
            var sp = new Stopwatch();
            sp.Start();
            var resultNonOptimazed = GeneratePasswordHashUsingSalt(pass, salt);
            var timeNonOptimized = sp.ElapsedMilliseconds;
            sp.Restart();
            var resultOptimized = GeneratePasswordHashUsingSalt_Optimized(pass, salt);
            var timeOprtimized = sp.ElapsedMilliseconds;

            Debug.Assert(string.Equals(resultNonOptimazed, resultOptimized));
            Console.WriteLine($"Time benefit from optimization: {timeNonOptimized - timeOprtimized} ms");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(400); // increased cb here to demonstrate increasing optimization improvement
            byte[] hashBytes = new byte[416];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 400);
            var passwordHash = Convert.ToBase64String(hashBytes);
            return passwordHash;
        }

        public static string GeneratePasswordHashUsingSalt_Optimized(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            var hash = GetBytes(400, pbkdf2);
            byte[] hashBytes = new byte[416];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash.ToArray(), 0, hashBytes, 16, 400);
            var passwordHash = Convert.ToBase64String(hashBytes);
            return passwordHash;
        }

        public static IEnumerable<byte> GetBytes(int cb, Rfc2898DeriveBytes pbkdf2)
        {
            for (var i = 0; i < cb; i ++)
                yield return pbkdf2.GetBytes(1)[0];
        }
    }
}
