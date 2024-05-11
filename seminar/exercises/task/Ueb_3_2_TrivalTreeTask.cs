// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.task
{

    /* Übung 3.2: Trivialen Baum aus Tasks bauen und warten
 
    Bauen Sie einen Baum aus Tasks (z.B. der Tiefe 5, und jeder Knoten hat zwei Kinder)

    Zeigen Sie, dass es passieren kann, dass das Programm sich beendet, bevor alle Tasks zuende gelaufen sind.

    Reparieren Sie das Verhalten, so dass das Programm genau dann zurückkehrt, wenn alle Tasks zuende gelaufen sind.

    */

    // Creates a binary tree of trivial tasks
    class Ueb_3_2_TrivalTreeTask
    {
        public static void MainTest(){

            int treeDepth = 4;

            new Ueb_3_2_TrivalTreeTask(treeDepth);

        }

        // Creaes binary tree of depth depth 
        Ueb_3_2_TrivalTreeTask(int depth)
        {
            TaskActionWaitStyle(depth);

            //TaskActionParentChildStyle(depth);

           // nötig, wenn nicht gewartet wird
           // Thread.Sleep(3000);
        }

        void TaskActionWaitStyle(int depth) {
            
            // end of recursion
            if (depth == 0)
            {
                return;
            }

            // Action is just a printout:
            Console.WriteLine("TaskActionWaitStyle: depth: " + depth + " TaskID: " + Task.CurrentId + " called in Thread: " + Thread.CurrentThread.ManagedThreadId);

            // fork left child and right child
            Task leftChild = Task.Factory.StartNew(() => TaskActionWaitStyle(depth - 1));
            Task rightChild = Task.Factory.StartNew(() => TaskActionWaitStyle(depth - 1));

            // Ohne diesen Aufruf endet das Programm abrupt nach dieser Methode, die Tasks laufen nicht alle an.
            Task.WaitAll(leftChild, rightChild);
            Console.WriteLine("TrivalTreeTask: depth: " + depth + " returns " );
        }


        // Beispiel für Verwendung von AttachedToParent
        void TaskActionParentChildStyle(int depth)
        {

            // end of recursion
            if (depth == 0)
            {
                return;
            }

            // Action is just a printout:
            Console.WriteLine("TaskActionParentChildStyle: depth: " + depth + "TaskID: " + Task.CurrentId + " called in Thread: " + Thread.CurrentThread.ManagedThreadId);

            // Was bedeutet AttachedToParent, ersetzt es das Wait?
            // Mir ist der Unterschied zwischen WaitAll und AttachedToParent nicht klar.

            // fork left child and right child
            Task leftChild = Task.Factory.StartNew(() => TaskActionParentChildStyle(depth - 1), TaskCreationOptions.AttachedToParent);
            Task rightChild = Task.Factory.StartNew(() => TaskActionParentChildStyle(depth - 1),  TaskCreationOptions.AttachedToParent);

            // Ohne diesen Aufruf endet das Programm abrupt nach dieser Methode, die Tasks laufen nicht alle an.
            Task.WaitAll(leftChild, rightChild);
            Console.WriteLine("TrivalTreeTask: depth: " + depth + " returns ");
        }
    }
}
