using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SeminarParallelComputing.exercises.task.u_3_7_parallel_for.solution
{
    /*
     * Uebung Parallel For
     * 
     * 1) Verhalten: Baue eine einfache PArallel.For Schleife und zeeige:
     *  - Parallel.For wir von Tasks ausgeführt wird, und nicht direkt von Threads
     *  - Die Schleife wird in so viele Teile zerlegt, wie der Rechner CPUs hat.
     * 
     * 
     * 2) Performance: Zeige, dass man durch Parallel.For einen Perfromance-Gewinn gegenueber einer 
     *  normalen nicht parallelisierten For-Schleife erzielen kann, wenn eine 
     *  CPU-intensive Funktion in der Schleife ausgefuehrt wird.
     *  
    */
    // Demonstrates use of Parallel.For / ForEach
    public class Ueb_3_7_ParallelForSolution
    {
        
        public static void TestMain() {
           

            // 1 Verahlten:
            //new ParallelForBehavioiur().ParallelForIsTaskBased();

            // 2 Performance
            new ParallelForPerformance().MeasureSpeedup();

        }

       

        /*
         * Zeigt das Verahlten von Parallel.For
         */
        class ParallelForBehavioiur {

            // Collection for IDs of threads and tasks
            //ConcurrentDictionary<int, int> taksDict = new ConcurrentDictionary<int, int>();
            ConcurrentBag<KeyValuePair<int, int>> taskBag = new ConcurrentBag<KeyValuePair<int, int>>();
            //ConcurrentDictionary<int, int> taskBag = new ConcurrentDictionary<int, int>();

            public void ParallelForIsTaskBased()
            {
                Parallel.For(0, 100, RecordTrheadIDAndTaskID);

                // Print results (shows that the implementation uses few threads and also few tasks, interestingly task and thread hava a one to one relationship)
                foreach (KeyValuePair<int, int> pair in taskBag.Distinct())
                {
                    Console.WriteLine(" TaskID: " + pair.Key + " ThreadID: " + pair.Value);
                }
            }

            // Stores the ID of the current Thread and the current Task in the Bag Datastructure
            void RecordTrheadIDAndTaskID(int arg)
            {
                Console.WriteLine("SimpleAction called with arg: " + arg + " in Thread " + Thread.CurrentThread.ManagedThreadId + " in Task: " + Task.CurrentId);
                //taskBag[Task.CurrentId] = Thread.CurrentThread.ManagedThreadId;
                taskBag.Add(new KeyValuePair<int, int>((int)Task.CurrentId, (int)Thread.CurrentThread.ManagedThreadId));
            }

        }


        class ParallelForPerformance
        {

            public void MeasureSpeedup()
            {
                int LOOP_COUNT = 50;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < LOOP_COUNT; i++) {
                    LongRunningMethod(i);
                }

                Console.WriteLine("/////////////////// /Sequantial Loop took: " + sw.ElapsedMilliseconds + " Milliseconds //////////////////////");

                sw = new Stopwatch();
                sw.Start();

                Parallel.For(0, LOOP_COUNT, LongRunningMethod);

                Console.WriteLine("////////////////////// Parallele.For Loop took: " + sw.ElapsedMilliseconds + " Milliseconds //////////////");


            }

            // This Method uses CPU, on my machine it runs roughly 1 Second//
            void LongRunningMethod(int arg)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                //Console.WriteLine("LongRunningMethod called with arg: " + arg + " in Thread " + Thread.CurrentThread.ManagedThreadId + " in Task: " + Task.CurrentId);
                for (long i = 1; i < 3000000; ++i)
                {
                    Math.Sqrt(i);
                }
                Console.WriteLine("LongRunningMethod() with arg: " + arg + " returning after " + sw.ElapsedMilliseconds + " Millis");
            }

           
        }
    }
}
