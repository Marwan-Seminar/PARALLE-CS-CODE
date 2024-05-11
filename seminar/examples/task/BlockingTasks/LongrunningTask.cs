// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeminarParallelComputing.seminar.examples.task.BlockingTasks
{
    // Behavior in case of Longrunning Tasks
   public class LongrunningTask
    {
        int global_data;
        public static void TestMain()
        {
           new LongrunningTask().RunTasks();
        }

        private  void RunTasks()
        {
            int NR_OF_TASKS = 6;
            Task[] tasks = new Task[NR_OF_TASKS];

            for (int i = 0; i < NR_OF_TASKS; ++i)
            {
               tasks[i] = Task.Factory.StartNew(LongrunningAction);
            }
            Task.WaitAll(tasks);
        }


        void LongrunningAction()
        {
            int id = Interlocked.Increment(ref global_data);
            Console.WriteLine("Task entered: " + id + " in thread " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(10000);
        }
    }
}
