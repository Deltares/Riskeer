// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsContextMenuBuilderTest
    {
        [Test]
        public void AddCreateCalculationGroupItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var calculationGroup = new CalculationGroup();
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddCreateCalculationGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Map toevoegen",
                                                              "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                              RingtoetsFormsResources.AddFolderIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddCreateCalculationItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var calculationGroup = new CalculationGroup();
                var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddCreateCalculationItem(calculationGroupContext, context => {}).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                              RingtoetsFormsResources.FailureMechanismIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutputMock
                }
            };
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup, treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                              RingtoetsFormsResources.ClearIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                              RingtoetsFormsResources.ClearIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();
            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutputMock
            });
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen dit toetsspoor.",
                                                              RingtoetsFormsResources.ClearIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new List<ICalculation>());

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanism,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInFailureMechanismItem(failureMechanism).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                              RingtoetsFormsResources.ClearIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddToggleRelevancyOfFailureMechanismItem_WhenBuild_ItemAddedToContextMenuEnabled(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.IsRelevant).Return(isRelevant);
            var failureMechanismContextMock = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContextMock.Expect(fmc => fmc.WrappedData).Return(failureMechanismMock);
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContextMock, null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                Bitmap checkboxIcon = isRelevant ? RingtoetsFormsResources.Checkbox_ticked : RingtoetsFormsResources.Checkbox_empty;
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "I&s relevant",
                                                              "Geeft aan of dit toetsspoor relevant is of niet.",
                                                              checkboxIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationWithOutputMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddClearCalculationOutputItem(calculationWithOutputMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis uitvoer...",
                                                              "Wis de uitvoer van deze berekening.",
                                                              RingtoetsFormsResources.ClearIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithoutOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithoutOutputMock.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationWithoutOutputMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddClearCalculationOutputItem(calculationWithoutOutputMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis uitvoer...",
                                                              "Deze berekening heeft geen uitvoer om te wissen.",
                                                              RingtoetsFormsResources.ClearIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddRenameItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddRenameItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddRenameItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddDeleteItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddDeleteItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddDeleteItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExpandAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddExpandAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddExpandAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCollapseAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddCollapseAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddCollapseAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddOpenItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddOpenItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddOpenItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddExportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddExportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddImportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddImportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddImportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddPropertiesItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddPropertiesItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddPropertiesItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddSeparator());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddSeparator();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuItemMock = mocks.StrictMock<StrictContextMenuItem>();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddCustomItem(contextMenuItemMock));

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddCustomItem(contextMenuItemMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Build_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.Build());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.Build();

            // Assert
            mocks.VerifyAll();
        }

        #region AddPerformCalculationItem

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanisMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformCalculationItem(calculation, calculationContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Be&rekenen",
                                                              "Voer deze berekening uit.",
                                                              RingtoetsFormsResources.CalculateIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanisMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "No valid data";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformCalculationItem(calculation, calculationContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Be&rekenen",
                                                              errorMessage,
                                                              RingtoetsFormsResources.CalculateIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        #endregion

        #region AddValidateCalculationItem

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateCalculationItem(calculationContext, null, c => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Valideren",
                                                              "Valideer de invoer voor deze berekening.",
                                                              RingtoetsFormsResources.ValidateIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "No valid data";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateCalculationItem(calculationContext, null, c => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Valideren",
                                                              errorMessage,
                                                              RingtoetsFormsResources.ValidateIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        #endregion

        #region AddPerformAllCalculationsInGroupItem

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze berekeningsmap uit.",
                                                              RingtoetsFormsResources.CalculateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              errorMessage,
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        #endregion

        #region AddValidateAllCalculationsInGroupItem

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                              RingtoetsFormsResources.ValidateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              errorMessage,
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        #endregion

        #region AddPerformAllCalculationsInFailureMechanismItem

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                              RingtoetsFormsResources.CalculateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              errorMessage,
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        #endregion

        #region AddValidateAllCalculationsInFailureMechanismItem

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen dit toetsspoor.",
                                                              RingtoetsFormsResources.ValidateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(
                    failureMechanismContext,
                    null,
                    fm => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              errorMessage,
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(
                    failureMechanismContext,
                    null,
                    fm => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        #endregion

        #region AddUpdateForeshoreProfileOfCalculationItem

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddUpdateForeshoreProfileOfCalculationItem_CalculationWithForeshoreProfile_ItemAddedToContextMenuEnabledIfNotSynchronized(
            bool synchronized)
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(synchronized);
            calculationMock.Expect(c => c.InputParameters).Return(input);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationMock,
                                                                treeViewControl);

                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddUpdateForeshoreProfileOfCalculationItem(
                    calculationMock,
                    inquiryHelperMock,
                    c => {}).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Bijwerken voorlandprofiel...",
                                                              synchronized
                                                                ? "Geselecteerd voorlandprofiel heeft geen wijzingingen om bij te werken."
                                                                : "Berekening bijwerken waar een voorlandprofiel geselecteerd is.",
                                                              RingtoetsFormsResources.UpdateItemIcon,
                                                              !synchronized);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddUpdateForeshoreProfileOfCalculationItem_CalculationWithoutForeshoreProfile_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var updateHandlerMock = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculationMock.Expect(c => c.InputParameters.ForeshoreProfile).Return(null);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                updateHandlerMock,
                                                                viewCommandsMock,
                                                                calculationMock,
                                                                treeViewControl);

                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddUpdateForeshoreProfileOfCalculationItem(
                    calculationMock,
                    inquiryHelperMock,
                    c => {}).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Bijwerken voorlandprofiel...",
                                                              "Geselecteerd voorlandprofiel heeft geen wijzingingen om bij te werken.",
                                                              RingtoetsFormsResources.UpdateItemIcon,
                                                              false);
            }

            mocks.VerifyAll();
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}

        #endregion

        #region Nested types

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
                base(wrappedFailureMechanism, parent) {}
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public TestCalculation()
            {
                Name = "Nieuwe berekening";
            }

            public string Name { get; set; }

            public Comment Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}
        }

        #endregion
    }
}