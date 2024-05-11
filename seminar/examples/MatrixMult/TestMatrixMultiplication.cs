// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SeminarParallelComputing.seminar.examples.MatrixMult.TestHelper;

namespace SeminarParallelComputing.seminar.examples.MatrixMult
{
    class TestMatrixMultiplication
    {
        public static void TestMain()
        {
        

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //TasksTest();

            PoolTest();

           //RecursiveDecompositionTest();

           
            watch.Stop();

            //ThreadsTest();

            Console.WriteLine("Time: " + watch.Elapsed);
        }

      

        // Good
        private static void TasksTest()
        {
            Console.WriteLine("TasksTest Matrix Multiply");
            TestMatricesMultTasks tasks = new TestMatricesMultTasks();

            tasks.TestPerformanceTaskslMultyply();
        }


        // Good
        private static void PoolTest()
        {
            Console.WriteLine("Pool Matrix Multiply");
            TestMatricesMultThreadPool pool = new TestMatricesMultThreadPool();

            pool.TestPerformanceTaskslMultyply();
            //pool.TestMultiplyMatrix();
        }

        static void RecursiveDecompositionTest()
        {
            TestMatricesRecursiveDecomposition recursiveDecomp = new TestMatricesRecursiveDecomposition();
            recursiveDecomp.TestMultiplicationRecursivePerformance();
        }
        // Bad
        private static void ThreadsTest()
        {
            TestMatricesMultThreads threadMult = new TestMatricesMultThreads();

            threadMult.TestPerformanceThreadlMultyply();
        }
    }
}
