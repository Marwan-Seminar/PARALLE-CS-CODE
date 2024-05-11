using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace SeminarParallelComputing.seminar.examples.MatrixMult.TestHelper
{
    
    public class TestMatricesMultThreads : TestMatrices
    {
        public void TestMultiplyMatrix()
        {

            Console.WriteLine("TestMultiplyMatrix");
            /* 
             should yield in 2X 2 case:      
             7 10 
             15 22 
            
             in 4X4 case:
             
             90 100 110 120 

             202 228 254 280 

             314 356 398 440 

             426 484 542 600 
             
             
            */

            Matrix matrix_A = FillMatrixRegular(4, 4);
            Matrix matrix_B = FillMatrixRegular(4, 4);

            Matrix multResult = MatrixMultThreads.Multiply(matrix_A, matrix_B);

            PrintMatrix(multResult);

        }

        // DANGEROUS: Can crash your Operating-System, creates way too many threads.   
        public void TestPerformanceThreadlMultyply()
        {
            // E.G. (1000 X 1000) * (1000 X 1000) takes approx. 50 Seconds on my Dual-Core machine
            Matrix matrix_A = FillMatrixRegular(1000, 1000);
            //PrintMatrix(matrix_A);
            Matrix matrix_B = FillMatrixRegular(1000, 1000);
            //PrintMatrix(matrix_B);


            Matrix multResult = MatrixMultThreads.Multiply(matrix_A, matrix_B);
            //PrintMatrix(multResult);

        }

        // Tests that should throw an Exception

        void TestMultiplyWrongDimensions()
        {

            Console.WriteLine("TestMultiplyMatrix");

            Matrix matrix_A = FillMatrixRegular(2, 2);
            PrintMatrix(matrix_A);
            Matrix matrix_B = FillMatrixRegular(3, 4);
            PrintMatrix(matrix_B);

            Matrix multResult = MatrixMultSequential.Multiply(matrix_A, matrix_B);

        }

        

        

    }
}

