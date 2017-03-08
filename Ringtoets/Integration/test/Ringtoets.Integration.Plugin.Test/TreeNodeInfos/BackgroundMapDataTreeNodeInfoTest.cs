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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class BackgroundMapDataTreeNodeInfoTest : NUnitFormTest
    {
        private const int selectContextMenuIndex = 0;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
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
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var backgroundMapData = new BackgroundMapData();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(backgroundMapData);

                // Assert
                Assert.AreEqual("Achtergrondkaart", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var backgroundMapData = new BackgroundMapData();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(backgroundMapData);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.Map, image);
            }
        }

        [Test]
        public void ForeColor_ConnectedMapData_ReturnControlText()
        {
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundMapData backgroundMapData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundMapData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), image);
            }
        }

        [Test]
        public void ForeColor_UnconnectedMapData_ReturnGrayText()
        {
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            BackgroundMapData backgroundMapData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundMapData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), image);
            }
        }

        [Test]
        public void ForeColor_MapDataNull_ReturnGrayText()
        {
            var backgroundMapData = new BackgroundMapData();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundMapData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mockRepository = new MockRepository();

            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }
            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_ContextMenuItemSelectMapLayerEnabled()
        {
            // Setup
            var mockRepository = new MockRepository();
            var backgroundMapData = new BackgroundMapData();

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.Get(backgroundMapData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(backgroundMapData, assessmentSection, treeViewControl))
                    {
                        const string expectedItemText = "Selecteren...";
                        const string expectedItemTooltip = "Selecteer een achtergrondkaart.";
                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, selectContextMenuIndex,
                                                                      expectedItemText, expectedItemTooltip,
                                                                      RingtoetsCommonFormsResources.MapsIcon);
                    }
                }
            }
            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenNoMapDataSet_WhenSelectingValidMapDataFromContextMenu_ThenMapDataSetAndNotifiesObserver()
        {
            // Given
            var mockRepository = new MockRepository();
            var assessmentSectionObserver = mockRepository.StrictMock<IObserver>();
            assessmentSectionObserver.Expect(o => o.UpdateObserver());

            var backgroundMapDataObserver = mockRepository.StrictMock<IObserver>();
            backgroundMapDataObserver.Expect(o => o.UpdateObserver());

            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            BackgroundMapData backgroundMapData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(backgroundMapData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.BackgroundMapData2.Attach(backgroundMapDataObserver);

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    tester.DialogResult = DialogResult.OK;
                    tester.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(backgroundMapData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundMapDataProperties(mapData, assessmentSection.BackgroundMapData2);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenNoMapDataSet_WhenSelectingMapDataFromContextMenuCancelled_ThenNoObserversNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var assessmentSectionObserver = mockRepository.StrictMock<IObserver>();
            var backgroundMapDataObserver = mockRepository.StrictMock<IObserver>();

            var backgroundMapData = new BackgroundMapData
            {
                Name = "background map data"
            };

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(backgroundMapData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.BackgroundMapData2.Attach(backgroundMapDataObserver);
                
                BackgroundMapData oldBackgroundMapData = assessmentSection.BackgroundMapData2;

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    tester.DialogResult = DialogResult.Cancel;
                    tester.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(backgroundMapData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundMapDataProperties(oldBackgroundMapData, assessmentSection.BackgroundMapData2);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingValidMapDataFromContextMenu_ThenMapDataSetAndNotifiesObserver()
        {
            // Given
            var mockRepository = new MockRepository();
            var assessmentSectionObserver = mockRepository.StrictMock<IObserver>();
            assessmentSectionObserver.Expect(o => o.UpdateObserver());

            var backgroundMapDataObserver = mockRepository.StrictMock<IObserver>();
            backgroundMapDataObserver.Expect(o => o.UpdateObserver());

            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            WmtsMapData newMapData = WmtsMapData.CreateDefaultPdokMapData();
            var newBackgroundMapdata = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(newMapData);

            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(newBackgroundMapdata, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.BackgroundMapData2.Attach(backgroundMapDataObserver);
                assessmentSection.BackgroundMapData2.Name = mapData.Name;
                assessmentSection.BackgroundMapData2.IsVisible = mapData.IsVisible;
                assessmentSection.BackgroundMapData2.IsConfigured = mapData.IsConfigured;
                assessmentSection.BackgroundMapData2.Transparency = mapData.Transparency;
                assessmentSection.BackgroundMapData2.BackgroundMapDataType = BackgroundMapDataType.Wmts;
                assessmentSection.BackgroundMapData2.Parameters["SourceCapabilitiesUrl"] = mapData.SourceCapabilitiesUrl;
                assessmentSection.BackgroundMapData2.Parameters["SelectedCapabilityIdentifier"] = mapData.SelectedCapabilityIdentifier;
                assessmentSection.BackgroundMapData2.Parameters["PreferredFormat"] = mapData.PreferredFormat;

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    tester.DialogResult = DialogResult.OK;
                    tester.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundMapdata, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundMapDataProperties(newMapData, assessmentSection.BackgroundMapData2);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingMapDataFromContextMenuCancelled_ThenNoObserversNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var assessmentSectionObserver = mockRepository.StrictMock<IObserver>();
            var backgroundMapDataObserver = mockRepository.StrictMock<IObserver>();

            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            BackgroundMapData backgroundMapData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);
            BackgroundMapData newBackgroundMapData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(WmtsMapData.CreateDefaultPdokMapData());

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(newBackgroundMapData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.BackgroundMapData2.Attach(backgroundMapDataObserver);

                assessmentSection.BackgroundMapData2.Name = backgroundMapData.Name;
                assessmentSection.BackgroundMapData2.IsVisible = backgroundMapData.IsVisible;
                assessmentSection.BackgroundMapData2.IsConfigured = backgroundMapData.IsConfigured;
                assessmentSection.BackgroundMapData2.Transparency = backgroundMapData.Transparency;
                assessmentSection.BackgroundMapData2.BackgroundMapDataType = backgroundMapData.BackgroundMapDataType;

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    tester.DialogResult = DialogResult.Cancel;
                    tester.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundMapData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundMapDataProperties(backgroundMapData, assessmentSection.BackgroundMapData2);
                }
            }
            mockRepository.VerifyAll();
        }

        private static void AssertBackgroundMapDataProperties(WmtsMapData mapData, BackgroundMapData backgroundMapData)
        {
            Assert.AreEqual(mapData.Name, backgroundMapData.Name);
            Assert.IsTrue(backgroundMapData.IsVisible);
            Assert.AreEqual(mapData.IsConfigured, backgroundMapData.IsConfigured);
            Assert.AreEqual(mapData.Transparency, backgroundMapData.Transparency);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, backgroundMapData.Parameters["SourceCapabilitiesUrl"]);
            Assert.AreEqual(mapData.SelectedCapabilityIdentifier, backgroundMapData.Parameters["SelectedCapabilityIdentifier"]);
            Assert.AreEqual(mapData.PreferredFormat, backgroundMapData.Parameters["PreferredFormat"]);
        }

        private static void AssertBackgroundMapDataProperties(BackgroundMapData expectedBackgroundMapData, BackgroundMapData actualBackgroundMapData)
        {
            Assert.AreEqual(expectedBackgroundMapData.Name, actualBackgroundMapData.Name);
            Assert.AreEqual(expectedBackgroundMapData.IsVisible, actualBackgroundMapData.IsVisible);
            Assert.AreEqual(expectedBackgroundMapData.IsConfigured, actualBackgroundMapData.IsConfigured);
            Assert.AreEqual(expectedBackgroundMapData.Transparency, actualBackgroundMapData.Transparency);
            CollectionAssert.AreEquivalent(expectedBackgroundMapData.Parameters, actualBackgroundMapData.Parameters);
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(BackgroundMapData));
        }
    }
}