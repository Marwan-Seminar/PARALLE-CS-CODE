// Copyright Marwan Abu-Khalil 2012


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeminarParallelComputing.seminar.examples.MatrixMult
{
    // This class demonstrates Recursive Decomposition of Matrix Multiplication, according to Herlihy / Shavit "The Art of Multiprocessor Programming" p. 374
    // The advantage is, that the process of task creation is parallelized itself. The tasks form a tree
    // where each node forks further tasks.
    // The downside is additional memory for intermediate results, as this is not an in-place algorthm: LHS and RHS are required in each task.
    // The underlying idea of the algoritm is to split both matices into 4 submatrices. Multiplication is then 
    // 8 Matrix-Multiplications of the submatirces and 1 Matrix-Addition of the intermediate results. The addition could be parallelized too.
    // 
    //                A00  A01     B00  B01         A00 * B00 + A01 * B01     A00 * B01 + A01 * B11
    //   C = A * B =             *              =                           
    //                A10  A11     B10  B11         A10 * B00 + A11 * B10     A10 * B01 + A11 * B11
    // The algorithm is as follows
    // - Create Matrix C for the results
    // - Split A, B into four submatirces each (in Herlihy's algorithm also C is split, this would allow also a parallelization of the addition)
    // - Create two new matrices LHS and RHS for the intemediate results and split them into four Submatirces.
    //   LHS contains als Left Hand Sides of the additions, RHS the Right Hand Sides. 
    //   This step creates additional memory consuption, as it is not done in-place.
    // - Matrix-Addition of the intermediata matrices: 
    //      C = LHS + RHS
    //      
    // This process can be repeated recursivly until the submatirces are small enough to use serial matrix multiplication.
    //
    // Limitation: This implemntation works only for quadratic matrices whith dimension 2^n.
   public class MatrixMultRecursiveDecomposition
    {
        // Defines end of recursion: if dimension is smaller than threashold, sequential matrix multiplication is applied.
        const int THRESHOLD = 128;

        public static MatrixDecomposable Multiply(MatrixDecomposable A, MatrixDecomposable B)
        {
            if(A.dimOfCol() != A.dimOfRow() || A.dimOfRow() != B.dimOfCol() || B.dimOfCol() != B.dimOfRow()){
                throw new Exception("Dimensions do not match. Required are quadratic matrices whith dimension 2^n. A.dimOfCol() " 
                    + A.dimOfCol() + " A.dimOfRow() " + A.dimOfRow() + " B.dimOfCol() " + B.dimOfCol() + "  B.dimOfRow() " +   B.dimOfRow());
            }

            int dim = A.dimOfCol();

            // Create matrix C to contain the final results
            MatrixDecomposable C = new MatrixDecomposable(dim, dim);

            return Multiply(A, B, C);

        }

        public static MatrixDecomposable Multiply(MatrixDecomposable A, MatrixDecomposable B, MatrixDecomposable C)
        {
            if (A.dimOfCol() != A.dimOfRow() || A.dimOfRow() != B.dimOfCol() || B.dimOfCol() != B.dimOfRow() || B.dimOfRow() != C.dimOfCol() || C.dimOfCol() != C.dimOfRow())
            {
                throw new Exception("Dimensions do not match. Required are quadratic matrices whith dimension 2^n. A.dimOfCol() "
                    + A.dimOfCol() + " A.dimOfRow() " + A.dimOfRow() + " B.dimOfCol() " + B.dimOfCol() + "  B.dimOfRow() " + B.dimOfRow()
                    + " C.dimOfCol() " + C.dimOfCol() + " C.dimOfRow() " + C.dimOfRow());
            }

            int dim = A.dimOfCol();

            // End of recursion: if dimension is smaller than threshold, sequential matrix multiplication is applied.
            // Currently recursion goes down to a 1X1 Matrix
            if (dim <= THRESHOLD)
            //if (dim == 1)
            {
                MatrixDecomposable.MultiplySequential(A, B, C);
                //int value = A.GetValueAt(0,0) * B.GetValueAt(0,0);
                //C.SetValueAt(0, 0, value);
                
            }
            // Recurse and fork subtasks
            else 
            {

                // Split A and B
                MatrixDecomposable[,] A_split = A.Split();
                MatrixDecomposable[,] B_split = B.Split();

                // Create and split LHS and RHS for intermediate results
                MatrixDecomposable LHS = new MatrixDecomposable(dim, dim);
                MatrixDecomposable RHS = new MatrixDecomposable(dim, dim);

                MatrixDecomposable[,] LHS_split = LHS.Split();
                MatrixDecomposable[,] RHS_split = RHS.Split();

                // Create a list of the tasks to wait for
                List<Task> multiplyTasks = new List<Task>();

                // The recursive multiplications of the submatrices
                
                //LHS[0,0] = A[0,0] * B[0,0]
                //Console.WriteLine("LHS[0,0] = A[0,0] * B[0,0]");
                multiplyTasks.Add(Task.Factory.StartNew(() =>
                    Multiply(A_split[0, 0], B_split[0, 0], LHS_split[0, 0])));
                //LHS[1,0] = A[1,0] * B[0,0]
                //Console.WriteLine("LHS[1,0] = A[1,0] * B[0,0]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[1, 0], B_split[0, 0], LHS_split[1, 0])));
                //LHS[0,1] = A[0,0] * B[0,1]
                //Console.WriteLine("LHS[0, 1] = A[0, 0] * B[0, 1]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[0, 0], B_split[0, 1], LHS_split[0, 1])));
                //LHS[1,1] = A[1,0] * B[0,1]
                //Console.WriteLine("LHS[1,1] = A[1,0] * B[0,1]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[1, 0], B_split[0, 1], LHS_split[1, 1])));

                //RHS[0,0] = A[0,1] * B[1,0]
                //Console.WriteLine("RHS[0,0] = A[0,1] * B[1,0]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[0, 1], B_split[1, 0], RHS_split[0, 0])));
                //RHS[1,0] = A[1,1] * B[1,0]
                //Console.WriteLine("RHS[1,0] = A[1,1] * B[1,0]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[1, 1], B_split[1, 0], RHS_split[1, 0])));
                //RHS[0,1] = A[0,1] * B[1,1]
                //Console.WriteLine("RHS[0,1] = A[0,1] * B[1,1]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[0, 1], B_split[1, 1], RHS_split[0, 1])));
                //RHS[0,1] = A[0,1] * B[1,1]
                //Console.WriteLine("RHS[0,1] = A[0,1] * B[1,1]");
                multiplyTasks.Add(Task.Factory.StartNew(() => 
                    Multiply(A_split[1, 1], B_split[1, 1], RHS_split[1, 1])));
                

                //maybe more elegant :
                /*
                for (int i = 0; i < 2; ++i)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        int tmp_i = i;
                        int tmp_j = j;
                        // TODO fork Tasks 
                        //LHS[i,j] = A[i,j] * B[i,j]
                        multiplyTasks.Add(Task.Factory.StartNew(() =>
                            Multiply(A_split[tmp_i, 0], B_split[0, tmp_j], LHS_split[tmp_i, tmp_j])));

                        //RHS[i,j] = A[i,j] * B[i,j]
                        multiplyTasks.Add(Task.Factory.StartNew(() =>
                             Multiply(A_split[tmp_i, 1], B_split[1, tmp_j], RHS_split[tmp_i, tmp_j]))); 
                     
                    }
                }
                */
                

               
                // Wait for Tasks to return
                Task.WaitAll(multiplyTasks.ToArray());

                // Add the intermediate results
                MatrixDecomposable.AddSequentialInplace(LHS, RHS, C);
            }


            return C;
        }

    }
}
