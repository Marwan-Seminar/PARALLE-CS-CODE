using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SeminarParallelComputing.seminar.exercises.memorymodel
{
    // volatile nötig damit beide Threads die selben Daten sehen. 
    // Genauer: Damit der lesende Thread den Wert sieht, den der schreibende Thread der Variablen global_data zuweist.
    // Muss jedoch als Release kompiliert werden und als Release gestartet werden. 
    class Ueb_4_1_Volatile
    {
        //volatile
        int global_data;

        volatile Ueb_4_1_Volatile test; 
        void ReadingMethod()
        {

            

            Console.WriteLine("Volatile Example: Reader entering loop, global data: " + global_data);
            global_data = 1;
            while (global_data == 1)
            {
                // Hier kein Printout, sonst funtktioniert das Beispiel nicht wie erwartet. 
                
            }
            Console.WriteLine("\nVolatile Example Returned: READER THREAD SAW CHANGED DATA!" + global_data + "\n");
            

        }

        void WritingMethod()
        {
            while (true)
            {
                Console.WriteLine("Hit any key to get  set global data to 0");
                Console.ReadLine();
                Console.WriteLine("Global data as seen from writing thread before reset: " + global_data);
                global_data = 0;
                Console.WriteLine("Global data as seen from writing thread after reset: " + global_data);
              
            }
        }

        void StartThreads()
        {
            new Thread(ReadingMethod).Start();
            Thread.Sleep(1000);
            new Thread(WritingMethod).Start();
        }


        public static void TestMain()
        {
            new Ueb_4_1_Volatile().StartThreads();
        }
    }
}
