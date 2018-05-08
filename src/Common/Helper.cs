using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Common
{
    public static class Helper
    {
        public static int DigitsInput(string message)
        {
            int input;
            bool inputDone;
            do
            {
                Console.WriteLine($"{message} (only digits)");
                inputDone = int.TryParse(Console.ReadLine(), out input);
            }
            while (!inputDone);

            return input;
        }

        public static int DigitVariantInput(string message, params int[] variants)
        {
            int input;
            bool inputDone;
            do
            {
                Console.WriteLine($"{message}");
                inputDone = int.TryParse(Console.ReadLine(), out input) && variants.Any(v => v == input);
            }
            while (!inputDone);

            return input;
        }

        public static bool IsStruct(this Type source)
        {
            return source.IsValueType && !source.IsPrimitive && !source.IsEnum;
        }

        public static string ComputeChecksum(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        public static string ComputeChecksum(byte[] buffer)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(buffer);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
