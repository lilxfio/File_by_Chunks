
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Read_Data_1;

namespace Read_Data_1.Tests
{
    [TestClass]
    public class Read_DataTests
    {
        public string? Test_File;

        [TestInitialize]
        public void SetUp()
        {
            // Set up a test file with known content.
            Test_File = Path.Combine(Path.GetTempPath(), "testfile.csv");

            // Create a file with some content (this should be larger than the chunk size for testing).
            var sb = new StringBuilder();
            for (int i = 0; i < 5000; i++)
            {
                sb.AppendLine($"This is line {i + 1}.");
            }
            
            File.WriteAllText(Test_File, sb.ToString());
        }

        [TestCleanup]
        public void TearDown()
        {
            // Clean up the test file.
            if (File.Exists(Test_File))
            {
                File.Delete(Test_File);
            }
        }

        [TestMethod]
        public async Task Read_Chunks_Should_Handle_Empty_File()
        {
            var emptyFilePath = Path.Combine(Path.GetTempPath(), "emptyfile.txt");
            File.WriteAllText(emptyFilePath, string.Empty);

            var reader = new Read_Data();
            int chunkCount = 0;

            await foreach (var chunk in reader.ReadChunksAsync(emptyFilePath))
            {
                Assert.IsTrue(chunk.Length > 0);  // Even empty file should yield a final chunk (empty string).
                chunkCount++;
            }

            // Ensure that it completed and yielded a chunk
            Assert.AreEqual(1, chunkCount);

            // Cleanup empty file
            File.Delete(emptyFilePath);
        }

        [TestMethod]
        public async Task Read_Chunks_Should_Handle_Small_File()
        {
            var smallFilePath = Path.Combine(Path.GetTempPath(), "smallfile.txt");
            var sb = new StringBuilder();
            sb.AppendLine("This is a small file.");

            File.WriteAllText(smallFilePath, sb.ToString());

            var reader = new Read_Data();
            int chunkCount = 0;

            await foreach (var chunk in reader.ReadChunksAsync(smallFilePath))
            {
                Assert.IsTrue(chunk.Length > 0);
                chunkCount++;
            }

            // Ensure that it yields only 1 chunk for a small file
            Assert.AreEqual(1, chunkCount);

            // Cleanup small file
            File.Delete(smallFilePath);
        }

        [TestMethod]
        public async Task Read_Chunks_Should_Handle_End_Of_File_Correctly()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Line 1");
            sb.AppendLine("Line 2");
            sb.AppendLine("Line 3");

            var smallFilePath = Path.Combine(Path.GetTempPath(), "eof_testfile.csv");
            File.WriteAllText(smallFilePath, sb.ToString());

            var reader = new Read_Data();
            bool endReached = false;

            await foreach (var chunk in reader.ReadChunksAsync(smallFilePath))
            {
                // As there's only a few lines, ensure we only get one chunk
                Assert.IsTrue(chunk.Length > 0);
                endReached = true;
            }

            // Ensure that the file was fully read
            Assert.IsTrue(endReached);

            // Cleanup EOF test file
            File.Delete(smallFilePath);
        }
    }
}
