// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;



/*
Übung 3.3: Rekursive Task-Bäume, CPU-Time vs. Clock-Time
Bauen Sie einen Baum aus Tasks (z.B. der Tiefe 5, und jeder Knoten hat zwei Kinder)

Zeigen Sie, dass es passieren kann, dass das Programm sich beendet, bevor alle Tasks zuende gelaufen sind.

Reparieren Sie das Verhalten, so dass das Programm genau dann zurückkehrt, wenn alle Tasks zuende gelaufen sind.


 Output sollte in etwa so aussehen:
 
 Seminar Parallel Programming, Marwan Abu-Khalil 2012
Task: depth : 1 own time: 00:00:07.8941362 childeren time: 00:00:00
Task: depth : 1 own time: 00:00:08.1568188 childeren time: 00:00:00
Task: depth : 1 own time: 00:00:10.0873541 childeren time: 00:00:00
Task: depth : 2 own time: 00:00:12.7228220 childeren time: 00:00:18.244172
Task: depth : 1 own time: 00:00:06.8169214 childeren time: 00:00:00
Task: depth : 2 own time: 00:00:10.6289534 childeren time: 00:00:14.711057
Task: depth : 3 own time: 00:00:12.4055990 childeren time: 00:00:56.307005
Wall-Clock-Time: 00:00:14.7732278 Sum of Tasks CPU time: 00:01:08.7126049
 
 */
namespace SeminarParallelComputing.seminar.exercises.task
{
    class Ueb_3_3_Rekursive_Baeume_Mit_Zeiten
    {
        public static void TestMain()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
           TimeSpan sum = ( new Ueb_3_3_Rekursive_Baeume_Mit_Zeiten().CreateSubtasksWithTiming(3));
            watch.Stop();
            TimeSpan elapsed = watch.Elapsed;

            Console.WriteLine("Wall-Clock-Time: " + elapsed + " Sum of Tasks CPU time: " + sum);
        }

        TimeSpan CreateSubtasksWithTiming(int depth){
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (depth == 0) return TimeSpan.FromSeconds(0);



            Task<TimeSpan> sub_1 = new Task<TimeSpan>(() => CreateSubtasksWithTiming(depth - 1));
            Task<TimeSpan> sub_2 = new Task<TimeSpan>(() => CreateSubtasksWithTiming(depth - 1));

            sub_1.Start();
            sub_2.Start();

            // Own computing
            int data = 3;
            //Thread.Sleep(1000);
            for (long i = 0; i < 1000000000L; ++i)
            {
               data = data * data;
            }

            watch.Stop();
            TimeSpan myTime = watch.Elapsed;

            TimeSpan children_time = sub_1.Result + sub_2.Result;
            Console.WriteLine("Task: depth : " + depth + " own time: " + myTime + " childeren time: " + children_time);
            return myTime + children_time;


        }
    }
}
