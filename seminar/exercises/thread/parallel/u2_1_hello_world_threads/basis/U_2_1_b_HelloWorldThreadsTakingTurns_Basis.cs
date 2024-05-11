using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// Copyright Marwan Abu-Khalil 2012
namespace SeminarParallelComputing.exercises.thread.parallel.u2_1_hello_world_threads.basis
{
    /*
     * Basis für Uebung 1 b: Threads taking turns.
     *
     * Enthält zwei Threads die jeweils ein Wort in einer unendlichen Schleife auf die Konsole schreiben.
     * 
     * Aufgabe ist, sicherzustellen, dass die beiden Worte immer genau abwechselnd geschrieben werden.
     * 
     */
    public class U_2_1_b_HelloWorldThreadsTakingTurns_Basis {
        public static void MainTest()
        {
		    new TakingTurnsThread().startThreads();
	    }
    }

    // Taking turns to print Hello or World. 
    // No Monitor is used, but the price is: Busy Waiting!
    public class TakingTurnsThread{
	
	
	    const String HELLO = "Hello";
	    const String WORLD = "World";
	
	    // TODO: find a mechanism that tells each thread, if now it is his turn
	    bool CheckIfItIsMyTurn(bool myID) {
            
            // TODO: This does NOT work, it is just a stub for compilation.
            return myID;
	    }

	    // Method for the "Hello" Thread
        void HelloLoop() {
            while (true) {
                if (CheckIfItIsMyTurn(true)){
                    Console.WriteLine(HELLO);
                }
            }
        }

        // Method for the "World" Thread
	    void WorldLoop() {
		    while (true) {
			    if (CheckIfItIsMyTurn(false)) {
			        Console.WriteLine(WORLD);
			    }
		    }
	    }
	
	    /*
         * Start the two threads
         */
	    public void startThreads(){
		
		    Thread helloThread = new Thread(HelloLoop);

		    Thread worldThread = new Thread(WorldLoop);
		
		    helloThread.Start();
		    worldThread.Start();
	    }

    }
}
