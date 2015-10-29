using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using Core.Common.TestUtils;
using Core.Common.Utils.IO;
using log4net;
using NUnit.Framework;
using SharpTestsEx;

namespace Core.Common.Utils.Tests
{
    [TestFixture]
    public class TestHelperTests
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestHelperTests));

        [Test]
        public void TestDataDirectory()
        {
            var dir = TestHelper.TestDataDirectory;
            Assert.IsTrue(Directory.Exists(dir));
        }

        [Test]
        public void GetFullPathForTestFile()
        {
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), ".."));
            string fullPath = TestHelper.GetTestFilePath("Test.txt");
            Assert.IsTrue(File.Exists(fullPath));
        }

        [Test]
        public void TestPredefinedTestPath()
        {
            //common
            var path = TestHelper.GetTestDataPath(TestDataPath.Common.DelftTools.DelftToolsTests);
            Assert.IsTrue(Directory.Exists(path));

            //deltashell
            path = TestHelper.GetTestDataPath(TestDataPath.DeltaShell.DeltaShellDeltaShellPluginsSharpMapGisTests);
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void TestSolutionRoot()
        {
            Assert.IsTrue(Directory.Exists(TestHelper.SolutionRoot));
        }

        [Test]
        public void TestAssertLogMessage()
        {
            TestHelper.AssertLogMessageIsGenerated(() => log.Warn("hello"), "hello");
            TestHelper.AssertLogMessageIsGenerated(() => log.Debug("hello"), "hello");
            TestHelper.AssertLogMessageIsGenerated(() => log.Error("hello"), "hello");
        }

        [Test]
        public void GetPerformanceColors()
        {
            var type = typeof(TestHelper);
            var methodInfo = type.GetMethod("GetPerformanceColor", BindingFlags.NonPublic | BindingFlags.Static);

            var colorRed = (Color) methodInfo.Invoke(null, new object[]
            {
                -0.05
            });
            var colorYellow1 = (Color) methodInfo.Invoke(null, new object[]
            {
                0.0
            });
            var colorYellow2 = (Color) methodInfo.Invoke(null, new object[]
            {
                1.0
            });
            var colorYellow3 = (Color) methodInfo.Invoke(null, new object[]
            {
                1.05
            });
            var colorGreen = (Color) methodInfo.Invoke(null, new object[]
            {
                0.25
            });

            colorRed.Name
                    .Should().Be.EqualTo("Red");
            colorYellow1.Name
                        .Should().Be.EqualTo("fffcfe00");
            colorYellow2.Name
                        .Should().Be.EqualTo("fffcfe00");
            colorYellow3.Name
                        .Should().Be.EqualTo("fffcfe00");
            colorGreen.Name
                      .Should().Be.EqualTo("ff008000");

            // dump all colors to html file for visual test
            const string path = "GetPerformanceColors.html";
            FileUtils.DeleteIfExists(path);
            var contents = "<table>";
            for (var i = -0.05; i <= 1.1; i += 0.05)
            {
                var color = (Color) methodInfo.Invoke(null, new object[]
                {
                    i
                });
                var htmlColor = ColorTranslator.ToHtml(color);
                contents += string.Format(CultureInfo.InvariantCulture, "<tr><td bgcolor=\"{0}\">{1:G5}%</td></tr>", htmlColor, i);
            }
            contents += "</table>";

            File.AppendAllText(path, contents);
        }

        [Test]
        public void TestAssertLogExpectedAndUnwantedMessages()
        {
            TestHelper.AssertLogExpectedAndUnwantedMessages(() => log.Error("Test 1"),
                                                            new[]
                                                            {
                                                                "Test 1"
                                                            },
                                                            new[]
                                                            {
                                                                "Test1",
                                                                "Test 2"
                                                            });
            TestHelper.AssertLogExpectedAndUnwantedMessages(() => log.Warn("Test 1"),
                                                            new[]
                                                            {
                                                                "Test 1"
                                                            },
                                                            new[]
                                                            {
                                                                "Test1",
                                                                "Test 2"
                                                            });
            TestHelper.AssertLogExpectedAndUnwantedMessages(() => log.Info("Test 1"),
                                                            new[]
                                                            {
                                                                "Test 1"
                                                            },
                                                            new[]
                                                            {
                                                                "Test1",
                                                                "Test 2"
                                                            });
        }

        [Test]
        public void TestAssertLogMessagesCount()
        {
            TestHelper.AssertLogMessagesCount(() => log.Error("test 1"), 1);
            TestHelper.AssertLogMessagesCount(() => log.Warn("test 2"), 1);
            TestHelper.AssertLogMessagesCount(() => log.Info("test 3"), 1);
        }
    }
}