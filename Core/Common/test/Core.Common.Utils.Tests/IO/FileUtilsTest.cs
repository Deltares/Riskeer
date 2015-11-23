using System;
using System.IO;
using System.Threading;
using Core.Common.TestUtils;
using Core.Common.Utils.IO;
using NUnit.Framework;

namespace Core.Common.Utils.Tests.IO
{
    /// <summary/>
    [TestFixture]
    public class FileUtilsTest
    {
        private readonly string fileWithContentPath = TestHelper.GetTestDataPath(TestDataPath.Common.CoreCommonUtilsTests, "File_with_content.txt");
        private const int allowedTimeForDelete = 2000;
        private const string expectedChecksumForFileWithContent = "ec6ff8d1dbda4ecf65665fd7a3a057f2";

        private static bool DoesDirectoryExistsAfterTimeout(string directory)
        {
            var start = DateTime.Now;
            while (Directory.Exists(directory))
            {
                // Due to a race condition on the FileSystem, a while loop was required
                var timeSpan = DateTime.Now - start;
                if (timeSpan.Milliseconds > allowedTimeForDelete)
                {
                    return true;
                }
                Thread.Sleep(50);
            }
            return false;
        }

        [Test]
        public void IsSubDirectoryTest()
        {
            const string rootDir = "D:/Habitat";

            Assert.IsFalse(FileUtils.IsSubdirectory(rootDir, "D:/Habitat Kaarten/"));

            Assert.IsTrue(FileUtils.IsSubdirectory(rootDir, "D:/Habitat/Kaarten/"));

            Assert.IsTrue(
                FileUtils.IsSubdirectory(rootDir, "D:/Habitat/Natura2000 presentation 3.prj_data/109/kaart.bil"));

            Assert.IsFalse(FileUtils.IsSubdirectory(rootDir, "C:/Habitat/Kaarten/"));

            Assert.IsTrue(FileUtils.IsSubdirectory(rootDir, "D:/habitat/kaarten/"));

            Assert.IsTrue(FileUtils.IsSubdirectory(rootDir, @"D:\habitat\kaarten"));
        }

        [Test]
        public void GetRelativePath_UsingSubfolder_ReturnsAbsolutePath()
        {
            const string projectpath = @"c:\project\composite\";
            const string fullpath = @"c:\project\model1\model1.mdl";

            Assert.AreEqual(@"..\model1\model1.mdl", FileUtils.GetRelativePath(projectpath, fullpath));
        }

        [Test]
        public void GetRelativePath_UsingOtherCasing_ReturnsAbsolutePath()
        {
            const string projectpath = @"c:\Project\composite\";
            const string fullpath = @"c:\project\model1\model1.mdl";

            Assert.AreEqual(@"..\model1\model1.mdl", FileUtils.GetRelativePath(projectpath, fullpath));
        }

        [Test]
        public void GetRelativePath_OnDifferentDrives_RetunsFullPath()
        {
            const string projectpath = @"c:\project\composite\";
            const string fullpath = @"d:\project\model1\model1.mdl";

            Assert.AreEqual(@"d:\project\model1\model1.mdl", FileUtils.GetRelativePath(projectpath, fullpath));
        }

        [Test]
        public void GetRelativePath_UsingFileInfo_ReturnsAbsolutePath()
        {
            var info = new FileInfo("./data/myfile.myext");
           
            const string fullpath = @"c:\project\data\myfile.myext";
            const string projectpath = @"c:\project";
            var result = FileUtils.GetRelativePath(projectpath, fullpath);
            var resultFile = new FileInfo(result);

            Assert.AreEqual(info.FullName, resultFile.FullName);
        }

        [Test]
        public void GetRelativePath_UsingRelativeFolderPath_ReturnsFullFilePath()
        {
            const string filePath = @"myfolder\myfile.txt";
            const string folderPath = "otherfolder";

            var result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(filePath, result);
        }

        [Test]
        public void GetRelativePath_UsingAbsoluteFolderPath_ReturnsRelativeFilePath()
        {
            const string filePath = @"C:\myfolder\myfile.txt";
            const string folderPath = @"C:\myfolder";
            var result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(@"myfile.txt", result);
        }

        [Test]
        public void GetRelativePath_UsingAbsoluteFolderPathInAdjacentFolder_ReturnsRelativeFilePath()
        {
            const string filePath = @"C:\folder\myfile.txt";
            const string folderPath = @"C:\myfolder";
            var result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(@"..\folder\myfile.txt", result);
        }

        [Test]
        public void GetRelativePath_UsingNetworkPathPath_ReturnsRelativeFilePath()
        {
            const string filePath = @"\\myfolder\myfile.txt";
            const string folderPath = @"\\myfolder";
            var result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(@".\myfile.txt", result);
        }

        [Test]
        public void GetRelativePath_UsingNetworkPathPathFromAdjacentFolder_ReturnsRelativeFilePath()
        {
            const string filePath = @"\\share\folder\myfile.txt";
            const string folderPath = @"\\share\myfolder";
            var result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(@"..\folder\myfile.txt", result);
        }

