// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Core.Common.TestUtil.Test.Properties;
using log4net;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class TestHelperTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestHelperTest));

        [Test]
        public void CanOpenFileForWrite_InvalidPath_DoesNotThrowAnyExceptions()
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
        public void GetTestDataPath_Always_VerifiedTestPaths()
        {
            string path = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Gui);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service);
            Assert.IsTrue(Directory.Exists(path));
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin);
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
            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.Plugin);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityStoneCover.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO);
            Assert.IsTrue(Directory.Exists(path));

            path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.WaveImpactAsphaltCover.IO);
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
            TestDelegate call = () => { TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text + "someThing", testItem.ToolTipText, testItem.Image); };

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
            TestDelegate call = () => { TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText + "someThing", testItem.Image); };

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
            TestDelegate call = () => { TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, Resources.acorn); };

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
            TestDelegate call = () => { TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, 0, testItem.Text, testItem.ToolTipText, testItem.Image, !enabled); };

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
            var testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => { TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text + "someThing", testItem.ToolTipText, testItem.Image); };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_MenuItemWithDifferentToolTip_ThrowsAssertionException()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            var testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => { TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText + "someThing", testItem.Image); };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_MenuItemWithDifferentImage_ThrowsAssertionException()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            var testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => { TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText, Resources.acorn); };

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
            var testItem = CreateContextMenuItem();
            testItem.Enabled = enabled;
            dropDownItem.DropDownItems.Add(testItem);

            // Call
            TestDelegate call = () => { TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText, testItem.Image, !enabled); };

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertDropDownItemContainsItem_SameMenuItemProperties_NoExceptions()
        {
            // Setup
            var dropDownItem = new TestToolStripDropDownItem();
            var testItem = CreateContextMenuItem();
            dropDownItem.DropDownItems.Add(testItem);

            // Call & Assert
            TestHelper.AssertDropDownItemContainsItem(dropDownItem, 0, testItem.Text, testItem.ToolTipText, testItem.Image);
        }

        [Test]
        public void AssertExceptionCustomMessage_NoException_ThrowsAssertionException()
        {
            // Setup
            TestDelegate t = () => { };

            // Call
            TestDelegate call = () => { TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, string.Empty); };

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
            TestDelegate call = () => { TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(t, differentMessage); };

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
            var someMessage = string.Join(Environment.NewLine, "Exception", "second line");
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
    }
}