using System;
using System.Linq;
using System.Threading.Tasks;

namespace V1.TaskMulth._02
{
    /// <summary>
    /// Write a program, which creates a chain of four Tasks. First Task – creates an array of 10 random integer. Second Task – multiplies this array with another random integer. 
    /// Third Task – sorts this array by ascending. Fourth Task – calculates the average value. All this tasks should print the values to console
    /// </summary>
    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly object Lock = new object();
        static void Main()
        {
            int[] array= null;

            Task.Run(() => { array = CreateRandomIntArray(); Console.WriteLine(string.Join(", ", array)); })
                .ContinueWith(x => { array = MultiplyArrayWithRandomInt(array); Console.WriteLine(string.Join(", ", array));})
                .ContinueWith(x => { array = SortArrayByAsc(array); Console.WriteLine(string.Join(", ", array)); })
                .ContinueWith(x => {var avg = CalculateAverage(array); Console.WriteLine(avg); });

            Console.ReadLine();
        }

        private static int[] CreateRandomIntArray()
        {
            var arr = new int[10];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = GetRandomNumber();
            }

            return arr;
        }

        private static int[] MultiplyArrayWithRandomInt(int[] arr)
        {
            return arr.Select(x => x * GetRandomNumber()).ToArray();
        }

        private static int[] SortArrayByAsc(int[] arr)
        {
            return arr.OrderBy(x => x).ToArray();
        }

        private static double CalculateAverage(int[] arr)
        {
            return arr.Average();
        }

        private static int GetRandomNumber()
        {
            lock (Lock)
            {
                return Random.Next(1, 10);
            }
        }
    }
}