        [Test]
        public void CompareDirectories__ReturnsTrue()
        {
            Assert.IsTrue(FileUtils.CompareDirectories("./data/myfile.myext", @"data\myfile.myext"));
        }

        [Test]
        public void CompareDirectories_UsingExtensivePath_ReturnsTrue()
        {
            Assert.IsTrue(FileUtils.CompareDirectories("./data/myfile.myext", @"data\..\data\myfile.myext"));
        }
        
        [Test]
        public void DeleteFileIfItExists()
        {
            // no error if it does not exist
            FileUtils.DeleteIfExists("somefile.nc");

            File.Create("somefile.nc").Close();
            FileUtils.DeleteIfExists("somefile.nc");
            Assert.IsFalse(File.Exists("somefile.nc"));
        }

        [Test]
        public void DeleteEmptyDirectoryIfItExists()
        {
            // no error if it does not exist
            FileUtils.DeleteIfExists("mydir");


            FileUtils.CreateDirectoryIfNotExists("mydir");
            FileUtils.DeleteIfExists("mydir");

            if (DoesDirectoryExistsAfterTimeout("mydir"))
            {
                Assert.Fail("Given {0} ms to delete the file.", allowedTimeForDelete);
            }
        }

        [Test]
        public void DeleteNonEmptyDirectoryIfItExists()
        {
            FileUtils.CreateDirectoryIfNotExists("mydir2");
            FileUtils.CreateDirectoryIfNotExists("mydir2/subdir");
            File.Create("mydir2/somefile.nc").Close();

            FileUtils.DeleteIfExists("mydir2");

            if (DoesDirectoryExistsAfterTimeout("mydir2"))
            {
                Assert.Fail("Given {0} ms to delete the file.", allowedTimeForDelete);
            }
        }

        [Test]
        public void CopyToSameFileDoesNotDeleteIt()
        {
            var file = Path.GetTempFileName();
            FileUtils.CopyFile(file, file);

            Assert.IsTrue(File.Exists(file));
        }

        [Test]
        public void DeleteIfExistsSkipsReadOnlyAttributesOnFiles()
        {
            var path = "DeleteIfExistsSkipsReadOnlyAttributesOnFiles.txt";

            // clean-up from previous run
            FileUtils.DeleteIfExists(path);

            // create read-only file in source directory
            File.CreateText(path).Close();
            File.SetAttributes(path, FileAttributes.ReadOnly);

            FileUtils.DeleteIfExists(path); // crashes on System.IO.File.Delete
        }

        [Test]
        public void DeleteIfExistsSkipsReadOnlyAttributesOnDirectories()
        {
            var path = "DeleteIfExistsSkipsReadOnlyAttributesOnDirectories";

            // clean-up from previous run
            FileUtils.DeleteIfExists(path);

            // create read-only file in source directory
            Directory.CreateDirectory(path);
            File.SetAttributes(path, FileAttributes.ReadOnly);

            var pathSubdir = Path.Combine(path, "subdir");
            Directory.CreateDirectory(pathSubdir);
            File.SetAttributes(pathSubdir, FileAttributes.ReadOnly);

            FileUtils.DeleteIfExists(path); // crashes on System.IO.File.Delete
        }

        [Test]
        public void IsValidFileNameTest()
        {
            Assert.IsTrue(FileUtils.IsValidFileName("name.txt"));
            Assert.IsTrue(FileUtils.IsValidFileName("name,txt"));
            Assert.IsFalse(FileUtils.IsValidFileName("name/txt"));
            Assert.IsFalse(FileUtils.IsValidFileName(" "));
            Assert.IsFalse(FileUtils.IsValidFileName(null));
        }

        [Test]
        public void CheckIsDirectoryEmpty()
        {
            var path = "EmptyDirectory";
            FileUtils.CreateDirectoryIfNotExists(path, true);
            Assert.IsTrue(FileUtils.IsDirectoryEmpty(path));
            FileUtils.CreateDirectoryIfNotExists(path + Path.DirectorySeparatorChar + path);
            Assert.IsFalse(FileUtils.IsDirectoryEmpty(path));
            // cleanup
            FileUtils.DeleteIfExists(path);
        }

        [Test]
        public void GetChecksumTest()
        {
            // call
            var checkSum = FileUtils.GetChecksum(fileWithContentPath);

            // assert
            Assert.AreEqual(32, checkSum.Length,
                            "A 128bit hash should yield 16 bytes of data. Printed in hex, gives 2*16 = 32 characters.");
            Assert.AreEqual(expectedChecksumForFileWithContent, checkSum);
        }

        [Test]
        public void VerifyChecksumTest()
        {
            // setup
            var testPath = TestHelper.GetTestDataPath(TestDataPath.Common.CoreCommonUtilsTests, "Test.txt");

            // call & assert
            Assert.IsTrue(FileUtils.VerifyChecksum(fileWithContentPath, expectedChecksumForFileWithContent));
            Assert.IsFalse(FileUtils.VerifyChecksum(testPath, expectedChecksumForFileWithContent));
        }
    }
}