using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Read_Data_1
{
    /// <summary>
    /// Provides functionality to read large text files in chunks asynchronously.
    /// </summary>
    public class Read_Data
    {
        /// <summary>
        /// Reads data from a specified file in 8 MB chunks asynchronously and yields each chunk as a string.
        /// </summary>
        /// <param name="sourceFile">The path to the file to be read.</param>
        /// <returns>An asynchronous enumerable of strings, each representing a chunk of data read from the file.</returns>
        /// <remarks>
        /// Each chunk consists of approximately 8 MB of data, calculated based on an estimated line size. The method
        /// is optimized for reading large files by using a buffer size of 8 MB and reading line-by-line to avoid
        /// excessive memory usage.
        /// 
        /// When the end of the file is reached, any remaining data (if less than 8 MB) is also yielded as a final chunk.
        /// </remarks>
        /// <example>
        /// The following example demonstrates how to use <see cref="ReadChunksAsync"/> to read and process chunks of data from a file:
        /// <code>
        /// var reader = new Read_Data();
        /// await foreach (var chunk in reader.ReadChunksAsync("path/to/yourfile.txt"))
        /// {
        ///     Console.WriteLine(chunk);
        /// }
        /// </code>
        /// </example>
        public async IAsyncEnumerable<string> ReadChunksAsync(string sourceFile)
        {
           // Validate input parameter
           if (string.IsNullOrWhiteSpace(sourceFile))
           {
              throw new ArgumentException("The source file path cannot be null or empty.", nameof(sourceFile));
           }

           int bufferSize = 8 * 1024 * 1024; // 8MB buffer
           int estimatedLineSize = 100; // Average line size in bytes
           int linesPerChunk = bufferSize / estimatedLineSize; // ~81920 lines per chunk

           // Check if the file is empty and yield a placeholder text if so
           var fileInfo = new FileInfo(sourceFile);
           if (fileInfo.Length == 0)
           {
             yield return "Empty File"; // Yield a placeholder text for an empty file
             yield break;
           }

           using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true))
           using (StreamReader reader = new StreamReader(sourceStream))
           {
             StringBuilder chunkBuilder = new StringBuilder();
             int currentLineCount = 0;
             string? line;

             while ((line = await reader.ReadLineAsync()) != null)
             {
               chunkBuilder.AppendLine(line);
               currentLineCount++;

              // Yield a chunk when reaching the linesPerChunk limit
              if (currentLineCount >= linesPerChunk)
              {
                yield return chunkBuilder.ToString();
                chunkBuilder.Clear();
                currentLineCount = 0;
              }
             }

             // Yield any remaining lines in the buffer at the end
             if (chunkBuilder.Length > 0)
             {
              yield return chunkBuilder.ToString();
             }
           }
        }
    }
}
