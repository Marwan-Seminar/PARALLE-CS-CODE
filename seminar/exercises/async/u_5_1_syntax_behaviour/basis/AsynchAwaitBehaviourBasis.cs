using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace  SeminarParallelComputing.exercises.async.u_5_1_syntax_behaviour.basis
{
    /*
     * A) Schreiben Sie eine Methode DoAsynchronousWork_Async die als async deklariert ist, und die das Schlüsselwort await verwendet.
     *  Rufen Sie diese Methode auf.
     * 
     * 
     * B) Zeigen Sie durch Console Ausschriften, in welcher Reihenfolge die folgenden Teile Ihres Codes ausgeführt werden
     * - async Methode vor dem await
     * - async Methode nach dem await
     * - Methode, die My_Async aufruft
   

     */
    public class AsyncAwaitBehaviourBasis
    {

        public static void TestMain()
        {
            AsyncAwaitBehaviourBasis instance = new AsyncAwaitBehaviourBasis();

            // Running in Thread A
            Console.WriteLine("Main(): Before call to DoAsynchronousWork_Asynch(), Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);

            // Method call returns, as soon as await is reached inside the called method
            // TODO: Was ist der Rueckgabetyp der asynchronen Methode? Versuche es mit Task<int>
            int resultTask = instance.DoAsynchronousWork_Async();

            Console.WriteLine("Main(): After call to DoAsynchronousWork_Asynch(), Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);
            // Do other Work


            // TODO: greife auf den Wert der Rueckgabe-Objektes zu: resultTask.Result;
            
            // TODO: Gebe das result aus:
            Console.WriteLine("Received Result " + Thread.CurrentThread.ManagedThreadId);
        }

        // TODO Mache aus dieser Methode eine asynchrone Methode durch verwendung des async Keywords
        int DoAsynchronousWork_Async()
        {
            // Running in Thread A, outside of TPL
            Console.WriteLine("Async(): entered " + " Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);

            
            // DoCpuIntensiveWork() is run in a TPL Task, executed in a Thread B
            Task subTask = Task.Run ( () => DoCpuIntensiveWork());
            
            // Do other work in Thread A
            Console.WriteLine("Async(): After Task.Run() " + " Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);

            // !!! Here the control flow is returned to the caller !!!
            // TODO auf subTask warten, mit await 

            // This becomes a Continuation running in Thread B (or another Thread), but not running in a Task
            Console.WriteLine("Async(): After await" + " Thread: " +Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId) ;

            return 7;
        }

        // loops a while
        private void DoCpuIntensiveWork()
        {
            Console.WriteLine("DoCpuIntensiveWork() running in " + " Thread: " + Thread.CurrentThread.ManagedThreadId + " Task: " + Task.CurrentId);
            for (long i = 1; i < 10000000; ++i)
            {
                Math.Sqrt(i);
            }
            Console.WriteLine("DoCpuIntensiveWork() returning");

        }
    }
}
