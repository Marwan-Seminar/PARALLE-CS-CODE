// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeminarParallelComputing.seminar.examples.MatrixMult
{
    
    // Thread-Pool based version of Matrix-Multiplication
    public  class MatrixMultThreadPool
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
            ManualResetEvent[,] events = new ManualResetEvent[matrixA.NrOfRows(), matrixB.NrOfCols()];
            
            for (int row = 0; row < matrixA.NrOfRows(); ++row)
            {
                // Get row of A
                Matrix.Vector rowOfA = matrixA.GetRow(row);

                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {

                    // Get column of B
                    Matrix.Vector colOfB = matrixB.GetColumn(col);

                    StateContainer state = new StateContainer();
                    state.retMatrix = retMatrix;
                    state.rowIdx = row;
                    state.colIdx = col;
                    state.rowOfA = rowOfA;
                    state.colOfB = colOfB;

                    ThreadPool.QueueUserWorkItem(new WaitCallback(RunMultiplication), state);

                    // store the event for waiting on in
                    events[row, col] = state.doneEvent;
                }
            }

            // Waiting (probably inefficient approach)
            for (int row = 0; row < matrixA.NrOfRows(); ++row)
            {
                for (int col = 0; col < matrixB.NrOfCols(); ++col)
                {
                    events[row, col].WaitOne();
                }
            }
           

            return retMatrix; 
        }

        private static void RunMultiplication(Object stateObject)
        {
            StateContainer state = (StateContainer)stateObject;
            // Multiply them:
            int vectorProduct = Matrix.Vector.Multiply(state.rowOfA, state.colOfB);

            state.retMatrix.SetValueAt(state.rowIdx, state.colIdx, vectorProduct);

            state.doneEvent.Set();

        }

        class StateContainer
        {
           public ManualResetEvent doneEvent = new ManualResetEvent(false);

            public Matrix retMatrix;

            public int rowIdx;
            public Matrix.Vector rowOfA;
            public int colIdx;
            public Matrix.Vector colOfB;

        }
    }
}
