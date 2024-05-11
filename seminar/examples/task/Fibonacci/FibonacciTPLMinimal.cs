// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SeminarParallelComputing.seminar.examples.task
{

    // A minimalsitic version of Fibonacci with TPL (inefficient implementation, as no threashold defined)
    class FibonacciTPLMinimal
    {
        int RunFiboTask(int fiboStep)
        {
            // End recursion
            if (fiboStep <= 1) { return 1; }

            Task<int> fiboStep_minus_1 = Task<int>.Factory.StartNew(() => RunFiboTask(fiboStep - 1));
            Task<int> fiboStep_minus_2 = Task<int>.Factory.StartNew(() => RunFiboTask(fiboStep - 2));

            // Implicit wait
            return fiboStep_minus_1.Result + fiboStep_minus_2.Result;
        }


        public static void TestFibo()
        {
            int fiboArg = 10;

            int fiboResult = new FibonacciTPLMinimal().RunFiboTask(fiboArg);

            Console.WriteLine("FibonacciTPLMinimal.FiboTPL: Arg: " + fiboArg + " Result: " + fiboResult);
        }
    
    }
    
}
