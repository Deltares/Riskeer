﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Common.Utils.Test.Properties;
using log4net;
using NUnit.Framework;

namespace Core.Common.Utils.Test
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
            DeleteIfExists(path);
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

        private static void DeleteIfExists(string path)
        {
            if (!File.Exists(path) & !Directory.Exists(path))
            {
                return;
            }

            var attributes = File.GetAttributes(path);

            // if file is readonly - make it non-readonly
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(path, attributes ^ FileAttributes.ReadOnly);
            }

            // now delete everything
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (var path2 in Directory.GetDirectories(path).Union(Directory.GetFiles(path)))
                {
                    DeleteIfExists(path2);
                }
                Directory.Delete(path);
            }
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

        [Test]
        public void AssertContextMenuStripContainsItem_MenuNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate call = () => TestHelper.AssertContextMenuStripContainsItem(null, 0, "", "", null);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_NoMenuItemAtPosition_ThrowsAssertionException()
        {
            // Call
            TestDelegate call = () => TestHelper.AssertContextMenuStripContainsItem(new ContextMenuStrip(), 0, "", "", null);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_MenuItemWithDifferentText_ThrowsAssertionException()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            var testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () =>
            {
                TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text + "someThing", testItem.ToolTipText, testItem.Image);
            };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_MenuItemWithDifferentToolTip_ThrowsAssertionException()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            var testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () =>
            {
                TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText + "someThing", testItem.Image);
            };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_MenuItemWithDifferentImage_ThrowsAssertionException()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            var testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () =>
            {
                TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, Resources.acorn);
            };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssertContextMenuStripContainsItem_MenuItemWithDifferentEnabeldState_ThrowsAssertionException(bool enabled)
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            var testItem = CreateContextMenuItem();
            testItem.Enabled = enabled;
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () =>
            {
                TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, testItem.Image, !enabled);
            };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_SameMenuItemProperties_NoExceptions()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            var testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call & Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, testItem.Image);
        }

        [Test]
        public void AssertExceptionCustomMessage_NoException_ThrowsAssertionException()
        {
            // Setup
            TestDelegate t = () => { };

            // Call
            TestDelegate call = () =>
            {
                TestHelper.AssertExceptionCustomMessage<Exception>(t, String.Empty);
            };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertExceptionCustomMessage_ExceptionIncorrectMessage_ThrowsAssertionException()
        {
            // Setup
            var someMessage = "Exception";
            var differentMessage = "Different";
            TestDelegate t = () => { throw new Exception(someMessage); };

            // Call
            TestDelegate call = () =>
            {
                TestHelper.AssertExceptionCustomMessage<Exception>(t, differentMessage);
            };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertExceptionCustomMessage_ExceptionEqualMessage_NoExceptions()
        {
            // Setup
            var someMessage = "Exception";
            TestDelegate t = () => { throw new Exception(someMessage); };

            // Call & Assert
            TestHelper.AssertExceptionCustomMessage<Exception>(t, someMessage);
        }

        private static ToolStripMenuItem CreateContextMenuItem()
        {
            return new ToolStripMenuItem
            {
                Text = @"aText",
                ToolTipText = @"aToolTipText",
                Image = Resources.abacus
            };
        }
    }
}