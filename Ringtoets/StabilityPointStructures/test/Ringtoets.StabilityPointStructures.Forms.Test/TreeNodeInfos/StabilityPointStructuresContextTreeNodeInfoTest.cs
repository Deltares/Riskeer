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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Plugin;
using RingtoetsCommonFormsProperties = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(StabilityPointStructuresContext), info.TagType);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.CanRename);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnDrop);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNotNull(info.Text);
            }
        }

        [Test]
        public void Text_Always_ReturnExpectedText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityPointStructuresContext(new ObservableList<StabilityPointStructure>(),
                                                              assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                const string expectedText = "Kunstwerken";
                Assert.AreEqual(expectedText, text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityPointStructuresContext(new ObservableList<StabilityPointStructure>(),
                                                              assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Image image = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsProperties.GeneralFolderIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionHasElements_ReturnControlText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var structures = new ObservableList<StabilityPointStructure>
            {
                CreateStabilityPointStructure()
            };

            // Precondition
            CollectionAssert.IsNotEmpty(structures);

            var context = new StabilityPointStructuresContext(structures, assessmentSection);
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnStabilityPointStructures()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            StabilityPointStructure structure1 = CreateStabilityPointStructure();
            StabilityPointStructure structure2 = CreateStabilityPointStructure();
            var closingStructures = new ObservableList<StabilityPointStructure>
            {
                structure1,
                structure2
            };

            var context = new StabilityPointStructuresContext(closingStructures, assessmentSection);
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var children = info.ChildNodeObjects(context);

                // Assert
                Assert.AreEqual(2, children.Length);
                Assert.AreSame(structure1, children.ElementAt(0));
                Assert.AreSame(structure2, children.ElementAt(1));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var closingStructures = new ObservableList<StabilityPointStructure>();

            // Precondition
            CollectionAssert.IsEmpty(closingStructures);

            var context = new StabilityPointStructuresContext(closingStructures, assessmentSection);
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
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

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;
                var info = GetInfo(plugin);

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            mocks.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(StabilityPointStructuresPlugin gui)
        {
            return gui.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresContext));
        }

        private static StabilityPointStructure CreateStabilityPointStructure()
        {
            return new StabilityPointStructure("Name", "Id", new Point2D(1, 2),
                                               123.456,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               456.789, 0.456,
                                               567.890, 0.567,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               123.546, 0.123,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               555.555,
                                               456.789, 0.456,
                                               555.55,
                                               0.55,
                                               567.890, 0.567,
                                               7777777.777, 0.777,
                                               567.890, 0.567,
                                               42,
                                               0.55,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               StabilityPointStructureInflowModelType.FloodedCulvert
                );
        }
    }
}