// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using Core.Components.Gis.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Util.TypeConverters;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class BackgroundDataTreeNodeInfoTest : NUnitFormTest
    {
        private const int selectContextMenuIndex = 0;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
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
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(backgroundData);

                // Assert
                Assert.AreEqual("Achtergrondkaart", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(backgroundData);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.Map, image);
            }
        }

        [Test]
        public void ForeColor_ConnectedWtmsBackgroundDataConfiguration_ReturnControlText()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), image);
            }
        }

        [Test]
        public void ForeColor_UnconnectedWtmsBackgroundDataConfiguration_ReturnGrayText()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), image);
            }
        }

        [Test]
        public void ForeColor_WellKnownBackgroundDataConfiguration_ReturnControlText()
        {
            // Setup
            var random = new Random(21);
            var wellKnownTileSource = random.NextEnumValue<WellKnownTileSource>();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(new WellKnownTileSourceMapData(wellKnownTileSource));

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), image);
            }
        }

        [Test]
        public void ForeColor_ArbitraryBackgroundDataConfiguration_ReturnControlText()
        {
            // Setup
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color image = info.ForeColor(backgroundData);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mockRepository = new MockRepository();

            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
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
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.Get(backgroundData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(backgroundData, assessmentSection, treeViewControl))
                    {
                        const string expectedItemText = "&Selecteren...";
                        const string expectedItemTooltip = "Selecteer een achtergrondkaart.";
                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, selectContextMenuIndex,
                                                                      expectedItemText, expectedItemTooltip,
                                                                      RiskeerCommonFormsResources.MapsIcon);
                    }
                }
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenNoMapDataSet_WhenSelectingValidWmtsMapDataFromContextMenu_ThenBackgroundDataSetAndNotifiesObserver()
        {
            // Given
            var mockRepository = new MockRepository();
            var tileFactory = mockRepository.StrictMock<ITileSourceFactory>();
            var newMapData = new WmtsMapData("Actueel Hoogtebestand Nederland (AHN1)",
                                             "https://geodata.nationaalgeoregister.nl/tiles/service/wmts/ahn1?request=GetCapabilities",
                                             "()", "image/png");
            tileFactory.Expect(tf => tf.GetWmtsTileSources(null))
                       .IgnoreArguments()
                       .Return(new[]
                       {
                           new TestWmtsTileSource(newMapData)
                       });

            var backgroundDataObserver = mockRepository.StrictMock<IObserver>();
            backgroundDataObserver.Expect(o => o.UpdateObserver());

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "twoValidWmtsConnectionInfos")
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(backgroundData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                var assessmentSection = new AssessmentSectionStub();
                assessmentSection.BackgroundData.Attach(backgroundDataObserver);

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialog = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    var connectButton = new ButtonTester("connectToButton", dialog);
                    connectButton.Click();

                    var layersControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                    layersControl.SetCurrentCell(layersControl.GetCell(0, 0));

                    var button = new ButtonTester("selectButton", dialog);
                    button.Click();
                    dialog.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(backgroundData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundData(newMapData, assessmentSection.BackgroundData);
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
            var backgroundDataObserver = mockRepository.StrictMock<IObserver>();

            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(new WellKnownTileSourceMapData(WellKnownTileSource.BingHybrid));

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "EmptyWmtsConnectionInfo")
            }))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(backgroundData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                var assessmentSection = new AssessmentSectionStub();
                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.BackgroundData.Attach(backgroundDataObserver);

                BackgroundData oldBackgroundData = assessmentSection.BackgroundData;

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    tester.DialogResult = DialogResult.Cancel;
                    tester.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(backgroundData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    BackgroundDataTestHelper.AssertBackgroundData(oldBackgroundData, assessmentSection.BackgroundData);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingValidWmtsMapDataFromContextMenu_ThenBackgroundDataSetAndNotifiesObserver()
        {
            // Given
            var mockRepository = new MockRepository();

            var backgroundDataObserver = mockRepository.StrictMock<IObserver>();
            backgroundDataObserver.Expect(o => o.UpdateObserver());

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            WmtsMapData newMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData newBackgroundData = BackgroundDataConverter.ConvertTo(newMapData);

            var assessmentSection = new AssessmentSectionStub();

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "noConfig")
            }))
            using (new UseCustomTileSourceFactoryConfig(newMapData))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(newBackgroundData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                assessmentSection.BackgroundData.Attach(backgroundDataObserver);
                SetBackgroundData(assessmentSection, mapData);

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialog = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    var connectButton = new ButtonTester("connectToButton", dialog);
                    connectButton.Click();

                    var layersControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                    layersControl.SetCurrentCell(layersControl.GetCell(0, 0));

                    var button = new ButtonTester("selectButton", dialog);
                    button.Click();
                    dialog.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundData(newMapData, assessmentSection.BackgroundData);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingValidWellKnownMapDataFromContextMenu_ThenBackgroundDataSetAndNotifiesObserver()
        {
            // Given
            var mockRepository = new MockRepository();

            var backgroundDataObserver = mockRepository.StrictMock<IObserver>();
            backgroundDataObserver.Expect(o => o.UpdateObserver());

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            var assessmentSection = new AssessmentSectionStub();

            const WellKnownTileSource wellKnownTileSource = WellKnownTileSource.BingAerial;
            var newMapData = new WellKnownTileSourceMapData(wellKnownTileSource);
            BackgroundData newBackgroundData = BackgroundDataConverter.ConvertTo(newMapData);

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "EmptyWmtsConnectionInfo")
            }))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(newBackgroundData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                assessmentSection.BackgroundData.Attach(backgroundDataObserver);
                SetBackgroundData(assessmentSection, mapData);

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialog = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;

                    var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                    comboBox.SelectedItem = ((BackgroundMapDataSelectionControl[]) comboBox.DataSource).OfType<WellKnownMapDataControl>().First();
                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                    dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(0, 0));

                    var button = new ButtonTester("selectButton", dialog);
                    button.Click();
                    dialog.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundData(newMapData, assessmentSection.BackgroundData);
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
            var backgroundDataObserver = mockRepository.StrictMock<IObserver>();

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            WmtsMapData newMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData newBackgroundData = BackgroundDataConverter.ConvertTo(newMapData);

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "noConfig")
            }))
            using (new UseCustomTileSourceFactoryConfig(newMapData))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = mockRepository.Stub<IViewCommands>();
                var mainWindow = mockRepository.Stub<IMainWindow>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(newBackgroundData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                var assessmentSection = new AssessmentSectionStub();
                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.BackgroundData.Attach(backgroundDataObserver);

                SetBackgroundData(assessmentSection, mapData);

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = (BackgroundMapDataSelectionDialog) new FormTester(name).TheObject;
                    tester.DialogResult = DialogResult.Cancel;
                    tester.Close();
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundData, assessmentSection, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    BackgroundDataTestHelper.AssertBackgroundData(backgroundData, assessmentSection.BackgroundData);
                }
            }

            mockRepository.VerifyAll();
        }

        private static void SetBackgroundData(IAssessmentSection assessmentSection, WmtsMapData mapData)
        {
            assessmentSection.BackgroundData.Name = mapData.Name;
            assessmentSection.BackgroundData.IsVisible = mapData.IsVisible;
            assessmentSection.BackgroundData.Transparency = mapData.Transparency;

            assessmentSection.BackgroundData.Configuration = new WmtsBackgroundDataConfiguration(mapData.IsConfigured,
                                                                                                 mapData.SourceCapabilitiesUrl,
                                                                                                 mapData.SelectedCapabilityIdentifier,
                                                                                                 mapData.PreferredFormat);
        }

        private static void AssertBackgroundData(WmtsMapData mapData, BackgroundData backgroundData)
        {
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);

            var configuration = (WmtsBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(mapData.IsConfigured, configuration.IsConfigured);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, configuration.SourceCapabilitiesUrl);
            Assert.AreEqual(mapData.SelectedCapabilityIdentifier, configuration.SelectedCapabilityIdentifier);
            Assert.AreEqual(mapData.PreferredFormat, configuration.PreferredFormat);
        }

        private static void AssertBackgroundData(WellKnownTileSourceMapData mapData, BackgroundData backgroundData)
        {
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);

            var configuration = (WellKnownBackgroundDataConfiguration) backgroundData.Configuration;
            var wellKnownTileSource = (RiskeerWellKnownTileSource) mapData.TileSource;
            Assert.AreEqual(wellKnownTileSource, configuration.WellKnownTileSource);
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(BackgroundData));
        }
    }
}