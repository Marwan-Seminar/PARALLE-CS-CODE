using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.thread.parallel
{
    /* Exercise 2.3
     * 
     * Threadppool basierte Parallelisierung von Quicksort.
     * 
     * Was passiert, wenn man die Quicksort-Jobs in den Thread-Pool einfügt und nicht wartet?
     *  - Das Programm beendet sich, bevor das Array komplett sortiert ist.
     * Was passiert, wenn man die Quicksort-Jobs rekursiv in den Thread-Pool einfügt und wartet?
     *  - Das Programm arbeitet aber Da die Threads blockieren, startet der Threadpool immer neue Threads. 
     *  - Kein Vorteil gegenüber der von Hand mit Threads parallelisierten Lösung, keine effiziente Wiederverwendung von Threads
     *  
     * Lernziel: Thread-Pool APIs kennen und Begrenzungen von Thread-Pools einschätzen können, als Motivation für Task-Scheduler.
     * 
    */

    class Ueb_2_3_QuicksortParallelThreadPool
    {
       
        public static void TestMain()
        {
            Console.WriteLine("Ueb_2_3_QuicksortParallelThreadPool started: ");
           

            // Max 300000000 (300  Mio)
            int ARRAYSIZE = 250000000;
            //int ARRAYSIZE = 2500000;
           
            
            int[] testData = new int[ARRAYSIZE];
            Console.WriteLine("Arraysize: " + ARRAYSIZE);
            Random rand = new Random();
            for (int i = 0; i < ARRAYSIZE; ++i)
            {
                int nextInt = rand.Next();
                testData[i] = nextInt;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            
            // Parallelized Version with threadpool
            QuicksortParallelThreadPool instance = new QuicksortParallelThreadPool(testData);

            instance.sort();

            // Print is possible only for small arrays
            //print(testData);

            Console.WriteLine("Time elapsed: " + sw.Elapsed);
        }  


        public static void print(int[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                Console.WriteLine(data[i]);
            }
        }
    }



    /*
     * An Quicksort Version parallelized with a Thread-Pool.
     */
    class QuicksortParallelThreadPool  {
	    // Array of integers to be sorted
	    private int[] data;

	 
	    // Gets calculated in the topmost recursion by the public ctor
	    // and afterwards is propagated in each recursion step via private ctor
	    static  int threshold;
	
	    static int threadCount = 0;

        public QuicksortParallelThreadPool(int[] data)
        {
		    this.data = data;

		    // Define threshold	according to naive heuristics
            threshold = data.Length / Environment.ProcessorCount;
            Console.WriteLine("ProcessorCount: " + Environment.ProcessorCount);
	    }
	
        struct RecursionParameter{
            public RecursionParameter(int lower, int upper){
                this.lower = lower;
                this.upper = upper;
            }
            public int lower;
            public int upper;

            override public string ToString()
            {
                return "RecursionParameter lower: " + lower + " upper: " + upper;
            }
        }
         
	    public void sort() {
            Console.WriteLine("threshold:\t" + threshold);
		    Console.WriteLine("data.length:\t" + data.Length);

            RecursionParameter param = new RecursionParameter(0, data.Length - 1);

            sortRecursively(param);
		    
            Console.WriteLine("Number of used Threads: " +  threadCount);
	    }

        
        void sortRecursively(object parameterObjct){

            RecursionParameter parameter = (RecursionParameter)parameterObjct;        

            int l = parameter.lower;
            int u = parameter.upper;
		
		    if (l >= u) {
			    return;
		    }

		    // Choose a pivot element, arbitrarily.
		    int pivotIdx = l;

		    // running indices:
		    int i = l;
		    int j = u + 1;

		    // Sorts the subrange [l,u] of the data array into two sub-arrays such
		    // that
		    // all members of the left sub-array are smaller than all members of the
		    // right sub-array.
		    // After this loop j is the index of the dividing element.
		    while (true) {
			    // i points to the left end of the range under consideration.
			    // Shift i to the right as long as elements are <= pivot.
			    // After this loop, i points to an element which is bigger than
			    // pivot, of i == j.
			    do {
				    i++;
			    } while (i <= u && data[i] <= data[pivotIdx]);

			    // Do the analogous action for the right end pointer j
			    do {
				    j--;
			    } while (data[j] > data[pivotIdx]);

			    if (i > j)
				    break;

			    // Swap
			    Swap(i, j);
		    }
		    Swap(pivotIdx, j);

		    // Recursive Calls
		    // Check threshold to decide for sequential or parallel processing
            if (u - l < threshold)
            {
			   // Sequential recursion, does not start new threads!
                this.sortRecursively(new RecursionParameter( l, j - 1));
                this.sortRecursively(new RecursionParameter(j + 1, u));
			
		    } else{
			    // Parallel recursion, enqueue jobs to the Thread-Pool

                CountdownEvent contDownEvent = new CountdownEvent(2);

                RecursionParameter leftArg = new RecursionParameter(l, j - 1);
                ThreadPool.QueueUserWorkItem(arg =>
                    {
                        Console.WriteLine("ThreadPool Job: " + arg);
                        sortRecursively(arg);
                        contDownEvent.Signal();
                    }, leftArg);
                RecursionParameter rightArg = new RecursionParameter(j + 1, u);
                ThreadPool.QueueUserWorkItem(arg =>
                {
                    Console.WriteLine("ThreadPool Job: " + arg + " Processed in Thread: " + Thread.CurrentThread.ManagedThreadId);
                    sortRecursively(arg);
                    contDownEvent.Signal();
                }, rightArg);
                //ThreadPool.QueueUserWorkItem(this.sortRecursively, new RecursionParameter(j + 1, u));

                contDownEvent.Wait();
		    }				
	    }

	    private void Swap(int i, int j) {

		    int tmp = data[i];
		    data[i] = data[j];
		    data[j] = tmp;
	    }
    }

}
