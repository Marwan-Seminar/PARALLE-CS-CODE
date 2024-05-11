// Copyright Marwan Abu-Khalil 2012


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeminarParallelComputing.seminar.examples.MatrixMult
{
   

    // Represents an MXN matrix
    public  class Matrix
    {

        public Matrix(int M, int N){
            dimM = M;
            dimN = N;

            data = new int[dimM,dimN];

        }
           
        // Array holding the matrix data
        int[,] data;

        // Dimension N is defining the number of rows M (i.e. each column has dimM elements)
        int dimM;

        // defines the number of colums N (i.e. each row has dimCols elements)
        int dimN;

        public int NrOfCols()
        {
            return dimN;
        }
        public int NrOfRows()
        {
            return dimM;
        }
        public int dimOfCol()
        {
            return dimM;
        }
        public int dimOfRow()
        {
            return dimN;
        }

        public void SetValueAt(int row, int colum, int value)
        {
            data[row, colum] = value;
        }
        public int GetValueAt(int row, int col)
        {
            return data[row, col];
        }

        /*
        // Multiplies two matrices
        // A: MXN, B: NXK. Resulting Matrix: MXK
        public static Matrix Multiply(Matrix matrixA, Matrix matrixB){
            if(matrixA.dimN != matrixB.dimM){
                throw new Exception("Dimensions do not match: A: " + matrixA.dimN + " B: " + matrixB.dimM);
            }
            int commonVectorLenght = matrixA.dimN;

            Matrix retMatrix = new Matrix(matrixA.dimM, matrixB.dimN);
            
            // This is the expensive loop!
            for(int row = 0; row < matrixA.NrOfRows(); ++row){
                
                // Get row of A
                Vector rowOfA = matrixA.GetRow(row);

                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    // Get column of B
                    Vector colOfB = matrixB.GetColumn(col);

                    // Multiply them:
                    int vectorProduct = Vector.Multiply(rowOfA, colOfB);

                    retMatrix.SetValueAt(row, col, vectorProduct);
                }
            }

            return retMatrix;
        }
        */

        // Gets a row or a column
        public Vector GetVector(Vector.Direction direction, int position)
        {
            if (direction == Vector.Direction.ROW)
            {
                return GetRow(position);
            }
            else
            {
                return GetColumn(position);
            }
        }

        // Returns a Vector representing row rowIdx
        public Vector GetRow(int rowIdx){
            return new Vector(this, Vector.Direction.ROW, rowIdx);
        }

        // Returns a Vector representing columns colIdx
        public Vector GetColumn(int colIdx)
        {
            return new Vector(this, Vector.Direction.COL, colIdx);
        }

        // represents a Vector backed by a Matrix, used to access rows and columns of this Matrix
        public class Vector
        {
            Matrix matrix;
            int position;
            Direction direction;

            public enum Direction { ROW, COL }

            // Direction definies if it is a row or a column, position defines, where in the backing matrix the Vector is located
            public Vector(Matrix backingMatrix, Direction dir, int position)
            {
                this.matrix = backingMatrix;
                this.direction = dir;
                this.position = position;
            }

            public int GetSize(){
                return direction == Direction.ROW? matrix.dimOfRow() : matrix.dimOfCol(); 
            }

            public Direction GetDirection()
            {
                return direction;
            }

            public int GetValueAt(int i)
            {
                if (direction == Direction.ROW)
                {
                    return matrix.data[position, i];
                }
                else if (direction == Direction.COL)
                {
                    return matrix.data[i, position];
                }
                else
                {
                    throw new Exception(" Impossible direction:"+ direction);
                }
            }
            
            // Shortcut for GetValueAt(i)
            int this[int i]
            {
                get
                {
                    return GetValueAt(i);
                }
            }

            // Multiplies two Vectors
            public static int Multiply(Vector vectorA, Vector vectorB){
                if(vectorA.GetSize()!= vectorB.GetSize()){
                    throw new Exception("Dimensions do not match");
                }
                int dimension = vectorA.GetSize();
                int retVal = 0;

                for (int idx = 0; idx < dimension; ++idx)
                {
                    int prod = vectorA[idx] * vectorB[idx];
                    retVal += prod;
                }
                return retVal;

            }
        }

    }
}
