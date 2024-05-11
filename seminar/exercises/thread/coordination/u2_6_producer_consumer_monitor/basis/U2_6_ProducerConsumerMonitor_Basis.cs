using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace SeminarParallelComputing.exercises.thread.coordination.u2_6_producer_consumer.basis
{

    /**
     * Basis zu Aufgabe 2.6, Producer Consumer mit Monitor
     * 
     * 
     * Aufgabenstellung
     * - Es gibt zwei Threads und eine Queue. Ein Thread, der Producer, schreibt fortwährend neue Items in die Queue.
     * - Wenn die Queue "voll" ist, d.h. eine bestimmte Anzahl Items enthält, dann blockiert der Producer-Thread. 
     * - Der Consumer-Thread ist dafür verantwortlich den Producer-Thread aufzuwecken, sobald wieder ein Item in der Queue ist.
     * - Wenn die Queue leer ist, d.h. 0 Items enthält, so blockiert der Consumer-Thread.
     * - Der Producer-Thread ist dafür verantwortlich, den Consumer-Thread zu wecken, wenn wieder "Patz" in der Queue ist.
     * 
     * Diese Ausgangbasis enthält den Producer-Thread, den Consumer-Threadund eine Queue.
     * Es fehlt jedoch der Code, der den Consumer anhält wenn die Queue leer ist und den Producer anhält wenn die Queue eine bestimmte Länge erreicht hat. 
     * Ebenso fehlt die Logik, die beiden wieder aufzuwecken.
     * 
     * Benutzen Sie für die Lösung die Klasse Monitor
     * 
     * mit den folgenden Methoden:
     *  Monitor.Pulse(queue);
     *  Monitor.Wait(queue);
     *  
     * sowie das Konstrukt:
     *   lock (queue)
     * 
     */
    class U2_6_ProducerConsumerMonitor_Basis
    {
        Queue<int> queue = new Queue<int>();

        const int QUEUE_MAX_SIZE = 100;

        // Takes items from the queue
        void Consume()
        {
            while (true)
            {
                
                while (queue.Count == 0)
                {
                    // TODO: This is a busy wait loop. Change that to  wait while Queue is empty, without consuming Processor Time!
                    Console.WriteLine("Consumer " + Thread.CurrentThread.ManagedThreadId + " WAITING on EMPTY queue: Queue-Count " + queue.Count);
                }

                // Now hopefully there is an elemnt in the queue.
                int nextItem = queue.Dequeue();

                // TODO: wake up waiting producers
            }
        }

        void Produce()
        {
            int i = 1;
            while (true)
            {

                while (queue.Count == QUEUE_MAX_SIZE)
                {
                    // TODO: This is a busy wait loop. Change it to wait while the queue is full, without consuming Processor Time
                    Console.WriteLine("Producer " + Thread.CurrentThread.ManagedThreadId + " WAITING on FULL queue: Queue-Count " + queue.Count);

                }

                // Now hopefully there is space in the queue.
                queue.Enqueue(i++);

                // TODO: wake up waiting consumers
            }
        }


        public static void TestMain()
        {
            U2_6_ProducerConsumerMonitor_Basis instance = new U2_6_ProducerConsumerMonitor_Basis();

            // Start Consumer
            new Thread(() =>
            {
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
