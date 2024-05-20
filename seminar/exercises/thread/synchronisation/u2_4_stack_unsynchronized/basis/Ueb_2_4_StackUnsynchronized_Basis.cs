using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.thread.synchronization.u2_4_stack_unsynchronized.basis
{

    /*

        Ü 2.4 Ein unkorrekt synchronisiertes Programm durch Synchronisation reparieren. (Stack).
	
        a)	Schreiben Sie ein Programm, bei dem eine Datenstruktur von zwei nebenläufigen Threads korrumpiert wird. Z.B. ein Stack, 
            der einen Pointer auf das oberste Element hat (oder einen Zähler, der definiert, welches das oberste Element ist).  
            Zeigen sie, dass das Programm fehlerhafte Zustände erzeugt. 
     
        b)	Synchronisieren sie das Programm korrekt und zeigen Sie, dass es nun korrekt läuft.
	
        Lernziel: Risiken der unkorrekten Synchronisation verstehen, Gegenmaßnahmen finden.

        Hinweis: Lösungen können  basieren auf:
        - Die Klasee Stack implementiert einen Stack, der Fehlerzustände liefert, wenn er unsynchronisiert benutzt wird, 
        - die Klasse Ueb_2_4_StackUnsynchronized enthält den Testcode
            - Stack.push(): Befüllt den Stack
            - Stack.pop(): Entnimmt aus dem Stack
            - Stack.checkStackInvariant(): Prüft die Konsistenz der internen Datenstruktur und wirft eine Exception im Falle von Inkonsistenz
     
        Synchronisation kann basieren auf:
            1. lock(ein geeingnets Objekt){ ...}
            oder 
            2. [MethodImplAttribute(MethodImplOptions.Synchronized)]

     */



    /*
     * This implementation of a  Stack datastructure creates inconsistent states and Errors if not synchronized correctly
     */
    class Stack
    {

        const int STACK_SIZE = 1000000;

        // Stack is represented as array. Uninitializes stack positions are marked "null"
        Nullable<int>[] stackData = new Nullable<int>[STACK_SIZE];

        // Points to the topmost occupied slot, i.e. stackPointer is element of[-1, STACK_SIZE-1]
        int stackPointer = -1;

     
        public bool push(int pushValue)
        {
          
            if (stackPointer >= STACK_SIZE - 1)
            {
                // stack is full
                return false;
            }

            // push element
            stackData[stackPointer + 1] = pushValue;

            // increment stackpointer
            stackPointer++;

            return true;
        }
        
      
        public bool pop(ref int popValue)
        {
           
            if (stackPointer <= -1)
            {
                // stack is empty
                return false;
            }

            if (stackData[stackPointer] == null)
            {
                throw new Exception("Illegal stack state: null at stackPointer " + stackPointer);
            }

            popValue = (int)stackData[stackPointer];
            stackData[stackPointer] = null;

            // decrement stackpointer
            stackPointer--;

            return true;
        }
        

        public void checkStackInvariant()
        {
            
            if (stackPointer < -1 || stackPointer >= STACK_SIZE)
            {
                throw new Exception("Invariant violated: range: " + stackPointer);
            }

            if (stackPointer > -1 && stackData[stackPointer] == null)
            {
                throw new Exception("Invariant violated: null at: " + stackPointer);
            }
           
        }


    }


    /*
    * This class contains the test code for the stack datastructure. 
    * It starts threads that push to and pop from the stack
    */
    public class Ueb_2_4_StackUnsynchronized_Basis
    {

        public static void MainTest()
        {
            Ueb_2_4_StackUnsynchronized_Basis testInstance = new Ueb_2_4_StackUnsynchronized_Basis();

            // Error in unsynchronized case
            testInstance.pudhAndPopMT();

            // Error in unsynchronized case
            //testInstance.fillAndCheckMT();


            // No Errors, as single threaded calls
            //testInstance.fillAndCheckST();
            //testInstance.emptyAndCheckST();


        }



        // Simultaneously pushes and pops.
        void pudhAndPopMT()
        {
            // THE central Stack datastructure
            Stack stack = new Stack();

            // Push-Thread
            Thread pushThread = new Thread(() =>
            {

                // Push onto Stack in infinite Loop
                int i = 0;
                while (true)
                {
                    i++;
                    stack.checkStackInvariant();
                    bool success = stack.push(i);
                    if (success &&i % 1000000 == 0)
                    {
                        Console.WriteLine("Pushed " + i);
                    }
                }

                // finite variant
                /*
                    for (int i = 0; i < 100000; ++i)
                    {
                       stack.checkStackInvariant();
                       stack.push(i);
                    }
                */

            });

            // Pop-Thread
            Thread popThread = new Thread(() =>
            {
                // Pop off from Stack in an infinite loop
                while (true)
                {
                    int poppedValue = 0;
                    bool valid = stack.pop(ref poppedValue);
                    stack.checkStackInvariant();
                    if (valid && poppedValue % 1000000 == 0)
                    {
                        Console.WriteLine("Popped: " + poppedValue);
                    }

                }
                /*
                for (int i = 0; i < 100000; ++i)
                {                    
                    int poppedValue = 0;
                    stack.pop(ref poppedValue);     
                }*/
            });

            // Starting the two threads
            pushThread.Start();
            popThread.Start();

            // Waiting for the threads to complete.
            pushThread.Join();
            popThread.Join();

            // if this line is reached, test ended without exception.
            Console.WriteLine("TEST-RESULT pudhAndPopMT(): SUCCESS");
        }

        /// ////////////////////////////////////// VARIANTS OF THE TEST CODE //////////////////////////
        
        void fillAndCheckST()
        {

            Stack stack = new Stack();
            const int FILL_SIZE = 300;
            for (int i = 0; i < FILL_SIZE; ++i)
            {
                stack.push(i);
                stack.checkStackInvariant();
            }

            Console.WriteLine("Stack filled with " + FILL_SIZE + " values ");

            Console.WriteLine("TEST-RESULT: SUCCESS");
        }

        void emptyAndCheckST()
        {

            Stack stack = new Stack();

            for (int i = 0; i < 300; ++i)
            {
                stack.push(i);
                stack.checkStackInvariant();
            }

            for (int i = 0; i < 400; ++i)
            {
                int poppedValue = 0;
                bool stackEmpty = stack.pop(ref poppedValue);
                stack.checkStackInvariant();
                Console.WriteLine(stackEmpty ? ("Popped:" + poppedValue) : "Stack Empty: pop() failed");
            }

            Console.WriteLine("TEST-RESULT: SUCCESS");
        }

        // Pushes simultaneously in 2 Threads
        void fillAndCheckMT()
        {

            Stack stack = new Stack();
            int NR_OF_PUSH_THREADS = 2;

            for (int threadCout = 0; threadCout < NR_OF_PUSH_THREADS; threadCout++)
            {
                new Thread(() =>
                {
                    int i = 0;
                    while (true)
                    {
                        i++;
                        stack.checkStackInvariant();
                        stack.push(i);
                        stack.checkStackInvariant();

                    }
                }).Start();
            }
        }


    }

}
