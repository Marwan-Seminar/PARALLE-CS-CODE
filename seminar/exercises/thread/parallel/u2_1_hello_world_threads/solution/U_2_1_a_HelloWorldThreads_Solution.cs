using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.exercises.thread.parallel.u2_1_hello_world_threads.solution
{

 // Copyright Marwan Abu-Khalil 2012
    public class U_2_1_a_HelloWorldThreads_Solution
    {

        /*
         * Ü 2.1 Triviale Parallelisierung mit Threads: „Hello World“ 
         * 
         * Schreiben Sie ein Programm mit zwei Threads. Ein Thread schreibt das Wort „Hello“ auf
         * die Konsole, der andere das Wort „World“.
         * 
         * a ) Die Threads schreiben in beliebiger Abfolge
         * 
         * b) Versuchen Sie, die Lösung von a) so verfeinern, dass die Worte immer
         * genau abwechselnd ausgeschrieben werden (ohne wait() und notify() zu
         * verwenden)
         * 
         * Lernziel: Thread API kennenlernen, beobachten, dass das OS die Threads
         * abwechselnd bzw. gleichzeitig schedult.
         */

        public static void MainTest()
        {

            U_2_1_a_HelloWorldThreads_Solution instance = new U_2_1_a_HelloWorldThreads_Solution();
            // a
            instance.startTwoSimpleThrads();
            // b siehe  class HelloWorldTakingTurns

        }

        void PrintLoop(object text)
        {
            while (true)
            {
                Console.WriteLine(text);
            }
        }

        void startTwoSimpleThrads()
        {
            new Thread(PrintLoop).Start("Hello");
            new Thread(PrintLoop).Start("World");

        }
    }
}
