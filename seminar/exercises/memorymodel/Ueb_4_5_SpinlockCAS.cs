using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.memorymodel
{
    /* This file contains a class SpinlockCAS that implements a spinlock based on CompareExchange (aka CMPXCHG). 
     * It also contains testcode for the correctness of locking functionality. 
     * 
     * To see erroneous behavior comment the while-loop in SpinlockCAS, marked with 
     * COMMENT THE FOLLOWING WHILE LOOP..
     */

    // This class implements a spinlock bases on Interlocked.CompareExchange
    public class SpinlockCAS
    {
        int LOCKWORD;

        public void acquire()
        {
            int myThreadID = Thread.CurrentThread.ManagedThreadId;

           // COMMENT THE FOLLOWING WHILE LOOP TO SEE ERRONEOUS BEHAVIOR 
           while (Interlocked.CompareExchange(ref LOCKWORD, myThreadID, 0) != 0)
            {
                // Spin
            }
        }

        public void release()
        {
            LOCKWORD = 0;
        }
    }



    // Test Code using a  Stack-Implementation that throws an Exception in case of unsynchronized access.
    // It is ssing SpinlockCAS to synchronize access to that stack datastructure that would become inconsistent without locking.
    public class Ueb_4_5_TestSpinlockCAS
    {

        public static void MainTest()
        {
            Ueb_4_5_TestSpinlockCAS testInstance = new Ueb_4_5_TestSpinlockCAS();
            // Error in unsynchronized case
            testInstance.pushAndPopMT();

        }

        // Simultaneously pushes and pops.
        void pushAndPopMT()
        {
            Stack_With_Synch_Check stack = new Stack_With_Synch_Check();


            new Thread(() =>
            {
                //for(int i = 0; i < 10000; ++i){
                int i = 0;
                while (true)
                {
                    i++;
                    // Synchronization with Peterson: This push thread identifies itself as "0"
                    // 1. COMMENT THE FOLLOWING LINE TO SEE ERRONEOUS BEHAVIOR
                    stack.spinlockCAS.acquire();
                    stack.checkStackInvariant();
                    stack.push(i);
                    // 2. COMMENT THE FOLLOWING LINE TO SEE ERRONEOUS BEHAVIOR
                    stack.spinlockCAS.release();
                    if (i % 1000000 == 0)
                    {
                        Console.WriteLine("Pushed " + i);
                    }
                }
            }
                ).Start();

            new Thread(
                () =>
                {
                    //for(int i = 0; i < 10000; ++i){
                    while (true)
                    {
                        // Synchronization with Peterson: This push thread identifies itself as "1"
                        // 3. COMMENT THE FOLLOWING LINE TO SEE ERRONEOUS BEHAVIOR
                        stack.spinlockCAS.acquire();
                        int popped = stack.pop();
                        stack.checkStackInvariant();
                        // 4. COMMENT THE FOLLOWING LINE TO SEE ERRONEOUS BEHAVIOR
                        stack.spinlockCAS.release();
                        if (popped % 1000000 == 0)
                        {
                            Console.WriteLine("Popped: " + popped);
                        }

                    }
                }
            ).Start();
        }
    }


    /*
     *  This Stack implementation is adapted from:
     *  SeminarParallelComputingDEVELOP/src/seminar/exercises/thread/synchronization/Stack.java
     *  
     * This class creates inconsistent states and Errors if not synchronized.
     * To make the program correct, uncomment 3 (three!) synchronized statements marked with "TODO"
     * 
     * The code is synchronized with a Spinlock. This impl of PL can handle two threads only, 0 and 1. 
     * The pushing thread alwas identifies itself a 0, the other one as 1. This holds also for calls of 
     * the checkStackInvariant function, which needs the current thread ID as arg.
     */
    public class Stack_With_Synch_Check
    {
        // Synchronization object that implements a spin lock.

        public SpinlockCAS spinlockCAS = new SpinlockCAS();

        static int STACK_SIZE = 10000;

        int[] stackData = new int[STACK_SIZE];

        // Points to the topmost occupied slot, i.e is element of[-1, STACK_SIZE-1]
        int stackPointer = -1;

        // If NOT synchronized, mutltithreaded access violates the stack invariant
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]
        public bool push(int arg)
        {

            if (stackPointer >= STACK_SIZE - 1)
            {
                // stack is full
                return false;
            }

            // push element
            stackData[stackPointer + 1] = arg;

            // increment stackpointer
            stackPointer++;

            return true;
        }


        // If NOT synchronized, mutltithreaded access violates the stack invariant
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]
        public int pop()
        {

            if (stackPointer <= -1)
            {
                // stack is empty
                return -1;
            }

            int elementToPop = stackData[stackPointer];
            stackData[stackPointer] = -1;

            // decrement stackpointer
            stackPointer--;

            return elementToPop;
        }


        // If NOT synchronized, mutltithreaded access violates the stack invariant
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]
        public void checkStackInvariant()
        {
            if (stackPointer < -1 || stackPointer >= STACK_SIZE)
            {
                throw new Exception("Invariant violated: range: " + stackPointer);
            }

            if (stackPointer > -1 && stackData[stackPointer] == -1)
            {
                throw new Exception("Invariant violated: null at: " + stackPointer);
            }
        }
    }

}