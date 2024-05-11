﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace SeminarParallelComputing.seminar.examples.MatrixMult.TestHelper
{
    public class TestMatricesMultSequential : TestMatrices
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

            Matrix multResult = MatrixMultSequential.Multiply(matrix_A, matrix_B);

            PrintMatrix(multResult);

        }

       

        public void TestPerformanceSeqentialMultyply()
        {
            // E.G. (1000 X 1000) * (1000 X 1000) takes approx. 50 Seconds on my Dual-Core machine
            Matrix matrix_A = FillMatrixRegular(1000, 1000);
            //PrintMatrix(matrix_A);
            Matrix matrix_B = FillMatrixRegular(1000, 1000);
            //PrintMatrix(matrix_B);


            Matrix multResult = MatrixMultSequential.Multiply(matrix_A, matrix_B);
            //PrintMatrix(multResult);

        }

    }
}

