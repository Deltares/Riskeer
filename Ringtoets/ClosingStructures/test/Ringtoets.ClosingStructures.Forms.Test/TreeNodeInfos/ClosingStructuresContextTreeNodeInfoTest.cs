// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Plugin;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.ClosingStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresContextTreeNodeInfoTest
    {
        private ClosingStructuresPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(ClosingStructuresContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnExpectedText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var closingStructures = new ObservableList<ClosingStructure>();

            var closingStructuresContext = new ClosingStructuresContext(closingStructures, assessmentSection);

            // Call
            string text = info.Text(closingStructuresContext);

            // Assert
            const string expectedText = "Kunstwerken";
            Assert.AreEqual(expectedText, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var closingStructures = new ObservableList<ClosingStructure>();

            var closingStructuresContext = new ClosingStructuresContext(closingStructures, assessmentSection);

            // Call
            Image image = info.Image(closingStructuresContext);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, image);
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionHasElementsEmpty_ReturnControlText()
        {
            // Setup
            var mocks = new MockRepository();
            var asssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var closingStructures = new ObservableList<ClosingStructure>
            {
                new TestClosingStructure()
            };

            // Precondition
            CollectionAssert.IsNotEmpty(closingStructures);

            var closingStructuresContext = new ClosingStructuresContext(closingStructures, asssessmentSection);

            // Call
            Color color = info.ForeColor(closingStructuresContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnClosingStructures()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ClosingStructure closingStructure1 = new TestClosingStructure();
            ClosingStructure closingStructure2 = new TestClosingStructure();
            var closingStructures = new ObservableList<ClosingStructure>
            {
                closingStructure1,
                closingStructure2
            };

            var closingStructuresContext = new ClosingStructuresContext(closingStructures, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(closingStructuresContext);

            // Assert
            Assert.AreEqual(2, children.Length);
            Assert.AreSame(closingStructure1, children.ElementAt(0));
            Assert.AreSame(closingStructure2, children.ElementAt(1));
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var asssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var closingStructures = new ObservableList<ClosingStructure>();

            // Precondition
            CollectionAssert.IsEmpty(closingStructures);

            var closingStructuresContext = new ClosingStructuresContext(closingStructures, asssessmentSection);

            // Call
            Color color = info.ForeColor(closingStructuresContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}
