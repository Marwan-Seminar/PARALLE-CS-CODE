// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace SeminarParallelComputing.seminar.exercises.memorymodel
{

    /*
     * This file containes an implementation of PetersonsLock and classes to tests it called Ueb_4_2_TestPetersonsLock and StackPL. 
     * To make this program run correctly, uncomment the lines marked with 
     * MEMORY BARRIER    
     */

    /*
     * Peterson's Lock is a classic implementation of Mutex in Software.
     * 
     * This implementation of PetersonsLock is used to demonstrate the behavior of the memory model,
     * It shows that writes to a variable in one thread are not necessarily visible 
     * inside another thread immediately.
     * 
     * To make it work correctly, Memory-Barriers must be used in two places. These are marked with:
     * MEMORY BARRIER
     * 
     * 
     * Memory barrieres are explicitly available in the class Thread. Several other syntactic construcs are also guaranteed to imply
     * a memory barrier. Among these are locked blocks and CompareExchange:
     * lock (this) { }
     * int lockword = 1; Interlocked.CompareExchange(ref lockword, 0, 0); 
     */
    public class PetersonsLock
    {
        // volatile turn or valtile interested fields are NOT ENOUGH to fix this mutex implementation (other than in Java). An explicit memory barrier is required.
        bool[] interested = new bool[2];
        
        int turn = 0;
        
        // Acquires lock for one of the two processes
        public void acquireLock(int callingProccess)
        {

            int otherProccess = 1 - callingProccess;

            interested[callingProccess] = true;

            turn = otherProccess;

            // Here alternatives for 

            // 1. MEMORY BARRIER
            //Thread.MemoryBarrier(); 

            int loopCount = 0;
            while ((turn == otherProccess) && (interested[otherProccess] == true))
            {
                // 2. MEMORY BARRIER
                // Theoretically required, practically on my platform not required.
                // Thread.MemoryBarrier();

                // busy wait
                if (++loopCount % 100000000 == 0)
                {
                    //System.out.println("BusyWait: " + Thread.currentThread().getName());
                }
            }        
        }

        // Releasing the lock
        public void releaseLock(int callingProccess)
        {
            interested[callingProccess] = false;
            
        }

    }

    // TESTCODE: Using Petersons Lock to synchronize access to a stack datastructure that throws Errors if the locking does not work correctly.
    public class Ueb_4_2_TestPetersonsLock
    {

        public static void MainTest()
        {
            Ueb_4_2_TestPetersonsLock testInstance = new Ueb_4_2_TestPetersonsLock();
            // Error in unsynchronized case
            testInstance.pushAndPopMT();
        }

      

        // Simultaneously pushes and pops.
        void pushAndPopMT()
        {
            StackPL stack = new StackPL();


            new Thread(() =>
            {
                //for(int i = 0; i < 10000; ++i){
                int i = 0;
                while (true)
                {
                    i++;
                    // Synchronization with Peterson: This push thread identifies itself as "0"
                    // 1. CALL TO PETERSONLOCK
                    stack.mutexSoft.acquireLock(0);
                    stack.checkStackInvariant();
                    stack.push(i);
                    // 2. CALL TO PETERSONLOCK
                    stack.mutexSoft.releaseLock(0);
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
                        // 3. CALL TO PETERSONLOCK
                        stack.mutexSoft.acquireLock(1);
                        int popped = stack.pop();
                        stack.checkStackInvariant();
                        // 4. CALL TO PETERSONLOCK
                        stack.mutexSoft.releaseLock(1);
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
     * This class throws errors if not called with synchronization aroud the calls to  push(), pop() and  checkStackInvariant().
     * 
     * The code is synchronized with a PetersonsLock. This impl of PL can handle two threads only, 0 and 1. 
     * The pushing thread alwas identifies itself a 0, the other one as 1. This holds also for calls of 
     * the checkStackInvariant function, which needs the current thread ID as arg.
     * 
     * An alternative apporach to synchronize this class it to uncomment the three(1) lines marked with TODO 
     */
    public class StackPL
    {
        // Synchronization object that implements a spin lock.
        public PetersonsLock mutexSoft = new PetersonsLock();

        static int STACK_SIZE = 10000;

        int[] stackData = new int[STACK_SIZE];

        // Points to the topmost occupied slot, i.e is element of[-1, STACK_SIZE-1]
        int stackPointer = -1;

        // TODO
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

        // TODO
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

        // TODO 
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


