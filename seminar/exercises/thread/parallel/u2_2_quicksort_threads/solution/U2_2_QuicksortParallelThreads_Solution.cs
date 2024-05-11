using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.exercises.thread.parallel.u2_2_quicksort_threads.solution
{

    /* 
     * Löung für Übung 2
     * Parallelisieren Sie den Quicksort-Algorithmus mit Hilfe von Threads.
     * Der sequentielle Algorithmus steht in in den Vortragsfolien und in der Klasse QuicksortSequential in dieser Datei.
     * 
     * Finden Sie eine Lösung, deren Laufzeit wesentlich besser ist als die des sequentiellen Algorithmus.Ideal ist: Laufzeit = Ursprüngliche Laufzeit / Anzahl CPUs
     * 
     * Hinweise: 
     * - Begrenzen Sie die Anzahl der Threads, die gestartet werden.
     * - Benutzen Sie jedoch keinen Threadppol o.ä.
     * 
     * Lernziel:
     * - Erkennen, dass ein massiver Speedup möglich ist (z.B. ca. eine Verdreifachung der Performance auf einer 4-Kern CPU)
     * - Probleme erkennen und lösen, die durch eine zu große Anzahl von Threads entstehen
     * 
     * Erläuterungen zum Code:
     * - class QuicksortSequential enthält eine sequentielle Implementierung von Quicksort
     * - class QuicksortParallelThreads enthält eine mit Threads paralellisierte Implementierung von Quicksort
     * - class Ueb_2_2_QuicksortParallelThreads  enthält den Testcode zu QuicksortParallelThreads
     * 
     * Die verschiedenen Versionen können durch ein- / auskommentieren der folgendermaßen markierten Zeilen aktiviert werden:
     * SEQUENTIAL_QUICKSORT
     * PARALLEL_QUICKSORT_ERRONEOUS
     * PARALLEL_QUICKSORT
    */

    class U2_2_QuicksortParallelThreads_Solution
    {

        public static void MainQuicksort()
        {
            Console.WriteLine("Ueb_2_2_QuicksortParallelThreads started: ");

            // Max 300000000 (300  Mio)
            // approx 12 Seconds parallel vs. 40 Sec. sequntial (on my machine, corei7 quadcore)
            int ARRAYSIZE = 250000000;


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

            // Three versions:

            // 1.SEQUENTIAL_QUICKSORT (approx 37 Seconds on 4-Core, 250.000.000 array)
            //QuicksortSequential instance = new QuicksortSequential(testData);

            // 2. PARALLEL_QUICKSORT_ERRONEOUS Oversimplified parallel version (does not work for big arrays)
            // QuicksortParallelSimple instance = new QuicksortParallelSimple(testData);

            // 3. PARALLEL_QUICKSORT Parallelized Version with threshold to limit nr. of threads started (approx 12 Seconds on 4-Core, 250.000.000 array)
            QuicksortParallelThreads instance = new QuicksortParallelThreads(testData);

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

    // This is the sequential recursice algorithm, it can be used as starting point for this exercise.
    class QuicksortSequential
    {

        // Array of integers to be sorted
        private int[] data;

        public QuicksortSequential(int[] data)
        {
            this.data = data;
        }

        public void sort()
        {
            sortRecursively(0, data.Length - 1);
        }

        void sortRecursively(int l, int u)
        {

            if (l >= u)
            {
                return;
            }

            // Choose a pivot element, arbitrarily.
            int pivotIdx = l;

            // running idices:
            int i = l;
            int j = u + 1;

            // Sorts the subrange [l,u] of the data array into two sub-arrays such that
            // all members of the left sub-array are smaller than all members of the right sub-array.
            // After this loop j is the index of the dividing element.
            while (true)
            {
                // i points to the left end of the range under consideration. 
                // Shift i to the right as long as elements are <= pivot.
                // After this loop, i points to an element which is bigger than pivot, of i == j.
                do
                {
                    i++;
                } while (i <= u && data[i] <= data[pivotIdx]);


                // Do the analogous action for the right end pointer j
                do
                {
                    j--;
                } while (data[j] > data[pivotIdx]);

                if (i > j) break;

                // Swap
                Swap(i, j);
            }

            Swap(pivotIdx, j);

            // Recursive Calls
            sortRecursively(l, j - 1);
            sortRecursively(j + 1, u);
        }


        private void Swap(int i, int j)
        {

            int tmp = data[i];
            data[i] = data[j];
            data[j] = tmp;
        }

    }


    // This is a oversimplified paralleization, it does not work for bigger arrays.
    class QuicksortParallelSimple
    {

        // Array of integers to be sorted
        private int[] data;

        public QuicksortParallelSimple(int[] data)
        {
            this.data = data;
        }

        public void sort()
        {
            sortRecursively(0, data.Length - 1);
        }

        void sortRecursively(int l, int u)
        {

            if (l >= u)
            {
                return;
            }

            // Choose a pivot element, arbitrarily.
            int pivotIdx = l;

            // running idices:
            int i = l;
            int j = u + 1;

            // Sorts the subrange [l,u] of the data array into two sub-arrays such that
            // all members of the left sub-array are smaller than all members of the right sub-array.
            // After this loop j is the index of the dividing element.
            while (true)
            {
                // i points to the left end of the range under consideration. 
                // Shift i to the right as long as elements are <= pivot.
                // After this loop, i points to an element which is bigger than pivot, of i == j.
                do
                {
                    i++;
                } while (i <= u && data[i] <= data[pivotIdx]);


                // Do the analogous action for the right end pointer j
                do
                {
                    j--;
                } while (data[j] > data[pivotIdx]);

                if (i > j) break;

                // Swap
                Swap(i, j);
            }

            Swap(pivotIdx, j);

            // Recursive Calls
            Thread leftThread = new Thread(() => sortRecursively(l, j - 1));
            Thread rightThread = new Thread(() => sortRecursively(j + 1, u));
            leftThread.Start();
            rightThread.Start();
            leftThread.Join();
            rightThread.Join();

        }


        private void Swap(int i, int j)
        {

            int tmp = data[i];
            data[i] = data[j];
            data[j] = tmp;
        }

    }


    /*
     * One possible Solution: 
     * 
     * This class implements a parallel version of quicksort. 
     * Recursively new threads are started, until a the sub-array to be sorted is smaller than a certaion threshold. 
     * From that stage on quicksort is exwcuted sequentially on that sub-array.
     *
     *
     */
    class QuicksortParallelThreads
    {
        // Array of integers to be sorted
        private int[] data;


        // Gets calculated in the topmost recursion by the public ctor
        // and afterwards is propagated in each recursion step via private ctor
        static int threshold;

        static int threadCount = 0;

        public QuicksortParallelThreads(int[] data)
        {
            this.data = data;

            // Define threshold	according to naive heuristics
            threshold = data.Length / Environment.ProcessorCount;
            Console.WriteLine("ProcessorCount: " + Environment.ProcessorCount);
        }

        struct RecursionParameter
        {
            public RecursionParameter(int lower, int upper)
            {
                this.lower = lower;
                this.upper = upper;
            }
            public int lower;
            public int upper;
        }

        public void sort()
        {
            Console.WriteLine("threshold:\t" + threshold);
            Console.WriteLine("data.length:\t" + data.Length);

            RecursionParameter param = new RecursionParameter(0, data.Length - 1);

            sortRecursively(param);

            Console.WriteLine("Number of used Threads: " + threadCount);
        }


        void sortRecursively(object parameterObjct)
        {

            RecursionParameter parameter = (RecursionParameter)parameterObjct;

            int l = parameter.lower;
            int u = parameter.upper;

            if (l >= u)
            {
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
            while (true)
            {
                // i points to the left end of the range under consideration.
                // Shift i to the right as long as elements are <= pivot.
                // After this loop, i points to an element which is bigger than
                // pivot, of i == j.
                do
                {
                    i++;
                } while (i <= u && data[i] <= data[pivotIdx]);

                // Do the analogous action for the right end pointer j
                do
                {
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
                this.sortRecursively(new RecursionParameter(l, j - 1));
                this.sortRecursively(new RecursionParameter(j + 1, u));

            }
            else
            {
                // Parallel recursion, starts new Threads

                Thread leftThread = new Thread(this.sortRecursively);
                Interlocked.Increment(ref threadCount);
                Thread rightThread = new Thread(this.sortRecursively);
                Interlocked.Increment(ref threadCount);

                // Fork new threads
                leftThread.Start(new RecursionParameter(l, j - 1));
                rightThread.Start(new RecursionParameter(j + 1, u));


                // Slower version! (based on Lambdas. Why?)
                /*
                Thread leftThread = new Thread(() =>
                {
                    this.sortRecursively(new RecursionParameter(l, j - 1));
                    Interlocked.Increment(ref threadCount);
                });
                Thread rightThread = new Thread(() =>
                {
                    this.sortRecursively(new RecursionParameter(j + 1, u));
                    Interlocked.Increment(ref threadCount);
                });

                // Fork new threads
			    leftThread.Start();
                rightThread.Start();
                */

                // Wait for Threads to finish
                leftThread.Join();
                rightThread.Join();
            }
        }

        private void Swap(int i, int j)
        {

            int tmp = data[i];
            data[i] = data[j];
            data[j] = tmp;
        }
    }

}
