using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.exercises.thread.parallel.u2_3_fibonacci_threads.basis
{

    /*
     * This file contains the sequential recursive fibonacci algorithm.
    */
    class FibonacciThreadMain_Basis
    {
        public static void TestFiboMain()
        {

            int fibonacciInputValue = 46; // 46; //  42; // 16; // 10;

            Console.WriteLine("Fibonacci Basis entered");

            // Seqeuntial calculation of Fibonacci
            RunFibonacciSequntial(fibonacciInputValue);


        }

        private static void RunFibonacciSequntial(int fibonacciInputValue)
        {
            Console.WriteLine("Sequntial Fibonacci entered, inputvalue: " + fibonacciInputValue);

            Stopwatch swSeq = new Stopwatch();
            swSeq.Start();

            // run sequential Fibonacci:
            long result = (new FibonacciSequential()).calculate(fibonacciInputValue);

            Console.WriteLine("  Sequential Fibonacci  returned. Argument: " + fibonacciInputValue + " Result: " + result + " Time: " + swSeq.Elapsed);
        }



    }

    class FibonacciSequential
    {
        public long calculate(long n)
        {
            if (n <= 1)
            {
                return 1;
            }
            else
            {

                // TODO use threads here to parallelize the recursion
                long fibo_n_minus_one = calculate(n - 1);

                long fibo_n_minus_two = calculate(n - 2);

                return fibo_n_minus_one + fibo_n_minus_two;
            }
        }
    }
}