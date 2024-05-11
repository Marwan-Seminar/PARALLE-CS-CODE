// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.examples.thread
{
    // Demonstrates basic Monitor usage with Wait() and Signal()
   public class MonitorUsage
    {
        bool condition = false;

        // Explicit lock
        void MyMonitorWaitingdMethod()
        {
            lock(this){
                while (!condition)
                {
                    Monitor.Wait(this);
                }
            }
        }

        void MyMonitorNotifyingMethod()
        {
            lock (this)
            {
                condition = true;
                Monitor.Pulse(this);
            }
        }

        public static void TestBehavior()
        {
            MonitorUsage instance = new MonitorUsage();
            new Thread(
            () =>{
                Console.WriteLine("Calling waiting method");
                instance.MyMonitorWaitingdMethod();
                Console.WriteLine("Returned from waiting method");
            }).Start();

            Thread.Sleep(100);

            new Thread(
            () =>
            {
                Console.WriteLine("Calling notifying method");
                instance.MyMonitorNotifyingMethod();
                Console.WriteLine("Returned from notifying method");
            }).Start();
        }
    }
}
