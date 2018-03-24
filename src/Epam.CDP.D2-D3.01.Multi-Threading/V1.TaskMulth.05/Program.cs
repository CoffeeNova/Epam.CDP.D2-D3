using System;
using System.Threading;

namespace V1.TaskMulth._05
{
    /// <summary>
    /// Write a program which recursively creates 10 threads. Each thread should be with the same body and receive a state with integer number, 
    /// decrement it, print and pass as a state into the newly created thread. Use ThreadPool class for this task and Semaphore for waiting threads.
    /// </summary>
    class Program
    {
        private static readonly Semaphore Semaphore = new Semaphore(1, 1);

        static void Main()
        {
            CreateThread();
            Console.ReadLine();
        }

        private static void CreateThread(int number = 10)
        {
            if (number == 0)
                return;

            ThreadPool.QueueUserWorkItem(ThreadCallBack(), number);
            Semaphore.WaitOne();
        }

        private static WaitCallback ThreadCallBack()
        {
            return state =>
            {
                var number = (int)state;
                number--;
                Console.WriteLine($"magic number from task: {number} ");
                CreateThread(number);
            };
        }
    }
}
