using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
/*
* Examples for File Access with asynchronous library routines 
* using asynch await.
* 
* TUT NICHT
* Bei mir auf dem Visual Studio 2019 for MAC sind die aysych Funktionen nicht in der Klasse File vorhanden!
* Läuft aber mit Visual Code und .Net 8.
*/
namespace TPLTaskProgramming.AsynchAwait.Experiments_aa_2022.AA_library_calls
{
    public class AAFileAccess
    {
        public static async void MainTest()
        {
            Console.WriteLine("AA File Access entered");

            AAFileAccess instance = new AAFileAccess();
            // Achtung: Wenn man hier await davorschreibt, dann beendet sich das PRogramm, weil der Aufruf in 
            // die Main Methode zurückkehrt!!!

            instance.SimpleWriteAsync();
            Console.WriteLine("SimpleWriteAsync returned ");
            // Hack to keep program alive
            Thread.Sleep(10000);
        }


        void fileAccessSynchronous()
        {

            // File.ReadAllBytes
        }

        public async System.Threading.Tasks.Task SimpleWriteAsync()
        {
            Console.WriteLine("SimpleWriteAsync entered");

            string filePath = "simple.txt";
            string text = $"Hello World";
            //await File.WriteAllTextAsync(filePath, text);
            Console.WriteLine("SimpleWriteAsync wtite file returned");
            //var httpClient = new TcpClient();

           // string read_text = await File.ReadAllTextAsync(filePath);
            Console.WriteLine("SimpleWriteAsync read file returned");

            Console.WriteLine(text);
        }
    }
}
