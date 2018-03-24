using System;
using System.Threading;
using System.Threading.Tasks;

namespace V1.TaskMulth._07
{
    class Program
    {
        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        /// <summary>
        /// 
        /// </summary>
        static void Main()
        {
            Cts.CancelAfter(5000);
            Console.WriteLine($"Main Thread id: {Thread.CurrentThread.ManagedThreadId}");

            //Create a Task and attach continuations to it according to the following criteria:
            var taskA = Task.Run(() =>
            {
                Console.WriteLine($"Task A started. Thread id: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
            });

            //a. Continuation task should be executed regardless of the result of the parent task.
            var taskB = taskA.ContinueWith(a =>
            {
                Console.WriteLine($"Task B started. Parent status: {a.Status}. Thread id: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                throw new Exception();
            }, Cts.Token, TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);

            //b. Continuation task should be executed when the parent task finished without success.
            var taskC = taskB.ContinueWith(a =>
            {
                Console.WriteLine($"Task C started. Parent status: {a.Status}. Thread id: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                throw new Exception();
            }, TaskContinuationOptions.OnlyOnFaulted);

            //c. Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
            var taskD = taskC.ContinueWith(a =>
            {
                Console.WriteLine($"Task D started. Parent status: {a.Status}. Thread id: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Cts.Token.WaitHandle.WaitOne();
                Cts.Token.ThrowIfCancellationRequested();
            }, Cts.Token, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            //d. Continuation task should be executed outside of the thread pool when the parent task would be cancelled
            taskD.ContinueWith(a =>
            {
                Console.WriteLine($"Task E started. Parent status: {a.Status}. Thread id: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
            }, new CancellationToken(), TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.Current);


            Console.ReadLine();
        }
    }
}
