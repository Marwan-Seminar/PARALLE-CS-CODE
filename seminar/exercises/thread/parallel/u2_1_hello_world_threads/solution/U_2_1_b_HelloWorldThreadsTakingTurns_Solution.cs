using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// Copyright Marwan Abu-Khalil 2012
namespace SeminarParallelComputing.exercises.thread.parallel.u2_1_hello_world_threads.solution
{
    // Lösung zu Übung Ü 2.1.b 
    public class U_2_1_b_HelloWorldThreadsTakingTurns_Solution {
        public static void MainTest()
        {
		    new TakingTurnsThread().startThreads();
	    }
    }

    // Taking turns to print Hello or World. 
    // No Monitor is used, but the price is: Busy Waiting!
    public class TakingTurnsThread{
	
	    // Used as Thread-Identifier. The Threads identify themselves as true (Thread 1) or false (Thread 2)
	    private bool lastOne = false;
	
	    const String HELLO = "Hello";
	    const String WORLD = "World";
	
	
	    bool CheckIfItIsMyTurn(bool myID) {
            lock(this){
		        if (lastOne == myID) {
			        // I was the last one to run, so I have to wait now.
			        return false;
		        } else {
			        // The last one to run was the other Thread. Now it is my turn,
			        // mark this in lastOne (toggle)
			        lastOne = !lastOne;
			        // sanity check
			        if (lastOne != myID) {
				        throw new Exception("Insane");
			        }
			        return true;
		        }
            }
	    }

	    // The Hello Threads identifies himself as "true"
	    void HelloLoop() {
		    while (true) {
			    lock(this){
				    if (CheckIfItIsMyTurn(true)) {
					    Console.WriteLine(HELLO);
				    }
			    }
		    }
	    }

	    // The Hello Threads identifies himself as "false"
	    void WorldLoop() {
		    while (true) {
			    lock(this){
				    if (CheckIfItIsMyTurn(false)) {
					    Console.WriteLine(WORLD);
				    }
			    }
		    }
	    }
	
	    //
	    public void startThreads(){
		
		    Thread helloThread = new Thread(HelloLoop);

		    Thread worldThread = new Thread(WorldLoop);
		
		    helloThread.Start();
		    worldThread.Start();
	    }

    }
}
