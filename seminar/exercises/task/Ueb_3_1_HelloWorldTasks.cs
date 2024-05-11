// Copyright Marwan Abu-Khalil 2012

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.task
{
    /*
    
    Programmieren Sie zwei triviale Tasks. Eine dieser beiden schreibt „Hello“ auf die Konsole, die andere schreibt „World“

    a) Starten Sie viele dieser Tasks. Stellen Sie dabei sicher, dass die Tasks 
       gleichzeitig laufen, so dass die Worte in unvorhersehbarer Reihenfolge geschrieben werden. 

    b) Versuchen Sie sicher zu stellen, dass die Worte immer genau abwechselnd geschrieben werden, so dass stets der Text “Hello World” erscheint
      (ohne Verwendung der Thread-Synchronisationsmittel Lock oder Monitor).
      
    c) Stellen Sie fest, wie viele Tasks sie innehalb von 10 Sekunden tatsächlich zum Laufen bringen und wie viele Threads jeweils beteiligt sind.

     
     * 
     Diese Musterlösung enthält drei Versionen:
     a) unsortierte Tasks
     b) Sortierte Tasks, iterativ erzeugt
     b.2 ) Sortierte Tasks, rekursiv erzeugt.
     
     Es wird ausgedruckt, wie viele Tasks tatsächlich innerhalb von 10 Sekunden zum Laufen kommen.
     Erstaunliches Ergebnis:
     a) Tasks run HelloWorld_a: 169603 (in vier Threads)
     b) Tasks run HelloWorld_b sequential : 325481 (alles in einem Thread)
     b.2) Tasks run  HelloWorld_b recursive: 15284 (in ca. 15 Threads)
     */
    class Ueb_3_1_HelloWorldTasks
    {

        public static void TestMain()
        {
            //new HelloWorld_a().StartTasks();

            //new HelloWorld_b().StartHelloWorldWithWait();
            
            new HelloWorld_b().StartHelloAndWorldForkRecursively();

            
        }

    }

    class HelloWorld_a
    {
        public volatile int NrOfTasks = 0;
        
        void startHelloAndWorldTasks()
        {
            while(true) //for (int i = 1; i < 100; ++i )
            {
                Task helloTask = new Task(() => {
                    Console.WriteLine("Hello from Thread: " + Thread.CurrentThread.ManagedThreadId);
                    Interlocked.Increment(ref NrOfTasks);
                });
                Task worldTask = new Task(() => { 
                    Console.WriteLine("World from Thread: " + Thread.CurrentThread.ManagedThreadId);
                    Interlocked.Increment(ref NrOfTasks);
                });
                worldTask.Start();
                helloTask.Start();
            }

            
           
        }

       public void StartTasks()
        {
            new Task(startHelloAndWorldTasks).Start();

            Thread.Sleep(10000);
            Console.WriteLine("Tasks run HelloWorld_a: " + NrOfTasks);
        }
    }


    // Mir fallen zwei Loesungen ein: Eine Wait-Lösung und eine Fork-Lösung
    class HelloWorld_b
    {
        const String HELLO = "HELLO";
        const String WORLD = "WORLD";

        public volatile int NrOfTasks = 0;

        private void HelloWorldWithWaitStartLoop()
        {
            while(true) //for (int i = 0; i < 100; ++i)
            {
                Task helloTask = new Task(()=>{
                    Console.WriteLine("Hello from Thread: " + Thread.CurrentThread.ManagedThreadId); 
                    Interlocked.Increment(ref NrOfTasks);
                });

                Task worldTask = new Task(() => {
                    Console.WriteLine("World from Thread: " + Thread.CurrentThread.ManagedThreadId);
                    Interlocked.Increment(ref NrOfTasks);
                });
                helloTask.Start();
                helloTask.Wait();
                worldTask.Start();
                worldTask.Wait();
            }
        }

        public void StartHelloWorldWithWait()
        {
            new Task(HelloWorldWithWaitStartLoop).Start();

            Thread.Sleep(10000);
            Console.WriteLine("Tasks run HelloWorld_b sequential : " + NrOfTasks);
        
        }


        public void StartHelloAndWorldForkRecursively(){
            new Task(()=>RecursivelyForkTask(HELLO)).Start();
            
            Thread.Sleep(10000);
            Console.WriteLine("Tasks run  HelloWorld_b recursive: " + NrOfTasks);
        }
        
        void RecursivelyForkTask(String argument)
        {
            Console.WriteLine(argument + " from Thread: " + Thread.CurrentThread.ManagedThreadId);
            Interlocked.Increment(ref NrOfTasks);

            Task helloOrWorldTask;
            if(argument == HELLO){
                helloOrWorldTask = new Task(()=>RecursivelyForkTask(WORLD));
            }else if(argument == WORLD){
                helloOrWorldTask = new Task(() => RecursivelyForkTask(HELLO));
            }else{
                throw new Exception("Illegal");
            }

            helloOrWorldTask.Start();
            helloOrWorldTask.Wait();
        }

      

    }
}
