// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
using Core.Common.TestUtil.Test.Properties;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class TestHelperTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestHelperTest));

        private static IEnumerable<TestCaseData> AssertAreEqualButNotSameSource
        {
            get
            {
                var objectA = new TestEqualSameObject(1);
                var objectB = new TestEqualSameObject(1);
                var objectC = new TestEqualSameObject(2);
                yield return new TestCaseData(objectA, objectA, false).SetName("EqualAndSameObjects_False");
                yield return new TestCaseData(objectA, objectB, true).SetName("EqualNotSameObjects_True");
                yield return new TestCaseData(objectA, objectC, false).SetName("NotEqualNotSameObjects_False");
                yield return new TestCaseData(null, null, true).SetName("BothNull_True");
                yield return new TestCaseData(objectA, null, false).SetName("ObjectBNull_False");
                yield return new TestCaseData(null, objectB, false).SetName("ObjectANull_False");
            }
        }

        [Test]
        public void CanOpenFileForWrite_PathDoesNotExist_DoesNotThrowAnyExceptions()
        {
            const string invalidPath = @".\DirectoryDoesNotExist\fileDoesNotExist";
            var canOpenForWrite = true;

            // Call
            TestDelegate call = () => canOpenForWrite = TestHelper.CanOpenFileForWrite(invalidPath);

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsFalse(canOpenForWrite);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanOpenFileForWrite_InvalidPath_ThrowsException(string invalidPath)
        {
            // Call
            TestDelegate call = () => TestHelper.CanOpenFileForWrite(invalidPath);

            // Assert
            Assert.Catch(call);
        }

        [Test]
        public void CanOpenFileForWrite_ValidPath_DoesNotThrowAnyExceptions()
        {
            const string validPath = @".\fileDoesNotExist";
            var canOpenForWrite = false;

            // Call
            TestDelegate call = () => canOpenForWrite = TestHelper.CanOpenFileForWrite(validPath);

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsTrue(canOpenForWrite);

            // Cleanup
            Assert.IsTrue(File.Exists(validPath));
            File.Delete(validPath);
        }

        [Test]
        public void CanWriteInDirectory_PathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => TestHelper.CanWriteInDirectory(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CanWriteInDirectory_InvalidDirectory_ThrowsArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();
            string invalidPath = $"f*l{invalidPathChars[2]}er";

            // Call
            TestDelegate call = () => TestHelper.CanWriteInDirectory(invalidPath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void CanWriteInDirectory_DirectoryDoesNotExist_ReturnsFalse()
        {
            // Setup
            string validPath = Path.Combine(TestHelper.GetScratchPadPath(), nameof(CanWriteInDirectory_CanWriteInDirectory_ReturnsTrue));

            // Call
            bool canWrite = TestHelper.CanWriteInDirectory(validPath);

            // Assert
            Assert.IsFalse(canWrite);
        }

        [Test]
        public void CanWriteInDirectory_CanWriteInDirectory_ReturnsTrue()
        {
            // Setup
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(CanWriteInDirectory_CanWriteInDirectory_ReturnsTrue)))
            {
                string validPath = TestHelper.GetScratchPadPath(nameof(CanWriteInDirectory_CanWriteInDirectory_ReturnsTrue));

                // Call
                bool canWrite = TestHelper.CanWriteInDirectory(validPath);

                // Assert
                Assert.IsTrue(canWrite);
            }
        }

        [Test]
        public void CanWriteInDirectory_CanNotWriteInDirectory_ReturnsFalse()
        {
            // Setup
            using (var helper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(CanWriteInDirectory_CanWriteInDirectory_ReturnsTrue)))
            {
                string validPath = TestHelper.GetScratchPadPath(nameof(CanWriteInDirectory_CanWriteInDirectory_ReturnsTrue));
                helper.LockDirectory(FileSystemRights.Write);

                // Call
                bool canWrite = TestHelper.CanWriteInDirectory(validPath);

                // Assert
                Assert.IsFalse(canWrite);
            }
        }

        [Test]
        public void GetScratchPadPath_WithoutPath_ReturnRootFolder()
        {
            // Call
            string actualPath = TestHelper.GetScratchPadPath();

            // Assert
            string expectedPath = Path.Combine(Path.GetTempPath(), "Ringtoets_Scratchpad");
            Assert.AreEqual(expectedPath, actualPath);
            Assert.IsTrue(Directory.Exists(actualPath),
                          $"The directory '{expectedPath}' should exist, such that unit tests have a clean environment to temporarily write files and directories to.");
        }

        [Test]
        public void GetScratchPadPath_WithSubPath_ReturnPathInScratchPad()
        {
            // Setup
            string subPath = Path.Combine("test", "1.234");

            // Call
            string actualPath = TestHelper.GetScratchPadPath(subPath);

            // Assert
            string expectedPath = Path.Combine(Path.GetTempPath(), "Ringtoets_Scratchpad", subPath);
            Assert.AreEqual(expectedPath, actualPath);
            Assert.IsFalse(File.Exists(actualPath),
                           $"The file '{expectedPath}' should not exist, as the folder should always be empty at the start of any unit test.");
            Assert.IsFalse(Directory.Exists(actualPath),
                           $"The directory '{expectedPath}' should not exist, as the folder should always be empty at the start of any unit test.");
        }

        [Test]
        public void GetTestDataPath_Always_VerifiedTestPaths()
        {
            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Migration.Core);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Storage.Core);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Util);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.Service);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.Integration);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionOutwards.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.Integration);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.ClosingStructures.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityPointStructures.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.AssemblyTool.IO);
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void ToUncPath_InvalidPath_ThrowArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => TestHelper.ToUncPath(invalidPath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void ToUncPath_NonRootedPath_ThrowArgumentException()
        {
            // Setup
            string unrootedPath = Path.Combine("a", "b.c");

            // Call
            TestDelegate call = () => TestHelper.ToUncPath(unrootedPath);

            // Assert
            const string expectedMessage = "Must be a rooted path.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("rootedPath", paramName);
        }

        [Test]
        public void ToUncPath_ForRootedFilePath_ReturnUncPath()
        {
            // Setup
            const string rootedFilePath = @"C:\a\b.c";

            // Call
            string uncPath = TestHelper.ToUncPath(rootedFilePath);

            // Assert
            Assert.AreEqual(@"\\localhost\C$\a\b.c", uncPath);
        }

        [Test]
        public void ToUncPath_ForRootedFolderPath_ReturnUncPath()
        {
            // Setup
            const string rootedFolderPath = @"D:\e\f\g";

            // Call
            string uncPath = TestHelper.ToUncPath(rootedFolderPath);

            // Assert
            Assert.AreEqual(@"\\localhost\D$\e\f\g", uncPath);
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
        public void TestAssertLogMessagesCount()
        {
            TestHelper.AssertLogMessagesCount(() => log.Error("test 1"), 1);
            TestHelper.AssertLogMessagesCount(() => log.Warn("test 2"), 1);
            TestHelper.AssertLogMessagesCount(() => log.Info("test 3"), 1);
        }

        [Test]
        public void TestAssertLogMessageWithLevel()
        {
            TestHelper.AssertLogMessageWithLevelIsGenerated(() => log.Error("test 1"), Tuple.Create("test 1", LogLevelConstant.Error));
            TestHelper.AssertLogMessageWithLevelIsGenerated(() => log.Warn("test 2"), Tuple.Create("test 2", LogLevelConstant.Warn));
            TestHelper.AssertLogMessageWithLevelIsGenerated(() => log.Info("test 3"), Tuple.Create("test 3", LogLevelConstant.Info));
        }

        [Test]
        public void TestAssertLogMessagesWithLevelAreGenerated()
        {
            TestHelper.AssertLogMessagesWithLevelAreGenerated(() =>
            {
                log.Error("test 1");
                log.Warn("test 2");
                log.Info("test 3");
            }, new[]
            {
                Tuple.Create("test 1", LogLevelConstant.Error),
                Tuple.Create("test 2", LogLevelConstant.Warn),
                Tuple.Create("test 3", LogLevelConstant.Info)
            });
        }

        [Test]
        [TestCase(nameof(Resources.abacus))]
        [TestCase(nameof(Resources.double_abacus))]
        public void AssertImagesAreEqual_TwoIdenticalImages_NoAssertionErrors(string resourceName)
        {
            // Setup
            var image = Resources.ResourceManager.GetObject(resourceName) as Bitmap;

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
            ToolStripMenuItem testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text + "someThing", testItem.ToolTipText, testItem.Image);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_MenuItemWithDifferentToolTip_ThrowsAssertionException()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText + "someThing", testItem.Image);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_MenuItemWithDifferentImage_ThrowsAssertionException()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, Resources.acorn);

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
            ToolStripMenuItem testItem = CreateContextMenuItem();
            testItem.Enabled = enabled;
            contextMenuStrip.Items.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, testItem.Image, !enabled);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertContextMenuStripContainsItem_SameMenuItemProperties_NoExceptions()
        {
            // Setup
            var contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            contextMenuStrip.Items.Add(testItem);

            // Call & Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, testItem.Image);
        }

        [Test]
        public void AssertDropDownItemContainsItem_MenuNull_ThrowsAssertionException()
        {
            // Call
            TestDelegate call = () => TestHelper.AssertDropDownItemContainsItem(null, 0, "", "", null);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_NoMenuItemAtPosition_ThrowsAssertionException()
        {
            // Call
            TestDelegate call = () => TestHelper.AssertDropDownItemContainsItem(new TestToolStripDropDownItem(), 0, "", "", null);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_MenuItemWithDifferentText_ThrowsAssertionException()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text + "someThing", testItem.ToolTipText, testItem.Image);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_MenuItemWithDifferentToolTip_ThrowsAssertionException()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText + "someThing", testItem.Image);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_MenuItemWithDifferentImage_ThrowsAssertionException()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText, Resources.acorn);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssertDropDownItemContainsItem_MenuItemWithDifferentEnabeldState_ThrowsAssertionException(bool enabled)
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            testItem.Enabled = enabled;
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText, testItem.Image, !enabled);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_SameMenuItemProperties_NoExceptions()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            ToolStripMenuItem testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call & Assert
            TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText, testItem.Image);
        }

        [Test]
        public void AssertExceptionCustomMessage_NoException_ThrowsAssertionException()
        {
            // Setup
            TestDelegate t = () => {};

            // Call
            TestDelegate call = () => TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, string.Empty);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertExceptionCustomMessage_ExceptionIncorrectMessage_ThrowsAssertionException()
        {
            // Setup
            const string someMessage = "Exception";
            const string differentMessage = "Different";
            TestDelegate t = () => { throw new ArgumentException(someMessage); };

            // Call
            TestDelegate call = () => TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, differentMessage);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCase("param")]
        [TestCase(null)]
        public void AssertExceptionCustomMessage_ExceptionEqualMessage_NoExceptions(string argument)
        {
            // Setup
            const string someMessage = "Exception";
            TestDelegate t = () => { throw new ArgumentException(someMessage, argument); };

            // Call & Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, someMessage);
        }

        [Test]
        [TestCase("param")]
        [TestCase(null)]
        public void AssertExceptionCustomMessage_MessageWithNewLinesMessage_NoExceptions(string argument)
        {
            // Setup
            string someMessage = string.Join(Environment.NewLine, "Exception", "second line");
            TestDelegate t = () => { throw new ArgumentException(someMessage, argument); };

            // Call & Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, someMessage);
        }

        [Test]
        public void AssertThrowsArgumentExceptionAndTestMessage_Always_ReturnsException()
        {
            // Setup
            const string someMessage = "Exception";
            var argumentException = new ArgumentException(someMessage);

            TestDelegate t = () => { throw argumentException; };

            // Call
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, someMessage);

            // Assert
            Assert.IsInstanceOf<ArgumentException>(exception);
            Assert.AreSame(argumentException, exception);
        }

        [Test]
        public void AssertTypeConverterWithExpression_PropertyWithoutTypeConverterAttribute_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => TestHelper.AssertTypeConverter<TestClass, Int32Converter>(nameof(TestClass.PropertyWithoutTypeConverter));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertTypeConverterWithExpression_PropertyWithDifferentTypeConverterAttribute_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => TestHelper.AssertTypeConverter<TestClass, Int32Converter>(nameof(TestClass.PropertyWithTypeConverter));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertTypeConverterWithExpression_PropertyWithMatchingTypeConverterAttribute_DoesNotThrowException()
        {
            // Call
            TestDelegate test = () => TestHelper.AssertTypeConverter<TestClass, DoubleConverter>(nameof(TestClass.PropertyWithTypeConverter));

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertTypeConverterWithExpression_TypeConverterAttributeInherited_ReturnTrue()
        {
            // Call
            TestDelegate test = () => TestHelper.AssertTypeConverter<DerivedTestClass, DoubleConverter>(nameof(DerivedTestClass.PropertyWithTypeConverter));

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertTypeConverterWithoutExpression_ClassWithoutTypeConverterAttribute_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = TestHelper.AssertTypeConverter<TestClassWithoutConverter, Int32Converter>;

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertTypeConverterWithoutExpression_ClassWithDifferentTypeConverterAttribute_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = TestHelper.AssertTypeConverter<TestClass, Int32Converter>;

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertTypeConverterWithoutExpression_ClassWithMatchingTypeConverterAttribute_DoesNotThrowException()
        {
            // Call
            TestDelegate test = TestHelper.AssertTypeConverter<TestClass, DoubleConverter>;

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertTypeConverterWithoutExpression_TypeConverterAttributeInherited_ReturnTrue()
        {
            // Call
            TestDelegate test = TestHelper.AssertTypeConverter<DerivedTestClass, DoubleConverter>;

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCaseSource(nameof(AssertAreEqualButNotSameSource))]
        public void AssertAreEqualButNotSame_DifferentObjects_ReturnExpectedValues(object objectA, object objectB, bool shouldSucceed)
        {
            // Call
            TestDelegate test = () => TestHelper.AssertAreEqualButNotSame(objectA, objectB);

            // Assert
            if (shouldSucceed)
            {
                Assert.DoesNotThrow(test);
            }
            else
            {
                Assert.Throws<AssertionException>(test);
            }
        }

        [Test]
        public void AssertCollectionAreSame_ExpectedCollectionShorterThanActual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreSame(new[]
            {
                objectA,
                objectB,
                objectC
            }, new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCollectionAreSame_ExpectedCollectionLongerThanActual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreSame(new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, new[]
            {
                objectA,
                objectB,
                objectC
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCollectionAreSame_CollectionsPartiallyDifferent_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreSame(new[]
            {
                objectA,
                objectB,
                objectD
            }, new[]
            {
                objectA,
                objectB,
                objectC
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCollectionAreSame_CollectionsCompletelySame_DoesNotThrowException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreSame(new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            });

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertCollectionAreNotSame_ExpectedCollectionShorterThanActual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreNotSame(new[]
            {
                objectA,
                objectB,
                objectC
            }, new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCollectionAreNotSame_ExpectedCollectionLongerThanActual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreNotSame(new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, new[]
            {
                objectA,
                objectB,
                objectC
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCollectionAreNotSame_CollectionsCompletelyDifferent_DoesNotThrowException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreNotSame(new[]
            {
                objectB,
                objectC,
                objectA
            }, new[]
            {
                objectA,
                objectB,
                objectC
            });

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertCollectionAreNotSame_CollectionsPartiallySame_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionAreNotSame(new[]
            {
                objectB,
                objectD,
                objectC,
                objectA
            }, new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertCollectionsAreEqual_ExpectedCollectionShorterThanActual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            var mocks = new MockRepository();
            var comparer = mocks.Stub<IEqualityComparer<object>>();
            comparer.Stub(c => c.Equals(null, null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(new[]
            {
                objectA,
                objectB,
                objectC
            }, new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, comparer);

            // Assert
            Assert.Throws<AssertionException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void AssertCollectionsAreEqual_ExpectedCollectionLongerThanActual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            var mocks = new MockRepository();
            var comparer = mocks.Stub<IEqualityComparer<object>>();
            comparer.Stub(c => c.Equals(null, null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, new[]
            {
                objectA,
                objectB,
                objectC
            }, comparer);

            // Assert
            Assert.Throws<AssertionException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void AssertCollectionsAreEqual_CollectionsCompletelyEqual_DoesNotThrowException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();

            var mocks = new MockRepository();
            var comparer = mocks.Stub<IEqualityComparer<object>>();
            comparer.Stub(c => c.Equals(null, null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(new[]
            {
                objectA,
                objectB,
                objectC
            }, new[]
            {
                objectA,
                objectB,
                objectC
            }, comparer);

            // Assert
            Assert.DoesNotThrow(test);
            mocks.VerifyAll();
        }

        [Test]
        public void AssertCollectionsAreEqual_CollectionsPartiallyEqual_ThrowsException()
        {
            // Setup
            var objectA = new object();
            var objectB = new object();
            var objectC = new object();
            var objectD = new object();

            var mocks = new MockRepository();
            var comparer = mocks.StrictMock<IEqualityComparer<object>>();
            comparer.Expect(c => c.Equals(objectA, objectA)).Return(true);
            comparer.Expect(c => c.Equals(objectB, objectB)).Return(true);
            comparer.Expect(c => c.Equals(objectC, objectC)).Return(false);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, new[]
            {
                objectA,
                objectB,
                objectC,
                objectD
            }, comparer);

            // Assert
            Assert.Throws<AssertionException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void AssertCollectionsAreEqual_ExpectedCollectionNull_ThrowsException()
        {
            // Setup
            var mocks = new MockRepository();
            var comparer = mocks.Stub<IEqualityComparer<object>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(null, Enumerable.Empty<object>(), comparer);

            // Assert
            Assert.Throws<AssertionException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void AssertCollectionsAreEqual_ActualCollectionNull_ThrowsException()
        {
            // Setup
            var mocks = new MockRepository();
            var comparer = mocks.Stub<IEqualityComparer<object>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(Enumerable.Empty<object>(), null, comparer);

            // Assert
            Assert.Throws<AssertionException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void AssertCollectionsAreEqual_ComparerNull_ThrowsException()
        {
            // Call
            TestDelegate test = () => TestHelper.AssertCollectionsAreEqual(Enumerable.Empty<object>(), Enumerable.Empty<object>(), null);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void PerformActionWithDelayedAssert_WithAllData_PerformsActions()
        {
            // Setup
            var callPerformed = false;
            var assertPerformed = false;
            var callAction = new Action(() => callPerformed = true);
            var assertAction = new Action(() => assertPerformed = true);
            const int delay = 10;

            // Call
            TestHelper.PerformActionWithDelayedAssert(callAction, assertAction, delay);

            // Assert
            Assert.IsTrue(callPerformed);
            Assert.IsTrue(assertPerformed);
        }

        private class TestEqualSameObject
        {
            private readonly int someInt;

            public TestEqualSameObject(int someInt)
            {
                this.someInt = someInt;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((TestEqualSameObject) obj);
            }

            public override int GetHashCode()
            {
                return someInt;
            }

            private bool Equals(TestEqualSameObject other)
            {
                return someInt == other.someInt;
            }
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

        private class TestToolStripDropDownItem : ToolStripDropDownItem {}

        private class TestClassWithoutConverter {}

        [TypeConverter(typeof(DoubleConverter))]
        private class TestClass
        {
            public double PropertyWithoutTypeConverter { get; private set; }

            [TypeConverter(typeof(DoubleConverter))]
            public virtual double PropertyWithTypeConverter { get; private set; }
        }

        private class DerivedTestClass : TestClass {}
    }
}