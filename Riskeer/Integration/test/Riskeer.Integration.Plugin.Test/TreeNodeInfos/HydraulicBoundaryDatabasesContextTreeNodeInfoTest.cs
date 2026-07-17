// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabasesContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.OnRemoveConfirmationText);
                Assert.IsNotNull(info.OnRemoveChildNodesConfirmationText);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("HRD bestanden", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var menuBuilder = Substitute.For<IContextMenuBuilder>();

            menuBuilder.AddImportItem(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Image>()).Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddDeleteChildrenItem().Returns(menuBuilder);
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(null, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddImportItem(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Image>());
                menuBuilder.AddSeparator();
                menuBuilder.AddDeleteChildrenItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_WithContext_AddImportItem()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicBoundaryDatabasesContext(assessmentSection.HydraulicBoundaryData, assessmentSection);
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            importCommandHandler.GetSupportedImportInfos(Arg.Any<object>()).Returns(new[]
            {
                new ImportInfo()
            });
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     context,
                                                     treeViewControl);

                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeViewControl).Returns(builder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(context, assessmentSection, treeViewControl))
                    {
						// Assert
                        Assert.AreEqual(6, contextMenuStrip.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(contextMenuStrip, contextMenuImportHydraulicBoundaryDatabaseIndex,
                                                                      "HRD bestand &toevoegen...",
                                                                      "Voeg een nieuw HRD bestand toe aan deze map.",
                                                                      RiskeerCommonFormsResources.DatabaseIcon);
                    }
                }
            }
        }

        [Test]
        public void ChildNodeObjects_WithContext_ReturnsChildrenOfData()
        {
            // Setup
            var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.AddRange(new[]
            {
                hydraulicBoundaryDatabase1,
                hydraulicBoundaryDatabase2
            });

            var context = new HydraulicBoundaryDatabasesContext(assessmentSection.HydraulicBoundaryData, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(2, objects.Length);

                var hydraulicBoundaryDatabaseContext1 = (HydraulicBoundaryDatabaseContext) objects[0];
                Assert.AreSame(hydraulicBoundaryDatabase1, hydraulicBoundaryDatabaseContext1.WrappedData);
                Assert.AreSame(assessmentSection.HydraulicBoundaryData, hydraulicBoundaryDatabaseContext1.HydraulicBoundaryData);
                Assert.AreSame(assessmentSection, hydraulicBoundaryDatabaseContext1.AssessmentSection);

                var hydraulicBoundaryDatabaseContext2 = (HydraulicBoundaryDatabaseContext) objects[1];
                Assert.AreSame(assessmentSection.HydraulicBoundaryData, hydraulicBoundaryDatabaseContext2.HydraulicBoundaryData);
                Assert.AreSame(assessmentSection, hydraulicBoundaryDatabaseContext2.AssessmentSection);
            }
        }

        [Test]
        public void OnRemoveChildNodesConfirmationText_Always_ReturnsConfirmationMessage()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string onRemoveConfirmationText = info.OnRemoveChildNodesConfirmationText(null);

                // Assert
                string expectedText = "Als u deze HRD bestanden verwijdert, dan wordt de uitvoer van alle ervan afhankelijke berekeningen verwijderd. Ook worden alle referenties naar de bijbehorende hydraulische belastingenlocaties verwijderd uit de invoer van de sterkteberekeningen."
                                      + Environment.NewLine
                                      + Environment.NewLine
                                      + "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedText, onRemoveConfirmationText);
            }
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabasesContext));
        }
    }
}