using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TPLTaskProgramming.MatrixMult
{

    // WARNING this code can crash easily, as it allocates too many threads.

    // This class demonstrates the use of threads to parallelize the multiplication of two matrices.
    // The approach demonstrated here is very inefficient, as it utilizes too many threads. 
    // The use of this class is just to motivate a parallelization approach based on TPL Tasks (see MatrixMultTasks) 
    public  class MatrixMultThreads
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
            Thread[,] threads = new Thread[matrixA.NrOfRows(), matrixB.NrOfCols()];
            
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

                    // fork new thread
                    Thread thread = new Thread(() => RunMultiplication(retMatrix, copyRow, rowOfA, copyCol, colOfB));
                    thread.Start();
                    // store thread to join int later
                    threads[row, col] = thread;


                }
            }

            // Join all threads (i.e. wait for them to terminate)
            for (int row = 0; row < matrixA.NrOfRows(); ++row)
            {
                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    threads[row, col].Join();
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
