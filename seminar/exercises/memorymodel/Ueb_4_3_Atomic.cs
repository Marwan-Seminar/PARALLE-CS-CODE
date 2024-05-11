using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.memorymodel
{
    /*
     * Shows that increment operations are not atomic and demonstrates use of atomic increment methods.
     * This program increments a global variable from within two threads. Each thread increments it 5000 times.
     * A correct result would be a final value of 10000. The program yields an incorrect result, the sum is printed is below 1000.
     * To observe a correct result, based on incremnt with CMPXCHG apply the following two changes:
     * 1. comment the line markded with:
     * ERRONEOUS BEHAVIOR
     * 2. uncomment the line marked with
     * CORRECT BEHAVIOR
     */
    
    public class Ueb_4_3_Atomic
    {
        int globalVariable = 0;

        const int LOOP_COUNT = 50000;
        
        void incrementNonAtomic()
        {
            globalVariable++;
        }


        void incrementAtomic()
        {
            Interlocked.Increment(ref globalVariable);
        }

        void CMPXCHG()
        {
            int myThreadID = 4711;
            Interlocked.CompareExchange(ref globalVariable, myThreadID, 0);
        }

        // Shows how CAS can be used to implement i++ atomic
        void AtomicIncCMPXCHG()
        {
            int originalValue;
            int incrementedValue;
            do
            {
                originalValue = globalVariable;
                incrementedValue = originalValue + 1;
            } while (Interlocked.CompareExchange(ref globalVariable, incrementedValue, originalValue) != originalValue);
            
        }

        public static void TestMain()
        {
            Ueb_4_3_Atomic instance = new Ueb_4_3_Atomic();
           // instance.incrementNonAtomic();
            instance.testMultithread();
        }

        void threadTestMethod(){
            for (int i = 0; i < LOOP_COUNT; ++i)
            {
                // 1. ERRONEOUS BEHAVIOR: Uncomment to see non atomic increment
                incrementNonAtomic();

                // 2. CORRECT BEHAVIOR: Uncomment to see atomic inrcrement based on Interlocked.Increment
                //incrementAtomic();
                // 3. CORRECT BEHAVIOR Uncomment to see atomic inrcrement based on Interlocked.CompareExchange (yields a correct result)
                // AtomicIncCMPXCHG();
            }
        }

        void testMultithread()
        {
            Thread thread_1 = new Thread(threadTestMethod);
            Thread thread_2 = new Thread(threadTestMethod);
            thread_1.Start();
            thread_2.Start();
            thread_1.Join();
            thread_2.Join();

            Console.WriteLine("Expecte Value is " + (2 * LOOP_COUNT) + " Measured Value is: " + this.globalVariable);
        }


    }
}
