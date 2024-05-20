using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SeminarParallelComputing.exercises.async.u_5_2_async_performance.basis
{

    /*
     * Aufgabe zu async await und Performance
     * 
     * Zeige, dass mit async / await ein wesentlicher Performance-Gewinn erzielt werden kann
     * 
     * Gehe in zwei Schritten vor:
     * Die Basis ehthält ein sequentielles Scenario mit drei langlaufenden
     * synchronen blockierenden Aufrufen: LogrunningJobBlocking();
     * 
     * Schritt 1: mache  LogrunningJobBlocking() zu einer async Methode, 
     *      die intern await verwendet, um den blockiernden Aufruf asynchron abzusetzen
     *      Dieser Schritt gibt den Thread frei, aber alleine erbringt er noch keine 
     *      Zeit-Ersparnis
     *     
     * Schritt 2: Verändere die Aufrufe von LogrunningJobBlocking() mit Hilfe 
     *      von await nun so, dass diese Aufrufe gleichzeitig laufen können.
     *      Zeige, dass Du nun eine Zeit-Erspaniss gewonnen hast!
     *      
    */

    /*
     * Demostrates speedup due to usage of asynch await in case of blocking Jobs (I/O)
     * Case 1: Blocking job without asynchronity
     * Case 2: Blocking job with async / await naive and slow
     * Case 3: Blocking job with async / await fast
    */
    public class AsyncAwaitSpeedupBlockingBasis
    {

        public static void MainTest()
        {
            
            Console.WriteLine("AA Speedup MainTest");

            // Aufruf des Basis-Codes
            BlockingSequential.RunTest();

        }




    }

    /*
     * Basis Code: Blockieren in einem sequentiellen Scenario
     * Ziel: Die blockierenden Aufrufe asynchron machen und Zeit sparen
     */
    class BlockingSequential
    {


        public static void RunTest()
        {
            new BlockingSequential().CallBlockingJobsSequantially();
        }
        /*
         * Calls several blocking tasks in sequential manner
         */
        void CallBlockingJobsSequantially()
        {
            Console.WriteLine("BlockingSequential: Calling Blocking Methods");


            Stopwatch sw = new Stopwatch();
            sw.Start();

            // 1. Aufruf der langlaufenden Methode
            // TODO (Schritt 2): hier ein Task Rückgabe-Objekt entgegennehmen und UNTEN darauf warten, mit await
            LogrunningJobBlocking();

            // 2. Aufruf der langlaufenden Methode
            // TODO(Schritt 2): hier ein Task Rückgabe-Objekt entgegennehmen und UNTEN darauf warten, mit await
            LogrunningJobBlocking();

            // 3. Aufruf der langlaufenden Methode
            // TODO (Schritt 2): hier ein Task Rückgabe-Objekt entgegennehmen und UNTEN darauf warten, mit await
            LogrunningJobBlocking();


            // TODO (Schritt 2): Hier mit await auf die drei Task-Objekte warten,
            // die von  LogrunningJobBlocking() zurueckgegeben wurden

            Console.WriteLine("Blocking jobs sequentially took: " + sw.ElapsedMilliseconds);
        }

        /*
         * Dies ist eine blockierende synchrone Methode.
         * 
         * TODO (Schritt 1): Baue diese Methode um, in eine asynchrone Methode,
         * die den blockierenden Aufruf in einen asynchronen Task verlagert und auf dessen
         * Beendigung mit await wartet. Der Nutzen ist, dass der aufrufende Thread in der Wartezeit 
         * frei wird!
         * 
         * Hinweise
         * - async keyword in die Methoden-Deklarartion einbringen
         * - Blockierenden Aufruf in einen Task auslagern
         * - Mit await warten, auf die Beendigung dieses Tasks
         */
        // TODO (Schritt 1) async keyword
        void LogrunningJobBlocking()
        {
            Console.WriteLine("Blocking Job going to sleep in Thread " + Thread.CurrentThread.ManagedThreadId);

            // Thread.Sleep(3000) ist ein blockiernder Aufruf
            // TODO (Schritt 1) Lagere diesen in einen asynchronen Task aus, auf den Du mit await warten kannst
            //await Task.Run(() => Thread.Sleep(3000));
            Thread.Sleep(3000);

            Console.WriteLine("Blocking Job woke up in Thread " + Thread.CurrentThread.ManagedThreadId);

        }
    }
}


