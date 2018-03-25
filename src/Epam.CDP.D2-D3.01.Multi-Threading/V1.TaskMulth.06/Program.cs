using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace V1.TaskMulth._06
{
    /// <summary>
    /// Write a program which creates two threads and a shared collection: the first one should add 10 elements into the collection and the second should print all elements in 
    /// the collection after each adding. Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    class Program
    {
        private static ObservableCollection<int> _collection;
        private const int ItemsCount = 10;
        private delegate void Del();
        private static readonly object Locker = new object();

        static void Main()
        {
            var message = $"Choose variant: {Environment.NewLine}" +
                              $"1. using propertyChanging with ObservableCollection;{Environment.NewLine}" +
                              "2. with mutex lock";
            var variant = Helper.DigitVariantInput(message, 1, 2);
            var del = variant == 1 
                ? new Del(FirstVariant)
                : SecondVariant;
            del.Invoke();

            Console.ReadLine();
        }

        private static void FirstVariant()
        {
            _collection = new ObservableCollection<int>();
            var taskAdd =Task.Run(() =>
            {
                var i = 0;
                while (i < ItemsCount)
                {
                    Thread.Sleep(100);
                    lock (Locker)
                    {
                        _collection.Add(i);
                    }
                    i++;
                }
            });
            var taskPrint = Task.Run(() =>
            {
                lock (Locker)
                {
                    _collection.CollectionChanged += _collection_CollectionChanged;
                }
            });

            Task.WaitAll(taskAdd, taskPrint);
        }

        private static void _collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine(string.Join(", ", (ObservableCollection<int>)sender));
            if (_collection.Count == ItemsCount)
                _collection.CollectionChanged -= _collection_CollectionChanged;
        }

        private static void SecondVariant()
        {
            _collection = new ObservableCollection<int>();
            var mutex = new Mutex();
            var done = false;

            Task.Run(() =>
            {
                var i = 0;
                while (i < ItemsCount)
                {
                    mutex.WaitOne();
                    _collection.Add(i);
                    mutex.ReleaseMutex();
                    i++;
                }

                done = true;
            });
            Task.Run(() =>
            {
                while (!done) 
                {
                    mutex.WaitOne();
                    Console.WriteLine(string.Join(", ", _collection));
                    Thread.Sleep(100);
                    mutex.ReleaseMutex();
                }
            });
        }
    }
}
