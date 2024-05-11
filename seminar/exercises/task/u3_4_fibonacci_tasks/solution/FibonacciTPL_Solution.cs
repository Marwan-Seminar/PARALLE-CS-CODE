// Copyright Marwan Abu-Khalil 2012


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SeminarParallelComputing.exercises.task.u3_4_fibonacci_tasks.solution
{
    // Demonstrates implementation of Fibonacci Sequence based on TPL Tasks
    // Naive and inefficient approach
    // This file contains two approaches, a parallel approach and a sequential approach. 
    // TODO introduce long instead of int, otherwise overflow errors!!!
    public class FibonacciTPL_Solution
    {
        public static void RunFibonacci()
        {
            int arg = 47; // 10;//42;
            Console.WriteLine("FibonacciTPL_Solution ");
            Console.WriteLine("Fibonacci calling with arg: " + arg);
            
            Stopwatch stopWatch = Stopwatch.StartNew();
            stopWatch.Start();

            bool RUN_PARALLEL = true;
            long result;

            if (RUN_PARALLEL)
            {
                Console.WriteLine(" Task-Parallel Approach");
                // Task-Parallel Approach
                FibonacciTask fiboTasks = new FibonacciTask(arg);
                result = fiboTasks.RunFibonacciAlgorithm();

            }
            else
            {
                Console.WriteLine("Sequential Approach");
                // Sequential Approach
                result = (new FibonacciSequential()).FibonacciAlgorithm(arg);
            }

            
            
            Console.WriteLine("Result: " + result);

            // Measure runtime
            stopWatch.Stop();
            TimeSpan elapsed = stopWatch.Elapsed;
            Console.WriteLine("Time elapsed: " + elapsed);
            
        }
    }


    class FibonacciSequential
    {


       public long FibonacciAlgorithm(int arg){
            if (arg == 0 || arg == 1)
            {
                return 1;

            }
            else
            {
                long fib_n_min_1 = FibonacciAlgorithm(arg - 1);

                long fib_n_min_2 = FibonacciAlgorithm(arg - 2);

                long fib_n = fib_n_min_1 + fib_n_min_2;
                //Console.WriteLine("FibonacciSequential arg: " + arg + " returning: " + fib_n + " thread " + Thread.CurrentThread.ManagedThreadId);
                return fib_n;
            }
        }
    }


    // Shows an TPL based implementation of the Fibonacci sequence.
    // (inefficient algorithm)
    // The following concepts are presented:
    // 1. Passing data into a task
    // 2. Getting return data from a task
    // 3. Waiting for a task to complete
    class FibonacciTask
    {
        bool VERBOSE = true;

        int initialArg;
        int grainsize;


       public FibonacciTask(int arg)
        {
            this.initialArg = arg;
            grainsize = arg - 5 ;// 5; //(int) ((arg / Environment.ProcessorCount) * 2 );
            Console.WriteLine("FibonacciTask arg: " + arg + " grainsize " + grainsize);
        }

        public long RunFibonacciAlgorithm(){
            return FibonacciAlgorithm(initialArg);    
        }

        
        long FibonacciAlgorithm(object arg_obj)
        {
            int arg = (int) arg_obj;
 
            //if (VERBOSE) Console.WriteLine("FibonacciTask arg: " + arg +  " thread " + Thread.CurrentThread.ManagedThreadId);
            
            if (arg == 0 || arg == 1)
            {
                return 1;

            }
            else if (arg > grainsize)
            {

                // parallel path

                // 1.a "Passing data", using a Lambda expression 
                Task<long> fibTask_min_1 = Task<long>.Factory.StartNew(() => FibonacciAlgorithm(arg - 1));
                Task<long> fibTask_min_2 = Task<long>.Factory.StartNew(() => FibonacciAlgorithm(arg - 2));



                // 1.b "Passing data", using a state object 
                //Task<int> fibTask_min_1 = Task<int>.Factory.StartNew(stateObj => FibonacciAlgorithm((int)stateObj), arg -1 );
                //Task<int> fibTask_min_2 = Task<int>.Factory.StartNew(stateObj => FibonacciAlgorithm((int) stateObj), arg - 2);

                // 1c
                //Task<int> fibTask_min_1 = new Task<int>(FibonacciAlgorithm, arg - 1);
                //fibTask_min_1.Start();
                //Task<int> fibTask_min_2 = new Task<int>(FibonacciAlgorithm, arg - 2);
                //fibTask_min_2.Start();

                // 2. Getting the return values of the tasks functions.
                // 3. Waiting happens implicitly, as the call to Result is a blocking operation.
                long fib_min_1 = fibTask_min_1.Result;
                long fib_min_2 = fibTask_min_2.Result;


                long fib_n = fib_min_1 + fib_min_2;
                if (VERBOSE) Console.WriteLine("FibonacciTask arg: " + arg + " returning " + fib_n + " thread " + Thread.CurrentThread.ManagedThreadId);
                return fib_n;
            }
            else
            {
                // This is the sequential path, it is taken if the argument is "small"
                // Interstingly, if I implement the recursion logic within this method, the code becomes very slow.
                // Maybe this is related to the Lamda Expressions in the parallel path.
                // As a workaround I use the class FibonacciSequential
                return (new FibonacciSequential()).FibonacciAlgorithm(arg);

                // THIS IS SLOW!!! DONT USE IT
                // return FibonacciAlgorithm(arg - 1) + FibonacciAlgorithm(arg - 2);
            }
        }
    }    
}
