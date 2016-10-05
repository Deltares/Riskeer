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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresCalculationContextTreeNodeInfoTest
    {
        private const int contextMenuValidateIndex = 0;
        private const int contextMenuCalculateIndex = 1;
        private const int contextMenuClearIndex = 2;

        private IGui guiMock;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private ClosingStructuresPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            guiMock = mocks.Stub<IGui>();
            plugin = new ClosingStructuresPlugin()
            {
                Gui = guiMock
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresCalculationContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(ClosingStructuresCalculationContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Image_Always_ReturnsCalculationIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnCollectionWithEmptyOutputObject()
        {
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new ClosingStructuresCalculation();
            var calculationContext = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.WrappedData);

            var closingStructuresInputContext = children[1] as ClosingStructuresInputContext;
            Assert.IsNotNull(closingStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, closingStructuresInputContext.WrappedData);

            var emptyOutput = children[2] as EmptyProbabilityAssessmentOutput;
            Assert.IsNotNull(emptyOutput);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnCollectionWithOutputObject()
        {
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new ClosingStructuresCalculation
            {
                Output = new TestClosingStructuresOutput()
            };

            var calculationContext = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.WrappedData);

            var closingStructuresInputContext = children[1] as ClosingStructuresInputContext;
            Assert.IsNotNull(closingStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, closingStructuresInputContext.WrappedData);

            var output = children[2] as ProbabilityAssessmentOutput;
            Assert.IsNotNull(output);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var calculation = new ClosingStructuresCalculation();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var calculation = new ClosingStructuresCalculation();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSectionMock, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(6, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateIndex,
                                                                  RingtoetsCommonFormsResources.Validate,
                                                                  RingtoetsCommonFormsResources.Validate_ToolTip,
                                                                  RingtoetsCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateIndex,
                                                                  RingtoetsCommonFormsResources.Calculate,
                                                                  RingtoetsCommonFormsResources.Calculate_ToolTip,
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex,
                                                                  RingtoetsCommonFormsResources.Clear_output,
                                                                  RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear,
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        private class TestClosingStructuresOutput : ProbabilityAssessmentOutput
        {
            public TestClosingStructuresOutput() : base(0, 0, 0, 0, 0) { }
        }
    }
}