using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SeminarParallelComputing.seminar.examples.MatrixMult.TestHelper
{
    public class TestMatricesRecursiveDecomposition
    {

        /* expected result in 4 X 4 case
        90 100 110 120 

        202 228 254 280 

        314 356 398 440 

        426 484 542 600
        */

        public void TestMultiplicationRecursive()
        {
            MatrixDecomposable matrixA = FillMatrixRegular(4, 4);
            MatrixDecomposable matrixB = FillMatrixRegular(4, 4);

            MatrixDecomposable multMatrix = MatrixMultRecursiveDecomposition.Multiply(matrixA, matrixB);

            Console.WriteLine("TestMultiplicationRecursive");

            PrintMatrix(multMatrix);

        }

        // E.g. dim = 1024, Threshold 128 29 seconds on my Dual-Core
        public void TestMultiplicationRecursivePerformance()
        {
            int dim = 1024;
            MatrixDecomposable matrixA = FillMatrixRegular(dim, dim);
            MatrixDecomposable matrixB = FillMatrixRegular(dim, dim);

            MatrixDecomposable multMatrix = MatrixMultRecursiveDecomposition.Multiply(matrixA, matrixB);

            Console.WriteLine("TestMultiplicationRecursive");

            //PrintMatrix(multMatrix);
            // Print Diagonal:
            for(int i = 0; i< dim; ++ i){
                Console.WriteLine(multMatrix.GetValueAt(i,i));
            }

        }

        public void TestSequentialAdd()
        {
            MatrixDecomposable matrixA = FillMatrixRegular(2, 4);
            MatrixDecomposable matrixB = FillMatrixRegular(2, 4);

            MatrixDecomposable sumMatrix = MatrixDecomposable.AddSequential(matrixA, matrixB);
        
            Console.WriteLine("Matrix Sum");

            PrintMatrix(sumMatrix);

        }

        // Sequential addidion even for large matirces is so fast, that parallelization does not make sense
        public void TestSequentialAddPerformance()
        {
            int dimM = 1000;
            int dimN = 10000;
            MatrixDecomposable matrixA = FillMatrixRegular(dimM, dimN);
            MatrixDecomposable matrixB = FillMatrixRegular(dimM, dimN);

            MatrixDecomposable sumMatrix = MatrixDecomposable.AddSequential(matrixA, matrixB);


            Console.WriteLine(" Last Value: " + sumMatrix.GetValueAt(dimM - 1, dimN - 1));
            
        }

        // This is much slower than seqential addition! 
        public void TestParallelRecursiveAddPerformance()
        {
            int dimM = 1000;
            int dimN = 10000;
            MatrixDecomposable matrixA = FillMatrixRegular(dimM, dimN);
            MatrixDecomposable matrixB = FillMatrixRegular(dimM, dimN);

            MatrixDecomposable sumMatrix = MatrixDecomposable.AddParallelRecursiveDecomposition(matrixA, matrixB);


            Console.WriteLine(" Last Value: " + sumMatrix.GetValueAt(dimM - 1, dimN - 1));

        }

        // Adds two matrices based on recursive decomposition into small tasks
        void TestParallelAdd()
        {
            MatrixDecomposable matrixA = FillMatrixRegular(1000, 1000);
            MatrixDecomposable matrixB = FillMatrixRegular(1000, 1000);

            MatrixDecomposable sumMatrix = MatrixDecomposable.AddParallelRecursiveDecomposition(matrixA, matrixB);

            //Console.WriteLine("Matrix Sum");

           // PrintMatrix(sumMatrix);

        }

        public void TestPrintVector()
        {

            Console.WriteLine("TestPrintVector");



            MatrixDecomposable matrix = FillMatrixRegular(2, 4);
            Console.WriteLine("Row 1");
            MatrixDecomposable.Vector row_1 = matrix.GetVector(MatrixDecomposable.Vector.Direction.ROW, 1);
            PrintVector(row_1);

            Console.WriteLine("Col 2");
            MatrixDecomposable.Vector col_2 = matrix.GetVector(MatrixDecomposable.Vector.Direction.COL, 2);
            PrintVector(col_2);


        }

        public void TestPrintMatrix()
        {

            Console.WriteLine("TestPrintMatrix");



            MatrixDecomposable matrix = FillMatrixRegular(8, 8);
          
            MatrixDecomposable[,] splitMatrices = matrix.Split();

            Console.WriteLine("Matrix");

            PrintMatrix(matrix);

            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Console.WriteLine("Matrix: " + i + " " + j);
                    PrintMatrix(splitMatrices[i, j]);

                }
            }
                
        }


        // Tests that should throw an Exception
        void TestMultiplyWrongDimensions()
        {

            Console.WriteLine("TestMultiplyMatrix");

            MatrixDecomposable matrix_A = FillMatrixRegular(2, 2);
            PrintMatrix(matrix_A);
            MatrixDecomposable matrix_B = FillMatrixRegular(3, 4);
            PrintMatrix(matrix_B);

            // TODO Matrix multResult = MatrixMultSequential.Multiply(matrix_A, matrix_B);

        }

        // Should throw an Exception
        void TestOutOfBounds()
        {
            MatrixDecomposable matrix = new MatrixDecomposable(20, 30);

            PrintMatrix(matrix);

            MatrixDecomposable.Vector vector = matrix.GetVector(MatrixDecomposable.Vector.Direction.ROW, 20);

            Console.WriteLine("Vector");

            PrintVector(vector);
            vector.GetValueAt(0);

        }

        public void PrintMatrix(MatrixDecomposable matrix)
        {
            for (int row = 0; row < matrix.NrOfRows(); ++row)
            {
                for (int col = 0; col < matrix.NrOfCols(); col++)
                {
                    Console.Write(matrix.GetValueAt(row, col) + " ");
                }
                Console.Write("\n\r");
            }
        }

        public void PrintVector(MatrixDecomposable.Vector vector)
        {
            for (int idx = 0; idx < vector.GetSize(); ++idx)
            {
                if (vector.GetDirection() == MatrixDecomposable.Vector.Direction.ROW)
                {
                    Console.Write(vector.GetValueAt(idx) + " ");
                }
                else
                {
                    Console.WriteLine(vector.GetValueAt(idx));
                }
            }
            Console.WriteLine("");
        }

        public void PrintVector(MatrixDecomposable matrix, MatrixDecomposable.Vector.Direction direction, int position)
        {
            MatrixDecomposable.Vector vector = matrix.GetVector(direction, position);
            PrintVector(vector);

        }

        // 1 2 3
        // 4 5 6
        // ......
        public MatrixDecomposable FillMatrixRegular(int nrOfRows, int nrOfCols)
        {
            MatrixDecomposable matrix = new MatrixDecomposable(nrOfRows, nrOfCols);
            int value = 0;
            for (int row = 0; row < matrix.NrOfRows(); ++row)
            {
                for (int col = 0; col < matrix.NrOfCols(); col++)
                {

                    matrix.SetValueAt(row, col, ++value);
                }
            }
            return matrix;
        }

    }
}
