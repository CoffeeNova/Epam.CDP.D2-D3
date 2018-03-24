using System;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace V1.TaskAsync._01
{
    class Program
    {
        /// <summary>
        /// Напишите консольное приложение для асинхронного расчета суммы целых чисел от 0 до N. N задается пользователем из консоли. 
        /// Пользователь вправе внести новую границу в процессе вычислений, что должно привести к перезапуску расчета. Это не должно привести к «падению» приложения.
        /// </summary>
        static void Main()
        {
            CancellationTokenSource cts = null;
            while (true)
            {
                var number = Helper.DigitsInput("Number =");
                cts?.Cancel();
                cts = new CancellationTokenSource();

                _sumTask = SumAsync(number, cts);
                _sumTask.ContinueWith(sum =>
                {
                    Console.WriteLine($"Sum = {sum.Result}");
                }, TaskContinuationOptions.RunContinuationsAsynchronously);
                _sumTask.ContinueWith(x =>
                {
                    Console.WriteLine("Restart calculations.");
                }, TaskContinuationOptions.OnlyOnCanceled);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static Task<int> _sumTask;
        private static async Task<int> SumAsync(int number, CancellationTokenSource cts)
        {
            return await Task.Run(() =>
            {
                var res = 0;
                for (int i = 0; i <= number; i++)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                    res += i;
                }
                return res;
            }, cts.Token);
        }
    }
}
