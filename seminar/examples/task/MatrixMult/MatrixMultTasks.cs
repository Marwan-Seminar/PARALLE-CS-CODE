using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPLTaskProgramming.MatrixMult
{
    
    // This class demonstrates the use of TPL tasks to parallelize the multiplication of two matrices.
    // This code differs only slightly from the thread based version MatrixMultThreads, but is is efficient and reliable.
    // The performance gain compared to the sequential version MatrixMultSequential is roughly proportional to the number
    // of processors.
    public  class MatrixMultTasks
    {   
        
        // Multiplies two matrices 
        // A: MXN, B: NXK. Resulting Matrix: MXK
        public static Matrix Multiply(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.dimOfRow() != matrixB.dimOfCol())
            {
                throw new Exception("Dimensions do not match: A: " + matrixA.dimOfRow() + " B: " + matrixB.dimOfCol());
            }
            int commonVectorLenght = matrixA.dimOfRow();

            Matrix retMatrix = new Matrix(matrixA.dimOfCol(), matrixB.dimOfRow());

            // This is the expensive loop!
            // Threre are M*K Vector-Multiplications to be performed
            Task[,] tasks = new Task[matrixA.NrOfRows(), matrixB.NrOfCols()];
            
            for (int row = 0; row < matrixA.NrOfRows(); ++row)
            {
                // Get row of A
                Matrix.Vector rowOfA = matrixA.GetRow(row);
                
                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    // Copys of the loop variables to be used in closures
                    int copyRow = row;
                    int copyCol = col;
                    
                    // Get column of B
                    Matrix.Vector colOfB = matrixB.GetColumn(col);

                    // fork new Task
                    Task task = new Task(() => RunMultiplication(retMatrix, copyRow, rowOfA, copyCol, colOfB));
                    task.Start();
                    // store Task to join it later
                    tasks[row, col] = task;


                }
            }

            // wait for all tasks to terminate)
            for (int row = 0; row < matrixA.NrOfRows(); ++row)
            {
                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    // probably inefficient
                    tasks[row, col].Wait();
                 }
            }
            return retMatrix;
        }

        private static void RunMultiplication(Matrix retMatrix, int row, Matrix.Vector rowOfA, int col, Matrix.Vector colOfB)
        {
            // Multiply them:
            int vectorProduct = Matrix.Vector.Multiply(rowOfA, colOfB);

            retMatrix.SetValueAt(row, col, vectorProduct);

        }
     
    }
}


