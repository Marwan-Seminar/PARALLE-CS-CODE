using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TPLTaskProgramming.AsynchAwait.AA_Speedup.AASpeedupCPUIntensiveJob
{
    /*
     * Demostrates speedup due to useage of asynch await in CPU intensive case
     * Case 1: CPU Intensive job without asynchronity
     * Case 2: CPU Intensive job with async / await naive and slow
     * Case 3: CPU Intensive job with async / await fast
    */
    public class AASpeedupCPUIntensive
    {

        public static void MainTest()
        {
            
            Console.WriteLine("AA Speedup MainTest");

            // Case 1
            CPUIntesiveSequential.RunTest();

            // Case 2
            CPUIntensiveAsynchronousNaive.RunTest();

           // Case 3
           CPUIntensiveAsynchronousFast.RunTest();
        }




    }

    /*
     * Case 1: Blocking in sequential scenario
     */
    class CPUIntesiveSequential
    {


        public static void RunTest()
        {
            new CPUIntesiveSequential().CallBlockingJobsSequantially();
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        void CallBlockingJobsSequantially()
        {
            Console.WriteLine("CPUIntesiveSequential: Calling CPU intensive Methods");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1.
            LogrunningJobCPUIntensive();

            // 2.
            LogrunningJobCPUIntensive();

            // 3.
            LogrunningJobCPUIntensive();

            Console.WriteLine("CPU-intensive jobs sequentially took: " + sw.ElapsedMilliseconds);
        }

        /*
         * A blocking task
         */
        void LogrunningJobCPUIntensive()
        {

            Console.WriteLine("CPU-intensive Job starting in Thread " + Thread.CurrentThread.ManagedThreadId);
            CPUIntensiveCall();
            Console.WriteLine("CPU-intensive Job returned  in Thread " + Thread.CurrentThread.ManagedThreadId);
        }

        // Runs roughly 5 seconds on my machine
        void CPUIntensiveCall()
        {
            for (Int64 i = 0; i < (Int64.MaxValue >> 30); ++i)
            {
                Int64 dummy = i * i;
            }
        }
    }



    /*
     * Case 2: Asnchrounus wrapping of blocking call, naively
     */
    class CPUIntensiveAsynchronousNaive
    {


        public static void RunTest()
        {
            Console.WriteLine("CPUIntensiveAsynchronousNaive: Calling Asynch Method");

            new CPUIntensiveAsynchronousNaive().CallCPUIntensiveJobsAsync();
            Console.WriteLine("CPUIntensiveAsynchronousNaive: Asynch Call returned");

            // Hack to keep program alive
            Thread.Sleep(20000);
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        async void CallCPUIntensiveJobsAsync()
        {
            Console.WriteLine("CallCPUIntensiveJobsAsync");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1.
           await LogrunningJobCPUIntensiveAsync();

            // 2.
           await LogrunningJobCPUIntensiveAsync();

            // 3.
           await LogrunningJobCPUIntensiveAsync();

           Console.WriteLine("CPUIntensiveAsynchronousNaive  took: " + sw.ElapsedMilliseconds);
        }



        /*
        * A asynchronous job based on Task.Run()
        */
        async Task LogrunningJobCPUIntensiveAsync()
        {

            Console.WriteLine("Async Job going to sleep in Thread " + Thread.CurrentThread.ManagedThreadId);

            await Task.Run(() => CPUIntensiveCall());

            Console.WriteLine("Async Job woke up in Thread " + Thread.CurrentThread.ManagedThreadId);

        }

        // Runs roughly 5 seconds on my machine
        void CPUIntensiveCall()
        {
            for (Int64 i = 0; i < (Int64.MaxValue >> 30); ++i)
            {
                Int64 dummy = i * i;
            }
        }

    }

    /*
     * Case 3: Asnchrounus wrapping of blocking call with speedup
     */
    class CPUIntensiveAsynchronousFast
    {


        public static void RunTest()
        {
            Console.WriteLine("CPUIntensiveAsynchronousFast: Calling Asynch Method");


            new CPUIntensiveAsynchronousFast().CallCPUIntensiveJobsAsync();

            Console.WriteLine("CPUIntensiveAsynchronousFast: Asynch Call returned");

            // Hack to keep program alive
            Thread.Sleep(10000);
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        async void CallCPUIntensiveJobsAsync()
        {
            Console.WriteLine("CallCPUIntensiveJobsAsync");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1.
            Task job_1_task = LogrunningJobCPUIntensiveAsync();

            // 2.
            Task job_2_task = LogrunningJobCPUIntensiveAsync();

            // 3.
            Task job_3_task = LogrunningJobCPUIntensiveAsync();

            Console.WriteLine("Three asynch jobs startet from Thread " + Thread.CurrentThread.ManagedThreadId);

            await job_1_task;
            Console.WriteLine("Job 1 await returned in Thread " + Thread.CurrentThread.ManagedThreadId);
            await job_2_task;
            Console.WriteLine("Job 2 await returned in Thread " + Thread.CurrentThread.ManagedThreadId);
            await job_3_task;
            Console.WriteLine("Job 3 await returned in Thread " + Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("CallCPUIntensiveJobsAsync took: " + sw.ElapsedMilliseconds);
        }



        /*
         * A asynchronous job based on Task.Run()
         */
        async Task LogrunningJobCPUIntensiveAsync()
        {

            Console.WriteLine("Async Job going to sleep in Thread " + Thread.CurrentThread.ManagedThreadId);

            await Task.Run(() =>  CPUIntensiveCall());

            Console.WriteLine("Async Job woke up in Thread " + Thread.CurrentThread.ManagedThreadId);

        }

        // Runs roughly 5 seconds on my machine
        void CPUIntensiveCall() {
            // Console.WriteLine("Int32.MaxValue" + UInt32.MaxValue + "Int64.MaxValue" + UInt64.MaxValue + "Int64.MaxValue >> 32 " +(Int64.MaxValue >> 32)) ;
            /*
            Console.WriteLine("UInt32.MaxValue: " + UInt32.MaxValue);
            Console.WriteLine("UInt32.MaxValue >> 16: " + (UInt32.MaxValue >> 16));
            Console.WriteLine("UInt64.MaxValue: " + UInt64.MaxValue);
            Console.WriteLine("UInt64.MaxValue >> 32: " + (UInt64.MaxValue >> 32));
            */

            for (Int64 i = 0; i < (Int64.MaxValue >> 30); ++i)
            {
                Int64  dummy = i * i;
            }
        }
        
    }
}

