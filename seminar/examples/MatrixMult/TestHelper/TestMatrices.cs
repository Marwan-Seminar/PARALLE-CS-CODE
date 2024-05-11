using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SeminarParallelComputing.seminar.examples.MatrixMult.TestHelper
{
    public class TestMatrices
    {
        public void TestPrintVector()
        {

            Console.WriteLine("TestPrintVector");



            Matrix matrix = FillMatrixRegular(2, 4);
            Console.WriteLine("Row 1");
            Matrix.Vector row_1 = matrix.GetVector(Matrix.Vector.Direction.ROW, 1);
            PrintVector(row_1);

            Console.WriteLine("Col 2");
            Matrix.Vector col_2 = matrix.GetVector(Matrix.Vector.Direction.COL, 2);
            PrintVector(col_2);


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

        // Should throw an Exception
        void TestOutOfBounds()
        {
            Matrix matrix = new Matrix(20, 30);

            PrintMatrix(matrix);

            Matrix.Vector vector = matrix.GetVector(Matrix.Vector.Direction.ROW, 20);

            Console.WriteLine("Vector");

            PrintVector(vector);
            vector.GetValueAt(0);

        }

        public void PrintMatrix(Matrix matrix)
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

        public void PrintVector(Matrix.Vector vector)
        {
            for (int idx = 0; idx < vector.GetSize(); ++idx)
            {
                if (vector.GetDirection() == Matrix.Vector.Direction.ROW)
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

        public void PrintVector(Matrix matrix, Matrix.Vector.Direction direction, int position)
        {
            Matrix.Vector vector = matrix.GetVector(direction, position);
            PrintVector(vector);

        }

        // 1 2 3
        // 4 5 6
        // ......
        public Matrix FillMatrixRegular(int nrOfRows, int nrOfCols)
        {
            Matrix matrix = new Matrix(nrOfRows, nrOfCols);
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
