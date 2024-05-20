using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPLTaskProgramming.AsynchAwait
{
    public class AsynchAwaitBehaviour
    {

       public static void TestMain()
        {
            AsynchAwaitBehaviour instance = new AsynchAwaitBehaviour();

            // Running in Thread A
            Console.WriteLine("Main(): Before call to DoAsynchronousWork_Asynch(), Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);

            // Method call returns, as soon as await is reached inside the called method
            Task<int> resultTask = instance.DoAsynchronousWork_Async();

            Console.WriteLine("Main(): After call to DoAsynchronousWork_Asynch(), Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);
            // Do other Work

            int result = resultTask.Result;

            Console.WriteLine("Received Result " + result + " in Thread: " + Thread.CurrentThread.ManagedThreadId);
        }


        async Task<int> DoAsynchronousWork_Async()
        {
            // Running in Thread A, outside of TPL
            Console.WriteLine("Async(): entered " + " Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);

            
            // DoCpuIntensiveWork() is run in a TPL Task, executed in a Thread B
            Task t = Task.Run ( () => DoCpuIntensiveWork());
            
            // Do other work in Thread A
            Console.WriteLine("Async(): After Task.Run() " + " Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);

            // !!! Here the control flow is returned to the caller !!!
            await t;

            // This becomes a Continuation running in Thread B (or another Thread), but not running in a Task
            Console.WriteLine("Async(): After await" + " Thread: " +Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId) ;

            return 7;
        }

        // loops a while
        private void DoCpuIntensiveWork()
        {
            Console.WriteLine("DoCpuIntensiveWork() running in " + " Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);
            for(long i = 1; i < 10000000; ++i){
                Math.Sqrt(i);
            } 
            Console.WriteLine("DoCpuIntensiveWork() returning") ;

        }
    }
}
