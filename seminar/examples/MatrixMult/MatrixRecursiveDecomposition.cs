// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeminarParallelComputing.seminar.examples.MatrixMult
{
    // This class demonstrates recursive decomposition of matices for parallelization purposes.
    // The approach presented here is according to Herlihy / Shavit
    // Represents an MXN matrix
    public class MatrixDecomposable
    {
        const bool DEBUG = true;

        // Defines end of recursive decomposition, and as such limits the number of tasks.
        const int THRESHOLD = 4;

        // Array holding the matrix data. An instance of this class may "see" only a subset of the elements of data, as defined by the Split() method.
        int[,] data;

        // Dimension N is defining the number of rows M (i.e. each column has dimM elements)
        int dimM;

        // defines the number of colums N (i.e. each row has dimCols elements)
        int dimN;

        // The two offset fields define a submatrix. 
        // offsetM defines defines the first row of the submatix 
        int offsetM;
        // offsetM defines defines the first colum of the submatix 
        int offsetN;
     
        // The consturcor for the "backing" matrix, which holds the actual data
        public MatrixDecomposable(int dimM, int dimN)
        {
            this.dimM = dimM;
            this.dimN = dimN;

            // Creating the data array.
            data = new int[dimM, dimN];

        }

        // Constructor for submatices that point to a submatirx within the backing matrix
        public MatrixDecomposable(int[,] data, int dimM, int dimN, int offsetM, int offsetN)
        {
            this.data = data;
            this.dimM = dimM;
            this.dimN = dimN;

            this.offsetM = offsetM;
            this.offsetN = offsetN;
        }

        // This is the decomposing method. It splits the matrix into 4 submatices. 
        // Works for any dimensions, but returns matrices of unequal dimensions unless the original matrix is an NXN matrix and N is a power of 2. 
        public MatrixDecomposable[,] Split()
        {
            if (dimM < 2 || dimN < 2)
            {
                throw new Exception("Too small to split: +  dimM " + dimM + " dimN : " + dimN );
            }
            MatrixDecomposable[,] submatrices = new MatrixDecomposable[2, 2];

            int splitDimM = dimM / 2;
            int splitDimN = dimN / 2;
            submatrices[0, 0] = new MatrixDecomposable(this.data, splitDimM, splitDimN, this.offsetM, this.offsetN);
            submatrices[1, 0] = new MatrixDecomposable(this.data, dimM - splitDimM, splitDimN, this.offsetM + splitDimM, this.offsetN + 0);
            submatrices[0, 1] = new MatrixDecomposable(this.data, splitDimM, dimN - splitDimN, this.offsetM + 0, this.offsetN + splitDimN);
            submatrices[1, 1] = new MatrixDecomposable(this.data, dimM - splitDimM, dimN - splitDimN, this.offsetM + splitDimM, this.offsetN + splitDimN);

            return submatrices;
        }

        public static MatrixDecomposable AddSequential(MatrixDecomposable matrixA, MatrixDecomposable matrixB)
        {
            int dimM = matrixA.dimM;
            int dimN = matrixA.dimN;

            if(matrixA.dimOfRow() != matrixB.dimOfRow() || matrixA.dimOfCol() != matrixB.dimOfCol()){
                throw new Exception("AddSequential: Dimensions do not match: matrixA.dimOfRow() " + matrixA.dimOfRow() +" matrixB.dimOfRow() " +  matrixB.dimOfRow() + 
                    " matrixA.dimOfCol() " + matrixA.dimOfCol() + " matrixB.dimOfCol() " + matrixB.dimOfCol());
            }

            
            MatrixDecomposable sumMatrix = new MatrixDecomposable(dimM, dimN);
            
            AddSequentialInplace(matrixA, matrixB, sumMatrix);
            
            return sumMatrix;
        }

        // Returns sumMatrix containing the addition.
        public static void  AddSequentialInplace(MatrixDecomposable matrixA, MatrixDecomposable matrixB, MatrixDecomposable sumMatrix){
            int dimM = matrixA.dimM;
            int dimN = matrixA.dimN;

            if (matrixA.dimOfRow() != matrixB.dimOfRow() || matrixA.dimOfCol() != matrixB.dimOfCol() || matrixB.dimOfCol() != sumMatrix.dimOfCol() || matrixB.dimOfRow() != sumMatrix.dimOfRow())
            {
                throw new Exception("AddSequential: Dimensions do not match: matrixA.dimOfRow() " + matrixA.dimOfRow() +" matrixB.dimOfRow() " +  matrixB.dimOfRow() +
                    " matrixA.dimOfCol() " + matrixA.dimOfCol() + " matrixB.dimOfCol() " + matrixB.dimOfCol() + " sumMatrix.dimOfCol()" + sumMatrix.dimOfCol() + " sumMatrix.dimOfRow() " + sumMatrix.dimOfRow());
            }

            for (int rowIdx = 0; rowIdx < dimM; ++rowIdx)
            {
                for (int colIdx = 0; colIdx < dimN; ++colIdx)
                {
                    int scalarSum = matrixA.GetValueAt(rowIdx, colIdx) + matrixB.GetValueAt(rowIdx, colIdx);
                    sumMatrix.SetValueAt(rowIdx, colIdx, scalarSum);
                }
            }
       }


        // This is a method that demonstrates parallelization of Matrix-Addition via recursive decomposition.
        // It is only for demonstration purposes, as the algorithm that is parallelized is too trivial for achieving significant performance advantages. 
        // But this approach can be extended for Matrix Multiplication.
        // 
        // It decomposes the matrix recursively and in parallel within TPL Tasks. 
        // When the remaining pieces are small enough, the addition is performed sequentally.
        // Nota bene: This means that many small sequential additions are performed in parallel Tasks. 
        public static MatrixDecomposable AddParallelRecursiveDecomposition(MatrixDecomposable matrixA, MatrixDecomposable matrixB)
        {
            if (matrixA.dimOfRow() != matrixB.dimOfRow() || matrixA.dimOfCol() != matrixB.dimOfCol())
            {
                throw new Exception("AddSequential: Dimensions do not match: matrixA.dimOfRow() " + matrixA.dimOfRow() + " matrixB.dimOfRow() " + matrixB.dimOfRow() +
                    " matrixA.dimOfCol() " + matrixA.dimOfCol() + " matrixB.dimOfCol() " + matrixB.dimOfCol());
            }
            
            

            int dimM = matrixA.dimM;
            int dimN = matrixA.dimN;

            MatrixDecomposable sumMatrix = new MatrixDecomposable(dimM, dimN);

            return AddParallelRecursiveDecomposition(matrixA, matrixB, sumMatrix);

        }
        public static MatrixDecomposable AddParallelRecursiveDecomposition(MatrixDecomposable matrixA, MatrixDecomposable matrixB, MatrixDecomposable sumMatrix)
        {

            if (DEBUG)
            {
                if (matrixA.dimOfRow() != matrixB.dimOfRow() || matrixA.dimOfCol() != matrixB.dimOfCol())
                {
                    throw new Exception("AddSequential: Dimensions do not match: matrixA.dimOfRow() " + matrixA.dimOfRow() + " matrixB.dimOfRow() " + matrixB.dimOfRow() +
                        " matrixA.dimOfCol() " + matrixA.dimOfCol() + " matrixB.dimOfCol() " + matrixB.dimOfCol());
                }
            }

            int dimM = matrixA.dimM;
            int dimN = matrixA.dimN;

            // End of Recursion:
            if (dimM * dimN <= THRESHOLD)
            {
                // add sequentially
                for (int rowIdx = 0; rowIdx < dimM; ++rowIdx)
                {
                    for (int colIdx = 0; colIdx < dimN; ++colIdx)
                    {
                        int scalarSum = matrixA.GetValueAt(rowIdx, colIdx) + matrixB.GetValueAt(rowIdx, colIdx);
                        sumMatrix.SetValueAt(rowIdx, colIdx, scalarSum);
                    }
                }
            }
            // Split and recurse
            else 
            {
                
                MatrixDecomposable[,] subMatricesA = matrixA.Split();
                MatrixDecomposable[,] subMatricesB = matrixB.Split();
                MatrixDecomposable[,] subMatricesSum = sumMatrix.Split();

                Task task1 = Task.Factory.StartNew(() => AddParallelRecursiveDecomposition(subMatricesA[0, 0], subMatricesB[0, 0], subMatricesSum[0,0]));
                Task task2 = Task.Factory.StartNew(() => AddParallelRecursiveDecomposition(subMatricesA[0, 1], subMatricesB[0, 1], subMatricesSum[0, 1]));
                Task task3 = Task.Factory.StartNew(() => AddParallelRecursiveDecomposition(subMatricesA[1, 0], subMatricesB[1, 0], subMatricesSum[1, 0]));
                Task task4 = Task.Factory.StartNew(() => AddParallelRecursiveDecomposition(subMatricesA[1, 1], subMatricesB[1, 1], subMatricesSum[1, 1]));

                Task.WaitAll(task1, task2, task3, task4);
            }

            return sumMatrix;
        }


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

        public void SetValueAt(int rowIdx, int columIdx, int value)
        {
            data[rowIdx + offsetM, columIdx+offsetN] = value;
        }
        public int GetValueAt(int rowIdx, int colIdx)
        {
            return data[rowIdx + offsetM, colIdx + offsetN];
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
        public Vector GetRow(int rowIdx)
        {
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
            MatrixDecomposable matrix;
            int position;
            Direction direction;

            public enum Direction { ROW, COL }

            // Direction definies if it is a row or a column, position defines, where in the backing matrix the Vector is located
            public Vector(MatrixDecomposable backingMatrix, Direction dir, int position)
            {
                this.matrix = backingMatrix;
                this.direction = dir;
                // The position inside the backing matrix is based on the offset:
                this.position = direction == Direction.ROW ? position + matrix.offsetM : position + matrix.offsetN;
            }

            public int GetSize()
            {
                return direction == Direction.ROW ? matrix.dimOfRow() : matrix.dimOfCol();
            }

            public Direction GetDirection()
            {
                return direction;
            }

            public int GetValueAt(int i)
            {
                if (direction == Direction.ROW)
                {
                     if (i > matrix.dimN)
                    {
                        throw new Exception("Illegal index: " + i + " dimension N is: " + matrix.dimN);
                    }

                    // considers offset into rows
                    return matrix.data[position, i + matrix.offsetN];
                }
                else if (direction == Direction.COL)
                {
                   if (i > matrix.dimM)
                    {
                        throw new Exception("Illegal index: " + i + " dimension M is: " + matrix.dimM);
                    }

                    // consideres offset into colums
                    return matrix.data[i + matrix.offsetM, position];
                }
                else
                {
                    throw new Exception(" Impossible direction:" + direction);
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
            public static int Multiply(Vector vectorA, Vector vectorB)
            {
                if (vectorA.GetSize() != vectorB.GetSize())
                {
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

        // Multiplies two matrices sequentially (this can also be two submatrices and the result is written into a submatrix)
        public static void  MultiplySequential(MatrixDecomposable matrixA, MatrixDecomposable matrixB, MatrixDecomposable targetMatrix)
        {
            if (matrixA.dimOfRow() != matrixB.dimOfCol() || matrixA.dimOfCol() != targetMatrix.dimOfCol() || matrixB.dimOfRow() != targetMatrix.dimOfRow() )
            {
                throw new Exception("MultiplySequential Dimensions do not match: A row " + matrixA.dimOfRow() + "  A col " + matrixA.dimOfCol() +" B row " + matrixB.dimOfRow() + 
                    " B col " + matrixB.dimOfCol() + " taget col " + targetMatrix.dimOfCol() + " target row " + targetMatrix.dimOfRow());
            }
                     

            int commonVectorLenght = matrixA.dimOfRow();


            // This is the expensive loop!
            for (int row = 0; row < matrixA.NrOfRows(); ++row)
            {

                // Get row of A
                MatrixDecomposable.Vector rowOfA = matrixA.GetRow(row);

                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    // Get column of B
                    MatrixDecomposable.Vector colOfB = matrixB.GetColumn(col);

                    // Multiply them:
                    int vectorProduct = MatrixDecomposable.Vector.Multiply(rowOfA, colOfB);

                    targetMatrix.SetValueAt(row, col, vectorProduct);
                }
            }
        }
    }
}
