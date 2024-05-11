// Copyright Marwan Abu-Khalil 2012


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SeminarParallelComputing.exercises.task.u3_4_fibonacci_tasks.basis
{
    /*
     * Basis for Exercise 5 Fibonacci Parallelization with TPL Tasks
     * 
     * Contains the sequential Fibonacci Algorithm and an execution with time measurement. 
     * 
     * Hints for the solution:
     * 
     * Start a TPL Task:
     *  Task<int> fibTask = Task<int>.Factory.StartNew(() => FibonacciAlgorithm(arg - 1));
     *  
     * Waiting for this task to complete and fetch the result
     *  fibTask.Result;
     * 
     */
    public class FibonacciTPL_Basis
    {
        public static void RunFibonacci()
        {
            int arg = 45; // 10;//42;
            Console.WriteLine("Fibonacci calling with arg: " + arg);
            Stopwatch stopWatch = Stopwatch.StartNew();
            stopWatch.Start();


            // Sequential Approach
            int result = (new FibonacciSequential()).FibonacciAlgorithm(arg);

            
            Console.WriteLine("Result: " + result);

            // Measure runtime
            stopWatch.Stop();
            TimeSpan elapsed = stopWatch.Elapsed;
            Console.WriteLine("Time elapsed: " + elapsed);

        }
    }


    class FibonacciSequential
    {


        public int FibonacciAlgorithm(int arg)
        {
            if (arg == 0 || arg == 1)
            {
                return 1;

            }
            else
            {
                int fib_n_min_1 = FibonacciAlgorithm(arg - 1);

                int fib_n_min_2 = FibonacciAlgorithm(arg - 2);

                int fib_n = fib_n_min_1 + fib_n_min_2;
                //Console.WriteLine("FibonacciSequential arg: " + arg + " returning: " + fib_n + " thread " + Thread.CurrentThread.ManagedThreadId);
                return fib_n;
            }
        }
    }
}