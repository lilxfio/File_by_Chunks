using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Procces_File_1;

namespace procces_file_1.Tests
{
    [TestClass]
    public class Procces_FilesTests
    {
        private string Test_File_in="";
        private string Test_File_out="";

        [TestInitialize]
        public void SetUp()
        {
            // Set up temporary file paths for the source and destination files
            Test_File_in = Path.Combine(Path.GetTempPath(), "test_source.csv");
            Test_File_out = Path.Combine(Path.GetTempPath(), "test_destination.csv");
        }

        [TestCleanup]
        public void TearDown()
        {
            // Clean up both files after each test
            if (File.Exists(Test_File_in))
            {
                File.Delete(Test_File_in);
            }
            if (File.Exists(Test_File_out))
            {
                File.Delete(Test_File_out);
            }
        }

        [TestMethod]
        public async Task Process_File_Should_Transfer_Data_From_Source_To_Destination()
        {
            // Arrange: Create a source file with known content
            var content = new StringBuilder();
            for (int i = 0; i < 5000; i++)
            {
                content.AppendLine($"Line {i + 1}");
            }
            await File.WriteAllTextAsync(Test_File_in, content.ToString());

            // Act: Process the file
            await Procces_Files.ProcessFileAsync(Test_File_in, Test_File_out);

            // Assert: Verify the destination file has the same content as the source
            Assert.IsTrue(File.Exists(Test_File_out), "The destination file was not created.");
            string destinationContent = await File.ReadAllTextAsync(Test_File_out);
            Assert.AreEqual(content.ToString(), destinationContent, "Content in the destination file does not match the source.");
        }

        [TestMethod]
        public async Task Process_File_Should_Handle_Empty_Source_File()
        {
          // Arrange: Create an empty source file
          await File.WriteAllTextAsync(Test_File_in, string.Empty);

          // Act: Process the file
          await Procces_Files.ProcessFileAsync(Test_File_in, Test_File_out);

          // Assert: Ensure the destination file is also created and empty
          Assert.IsTrue(File.Exists(Test_File_out), "The destination file was not created for an empty source.");
    
          string destinationContent = await File.ReadAllTextAsync(Test_File_out);
          Assert.AreEqual(string.Empty, destinationContent, "The destination file is not empty when the source file is empty.");
        }


        [TestMethod]
        public async Task Process_File_Should_Handle_Non_Existent_Source_File()
        {
            // Act & Assert: Attempt to process a non-existent source file, expecting an exception or handling it gracefully
            try
            {
                await Procces_Files.ProcessFileAsync("non_existent_file.txt", Test_File_out);
                Assert.Fail("Expected an exception for a non-existent source file, but none was thrown.");
            }
            catch (FileNotFoundException)
            {
                // Expected behavior: FileNotFoundException is thrown
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An unexpected exception was thrown: {ex.Message}");
            }
        }
    }
}        