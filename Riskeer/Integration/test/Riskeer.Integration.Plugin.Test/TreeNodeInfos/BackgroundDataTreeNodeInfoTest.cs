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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using Core.Components.Gis.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Common.Util.TypeConverters;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.Forms.PresentationObjects;
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
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(Arg.Any<object>(), treeViewControl).Returns(menuBuilder);
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
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_Always_ContextMenuItemSelectMapLayerEnabled()
        {
            // Setup
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());
            var assessmentSectionStateRootContext = new AssessmentSectionStateRootContext(new AssessmentSection(AssessmentSectionComposition.Dike));
            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(backgroundData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(backgroundData, assessmentSectionStateRootContext, treeViewControl))
                    {
						// Assert
                        const string expectedItemText = "&Selecteren...";
                        const string expectedItemTooltip = "Selecteer een achtergrondkaart.";
                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, selectContextMenuIndex,
                                                                      expectedItemText, expectedItemTooltip,
                                                                      RiskeerCommonFormsResources.MapsIcon);
                    }
                }
            }
        }

        [Test]
        public void GivenNoMapDataSet_WhenSelectingValidWmtsMapDataFromContextMenu_ThenBackgroundDataSetAndNotifiesObserver()
        {
            // Given
            var tileFactory = Substitute.For<ITileSourceFactory>();
            var newMapData = new WmtsMapData("Actueel Hoogtebestand Nederland (AHN1)",
                                             "https://geodata.nationaalgeoregister.nl/tiles/service/wmts/ahn1?request=GetCapabilities",
                                             "()", "image/png");
            tileFactory.GetWmtsTileSources(Arg.Any<string>()).Returns(new[]
                       {
                           new TestWmtsTileSource(newMapData)
                       });

            var backgroundDataObserver = Substitute.For<IObserver>();

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
                var viewCommands = Substitute.For<IViewCommands>();
                var mainWindow = Substitute.For<IMainWindow>();

                IGui gui = StubFactory.CreateGuiStub();
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);
                gui.Get(backgroundData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                var assessmentSectionStateRootContext = new AssessmentSectionStateRootContext(assessmentSection);
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(backgroundData, assessmentSectionStateRootContext, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundData(newMapData, assessmentSection.BackgroundData);
                }
                backgroundDataObserver.Received().UpdateObserver();
                tileFactory.Received().GetWmtsTileSources(Arg.Any<string>());
            }
        }

        [Test]
        public void GivenNoMapDataSet_WhenSelectingMapDataFromContextMenuCancelled_ThenNoObserversNotified()
        {
            // Given
            var assessmentSectionObserver = Substitute.For<IObserver>();
            var backgroundDataObserver = Substitute.For<IObserver>();

            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(new WellKnownTileSourceMapData(WellKnownTileSource.BingHybrid));

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "EmptyWmtsConnectionInfo")
            }))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = Substitute.For<IViewCommands>();
                var mainWindow = Substitute.For<IMainWindow>();

                IGui gui = StubFactory.CreateGuiStub();
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);
                gui.Get(backgroundData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                var assessmentSectionStateRootContext = new AssessmentSectionStateRootContext(assessmentSection);
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(backgroundData, assessmentSectionStateRootContext, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    BackgroundDataTestHelper.AssertBackgroundData(oldBackgroundData, assessmentSection.BackgroundData);
                }
            }
            assessmentSectionObserver.DidNotReceive().UpdateObserver();
            backgroundDataObserver.DidNotReceive().UpdateObserver();
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingValidWmtsMapDataFromContextMenu_ThenBackgroundDataSetAndNotifiesObserver()
        {
            // Given
            var backgroundDataObserver = Substitute.For<IObserver>();

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            WmtsMapData newMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData newBackgroundData = BackgroundDataConverter.ConvertTo(newMapData);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSectionStateRootContext = new AssessmentSectionStateRootContext(assessmentSection);

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "noConfig")
            }))
            using (new UseCustomTileSourceFactoryConfig(newMapData))
            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RiskeerPlugin())
            {
                var viewCommands = Substitute.For<IViewCommands>();
                var mainWindow = Substitute.For<IMainWindow>();

                IGui gui = StubFactory.CreateGuiStub();
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);
                gui.Get(newBackgroundData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundData, assessmentSectionStateRootContext, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundData(newMapData, assessmentSection.BackgroundData);
                }
                backgroundDataObserver.Received().UpdateObserver();
            }
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingValidWellKnownMapDataFromContextMenu_ThenBackgroundDataSetAndNotifiesObserver()
        {
            // Given
            var backgroundDataObserver = Substitute.For<IObserver>();

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSectionStateRootContext = new AssessmentSectionStateRootContext(assessmentSection);

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
                var viewCommands = Substitute.For<IViewCommands>();
                var mainWindow = Substitute.For<IMainWindow>();

                IGui gui = StubFactory.CreateGuiStub();
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);
                gui.Get(newBackgroundData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundData, assessmentSectionStateRootContext, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    AssertBackgroundData(newMapData, assessmentSection.BackgroundData);
                }
                backgroundDataObserver.Received().UpdateObserver();
            }
        }

        [Test]
        public void GivenMapDataSet_WhenSelectingMapDataFromContextMenuCancelled_ThenNoObserversNotified()
        {
            // Given
            var assessmentSectionObserver = Substitute.For<IObserver>();
            var backgroundDataObserver = Substitute.For<IObserver>();

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
                var viewCommands = Substitute.For<IViewCommands>();
                var mainWindow = Substitute.For<IMainWindow>();

                IGui gui = StubFactory.CreateGuiStub();
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);
                gui.Get(newBackgroundData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                var assessmentSectionStateRootContext = new AssessmentSectionStateRootContext(assessmentSection);
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(newBackgroundData, assessmentSectionStateRootContext, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[selectContextMenuIndex].PerformClick();

                    // Then
                    BackgroundDataTestHelper.AssertBackgroundData(backgroundData, assessmentSection.BackgroundData);
                }
            }
            backgroundDataObserver.DidNotReceive().UpdateObserver();
            assessmentSectionObserver.DidNotReceive().UpdateObserver();
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