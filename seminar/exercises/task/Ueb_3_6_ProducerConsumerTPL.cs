// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.task
{
    // Demonstrates producer-consumer scenarios based on Tasks
   public class Ueb_3_6_ProducerConsumerTPL
    {
        // Max nr. of elements in the queue
        const int MAX_SIZE = 10;

       // Nr. of consumers
       const int NR_OF_CONSUMERS = 4;

       // Nr. of producers
       const int NR_OF_PRODUCERS = 4;


       // BlockingCollection<String> queue = new BlockingCollection<String>(MAX_SIZE);
       BlockingQueue queue = new BlockingQueue();

        public static void TestMain()
        {
            Ueb_3_6_ProducerConsumerTPL instance = new Ueb_3_6_ProducerConsumerTPL();

           instance.RunTasks();
          //instance.RunThreads();
        }

        void RunTasks()
        {
            List<Task> allTasks = new List<Task>();

            for(int prodCnt = 0; prodCnt< NR_OF_PRODUCERS; ++ prodCnt){
                Producer producer = new Producer(prodCnt);
                Task produceTask = Task.Factory.StartNew(() => producer.Produce(queue));
                allTasks.Add(produceTask);
            }

            for(int consCnt = 0; consCnt< NR_OF_CONSUMERS; ++ consCnt){
                Consumer consumer = new Consumer(consCnt);
                 Task consumeTask = Task.Factory.StartNew(() => consumer.Consume(queue));
                allTasks.Add(consumeTask);
            }

           
            Task.WaitAll(allTasks.ToArray());
        }

        void RunThreads()
        {
            List<Thread> allThreads = new List<Thread>();

            for (int prodCnt = 0; prodCnt < NR_OF_PRODUCERS; ++prodCnt)
            {
                Producer producer = new Producer(prodCnt);
                Thread produceThread = new Thread(() => producer.Produce(queue));
                produceThread.Start();
                allThreads.Add(produceThread);
            }

            for (int consCnt = 0; consCnt < NR_OF_CONSUMERS; ++consCnt)
            {
                Consumer consumer = new Consumer(consCnt);
                Thread consumeThread = new Thread(() => consumer.Consume(queue));
                consumeThread.Start();
                allThreads.Add(consumeThread);
            }

            foreach (Thread threadToWaitFor in allThreads)
            {
                threadToWaitFor.Join();
            }
        }



    }


    class Producer
    {

        public Producer(int name)
        {
            this.name+=name;
        }

        String name = "Producer-";

        int currentItem = 0;


        // Produces in an infinite loop
       //public void Produce(BlockingCollection<String> queue)
        public void Produce(BlockingQueue queue)
       {
            while (true)
            {
                queue.Add(name + "-" + (++currentItem) );

                //Console.WriteLine(name + " producing: " + currentItem + " Queue-Size: " + queue.Count() + " Task: " + Task.CurrentId + " Thread: " + Thread.CurrentThread.ManagedThreadId);
            }
        }
    }

    class Consumer
    {

        public Consumer(int name)
        {
            this.name+= name;
        }

        String name = "Consumer-";

        String currentItem = "";


        // Produces in an infinite loop
        //public void Consume(BlockingCollection<String> queue)
        public void Consume(BlockingQueue queue)
        {
            while (true)
            {
                currentItem = queue.Take();
                //Console.WriteLine(name + " consuming: " + currentItem + " Queue-Size: " + queue.Count() + " Task: " + Task.CurrentId + " Thread: " + Thread.CurrentThread.ManagedThreadId);


            }
        }
    }


    class BlockingQueue
    {
        const int MAX_ELEMENTS = 10;

        Queue<String> _queue = new Queue<String>();

        private Object lockObject = new Object();
        
        public void Add(String newItem)
        {
            try
            {
                Monitor.Enter(lockObject);

                while (_queue.Count >= MAX_ELEMENTS)
                {
                    Monitor.Wait(lockObject);
                }
                // Now _queue has space
                _queue.Enqueue(newItem);
                Console.WriteLine(" producing: " + newItem + " Queue-Size: " + _queue.Count() + " Task: " + Task.CurrentId + " Thread: " + Thread.CurrentThread.ManagedThreadId);
                Monitor.PulseAll(lockObject);

            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            
        }

        public String Take()
        {
            String takenItem = null;
            try
            {
                Monitor.Enter(lockObject);

                while (_queue.Count == 0)
                {
                    Monitor.Wait(lockObject);
                }

                // now _queue has at least one element
                takenItem = _queue.Dequeue();

                Console.WriteLine(" consuming: " + takenItem + " Queue-Size: " + _queue.Count() + " Task: " + Task.CurrentId + " Thread: " + Thread.CurrentThread.ManagedThreadId);


                Monitor.PulseAll(lockObject);
            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            if(takenItem == null){
                throw new Exception("Illegal: item taken from empty queue");
            }
            
            return takenItem;
        }

        public int Count()
        {
            // Threadsafe???
            //lock (lockObject)
            //{
                return _queue.Count;
            //}
            
        }
    }
}
