using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TPLTaskProgramming.AsynchAwait.AA_Speedup.AASpeedupBlocking
{
    /*
     * Demostrates speedup due to useage of asynch await in case of blocking Jobs (I/O)
     * Case 1: Blocking job without asynchronity
     * Case 2: Blocking job with async / await naive and slow
     * Case 3: Blocking job with async / await fast
    */
    public class AASpeedupBlockingCalls
    {

        public static void MainTest()
        {
            
            Console.WriteLine("AA Speedup MainTest");

            // Case 1
            BlockingSequential.RunTest();

            // Case 2
            BlockingAsynchronousNaive.RunTest();

            // Case 3
            BlockingAsynchronousFast.RunTest();
        }




    }

    /*
     * Case 1: Blocking in sequential scenario
     */
    class BlockingSequential
    {


        public static void RunTest()
        {
            new BlockingSequential().CallBlockingJobsSequantially();
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        void CallBlockingJobsSequantially()
        {
            Console.WriteLine("BlockingSequential: Calling Blocking Methods");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1.
            LogrunningJobBlocking();

            // 2.
            LogrunningJobBlocking();

            // 3.
            LogrunningJobBlocking();

            Console.WriteLine("Blocking jobs sequentially took: " + sw.ElapsedMilliseconds);
        }

        /*
         * A blocking task
         */
        void LogrunningJobBlocking()
        {

            Console.WriteLine("Blocking Job going to sleep in Thread " + Thread.CurrentThread.ManagedThreadId);
            // Using Wait is effectively the same as using Thread.Sleep(), its a kind of Hack to simulate a backgroud job.
            Task.Delay(3000).Wait();
            // Thread.Sleep(3000);
            Console.WriteLine("Blocking Job woke up in Thread " + Thread.CurrentThread.ManagedThreadId);
        }
    }



    /*
     * Case 2: Asnchrounus wrapping of blocking call, naively
     */
    class BlockingAsynchronousNaive
    {


        public static void RunTest()
        {
            Console.WriteLine("BlockingAsynchronous: Calling Asynch Method");

            new BlockingAsynchronousNaive().CallBlockingJobsAsync();
            Console.WriteLine("BlockingAsynchronousNaive: Asynch Call returned");

            // Hack to keep program alive
            Thread.Sleep(10000);
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        async void CallBlockingJobsAsync()
        {
            Console.WriteLine("CallBlockingJobsAsync");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1.
           await LongrunningJobAsync();

            // 2.
           await LongrunningJobAsync();

            // 3.
           await LongrunningJobAsync();

           Console.WriteLine("BlockingAsynchronousNaive  took: " + sw.ElapsedMilliseconds);
        }


  
        /*
         * A asynchroius job based on Task.Run()
         */
        async Task LongrunningJobAsync()
        {

            Console.WriteLine("Async Job going to sleep in Thread " + Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(3000);
            // This is only for demonstation purposes, as it is not according to the guidelines: blocking Thread within a Task.Run() is not efficient.
            //await System.Threading.Tasks.Task.Run(() => Thread.Sleep(3000));
            Console.WriteLine("Async Job woke up in Thread " + Thread.CurrentThread.ManagedThreadId);

        }

    }

    /*
     * Case 3: Asnchrounus wrapping of blocking call with speedup
     */
    class BlockingAsynchronousFast
    {


        public static void RunTest()
        {
            Console.WriteLine("BlockingAsynchronous: Calling Asynch Method");

            new BlockingAsynchronousFast().CallBlockingJobsAsync();
            Console.WriteLine("BlockingAsynchronousNaive: Asynch Call returned");

            // Hack to keep program alive
            Thread.Sleep(10000);
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        async void CallBlockingJobsAsync()
        {
            Console.WriteLine("CallBlockingJobsAsync");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1.
            Task job_1_task =  LongrunningJobAsync();

            // 2.
            Task job_2_task = LongrunningJobAsync();

            // 3.
            Task job_3_task = LongrunningJobAsync();

            Console.WriteLine("Three asynch jobs startet from Thread " + Thread.CurrentThread.ManagedThreadId);

            await job_1_task;
            Console.WriteLine("Job 1 await returned in Thread " + Thread.CurrentThread.ManagedThreadId);
            await job_2_task;
            Console.WriteLine("Job 2 await returned in Thread " + Thread.CurrentThread.ManagedThreadId);
            await job_3_task;
            Console.WriteLine("Job 3 await returned in Thread " + Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("BlockingAsynchronousFast took: " + sw.ElapsedMilliseconds);
        }



        /*
         * A asynchroius job based on Task.Run()
         */
        async Task LongrunningJobAsync()
        {

            Console.WriteLine("Async Job going to sleep in Thread " + Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(3000);
            // This is only for demonstation purposes, as it is not according to the guidelines: blocking Thread within a Task.Run() is not efficient.
            //await System.Threading.Tasks.Task.Run(() => Thread.Sleep(3000));
            Console.WriteLine("Async Job woke up in Thread " + Thread.CurrentThread.ManagedThreadId);

        }

    }
}


