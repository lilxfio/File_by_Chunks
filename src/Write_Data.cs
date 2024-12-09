using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Write_Data_1
{
    /// <summary>
    /// Provides functionality to write large text data to a file in chunks asynchronously.
    /// </summary>
    public class Write_Data
    {
        /// <summary>
        /// Asynchronously writes chunks of data to a specified destination file.
        /// </summary>
        /// <param name="destinationFile">The path to the file where data should be written.</param>
        /// <param name="chunks">An asynchronous enumerable of strings, each representing a chunk of data to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <remarks>
        /// This method uses an 8 MB buffer and writes each chunk of data asynchronously. Each chunk
        /// is appended directly to the file, making it suitable for handling large data sets without
        /// loading all content into memory at once.
        /// 
        /// The method creates or overwrites the destination file specified by <paramref name="destinationFile"/>.
        /// </remarks>
        /// <example>
        /// The following example demonstrates how to use <see cref="WriteChunksAsync"/> to write data to a file:
        /// <code>
        /// var writer = new Write_Data();
        /// await writer.WriteChunksAsync("path/to/destinationfile.txt", chunks);
        /// </code>
        /// </example>
        public async Task WriteChunksAsync(string destinationFile, IAsyncEnumerable<string> chunks)
        {
            int bufferSize = 8 * 1024 * 1024; // 8MB buffer

            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
            using (StreamWriter writer = new StreamWriter(destinationStream))
            {
                await foreach (string chunk in chunks)
                {
                    await writer.WriteAsync(chunk);
                }
            }
        }
    }
}

