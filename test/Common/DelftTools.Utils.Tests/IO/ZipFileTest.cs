using System;
using System.Collections.Generic;
using System.IO;
using DelftTools.TestUtils;
using DelftTools.Utils.IO;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.IO
{
    [TestFixture]
    public class ZipFileTest
    {
        [Test]
        public void ExtractFromZipFile()
        {
            File.Delete("LogoNLMW.xml");
            ZipFileUtils.Extract(@"..\..\..\..\..\test-data\Common\DelftTools.Tests\zipfile\MetaData.zip", ".");
            Assert.IsTrue(File.Exists("LogoNLMW.xml"));
        }

        [Test]
        public void CreateEmptyZipFile()
        {
            var path = TestHelper.GetCurrentMethodName() + ".zip";

            DeleteFilesIfExist(new[]
            {
                path
            });

            ZipFileUtils.Create(path, null);
            var fileExists = File.Exists(path);

            DeleteFilesIfExist(new[]
            {
                path
            });

            Assert.IsTrue(fileExists);
        }

        [Test]
        public void CreateEmptyZipFileWithOverwrite()
        {
            var path = TestHelper.GetCurrentMethodName() + ".zip";
            DeleteFilesIfExist(new[]
            {
                path
            });

            File.Create(path).Close();

            try
            {
                ZipFileUtils.Create(path, null, true);
            }
            catch (Exception)
            {
                DeleteFilesIfExist(new[]
                {
                    path
                });
                Assert.Fail("File should be overwritten");
            }

            var fileExists = File.Exists(path);

            DeleteFilesIfExist(new[]
            {
                path
            });

            Assert.IsTrue(fileExists);
        }

        [Test]
        public void CreateEmptyZipFileWithoutOverwrite()
        {
            var path = TestHelper.GetCurrentMethodName() + ".zip";

            DeleteFilesIfExist(new[]
            {
                path
            });

            File.Create(path).Close();

            try
            {
                ZipFileUtils.Create(path, null, false);
                Assert.Fail("ZipFileUtils.Create should throw an error if file already exists and the parameter 'overwriteIfExits' is false.");
            }
            catch (Exception)
            {
                DeleteFilesIfExist(new[]
                {
                    path
                });
            }
        }

        [Test]
        public void CreateZipFileWithFiles()
        {
            var zipFilePath = TestHelper.GetCurrentMethodName() + ".zip";
            const string file1Path = @".\file1.txt";
            const string file2Path = @".\file2.txt";

            DeleteFilesIfExist(new[]
            {
                zipFilePath,
                file1Path,
                file2Path
            });

            var file1 = File.CreateText(file1Path);
            var file2 = File.CreateText(file2Path);

            file1.WriteLine("TestFile1");
            file2.WriteLine("TestFile2");

            file1.Close();
            file2.Close();

            ZipFileUtils.Create(zipFilePath, new List<string>
            {
                file1Path, file2Path
            });

            var zipfileExists = File.Exists(zipFilePath);

            DeleteFilesIfExist(new[]
            {
                file1Path,
                file2Path
            });

            ZipFileUtils.Extract(zipFilePath, ".");

            var file1Exists = File.Exists(file1Path);
            var file2Exists = File.Exists(file2Path);

            DeleteFilesIfExist(new[]
            {
                zipFilePath,
                file1Path,
                file2Path
            });

            Assert.IsTrue(zipfileExists);
            Assert.IsTrue(file1Exists);
            Assert.IsTrue(file2Exists);
        }

        [Test]
        public void CreateZipFileWithFilesAndADirectory()
        {
            var zipFilePath = TestHelper.GetCurrentMethodName() + ".zip";
            const string file1Path = @".\file1.txt";
            const string file2Path = @".\dir\file2.txt";

            DeleteFilesIfExist(new[]
            {
                zipFilePath,
                file1Path,
                file2Path
            });

            Directory.CreateDirectory(@".\dir");

            CreateTestFile(file1Path);
            CreateTestFile(file2Path);

            ZipFileUtils.Create(zipFilePath, new List<string>
            {
                file1Path, file2Path
            });

            var zipfileExists = File.Exists(zipFilePath);

            Directory.Delete(@".\dir", true);
            DeleteFilesIfExist(new[]
            {
                file1Path,
                file2Path
            });

            ZipFileUtils.Extract(zipFilePath, ".");

            DeleteFilesIfExist(new[]
            {
                zipFilePath
            });

            var file1Exists = File.Exists(file1Path);
            var file2Exists = File.Exists(file2Path);

            DeleteFilesIfExist(new[]
            {
                zipFilePath,
                file1Path,
                file2Path
            });

            Assert.IsTrue(zipfileExists);
            Assert.IsTrue(file1Exists);
            Assert.IsTrue(file2Exists);
        }

        private static void CreateTestFile(string file1Path)
        {
            var file1 = File.CreateText(file1Path);
            file1.WriteLine(file1Path);
            file1.Close();
        }

        private static void DeleteFilesIfExist(string[] paths)
        {
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}