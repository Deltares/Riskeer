using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using Core.Common.TestUtils;
using Core.Common.Utils.IO;
using Core.Common.Utils.Tests.Properties;

using log4net;
using NUnit.Framework;

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
            // common
            var path = TestHelper.GetTestDataPath(TestDataPath.Common.Base.CoreCommonBaseTests);
            Assert.IsTrue(Directory.Exists(path));

            // Ringtoets
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.CorePluginsSharpMapGisTests);
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

            Assert.AreEqual("Red", colorRed.Name);
            Assert.AreEqual("fffcfe00", colorYellow1.Name);
            Assert.AreEqual("fffcfe00", colorYellow2.Name);
            Assert.AreEqual("fffcfe00", colorYellow3.Name);
            Assert.AreEqual("ff008000", colorGreen.Name);

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
        public void TestAssertLogMessagesCount()
        {
            TestHelper.AssertLogMessagesCount(() => log.Error("test 1"), 1);
            TestHelper.AssertLogMessagesCount(() => log.Warn("test 2"), 1);
            TestHelper.AssertLogMessagesCount(() => log.Info("test 3"), 1);
        }

        [Test]
        public void AssertImagesAreEqual_TwoIdenticalImages_NoAssertionErrors()
        {
            // Setup
            Bitmap image = Resources.abacus;

            // Call
            TestDelegate call = () => TestHelper.AssertImagesAreEqual(image, image);
            
            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssertImagesAreEqual_TwoDifferentImages_ThrowAssertionException()
        {
            // Setup
            Bitmap image1 = Resources.abacus;
            Bitmap image2 = Resources.acorn;

            // Call
            TestDelegate call = () => TestHelper.AssertImagesAreEqual(image1, image2);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertImagesAreEqual_ExpectedIsNullButActualIsNot_ThrowAssertionException()
        {
            // Call
            TestDelegate call = () => TestHelper.AssertImagesAreEqual(null, Resources.acorn);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertImagesAreEqual_ActualIsNullButExpectingImage_ThrowAssertionException()
        {
            // Call
            TestDelegate call = () => TestHelper.AssertImagesAreEqual(Resources.acorn, null);

            // Assert
            Assert.Throws<AssertionException>(call);
        }
    }
}