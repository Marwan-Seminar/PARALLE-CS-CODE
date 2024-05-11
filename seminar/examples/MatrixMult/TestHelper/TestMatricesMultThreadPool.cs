using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace SeminarParallelComputing.seminar.examples.MatrixMult.TestHelper
{
    
    public class TestMatricesMultParallelFor : TestMatrices
    {
        public void TestMultiplyMatrix()
        {

            Console.WriteLine("TestMultiplyMatrix");
            /* 
             should yield:      
             7 10 
             15 22 
            */

            Matrix matrix_A = FillMatrixRegular(2, 2);
            Matrix matrix_B = FillMatrixRegular(2, 2);

            Matrix multResult = MatrixMultParallelFor.Multiply(matrix_A, matrix_B);

            PrintMatrix(multResult);

        }

       
        public void TestPerformanceTaskslMultyply()
        {
            // E.G. (1000 X 1000) * (1000 X 1000) takes approx. 50 Seconds on my Dual-Core machine
            Matrix matrix_A = FillMatrixRegular(1000, 1000);
            //PrintMatrix(matrix_A);
            Matrix matrix_B = FillMatrixRegular(1000, 1000);
            //PrintMatrix(matrix_B);


            Matrix multResult = MatrixMultParallelFor.Multiply(matrix_A, matrix_B);
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

            Matrix multResult = MatrixMultParallelFor.Multiply(matrix_A, matrix_B);

        }

        

        

    }
}

