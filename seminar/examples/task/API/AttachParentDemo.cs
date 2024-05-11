// Copyright Marwan Abu-Khalil 2012


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.examples.task.API
{
    // Demonstrates waiting by AttachToPArent
    public class AttachParentDemo
    {
        Task DepthAction(int depth)
        {
            if (depth >= 3) return null;

            Console.WriteLine("Depth " + depth + " forking child taskID: " + Task.CurrentId + "thread ID:  "+ Thread.CurrentThread.ManagedThreadId);

            // Attach to parent does not stop the tasks execution. It just makes sure, the task does not return, until the chid task did return.
            //Task task =Task.Factory.StartNew(() => DepthAction(depth + 1), TaskCreationOptions.AttachedToParent);
            Task task = Task.Factory.StartNew(() => DepthAction(depth + 1));
            
            // wait changes the semantic of this program: each tasks waits untis its subtasks are completed: Output forr, fork, fork,... returns, returns, returns.
            task.Wait();

            if (depth == 2 )
            {
                Thread.Sleep(5000);
            }
            Console.WriteLine("Depth " + depth + " returns " + Task.CurrentId + " thread ID:  " + Thread.CurrentThread.ManagedThreadId);
            return task;
        }

       public static void TestMain()
        {
           (new AttachParentDemo().DepthAction(0)).Wait();
        }
    }

    public class AttachParentDemoMini
    {
       Task DepthAction(int depth){
         if (depth >= 3) return null;
  
            Console.WriteLine("Task " +depth +" fork"); 
         Task task = Task.Factory.StartNew(() =>    DepthAction(depth + 1) //);
           ,TaskCreationOptions.AttachedToParent);
         if (depth == 2) {
           Thread.Sleep(5000);
         }
         Console.WriteLine("Task " + depth + " return");
         return task;
    }
    
    public static void TestMain() {
       (new AttachParentDemoMini().
         DepthAction(0)).Wait();
    }

    }
}
