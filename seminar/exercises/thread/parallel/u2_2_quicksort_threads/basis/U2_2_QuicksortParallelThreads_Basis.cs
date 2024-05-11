using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.exercises.thread.parallel.u2_2_quicksort_threads.basis
{

    /* 
     * Basis für Übung 2 Quicksort Parallelisierung mit Threads
     * 
     * Parallelisieren Sie den Quicksort-Algorithmus mit Hilfe von Threads.
     * Der sequentielle Algorithmus steht  in den Vortragsfolien und in der Klasse QuicksortSequential in dieser Datei.
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
     * - class Ueb_2_2_QuicksortParallelThreads_Basis enthält den Ausfuehrungsrahmen: Aufbau eines Array mit zufälligen Werten, Zeitmessung etc.
     */

    class U2_2_QuicksortParallelThreads_Basis
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



            // SEQUENTIAL_QUICKSORT (approx 37 Seconds on 4-Core, 250.000.000 array)
            QuicksortSequential instance = new QuicksortSequential(testData);

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
}