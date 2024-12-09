using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Read_Data_1;
using Write_Data_1;

namespace Procces_File_1
{
    /// <summary>
    /// Provides methods to process files by reading chunks from a source file 
    /// and writing those chunks to a destination file.
    /// </summary>
    public class Procces_Files
    {
        /// <summary>
        /// Asynchronously processes a source file and writes its content in chunks to a destination file.
        /// The file is read using the <see cref="Read_Data_1.Read_Data"/> class, 
        /// and the chunks are written using the <see cref="Write_data_1.Write_Data"/> class.
        /// </summary>
        /// <param name="sourceFile">The path to the source file to be read.</param>
        /// <param name="destinationFile">The path to the destination file where data will be written.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
      
       public static async Task ProcessFileAsync(string sourceFile, string destinationFile)
       {
          // Check if the source file is empty
          if (new FileInfo(sourceFile).Length == 0)
          {
             // Create an empty destination file if it doesn't exist
             File.Create(destinationFile).Dispose();
             return;
          }

          // Create instances of the Read_Data and Write_Data classes
          var reader = new Read_Data_1.Read_Data();
          var writer = new Write_Data_1.Write_Data();

          try
          {
            // Read chunks from the source file asynchronously and write them to the destination file asynchronously
            await writer.WriteChunksAsync(destinationFile, reader.ReadChunksAsync(sourceFile));
          }
          catch (Exception ex)
          {  
            // Log or handle exceptions that may occur during file processing
            Console.WriteLine($"An error occurred while processing the file: {ex.Message}");
          }
       }
    }
}
