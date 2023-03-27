// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
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
        private const int contextMenuAddHydraulicBoundaryDatabaseIndex = 0;

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
            var mocks = new MockRepository();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem(null, null, null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(null, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                mocks.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var mockRepository = new MockRepository();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(null, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(null, null, treeViewControl))
                    {
                        // Assert
                        Assert.AreEqual(6, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddHydraulicBoundaryDatabaseIndex,
                                                                      "HRD bestand toevoegen",
                                                                      "Voeg een nieuw HRD bestand toe aan deze map.",
                                                                      RiskeerCommonFormsResources.DatabaseIcon);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddHydraulicBoundaryDatabaseItem_HydraulicBoundaryDatabaseAddedAndObserversNotified()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = "hlcd.sqlite"
                    }
                }
            };

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            var context = new HydraulicBoundaryDatabasesContext(hydraulicBoundaryData, assessmentSection);

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            hydraulicBoundaryData.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuAddHydraulicBoundaryDatabaseIndex].PerformClick();

                        // Assert
                        Assert.AreEqual(1, hydraulicBoundaryData.HydraulicBoundaryDatabases.Count);
                    }
                }
            }

            mockRepository.VerifyAll();
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

                var hydraulicBoundaryDatabaseContext2 = (HydraulicBoundaryDatabaseContext) objects[1];
                Assert.AreSame(assessmentSection.HydraulicBoundaryData, hydraulicBoundaryDatabaseContext2.HydraulicBoundaryData);
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