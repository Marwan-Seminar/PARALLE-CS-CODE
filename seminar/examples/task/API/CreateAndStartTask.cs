using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Copyright Marwan Abu-Khalil 2012

using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.examples.task.API
{
    // Demonstrates usage of start and create API.
    // Prints Task IDs and Thread IDs to the console
    class CreateAndStartTask
    {
        void MyAction()
        {
            int taskID = (int )Task.CurrentId;
            int threadID = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine("New Task! ID:" + taskID + " running in thread: " + threadID);
        }
        void RunMyTasks()
        {
            Task task = new Task(MyAction);

            task.Start();

            task.Wait();
        }

        public static void TestMain()
        {
            CreateAndStartTask instance = new CreateAndStartTask();
            instance.RunMyTasks();
        }
    }
}
