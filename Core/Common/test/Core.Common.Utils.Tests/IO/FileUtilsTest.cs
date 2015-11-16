using System;
using System.Diagnostics;
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

            Assert.IsTrue(FileUtils.IsSubdirectory(rootDir, "D:\\habitat\\kaarten"));
        }

        [Test]
        public void IsSubDirectoryOrEqualsTest()
        {
            const string rootDir = "D:/Habitat";

            Assert.IsFalse(FileUtils.IsSubdirectoryOrEquals(rootDir, "D:/Habitat Kaarten/"));

            Assert.IsTrue(FileUtils.IsSubdirectoryOrEquals(rootDir, "D:/Habitat/Kaarten/"));

            Assert.IsTrue(
                FileUtils.IsSubdirectoryOrEquals(rootDir, "D:/Habitat/Natura2000 presentation 3.prj_data/109/kaart.bil"));

            Assert.IsFalse(FileUtils.IsSubdirectoryOrEquals(rootDir, "C:/Habitat/Kaarten/"));

            Assert.IsTrue(FileUtils.IsSubdirectoryOrEquals(rootDir, "D:/habitat/kaarten/"));

            Assert.IsTrue(FileUtils.IsSubdirectoryOrEquals(rootDir, rootDir));
        }

        [Test]
        public void GetRelativePath()
        {
            string projectpath = @"c:\project\composite\";
            string fullpath = @"c:\project\model1\model1.mdl";

            Assert.AreEqual(@"..\model1\model1.mdl", FileUtils.GetRelativePath(projectpath, fullpath));
        }

        [Test]
        public void GetRelativePathOnDifferentDisks()
        {
            string projectpath = @"c:\project\composite\";
            string fullpath = @"d:\project\model1\model1.mdl";

            Assert.AreEqual(@"d:\project\model1\model1.mdl", FileUtils.GetRelativePath(projectpath, fullpath));
        }

        [Test]
        public void ConvertToRelativePath()
        {
            FileInfo info = new FileInfo("./data/myfile.myext");
            string fullpath = @"c:\Project\data\myfile.myext";
            string projectpath = @"c:\project";

            string result = FileUtils.GetRelativePath(projectpath, fullpath);
            FileInfo resultFile = new FileInfo(result);
            Assert.AreEqual(info.FullName, resultFile.FullName);

            fullpath = @"c:\project\data\myfile.myext";
            projectpath = "c:/project";
            result = FileUtils.GetRelativePath(projectpath, fullpath);
            resultFile = new FileInfo(result);
            Assert.AreEqual(info.FullName, resultFile.FullName);
        }

        [Test]
        public void GetRelativePath_UsingRelativeFolderPath_ReturnsFullFilePath()
        {
            string filePath = "myfolder\\myfile.txt";
            string folderPath = "otherfolder";

            string result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(filePath, result);
        }

        [Test]
        public void GetRelativePath_UsingAbsoluteFolderPath_ReturnsRelativeFilePath()
        {
            string filePath = "C:\\myfolder\\myfile.txt";
            string folderPath = "C:\\myfolder";

            string result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(".\\myfile.txt", result);
        }

        [Test]
        public void GetRelativePath_UsingNetworkPathPath_ReturnsRelativeFilePath()
        {
            string filePath = "\\\\myfolder\\myfile.txt";
            string folderPath = "\\\\myfolder";

            string result = FileUtils.GetRelativePath(folderPath, filePath);

            Assert.AreEqual(".\\myfile.txt", result);
        }

        [Test]
        public void CompareDirectories()
        {
            Assert.IsTrue(FileUtils.CompareDirectories(@"./data/myfile.myext", @"data\myfile.myext"));
        }

        //Todo Bad design here (http://forums.whirlpool.net.au/forum-replies-archive.cfm/887699.html)
        [Test]
        public void CanCopy()
        {
            //create file
            try
            {
                FileInfo fi = new FileInfo("CanCopy.txt");
                fi.Create();
                Assert.IsTrue(FileUtils.CanCopy(fi.Name, TestHelper.GetCurrentMethodName()));
                fi.Delete();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
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
            Assert.AreEqual(ExpectedChecksumForFileWithContent, checkSum);
        }

        [Test]
        public void VerifyChecksumTest()
        {
            // setup
            var testPath = TestHelper.GetTestDataPath(TestDataPath.Common.CoreCommonUtilsTests, "Test.txt");

            // call & assert
            Assert.IsTrue(FileUtils.VerifyChecksum(fileWithContentPath, ExpectedChecksumForFileWithContent));
            Assert.IsFalse(FileUtils.VerifyChecksum(testPath, ExpectedChecksumForFileWithContent));
        }

        private const string ExpectedChecksumForFileWithContent = "ec6ff8d1dbda4ecf65665fd7a3a057f2";
    }
}