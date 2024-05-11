using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.examples.task.Quicksort
{
    // This file contains classes that demonstrate the sppedup of Quicksort
    // based on Parallel-Tasks (TPL)
    // On my machine, Dual Core, with Hyperthreading, a speed up of factor 2 is achivable.
    // E.g. ARRAYSIZE = 100000000; 26 sec. parallel vs. 45 sequential. Interestingly CPU usage is 95 % vs. 30 %
    class QuicksortTPLTestMain
    {
        const bool PARALLEL = true;

        public static void TestMain()
        {


            Stopwatch sw = new Stopwatch();
            sw.Start();

             //MiniTest();
             BigTest();

            Console.WriteLine("Time elapsed: " + sw.Elapsed);
        }

        private static void BigTest()
        {
            // Big arraysize not possible with testframework. Run from commandline
            // Max 300000000 (300  Mio)
            int ARRAYSIZE = 250000000;
            int[] testData = new int[ARRAYSIZE];

            Random rand = new Random();
            for (int i = 0; i < ARRAYSIZE; ++i)
            {
                int nextInt = rand.Next();
                testData[i] = nextInt;
            }

            if (PARALLEL)
            {
                QuicksortParallelTasks sortingInstance = new QuicksortParallelTasks(testData);
                sortingInstance.sort();

                //print(testData);
            }
            else
            {
                // Sequential test
                QuicksortSequential instance = new QuicksortSequential(testData);
                instance.sort();
            }



        }

        private static void MiniTest()
        {
            int[] testData = { 1, 5, 7, 9, 2, 1, 3 };
            QuicksortSequential instance = new QuicksortSequential(testData);
            instance.sort();

            print(testData);
        }


        public static void print(int[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                Console.WriteLine(data[i]);
            }
        }
    }

    // This class demonstrates the use of TPL tasks to parallelize the Qucksort sorting algorithm
    // The following features are demonstrated:
    // 1. Forking parallel tasks
    // 2. Waiting for subtasks
    // 3. Using a "grainsize" to limit the number of parallel tasks
    class QuicksortParallelTasks
    {

        int grainsize;

        public QuicksortParallelTasks(int[] data)
        {
            this.data = data;

            // A oversimplified heuristic: 
            grainsize = (data.Length / Environment.ProcessorCount);
            Console.WriteLine("QuicksortParallelTasks ProcessorCount: " + Environment.ProcessorCount + " grainsize: " + grainsize + " datasize: " + data.Length);
        }

        public void sort()
        {
            Task[] topLEvelTasks = sortRecursively(0, data.Length - 1);
            Task.WaitAll(topLEvelTasks);
        }

        // Array of integers to be sorted
        private int[] data;


        Task[] sortRecursively(int l, int u)
        {
            //Console.WriteLine("sortRecursively: l: " + l + " u " + u + " thread " + Thread.CurrentThread.ManagedThreadId);

            if (l >= u)
            {
                return null;
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
            if (u - l > grainsize)
            {// parallel 

                Task t_left = Task.Factory.StartNew(() => sortRecursively(l, j - 1), TaskCreationOptions.AttachedToParent);
                Task t_right = Task.Factory.StartNew(() => sortRecursively(j + 1, u), TaskCreationOptions.AttachedToParent);
               // Console.WriteLine("Task forked: " + t_left.Id + " size " + ((j - l) + 1));
               // Console.WriteLine("Task forked: " + t_right.Id + " size " + ((u - j) + 1));
                //Task t_left = Task.Factory.StartNew(() => sortRecursively(l, j - 1));
                //Task t_right = Task.Factory.StartNew(() => sortRecursively(j + 1, u));
                // Waiting here until both subtasks have returned: TaskCreationOptions.AttachedToParent

                // ??? Warum genügt AttachToParent nicht???
                //Task.WaitAll(t_left, t_right);
                
                return new Task[] { t_left, t_right };
            }
            else
            {// sequential

                sortRecursively(l, j - 1);
                sortRecursively(j + 1, u);
                return null;
            }
        }


        private void Swap(int i, int j)
        {

            int tmp = data[i];
            data[i] = data[j];
            data[j] = tmp;
        }
    }
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
