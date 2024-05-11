// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.task
{
    /* Übung 3.5: Quicksort mit Tasks parallelisieren
    
    a) Parallelisieren Sie eine Algorithmus mit Hilfe von Tasks, z.B. Quicksort. (Code für den sequentiellen Algorithmus finden Sie in den Vortragsfolien)

    b) Zeigen Sie, dass Sie einen Performance-Vorteil gegenüber dem sequentiellen Algorithmus erzielen. 
    
     */

    // Mini VErsion of TPL Quciksort: No threashold for sequential sort
    public class Ueb_3_5_QuicksortMinimalTest
    {

        public static void TestMain()
        {


            MiniTest();
            
         }

        private static void MiniTest()
        {
            int[] testData = { 1, 5, 7, 9, 2, 1, 3, 9, 12, -6, 8, 13, 46, 6, 99, 106, 5, 205, 5, 6, 87, 1460, 1, 5, 7, 9, 2, 1, 3, 9, 12, -6, 8, 13, 46, 6, 99, 106, 5, 205, 5, 6, 87, 1460 };
            QuicksortParallelTasksMini instance = new QuicksortParallelTasksMini(testData);
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
    class QuicksortParallelTasksMini
    {
        public QuicksortParallelTasksMini(int[] data)
        {
            this.data = data;
        }

        public void sort()
        {
           Task[] topLevelTasks = sortRecursively(0, data.Length - 1);
           Task.WaitAll(topLevelTasks);
        }

        // Array of integers to be sorted
        private int[] data;

        Task[] sortRecursively(int l, int u)
        {  
            if (l >= u) {  return null; }
            int pivotIdx = l;
            int i = l;
            int j = u + 1;

            // Partitions subrange [l,u] of the data array into two sub-arrays according to a pivot element
            while (true) {
                do {
                    i++;
                } while (i <= u && data[i] <= data[pivotIdx]);
                do {
                    j--;
                } while (data[j] > data[pivotIdx]);

                if (i > j) break;
                Swap(i, j);
            }
            Swap(pivotIdx, j);

            Console.WriteLine("Quicksort-Task Range: [" + l + "," + u + "]\t"  + " TaskID: " + Task.CurrentId + " called in Thread: " + Thread.CurrentThread.ManagedThreadId);
            
            // AttachToPArent is required, as otherwise the enclosing code dies not know, when the algorithm is finised. 
            //It may acces sthe result-array too early and print unordered results
            // Fork tasks with AttachedToParent (Parent-Child not necessary for the algorithm, just to make sure that program waits for all tasks)
            Task t_left = Task.Factory.StartNew(() => sortRecursively(l, j - 1), TaskCreationOptions.AttachedToParent);
            Task t_right = Task.Factory.StartNew(() => sortRecursively(j + 1, u), TaskCreationOptions.AttachedToParent);

            // DOES NOT WORK, see above
            //Task t_left = Task.Factory.StartNew(() => sortRecursively(l, j - 1));
            //Task t_right = Task.Factory.StartNew(() => sortRecursively(j + 1, u));

            return new Task[] { t_left, t_right };
        }

        private void Swap(int i, int j)
        {

            int tmp = data[i];
            data[i] = data[j];
            data[j] = tmp;
        }
    }
    



}
