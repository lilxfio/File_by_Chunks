
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Procces_File_1;

class Main_Procces_File
{
        public static async Task Main(string[] args)
        {
            string sourceFile = "data.csv";
            string destinationFile = "data_copy.csv";

            // start to calculate implemetetion time 
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // Start the process
            await Procces_Files.ProcessFileAsync(sourceFile, destinationFile);

             // Stop the stopwatch and display the time
            stopwatch.Stop();
            Console.WriteLine($"File read, processed, and written in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        }
}
