// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeminarParallelComputing.seminar.exercises.task;
using SeminarParallelComputing.seminar.exercises.thread.coordination;
using SeminarParallelComputing.seminar.examples.thread;
using SeminarParallelComputing.seminar.examples.task.Fibonacci;
using SeminarParallelComputing.seminar.examples.task.API;
using SeminarParallelComputing.seminar.examples.task.BlockingTasks;
using SeminarParallelComputing.seminar.examples.MatrixMult;
using SeminarParallelComputing.seminar.examples.task.Quicksort;
using SeminarParallelComputing.seminar.exercises.memorymodel;
using SeminarParallelComputing.seminar.exercises.thread.parallel;
using SeminarParallelComputing.seminar.exercises.thread.synchronization;
using SeminarParallelComputing.exercises.thread.parallel.u2_1_hello_world_threads.basis;
using SeminarParallelComputing.exercises.thread.parallel.u2_1_hello_world_threads.solution;
using SeminarParallelComputing.exercises.thread.parallel.u2_2_quicksort_threads.basis;
using SeminarParallelComputing.exercises.thread.parallel.u2_2_quicksort_threads.solution;
using SeminarParallelComputing.exercises.thread.parallel.u2_3_fibonacci_threads.basis;
using SeminarParallelComputing.exercises.thread.parallel.u2_3_fibonacci_threads.solution;
using SeminarParallelComputing.exercises.thread.coordination.u2_6_producer_consumer.basis;
using SeminarParallelComputing.exercises.thread.coordination.u2_6_producer_consumer.solution;
using SeminarParallelComputing.seminar.exercises.thread.synchronization.u2_4_stack_unsynchronized.basis;
using SeminarParallelComputing.seminar.exercises.thread.synchronization.u2_4_stack_unsynchronized.solution;
using SeminarParallelComputing.exercises.task.u3_4_fibonacci_tasks.basis;
using SeminarParallelComputing.exercises.task.u3_4_fibonacci_tasks.solution;

namespace SeminarParallelComputing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Seminar Parallel Programming, Marwan Abu-Khalil 2012");

        // UEBUNGEN

        // Threads Uebungen

            // Uebung 2.1: Hello World Threads
            //U_2_1_a_HelloWorldThreads_Basis.MainTest();
            // U_2_1_a_HelloWorldThreads_Solution.MainTest();
            //U_2_1_b_HelloWorldThreadsTakingTurns_Basis.MainTest();
            // U_2_1_b_HelloWorldThreadsTakingTurns_Solution.MainTest();
            
            // Uebung 2.2 Quicksort 
            //U2_2_QuicksortParallelThreads_Basis.MainQuicksort();
            //U2_2_QuicksortParallelThreads_Solution.MainQuicksort();
            
            // Uebung 2.3 Fibonacci Threads
            //FibonacciThreadMain_Basis.TestFiboMain();
            //FibonacciThreadMain_Solution.TestFiboMain();

            // Uebung 2.4 Stack Unsynchronized
            //Ueb_2_4_StackUnsynchronized_Basis.MainTest();
            //Ueb_2_4_StackUnsynchronized_Solution.MainTest();

            //Ueb_2_5_HelloWorldTakingTurnsMonitor.TestMain();
            
            // Uebung 2.6 Producer Consumer
            //U2_6_ProducerConsumerMonitor_Basis.TestMain();
            //U2_6_ProducerConsumerMonitor_Solution.TestMain();
            
        // Tasks Uebungen
            //Ueb_3_1_HelloWorldTasks.TestMain();
            //Ueb_3_2_TrivalTreeTask.MainTest();
            //Ueb_3_3_Rekursive_Baeume_Mit_Zeiten.TestMain();
            
            // Uebung 3.4 Fibonacci with TPL Tasks
            //FibonacciTPL_Basis.RunFibonacci();
            //FibonacciTPL_Solution.RunFibonacci();
            
            //Ueb_3_5_QuicksortMinimalTest.TestMain();
            //Ueb_3_6_ProducerConsumerTPL.TestMain();

         // Memorymodel Uebungen
            //Ueb_4_1_Volatile.TestMain();
            //Ueb_4_2_TestPetersonsLock.MainTest();
            //Ueb_4_3_Atomic.TestMain();
            //Ueb_4_5_TestSpinlockCAS.MainTest();
        
        // Examples
            //MonitorUsage.TestBehavior();
            //FibonacciTPL.RunFibonacci();
            //CreateAndStartTask.TestMain();
            //AttachParentDemo.TestMain();
            //BlockingTaskTrivial.TestMain();
            //IOAction.TestMain();
            //TestMatrixMultiplication.TestMain();
            //QuicksortTPLTestMain.TestMain();

        //LIFE-CODING
            //Ueb_4_2_TestPetersonsLock.MainTest();
            //FibonacciThreadMain.TestFiboMain();
            FibonacciTPL_Solution.RunFibonacci();
            //Ueb_2_4_StackUnsynchronized_Basis.MainTest();
            //Ueb_2_4_StackUnsynchronized_Solution.MainTest();
            //AttachParentDemo.TestMain();
            //Ueb_4_1_Volatile.TestMain();


        }
    }
}
