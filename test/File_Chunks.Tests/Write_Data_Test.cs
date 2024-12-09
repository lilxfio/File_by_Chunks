using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Write_Data_1;

namespace Write_data_1.Tests
{
    [TestClass]
    public class Write_DataTests
    {
        public  string Test_File="";

        [TestInitialize]
        public void SetUp()
        {
            // Set up the path for the test file in the system's temp directory
            Test_File = Path.Combine(Path.GetTempPath(), "test_output.csv");
        }

        [TestCleanup]
        public void TearDown()
        {
            // Clean up the test file after each test
            if (File.Exists(Test_File))
            {
                File.Delete(Test_File);
            }
        }

        [TestMethod]
        public async Task Write_Chunks_Should_Write_Multiple_Chunks_Correctly()
        {
            var chunks = CreateChunks("First chunk of data.", "Second chunk of data.");
            var writer = new Write_Data();

            await writer.WriteChunksAsync(Test_File, chunks);

            // Check if file was created and has correct content
            Assert.IsTrue(File.Exists(Test_File));
            string writtenContent = await File.ReadAllTextAsync(Test_File);
            string expectedContent = "First chunk of data.Second chunk of data.";

            Assert.AreEqual(expectedContent, writtenContent, "File content does not match expected data.");
        }

        [TestMethod]
        public async Task Write_Chunks_Should_Handle_Empty_Chunks_Gracefully()
        {
            var chunks = CreateChunks("", "Valid chunk after empty chunk.");
            var writer = new Write_Data();

            await writer.WriteChunksAsync(Test_File, chunks);

            // Verify content written ignores empty chunk
            Assert.IsTrue(File.Exists(Test_File));
            string writtenContent = await File.ReadAllTextAsync(Test_File);
            string expectedContent = "Valid chunk after empty chunk.";

            Assert.AreEqual(expectedContent, writtenContent, "File content does not match expected data when handling empty chunks.");
        }

        [TestMethod]
        public async Task Write_Chunks_Should_Overwrite_Existing_File()
        {
            var initialChunks = CreateChunks("Initial file content.");
            var writer = new Write_Data();

            // First, write initial content
            await writer.WriteChunksAsync(Test_File, initialChunks);

            // Verify initial content
            string initialContent = await File.ReadAllTextAsync(Test_File);
            Assert.AreEqual("Initial file content.", initialContent, "Initial file content is incorrect.");

            // Write new content that should overwrite the previous content
            var newChunks = CreateChunks("New content overwriting the file.");
            await writer.WriteChunksAsync(Test_File, newChunks);

            // Verify file now has the new content only
            string updatedContent = await File.ReadAllTextAsync(Test_File);
            Assert.AreEqual("New content overwriting the file.", updatedContent, "File was not overwritten with new content as expected.");
        }

        [TestMethod]
        public async Task Write_Chunks_Should_Create_File_If_Not_Exist()
        {
            var chunks = CreateChunks("Chunk in a new file.");
            var writer = new Write_Data();

            // Ensure the file does not exist before running the test
            if (File.Exists(Test_File))
            {
                File.Delete(Test_File);
            }

            // Write chunks to a new file
            await writer.WriteChunksAsync(Test_File, chunks);

            // Verify the file exists and has the correct content
            Assert.IsTrue(File.Exists(Test_File), "File was not created.");
            string writtenContent = await File.ReadAllTextAsync(Test_File);
            Assert.AreEqual("Chunk in a new file.", writtenContent, "Content in the newly created file is incorrect.");
        }

        // Helper method to create IAsyncEnumerable of chunks.
        private static async IAsyncEnumerable<string> CreateChunks(params string[] chunks)
        {
            foreach (var chunk in chunks)
            {
                yield return chunk;
                await Task.Yield(); // Ensures asynchronous behavior for testing
            }
        }
    }
}

