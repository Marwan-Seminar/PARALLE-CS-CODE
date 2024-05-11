using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPLTaskProgramming.MatrixMult
{
    public class MatrixMultSequential
    {
        // Multiplies two matrices
        // A: MXN, B: NXK. Resulting Matrix: MXK
        public static Matrix Multiply(Matrix matrixA, Matrix matrixB){
            if(matrixA.dimOfRow() != matrixB.dimOfCol()){
                throw new Exception("Dimensions do not match: A: " + matrixA.dimOfRow() + " B: " + matrixB.dimOfCol());
            }
            int commonVectorLenght = matrixA.dimOfRow();

            Matrix retMatrix = new Matrix(matrixA.dimOfCol(), matrixB.dimOfRow());
            
            // This is the expensive loop!
            for(int row = 0; row < matrixA.NrOfRows(); ++row){
                
                // Get row of A
                Matrix.Vector rowOfA = matrixA.GetRow(row);

                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    // Get column of B
                    Matrix.Vector colOfB = matrixB.GetColumn(col);

                    // Multiply them:
                    int vectorProduct = Matrix.Vector.Multiply(rowOfA, colOfB);

                    retMatrix.SetValueAt(row, col, vectorProduct);
                }
            }

            return retMatrix;
        }
        
    }
}
