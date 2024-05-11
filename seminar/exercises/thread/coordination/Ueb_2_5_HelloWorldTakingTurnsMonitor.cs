using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.thread.coordination
{
    /*
     * Lösung zu Übung 2.5: Hello World, wobei die beiden Worte immer genau abwechselnd auf 
     * der Konsole erscheinen sollen. 
     * Dabei soll kein BusyWaiting verwendet werden!
     * 
     * Lernziel: Anwendung des Monitor-Patterns verstehen.
     * 
     */
    class Ueb_2_5_HelloWorldTakingTurnsMonitor
    {
        const string hello = "Hello";
        const string world = "World";

        public static void TestMain(){
            Ueb_2_5_HelloWorldTakingTurnsMonitor helloWorldInstance = new Ueb_2_5_HelloWorldTakingTurnsMonitor();
            helloWorldInstance.RunTest();
        }

        void RunTest()
        {
            // Start two threads, each responsible for printing one of the words
            new Thread(() => { PrintLoop(hello); }).Start();
            new Thread(() => { PrintLoop(world); }).Start();
        }

        // Points to the string that was printed most recently.
        string lastString = world;
       
        void PrintLoop(String stringToPrint)
        {
            while (true)
            {
                lock(this){
                    // Check, if my stringToPrint was printed most recently. If yes, wait to be woken up.
                    while(lastString == stringToPrint)
                    {
                        Monitor.Wait(this);
                    }

                    // Now it is known, that the "other" string was printed most recently, so print my stringToPrint.
                    Console.WriteLine(stringToPrint);

                    // Mark my stringToPrint as printed most recently, so that the other thread can print his word.
                    lastString = stringToPrint;

                    // Wake the other thread
                    Monitor.Pulse(this);
                }

            }
        }


       
    
    }
}
