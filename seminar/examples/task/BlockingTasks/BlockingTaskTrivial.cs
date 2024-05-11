// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace SeminarParallelComputing.seminar.examples.task.BlockingTasks
{
    // This class demonstrates the behavior of blocking TPL Tasks. This behavior is the basis for producer-consumer scenarios.
    // BLOCKING TASKS ARE DETECTED AND MANAGED BY THE TASK SCHEDULER
    // Interesting Result: Even if tasks are blocking, the task scheduler will run all tasks. BUT: on my 4-core machine, only 4 tasks 
    // start immediately. All further tasks take some time to be started in their own threads. This shows, that the task scheduler
    // starts further threads in its theadpool, when blocking tasks are detected. This is SLOW!
    // When the loop is repeated, it is fast. This shows, that once the threadpool is extended it remains like this.
    class BlockingTaskTrivial
    {

        static int NR_OF_TASKS = 7;

        public static void TestMain()
        {
            BlockingTaskTrivial instance = new BlockingTaskTrivial();

            instance.GetTaskSchedulerInfo();
        
            instance.RunBlockingTasks();
            NR_OF_TASKS = 9;
            instance.RunBlockingTasks();
        }

        void RunBlockingTasks()
        {
            List<Task> tasks = new List<Task>();

            for (int taskCnt = 0; taskCnt < NR_OF_TASKS; ++taskCnt)
            {
                Task blockingTask = new Task(TaskStartDelegate);

                blockingTask.Start();

                tasks.Add(blockingTask);
            }
            Task.WaitAll(tasks.ToArray());
        }

        void GetTaskSchedulerInfo()
        {
            Console.WriteLine("TaskScheduler.Current.MaximumConcurrencyLevel" + TaskScheduler.Current.MaximumConcurrencyLevel);
        }

        void TaskStartDelegate()
        {
            new IOAction().ReadConsoleInBlocking();
        }


    }


    public class IOAction
    {



        public void ReadConsoleInBlocking()
        {
           // Console.WriteLine("TaskScheduler.Current.MaximumConcurrencyLevel" + TaskScheduler.Current.MaximumConcurrencyLevel);
    
            Console.WriteLine("Task " + Task.CurrentId + " Thread " + Thread.CurrentThread.ManagedThreadId + " Type something to continue:...");
    
            String readLine = Console.ReadLine();

            Console.WriteLine("Task " + Task.CurrentId + " Thread " + Thread.CurrentThread.ManagedThreadId + " contiuing");
        }



        public static void TestMain()
        {
            Console.WriteLine("IOTask TestMain()");

            IOAction instance = new IOAction();

            // Single Threaded Version
            /*
            for (int i = 0; i < 2; ++i ){
               instance.ReadConsoleInBlocking();
            }
            */

            // Asynch Version
            List<Thread> threadList = new List<Thread>();
            for (int i = 0; i < 2; ++i)
            {
                Thread newIOThread = instance.StartBlockingIOThread();
                threadList.Add(newIOThread);
            }

            foreach(Thread nextThread in threadList){
                nextThread.Join();
            }

        }


        

        // Asynch version, returns immediately, runs in own thread
        private  Thread StartBlockingIOThread()
        {
            Console.WriteLine("StartBlockingIOThread");

            IOAction instance = new IOAction();

            Thread thread = new Thread(new ThreadStart( instance.ReadConsoleInBlocking));
            thread.Start();
            return thread;
        }

    }
}
