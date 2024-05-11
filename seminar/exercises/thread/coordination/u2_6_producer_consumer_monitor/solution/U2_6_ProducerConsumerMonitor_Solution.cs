using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace SeminarParallelComputing.exercises.thread.coordination.u2_6_producer_consumer.solution
{

    /**
     * Musterlösung zu Aufgabe 2.6
     * 
     * Die Klasse Ueb_2_6_ProducerConsumerMonitor implementiert ein Producer-Consumer Szenario.
     * 
     * - Es gibt zwei Threads und eine Queue. Ein Thread, der Producer, schreibt fortwährend neue Items in die Queue.
     * - Wenn die Queue "voll" ist, d.h. eine bestimmte Anzahl Items enthält, dann blockiert der Producer-Thread. 
     * - Der Consumer-Thread ist dafür verantwortlich den Producer-Thread aufzuwecken, sobald wieder ein Item in der Queue ist.
     * - Wenn die Queue leer ist, d.h. 0 Items enthält, so blockiert der Consumer-Thread.
     * - Der Producer-Thread ist dafür verantwortlich, den Consumer-Thread zu wecken, wenn wieder "Patz" in der Queue ist.
     * 
     * Diese Implementierung enthält folgende Vereinfachungen:
     * - Sie kann nur mit maximal einem Producer und maximal einem Consumer umgehen. Bei mehreren Producern oder Consumern kann es zu Deadlock kommen.
     * - Sie erzeugt überflüssige Aufweck-Aufrufe.
     * 
     */
    class U2_6_ProducerConsumerMonitor_Solution
    {
        Queue<int> queue = new Queue<int>();

        const int QUEUE_MAX_SIZE = 100;

        void Consume()
        {
            while (true)
            {
                lock (queue)
                {
                    while (queue.Count == 0)
                    {
                        Console.WriteLine("Consumer "+ Thread.CurrentThread.ManagedThreadId + " WAITING on EMPTY queue: Queue-Count " + queue.Count);
                        Monitor.Wait(queue);
                        Console.WriteLine("Consumer " + Thread.CurrentThread.ManagedThreadId + " WOKE UP: Queue-Count " + queue.Count);
                    }

                    // Now it is safe to dequeue, there is definitly an elemnt in the queue.
                    int nextItem = queue.Dequeue();
                    
                    //Console.WriteLine("Consumed item: " + nextItem);

                    // Notify producder, such that he wakes up, if he is waiting, and can check for the condition he is waiting for.
                    //if (queue.Count == QUEUE_MAX_SIZE - 1)
                    //{
                    Monitor.Pulse(queue);
                    //}

                }
            }
        }

        void Produce()
        {
            int i = 1;
            while (true)
            {
                lock (queue)
                {
                    while (queue.Count == QUEUE_MAX_SIZE)
                    {
                        Console.WriteLine("Producer " + Thread.CurrentThread.ManagedThreadId + " WAITING on FULL queue: Queue-Count " + queue.Count);
                        
                        // wait for space to become available in the queue
                        Monitor.Wait(queue);
                        
                        Console.WriteLine("Producer " + Thread.CurrentThread.ManagedThreadId + " WOKE UP: Queue-Count " + queue.Count);
                    }

                    // Now it is safe to enqueue, there is definitly space in the queue.
                    queue.Enqueue(i++);
                    
                    // Notify Consumer, so that he can check his condidtion, in case he is waiting. 
                    //if (queue.Count == 1)
                    //{
                    Monitor.Pulse(queue);
                    //}
                    
                }
            }
        }


        public static void TestMain()
        {
            U2_6_ProducerConsumerMonitor_Solution instance = new U2_6_ProducerConsumerMonitor_Solution();
            
            // Start Consumer
            new Thread(() => {
                instance.Consume();
            }).Start();

            //for (int i = 0; i < 10; ++i)
            //{
            
            // Start Producer
            new Thread(() =>
            {
                instance.Produce();
            }).Start();
            
            //}
        
        }
    }
}
