// Copyright Marwan Abu-Khalil 2012


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SeminarParallelComputing.seminar.examples.task.Fibonacci
{
    // Demonstrates implementation of Fibonacci Sequence based on TPL Tasks
    // Naive and inefficient approach
    // ATTENTION: The sequential version ist faster!!?? Maybe due to object boxing overhead in lambda function calls?
    // Idee: Evtl einen Weg finden, erst alle Tasks zu forken, und dann nach und dann die Resultate zusemmenfügen, so dass die CPUs bald ausgelastet sind.
    public class FibonacciTPL
    {
        public static void RunFibonacci()
        {
            int arg = 10;//42;
            Console.WriteLine("Fibonacci calling with arg: " + arg);
            Stopwatch stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            
            
            //int result = (new FibonacciSequential()).FibonacciAlgorithm(arg);


            FibonacciTask fiboTasks = new FibonacciTask(arg);
            int result = fiboTasks.RunFibonacciAlgorithm();

            Console.WriteLine("Result: " + result);

            // Measure runtime
            stopWatch.Stop();
            TimeSpan elapsed = stopWatch.Elapsed;
            Console.WriteLine("Time elapsed: " + elapsed);
            
        }
    }


    class FibonacciSequential
    {


       public int FibonacciAlgorithm(int arg){
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
            grainsize = arg - 5; //(int) ((arg / Environment.ProcessorCount) * 2 );
            Console.WriteLine("FibonacciTask arg: " + arg + " grainsize " + grainsize);
        }

        public int RunFibonacciAlgorithm(){
            return FibonacciAlgorithm(initialArg);    
        }

        
        int FibonacciAlgorithm(int arg)
        {
            //if (VERBOSE) Console.WriteLine("FibonacciTask arg: " + arg +  " thread " + Thread.CurrentThread.ManagedThreadId);
            
            if (arg == 0 || arg == 1)
            {
                return 1;

            }
            else if (arg > grainsize)
            {// parallel path

                // 1.a "Passing data", using a Lambda expression 
                Task<int> fibTask_min_1 = Task<int>.Factory.StartNew(() => FibonacciAlgorithm(arg - 1));
                Task<int> fibTask_min_2 = Task<int>.Factory.StartNew(() => FibonacciAlgorithm(arg - 2));

               
                
               // 1.b "Passing data", using a state object 
                //Task<int> fibTask_min_1 = Task<int>.Factory.StartNew(stateObj => FibonacciAlgorithm((int)stateObj), arg -1 );
                //Task<int> fibTask_min_2 = Task<int>.Factory.StartNew(stateObj => FibonacciAlgorithm((int) stateObj), arg - 2);


                // 2. Getting the return values of the tasks functions.
                // 3. Waiting happens implicitly, as the call to Result is a blocking operation.
                int fib_min_1 = fibTask_min_1.Result;
                int fib_min_2 = fibTask_min_2.Result;


                int fib_n = fib_min_1 + fib_min_2;
                if (VERBOSE) Console.WriteLine("FibonacciTask arg: " + arg + " returning " + fib_n + " thread " + Thread.CurrentThread.ManagedThreadId);
                return fib_n;
            }
            else
            {// sequential path
              int fib_min_1 =  FibonacciAlgorithm(arg - 1);
              int fib_min_2 =  FibonacciAlgorithm(arg - 2);

              int fib_n = fib_min_1 + fib_min_2;

              return fib_n;
            }
        }
    }    
}
