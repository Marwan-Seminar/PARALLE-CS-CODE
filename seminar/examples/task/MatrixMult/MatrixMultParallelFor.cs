using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPLTaskProgramming.MatrixMult
{
    
    // Parallel-For based version of Matrix-Multiplication
    public  class MatrixMultParallelFor
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

            Parallel.For(0, matrixA.NrOfRows(), rowIdx =>
            {
                // Get row of A
                Matrix.Vector rowOfA = matrixA.GetRow(rowIdx);

                Parallel.For(0, matrixB.NrOfCols(), colIdx =>
                {
                   
                    // Get column of B
                    Matrix.Vector colOfB = matrixB.GetColumn(colIdx);

                    // Multiply them:
                    int vectorProduct = Matrix.Vector.Multiply(rowOfA, colOfB);

                    retMatrix.SetValueAt(rowIdx, colIdx, vectorProduct);
                    //Console.WriteLine(" row" + rowIdx + " col " + colIdx + " thread: " + Thread.CurrentThread.ManagedThreadId + " task " + Task.CurrentId );

                });
            });

            return retMatrix;
        
        }

    }
}
