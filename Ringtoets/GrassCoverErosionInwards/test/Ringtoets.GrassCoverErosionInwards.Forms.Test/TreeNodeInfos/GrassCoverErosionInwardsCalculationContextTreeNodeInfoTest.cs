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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using Ringtoets.HydraRing.Data;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculationContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Image_Always_ReturnsPlaceHolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(GrassCoverErosionInwardsFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {           
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput());

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.CommentContainer);

            var grassCoverErosionInwardsCalculationContext = (GrassCoverErosionInwardsInputContext) children[1];
            Assert.AreSame(calculationContext.WrappedData.InputParameters, grassCoverErosionInwardsCalculationContext.WrappedData);

            var emptyOutput = (EmptyGrassCoverErosionInwardsOutput) children[2];
            Assert.IsNotNull(emptyOutput);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            
            var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput());
            
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput());

            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Clear_output, RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear, RingtoetsCommonFormsResources.ClearIcon, false);
        }

        [Test]
        public void ContextMenuStrip_CalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput())
            {
                Output = new GrassCoverErosionInwardsOutput(0, 0, 0, 0, 0)
            };

            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Clear_output, RingtoetsCommonFormsResources.Clear_output_ToolTip, RingtoetsCommonFormsResources.ClearIcon);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenCalculationOutputClearedAndNotified(bool confirm)
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var observer = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput());

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(calculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            int clearOutputItemPosition = 1;
            if (confirm)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.Output = new GrassCoverErosionInwardsOutput(0, 0, 0, 0, 0);
            calculation.Attach(observer);

            var contextMenuAdapter = info.ContextMenuStrip(calculationContext, null, treeViewControlMock);

            string messageBoxText = null, messageBoxTitle = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;
                if (confirm)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };

            // When
            contextMenuAdapter.Items[clearOutputItemPosition].PerformClick();

            // Then
            Assert.AreNotEqual(confirm, calculation.HasOutput);
            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);
            mocks.VerifyAll();
        }
    }
}