using System;
using System.Linq;

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
    }
}
