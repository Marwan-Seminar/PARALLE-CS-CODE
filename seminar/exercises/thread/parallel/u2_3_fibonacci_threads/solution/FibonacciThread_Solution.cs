using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.exercises.thread.parallel.u2_3_fibonacci_threads.solution
{

    // Three veríons of Fibonacci calculation are presented here (for simplicity shifted by one element):
    // 1. FibonacciSequential simple recursive calcualtion of the fibonacci sequence
    // 2. FibonacciThread: A parallelization of the algorithm, using a threshold to limit the number of threads created.
    // 3. FibonacciNaive: A parallelization of the algorithm, that does create new threads in each recursion. It creashes for bigger input numbers than 16.
    class FibonacciThreadMain_Solution
    {
        public static void TestFiboMain()
        {
            // Expected Results (on my machine, 8-Core intel core i7): 
            //  Input 10: Result 89,
            //  Input 16: Result 1597 : Naive version crashes from this input size on
            //  Input 42: Result: 433494437 Sequential Time 2 Sec, Parallel Time below 1 Sec threads used 177
            //  Input 46: REsult: 2971215073, Sequntial Time: 16 Sec, Parallel Time 4 Sec threads used 177
            int fibonacciInputValue = 46; // 46; //  42; // 16; // 10;

            Console.WriteLine("Fibonacci Test entered");

            // 1. Seqeuntial calculation of Fibonacci
            RunFibonacciSequntial(fibonacciInputValue);

            // 2. Threaded calculation of fimonacci, with threshold
            //RunFibonacciThreaded(fibonacciInputValue);

            // 3. Naive threaded calculation of fibonacci: Crash
            //RunFibonacciNaive(fibonacciInputValue);
        }

        private static void RunFibonacciSequntial(int fibonacciInputValue)
        {
            Console.WriteLine("Sequntial Fibonacci entered, inputvalue: " + fibonacciInputValue);

            Stopwatch swSeq = new Stopwatch();
            swSeq.Start();

            // run sequential Fibonacci:
           long result =  (new FibonacciSequential()).calculate(fibonacciInputValue);

           Console.WriteLine("  Sequential Fibonacci  returned. Argument: " + fibonacciInputValue + " Result: " + result + " Time: " + swSeq.Elapsed);
        }

        private static void RunFibonacciThreaded(int fibonacciInputValue)
        {
            Console.WriteLine("Threaded Fibonacci entered, input Value: " + fibonacciInputValue);

            Stopwatch swTh = new Stopwatch();
            swTh.Start();

            // run sequential Fibonacci:
            FibonacciThread fiboThread = new FibonacciThread(fibonacciInputValue);
            fiboThread.calculate();

            Console.WriteLine("  Threaded Fibonacci returned. Argument: " + fibonacciInputValue + " Result: " + fiboThread.result + " Time: " + swTh.Elapsed + " Threads used: " + FibonacciThread.threadCount);
        }

        // This method call can crash the program!
        private static void RunFibonacciNaive(int fibonacciInputValue)
        {
            Console.WriteLine("Naive Fibonacci entered, inputvalue: " + fibonacciInputValue);

            Stopwatch swNv = new Stopwatch();
            swNv.Start();

            // run naive Fibonacci parallelization:
            FibonacciNaive fiboNaive = new FibonacciNaive(fibonacciInputValue);
            fiboNaive.calculate();

            Console.WriteLine("  Naive Fibonacci returnded. Argument: " + fibonacciInputValue + " Result: " + fiboNaive.result + " Time: " + swNv.Elapsed + " Threads used: " + FibonacciNaive.threadCount);
        }
	
    }

    class FibonacciSequential
    {
        public long calculate(long n)
        {
		    if(n<=1){
			    return 1;
		    }else{
                return calculate(n - 1) + calculate(n - 2);
		    }
	    }
    }

    class FibonacciThread
    {
        long arg;
	    public long result;
	
	    static volatile int threshold;
	    static volatile bool thresholdInitialized = false;
	    // TODO use atomic integer
        public static int threadCount;
	
	    public FibonacciThread(long arg){
		    this.arg = arg;
		
		    if(!thresholdInitialized){
                threshold = (int) arg - Environment.ProcessorCount; 
                Console.WriteLine("  threshold " + threshold + " CPU count: " + Environment.ProcessorCount);
			    thresholdInitialized = true;
		    }
	    }
	
	    long fiboSequential(long seqArg){
		    if(seqArg <= 1){
			    return 1;
		    }else{
			    return fiboSequential(seqArg - 1) + fiboSequential(seqArg -2);
		    }
	    }

	    public void calculate(){
		    threadCount++;
		    if(arg <= 1){
			    result = 1;
		    }else if( arg < threshold ){
			    // Sequential calculation
			    result = fiboSequential(arg);
		    }
		    else{
			    // Parallel calculation: Start new Threads for recursion
               FibonacciThread fiboDataN_1 = new FibonacciThread(arg - 1);
               FibonacciThread fiboDataN_2 = new FibonacciThread(arg - 2);
		       // Start the threads
   			   Thread fiboThreadLeft = new Thread(() => fiboDataN_1.calculate());
               Thread fiboThreadRight = new Thread(() => fiboDataN_2.calculate());
               fiboThreadLeft.Start();
               fiboThreadRight.Start();
			    // Wait for the threads run() method to complete
               fiboThreadLeft.Join();
               fiboThreadRight.Join();
                
			    // use threads results
                result = fiboDataN_1.result + fiboDataN_2.result;
		    }
	    }
    }

    // This Parallelization demonstrates that it is necessary to use thresholds
    // in recursive parallelizations. 
    // The algoritm is correct, but the parallelization does not scale to bigger input numbers.
    // In case of input above 12 the program crashes, because too many threads are created
    class FibonacciNaive
    {
        long arg;
        public long result;

        // TODO use atomic integer
        public static int threadCount;

        public FibonacciNaive(long arg)
        {
            this.arg = arg;
        }

        public void calculate()
        {
            threadCount++;
            Console.WriteLine("Thread Count: " + threadCount);
            if (arg <= 1)
            {
                result = 1;
            }
            else
            {

                // Parallel calculation: Start new Threads for recursion
                FibonacciNaive fiboDataN_1 = new FibonacciNaive(arg - 1);
                FibonacciNaive fiboDataN_2 = new FibonacciNaive(arg - 2);
                // Start the threads
                Thread fiboThreadLeft = new Thread(() => fiboDataN_1.calculate());
                Thread fiboThreadRight = new Thread(() => fiboDataN_2.calculate());
                fiboThreadLeft.Start();
                fiboThreadRight.Start();
                // Wait for the threads run() method to complete
                fiboThreadLeft.Join();
                fiboThreadRight.Join();

                // use threads results
                result = fiboDataN_1.result + fiboDataN_2.result;
            }
        }
    }
}
