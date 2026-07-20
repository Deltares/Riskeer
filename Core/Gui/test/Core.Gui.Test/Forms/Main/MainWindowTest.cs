// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Chart.Forms;
using Core.Components.Gis.Forms;
using Core.Gui.Commands;
using Core.Gui.Forms.Chart;
using Core.Gui.Forms.Log;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.Map;
using Core.Gui.Forms.Project;
using Core.Gui.Forms.PropertyView;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using Core.Gui.Settings;
using Core.Gui.Test.Forms.ViewHost;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.Map;
using NSubstitute;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using FontFamily = System.Windows.Media.FontFamily;

namespace Core.Gui.Test.Forms.Main
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTest : NUnitFormTest
    {
        private MessageWindowLogAppender originalValue;

        [SetUp]
        public void SetUp()
        {
            originalValue = MessageWindowLogAppender.Instance;
            MessageWindowLogAppender.Instance = new MessageWindowLogAppender();
        }

        [TearDown]
        public override void TearDown()
        {
            MessageWindowLogAppender.Instance = originalValue;
            base.TearDown();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var mainWindow = new MainWindow())
            {
                // Assert
                Assert.IsInstanceOf<IMainWindow>(mainWindow);
                Assert.IsInstanceOf<IDisposable>(mainWindow);
                Assert.IsInstanceOf<ISynchronizeInvoke>(mainWindow);

                Assert.IsFalse(mainWindow.IsWindowDisposed);
                Assert.IsFalse(mainWindow.Visible);

                Assert.IsNull(mainWindow.MessageWindow);
                Assert.IsNull(mainWindow.PropertyGrid);
                Assert.IsNull(mainWindow.ProjectExplorer);

                Assert.IsNotNull(mainWindow.Handle);
                Assert.IsFalse(mainWindow.InvokeRequired,
                               "'mainWindow' instance on same thread as test, therefore invocation not required.");

                Assert.IsEmpty(mainWindow.Title);
                Assert.AreEqual(WindowState.Maximized, mainWindow.WindowState);
                Assert.AreEqual(ResizeMode.CanResizeWithGrip, mainWindow.ResizeMode);
                Assert.AreEqual(FlowDirection.LeftToRight, mainWindow.FlowDirection);
                Assert.AreEqual("RiskeerMainWindow", mainWindow.Name);

                Assert.IsNotNull(mainWindow.NewProjectCommand);
                Assert.IsNotNull(mainWindow.SaveProjectCommand);
                Assert.IsNotNull(mainWindow.SaveProjectAsCommand);
                Assert.IsNotNull(mainWindow.OpenProjectCommand);
                Assert.IsNotNull(mainWindow.CloseApplicationCommand);
                Assert.IsNotNull(mainWindow.ToggleBackstageCommand);
                Assert.IsNotNull(mainWindow.ToggleProjectExplorerCommand);
                Assert.IsNotNull(mainWindow.ToggleMapLegendViewCommand);
                Assert.IsNotNull(mainWindow.ToggleChartLegendViewCommand);
                Assert.IsNotNull(mainWindow.TogglePropertyGridViewCommand);
                Assert.IsNotNull(mainWindow.ToggleMessageWindowCommand);
                Assert.IsNotNull(mainWindow.OpenLogFileCommand);

                Assert.IsNull(mainWindow.BackstageViewModel);
            }
        }

        [Test]
        public void SetGui_Always_ExpectedValues()
        {
            // Setup
            var gui = Substitute.For<IGui>();
            gui.FixedSettings.Returns(new GuiCoreSettings());
            using (var mainWindow = new MainWindow())
            {
                // Call
                mainWindow.SetGui(gui);

                // Assert
                Assert.IsNotNull(mainWindow.BackstageViewModel);
            }
        }

        [Test]
        public void Dispose_SetIsWindowDisposedTrue()
        {
            // Setup
            using (var mainWindow = new MainWindow())
            {
                // Call
                mainWindow.Dispose();

                // Assert
                Assert.IsTrue(mainWindow.IsWindowDisposed);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Visible_SettingValueWithoutHavingSetGui_ThrowInvalidOperationException(bool newVisibleValue)
        {
            // Setup
            using (var mainWindow = new MainWindow())
            {
                // Call
                void Call() => mainWindow.Visible = newVisibleValue;

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }
        }

        [Test]
        public void Visible_SetToTrue_ShowMainForm()
        {
            // Setup
            var viewHost = Substitute.For<IViewHost>();
            viewHost.ToolViews.Returns(new IView[0]);

            var gui = Substitute.For<IGui>();
            gui.FixedSettings.Returns(new GuiCoreSettings());
            gui.Plugins.Returns(Enumerable.Empty<PluginBase>().ToList());
            gui.ViewHost.Returns(viewHost);
            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.Visible = true;

                // Assert
                Assert.IsTrue(mainWindow.Visible);
                Assert.IsTrue(mainWindow.IsVisible);
                Assert.AreEqual(Visibility.Visible, mainWindow.Visibility);
            }
        }

        [Test]
        public void Visible_SetToFalse_HideMainForm()
        {
            // Setup
            var viewHost = Substitute.For<IViewHost>();
            viewHost.ToolViews.Returns(new IView[0]);

            var gui = Substitute.For<IGui>();
            gui.FixedSettings.Returns(new GuiCoreSettings());
            gui.Plugins.Returns(Enumerable.Empty<PluginBase>().ToList());
            gui.ViewHost.Returns(viewHost);
            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.Visible = true;

                // Call
                mainWindow.Visible = false;

                // Assert
                Assert.IsFalse(mainWindow.Visible);
                Assert.IsFalse(mainWindow.IsVisible);
                Assert.AreEqual(Visibility.Hidden, mainWindow.Visibility);
            }
        }

        [Test]
        public void SubscribeToGui_NoGuiSet_DoNothing()
        {
            // Setup
            using (var mainWindow = new MainWindow())
            {
                // Call
                void Call() => mainWindow.SubscribeToGui();

                // Assert
                Assert.DoesNotThrow(Call);
            }
        }

        [Test]
        public void SubscribeToGui_GuiSet_AttachEvents()
        {
            // Setup
            var viewHost = Substitute.For<IViewHost>();
            EventHandler<ViewChangeEventArgs> opened = null;
            EventHandler<ViewChangeEventArgs> front = null;
            EventHandler<ViewChangeEventArgs> closed = null;
            EventHandler<EventArgs> changed = null;
            viewHost.When(x => x.ViewOpened += Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => opened = ci.Arg<EventHandler<ViewChangeEventArgs>>());
            viewHost.When(x => x.ViewBroughtToFront += Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => front = ci.Arg<EventHandler<ViewChangeEventArgs>>());
            viewHost.When(x => x.ViewClosed += Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => closed = ci.Arg<EventHandler<ViewChangeEventArgs>>());
            viewHost.When(x => x.ActiveDocumentViewChanged += Arg.Any<EventHandler<EventArgs>>())
                    .Do(ci => changed = ci.Arg<EventHandler<EventArgs>>());

            var gui = Substitute.For<IGui>();
            gui.ViewHost.Returns(viewHost);
            gui.FixedSettings.Returns(new GuiCoreSettings());
            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.SubscribeToGui();
            }

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opened, Is.Not.Null);
                Assert.That(front, Is.Not.Null);
                Assert.That(closed, Is.Not.Null);
                Assert.That(changed, Is.Not.Null);
            });
        }

        [Test]
        public void UnsubscribeFromGui_NoGuiSet_DoNothing()
        {
            // Setup
            using (var mainWindow = new MainWindow())
            {
                // Call
                void Call() => mainWindow.UnsubscribeFromGui();

                // Assert
                Assert.DoesNotThrow(Call);
            }
        }

        [Test]
        public void UnsubscribeFromGui_GuiSetAndSubscribed_DetachEvents()
        {
            // Setup
            var viewHost = Substitute.For<IViewHost>();
            EventHandler<ViewChangeEventArgs> opened = null;
            EventHandler<ViewChangeEventArgs> front = null;
            EventHandler<ViewChangeEventArgs> closed = null;
            EventHandler<EventArgs> changed = null;

            viewHost.When(x => x.ViewOpened += Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => opened = ci.Arg<EventHandler<ViewChangeEventArgs>>());
            viewHost.When(x => x.ViewBroughtToFront += Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => front = ci.Arg<EventHandler<ViewChangeEventArgs>>());
            viewHost.When(x => x.ViewClosed += Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => closed = ci.Arg<EventHandler<ViewChangeEventArgs>>());
            viewHost.When(x => x.ActiveDocumentViewChanged += Arg.Any<EventHandler<EventArgs>>())
                    .Do(ci => changed = ci.Arg<EventHandler<EventArgs>>());
            viewHost.When(x => x.ViewOpened -= Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => opened = null);
            viewHost.When(x => x.ViewBroughtToFront -= Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => front = null);
            viewHost.When(x => x.ViewClosed -= Arg.Any<EventHandler<ViewChangeEventArgs>>())
                    .Do(ci => closed = null);
            viewHost.When(x => x.ActiveDocumentViewChanged -= Arg.Any<EventHandler<EventArgs>>())
                    .Do(ci => changed = null);

            var gui = Substitute.For<IGui>();
            gui.ViewHost.Returns(viewHost);
            gui.FixedSettings.Returns(new GuiCoreSettings());
            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.SubscribeToGui();

                // Call
                mainWindow.UnsubscribeFromGui();
            }

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(opened, Is.Null);
                Assert.That(front, Is.Null);
                Assert.That(closed, Is.Null);
                Assert.That(changed, Is.Null);
            });
        }

        [Test]
        public void InitPropertiesWindowOrBringToFront_GuiNotSet_ThrowsInvalidOperationException()
        {
            // Setup
            using (var mainWindow = new MainWindow())
            {
                // Call
                void Call() => mainWindow.InitPropertiesWindowOrBringToFront();

                // Assert
                var exception = Assert.Throws<InvalidOperationException>(Call);
                Assert.AreEqual("Must call 'SetGui' before calling 'InitPropertiesWindowOrBringToFront'.", exception.Message);
            }
        }

        [Test]
        public void InitPropertiesWindowOrBringToFront_GuiSet_PropertyGridViewInitialized()
        {
            // Setup
            var selectedObject = new object();
            var viewHost = new AvalonDockViewHost();
            var selectedObjectProperties = Substitute.For<IObjectProperties>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(selectedObject)
                            .Returns(selectedObjectProperties);

            var gui = Substitute.For<IGui>();
            gui.ViewHost.Returns(viewHost);
            gui.Selection = selectedObject;
            gui.PropertyResolver.Returns(propertyResolver);
            gui.FixedSettings.Returns(new GuiCoreSettings());
            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitPropertiesWindowOrBringToFront();

                // Assert
                Assert.IsNull(viewHost.ActiveDocumentView);
                Assert.AreSame(viewHost.ToolViews.ElementAt(0), mainWindow.PropertyGrid, "PropertyGrid instance should remain the same.");
                Assert.AreEqual(selectedObject, mainWindow.PropertyGrid.Data);
            }

            propertyResolver.Received().GetObjectProperties(selectedObject);
        }

        [Test]
        public void InitPropertiesWindowOrBringToFront_GuiSetAndCalledTwice_PropertyGridViewInstanceNotUpdatedRedundantly()
        {
            // Setup
            var selectedObject = new object();
            var viewHost = new AvalonDockViewHost();
            var selectedObjectProperties = Substitute.For<IObjectProperties>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(selectedObject)
                            .Returns(selectedObjectProperties);

            var gui = Substitute.For<IGui>();
            gui.ViewHost.Returns(viewHost);
            gui.Selection = selectedObject;
            gui.PropertyResolver.Returns(propertyResolver);
            gui.FixedSettings.Returns(new GuiCoreSettings());
            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.InitPropertiesWindowOrBringToFront();

                IView originalPropertyGrid = mainWindow.PropertyGrid;

                // Call
                mainWindow.InitPropertiesWindowOrBringToFront();

                // Assert
                Assert.IsNull(viewHost.ActiveDocumentView);
                Assert.AreSame(originalPropertyGrid, mainWindow.PropertyGrid, "PropertyGrid instance should remain the same.");
                Assert.AreSame(selectedObject, mainWindow.PropertyGrid.Data);
            }

            propertyResolver.Received().GetObjectProperties(selectedObject);
        }

        [Test]
        public void InitializeToolWindows_GuiNotSet_ThrowInvalidOperationException()
        {
            // Setup
            using (var mainWindow = new MainWindow())
            {
                // Call
                void Call() => mainWindow.InitializeToolWindows();

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }
        }

        [Test]
        public void InitializeToolWindows_GuiSet_ToolWindowsInitialized()
        {
            // Setup
            var selectedObject = new object();
            var viewHost = new AvalonDockViewHost();

            var treeNodeInfos = new TreeNodeInfo[]
            {
                new TreeNodeInfo<IProject>()
            };
            var selectedObjectProperties = Substitute.For<IObjectProperties>();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(selectedObject)
                            .Returns(selectedObjectProperties);

            var viewCommands = Substitute.For<IViewCommands>();
            var project = Substitute.For<IProject>();

            var gui = Substitute.For<IGui>();
            gui.ViewHost.Returns(viewHost);
            gui.PropertyResolver.Returns(propertyResolver);
            gui.ViewCommands.Returns(viewCommands);
            gui.Project.Returns(project);
            gui.GetTreeNodeInfos().Returns(treeNodeInfos);
            gui.FixedSettings.Returns(new GuiCoreSettings());
            gui.Selection = selectedObject;

            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitializeToolWindows();

                // Assert
                Assert.IsInstanceOf<ProjectExplorer>(mainWindow.ProjectExplorer);
                AssertViewTitle(viewHost.DockingManager, mainWindow.ProjectExplorer, "Projectverkenner");
                Assert.IsNull(mainWindow.ProjectExplorer.Data);

                Assert.IsInstanceOf<PropertyGridView>(mainWindow.PropertyGrid);
                AssertViewTitle(viewHost.DockingManager, mainWindow.PropertyGrid, "Eigenschappen");
                Assert.AreEqual(selectedObject, mainWindow.PropertyGrid.Data);

                Assert.IsInstanceOf<MessageWindow>(mainWindow.MessageWindow);
                AssertViewTitle(viewHost.DockingManager, mainWindow.MessageWindow, "Berichten");

                Assert.IsInstanceOf<MapLegendView>(mainWindow.MapLegendView);
                AssertViewTitle(viewHost.DockingManager, mainWindow.MapLegendView, "Kaart");
                Assert.IsNull(GetMapControl(mainWindow.MapLegendView));

                Assert.IsInstanceOf<ChartLegendView>(mainWindow.ChartLegendView);
                AssertViewTitle(viewHost.DockingManager, mainWindow.ChartLegendView, "Grafiek");
                Assert.IsNull(GetChartControl(mainWindow.ChartLegendView));

                Assert.IsNull(viewHost.ActiveDocumentView);
            }

            propertyResolver.Received().GetObjectProperties(selectedObject);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenToggleProjectExplorerCalled_ThenProjectExplorerToggled(bool initiallyAdded)
        {
            ToggleToolViewAndAssert(window => window.ProjectExplorer, window => window.ToggleProjectExplorerCommand, initiallyAdded);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenTogglePropertyGridViewCalled_ThenPropertyGridViewToggled(bool initiallyAdded)
        {
            ToggleToolViewAndAssert(window => window.PropertyGrid, window => window.TogglePropertyGridViewCommand, initiallyAdded);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenToggleMessageWindowCalled_ThenMessageWindowToggled(bool initiallyAdded)
        {
            ToggleToolViewAndAssert(window => window.MessageWindow, window => window.ToggleMessageWindowCommand, initiallyAdded);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenToggleMapLegendViewCalled_ThenMapLegendViewToggled(bool initiallyAdded)
        {
            ToggleToolViewAndAssert(window => window.MapLegendView, window => window.ToggleMapLegendViewCommand, initiallyAdded);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenToggleChartLegendViewCalled_ThenChartLegendViewToggled(bool initiallyAdded)
        {
            ToggleToolViewAndAssert(window => window.ChartLegendView, window => window.ToggleChartLegendViewCommand, initiallyAdded);
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndNoStateInfos_WhenProjectSet_ThenNoDataSetOnProjectExplorer()
        {
            // Given
            var project = Substitute.For<IProject>();
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                // When
                gui.SetProject(project, null);

                // Then
                Assert.IsNull(mainWindow.ProjectExplorer.Data);
            }
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndSingleStateInfo_WhenProjectSet_ThenExpectedDataSetOnProjectExplorer()
        {
            // Given
            var project = Substitute.For<IProject>();
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin(new[]
                {
                    new StateInfo("Name", "Symbol", new FontFamily(), p => p)
                }));

                gui.Run();

                mainWindow.SetGui(gui);

                // When
                gui.SetProject(project, null);

                // Then
                Assert.AreSame(project, mainWindow.ProjectExplorer.Data);
            }
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndMultipleStateInfos_WhenProjectSet_ThenExpectedDataSetOnProjectExplorer()
        {
            // Given
            var project = Substitute.For<IProject>();
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin(new[]
                {
                    new StateInfo("Name", "Symbol", new FontFamily(), p => p),
                    new StateInfo("Name", "Symbol", new FontFamily(), p => new object())
                }));

                gui.Run();

                mainWindow.SetGui(gui);

                // When
                gui.SetProject(project, null);

                // Then
                Assert.AreSame(project, mainWindow.ProjectExplorer.Data);
            }
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewAdded_ThenComponentsUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                // Precondition
                Assert.IsNull(GetMapControl(mapLegendView));

                // When
                gui.ViewHost.AddDocumentView(view, string.Empty, string.Empty, null);

                // Then
                Assert.AreSame(view.Map, GetMapControl(mapLegendView));
            }
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewBroughtToFront_ThenComponentsUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestMapView();
                var view2 = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                gui.ViewHost.AddDocumentView(view1, string.Empty, string.Empty, null);
                gui.ViewHost.AddDocumentView(view2, string.Empty, string.Empty, null);

                // Precondition
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));

                // When
                gui.ViewHost.BringToFront(view1);

                // Then
                Assert.AreSame(view1.Map, GetMapControl(mapLegendView));
            }
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewRemoved_ThenComponentsUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                gui.ViewHost.AddDocumentView(view, string.Empty, string.Empty, null);

                // Precondition
                Assert.AreSame(view.Map, GetMapControl(mapLegendView));

                // When
                gui.ViewHost.Remove(view);

                // Then
                Assert.IsNull(GetMapControl(mapLegendView));
            }
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenOtherMapViewRemoved_ThenComponentsNotUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestMapView();
                var view2 = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                gui.ViewHost.AddDocumentView(view1, string.Empty, string.Empty, null);
                gui.ViewHost.AddDocumentView(view2, string.Empty, string.Empty, null);

                // Precondition
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));

                // When
                gui.ViewHost.Remove(view1);

                // Then
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));
            }
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenChartViewAdded_ThenComponentsUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                // Precondition
                Assert.IsNull(GetChartControl(chartLegendView));

                // When
                gui.ViewHost.AddDocumentView(view, string.Empty, string.Empty, null);

                // Then
                Assert.AreSame(view.Chart, GetChartControl(chartLegendView));
            }
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenChartViewBroughtToFront_ThenComponentsUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestChartView();
                var view2 = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                gui.ViewHost.AddDocumentView(view1, string.Empty, string.Empty, null);
                gui.ViewHost.AddDocumentView(view2, string.Empty, string.Empty, null);

                // Precondition
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));

                // When
                gui.ViewHost.BringToFront(view1);

                // Then
                Assert.AreSame(view1.Chart, GetChartControl(chartLegendView));
            }
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenChartViewRemoved_ThenComponentsUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                gui.ViewHost.AddDocumentView(view, string.Empty, string.Empty, null);

                // Precondition
                Assert.AreSame(view.Chart, GetChartControl(chartLegendView));

                // When
                gui.ViewHost.Remove(view);

                // Then
                Assert.IsNull(GetChartControl(chartLegendView));
            }
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenOtherChartViewRemoved_ThenComponentsNotUpdated()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestChartView();
                var view2 = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                gui.ViewHost.AddDocumentView(view1, string.Empty, string.Empty, null);
                gui.ViewHost.AddDocumentView(view2, string.Empty, string.Empty, null);

                // Precondition
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));

                // When
                gui.ViewHost.Remove(view1);

                // Then
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenNewProjectIsCalled_ThenCreateNewProject(bool backstageVisible)
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            projectFactory.CreateNewProject().Returns(Substitute.For<IProject>());
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                if (backstageVisible)
                {
                    mainWindow.ToggleBackstageCommand.Execute(null);
                }

                // When
                mainWindow.NewProjectCommand.Execute(null);

                // Then
                Assert.AreEqual(Visibility.Collapsed, mainWindow.BackstageDockPanel.Visibility);
                Assert.AreEqual(Visibility.Visible, mainWindow.MainDockPanel.Visibility);
            }

            projectFactory.Received().CreateNewProject();
        }

        [Test]
        public void GivenMainWindowWithoutGui_WhenSaveProjectCanExecuteIsCalled_ThenReturnsFalse()
        {
            // Given
            using (var mainWindow = new MainWindow())
            {
                // When
                bool canExecute = mainWindow.SaveProjectCommand.CanExecute(null);

                // Then
                Assert.IsFalse(canExecute);
            }
        }

        [Test]
        public void GivenMainWindowWithoutProject_WhenSaveProjectCanExecuteIsCalled_ThenReturnsFalse()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                // When
                bool canExecute = mainWindow.SaveProjectCommand.CanExecute(null);

                // Then
                Assert.IsFalse(canExecute);
            }
        }

        [Test]
        public void GivenMainWindowWithProject_WhenSaveProjectCanExecuteIsCalled_ThenReturnsTrue()
        {
            // Given
            var project = Substitute.For<IProject>();
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                gui.SetProject(project, null);

                // When
                bool canExecute = mainWindow.SaveProjectCommand.CanExecute(null);

                // Then
                Assert.IsTrue(canExecute);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenSaveProjectIsCalled_ThenProjectSaved(bool backstageVisible)
        {
            // Given
            string directoryPath = TestHelper.GetScratchPadPath(nameof(MainWindowTest));
            string someValidFilePath = Path.Combine(directoryPath, nameof(GivenMainWindow_WhenSaveProjectIsCalled_ThenProjectSaved));

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), directoryPath))
            {
                var project = Substitute.For<IProject>();

                var projectStore = Substitute.For<IStoreProject>();
                projectStore.SaveProjectFileFilter.Returns(string.Empty);
                projectStore.HasStagedProject.Returns(false);

                var projectMigrator = Substitute.For<IMigrateProject>();
                var projectFactory = Substitute.For<IProjectFactory>();
                DialogBoxHandler = (s, hWnd) =>
                {
                    var saveFileDialogTester = new SaveFileDialogTester(hWnd);
                    saveFileDialogTester.SaveFile(someValidFilePath);

                    DialogBoxHandler = (name, wnd) =>
                    {
                        // Expect progress dialog, which will close automatically.    
                    };
                };

                var guiCoreSettings = new GuiCoreSettings
                {
                    ApplicationIcon = SystemIcons.Application
                };

                using (var mainWindow = new MainWindow())
                using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
                {
                    gui.Run();

                    mainWindow.SetGui(gui);
                    gui.SetProject(project, null);

                    if (backstageVisible)
                    {
                        mainWindow.ToggleBackstageCommand.Execute(null);
                    }

                    // When
                    mainWindow.SaveProjectCommand.Execute(null);

                    // Then
                    Assert.AreEqual(Visibility.Collapsed, mainWindow.BackstageDockPanel.Visibility);
                    Assert.AreEqual(Visibility.Visible, mainWindow.MainDockPanel.Visibility);
                }

                projectStore.Received().SaveProjectAs(someValidFilePath);
                projectStore.Received().StageProject(project);
            }
        }

        [Test]
        public void GivenMainWindowWithoutGui_WhenSaveProjectAsCanExecuteIsCalled_ThenReturnsFalse()
        {
            // Given
            using (var mainWindow = new MainWindow())
            {
                // When
                bool canExecute = mainWindow.SaveProjectAsCommand.CanExecute(null);

                // Then
                Assert.IsFalse(canExecute);
            }
        }

        [Test]
        public void GivenMainWindowWithoutProject_WhenSaveProjectAsCanExecuteIsCalled_ThenReturnsFalse()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                // When
                bool canExecute = mainWindow.SaveProjectAsCommand.CanExecute(null);

                // Then
                Assert.IsFalse(canExecute);
            }
        }

        [Test]
        public void GivenMainWindowWithProject_WhenSaveProjectAsCanExecuteIsCalled_ThenReturnsTrue()
        {
            // Given
            var project = Substitute.For<IProject>();
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                gui.SetProject(project, null);

                // When
                bool canExecute = mainWindow.SaveProjectAsCommand.CanExecute(null);

                // Then
                Assert.IsTrue(canExecute);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenSaveProjectAsIsCalled_ThenProjectSaved(bool backstageVisible)
        {
            // Given
            string directoryPath = TestHelper.GetScratchPadPath(nameof(MainWindowTest));
            string someValidFilePath = Path.Combine(directoryPath, nameof(GivenMainWindow_WhenSaveProjectAsIsCalled_ThenProjectSaved));

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), directoryPath))
            {
                var project = Substitute.For<IProject>();

                var projectStore = Substitute.For<IStoreProject>();
                projectStore.SaveProjectFileFilter.Returns(string.Empty);
                projectStore.HasStagedProject.Returns(false);

                var projectMigrator = Substitute.For<IMigrateProject>();
                var projectFactory = Substitute.For<IProjectFactory>();
                DialogBoxHandler = (s, hWnd) =>
                {
                    var saveFileDialogTester = new SaveFileDialogTester(hWnd);
                    saveFileDialogTester.SaveFile(someValidFilePath);

                    DialogBoxHandler = (name, wnd) =>
                    {
                        // Expect progress dialog, which will close automatically.    
                    };
                };

                var guiCoreSettings = new GuiCoreSettings
                {
                    ApplicationIcon = SystemIcons.Application
                };

                using (var mainWindow = new MainWindow())
                using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
                {
                    gui.Run();

                    mainWindow.SetGui(gui);
                    gui.SetProject(project, null);

                    if (backstageVisible)
                    {
                        mainWindow.ToggleBackstageCommand.Execute(null);
                    }

                    // When
                    mainWindow.SaveProjectAsCommand.Execute(null);

                    // Then
                    Assert.AreEqual(Visibility.Collapsed, mainWindow.BackstageDockPanel.Visibility);
                    Assert.AreEqual(Visibility.Visible, mainWindow.MainDockPanel.Visibility);
                }

                projectStore.Received().SaveProjectAs(someValidFilePath);
                projectStore.Received().StageProject(project);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenOpenProjectIsCalled_ThenProjectOpened(bool backstageVisible)
        {
            // Given
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Core.Gui);
            string filePath = Path.Combine(directoryPath, nameof(MainWindowTest), "Project.risk");
            var projectStore = Substitute.For<IStoreProject>();
            projectStore.LoadProject(filePath).Returns(Substitute.For<IProject>());
            projectStore.OpenProjectFileFilter.Returns(string.Empty);

            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            DialogBoxHandler = (s, hWnd) =>
            {
                var openFileDialogTester = new OpenFileDialogTester(hWnd);
                openFileDialogTester.OpenFile(filePath);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect progress dialog, which will close automatically.    
                };
            };

            var guiCoreSettings = new GuiCoreSettings
            {
                ApplicationIcon = SystemIcons.Application
            };

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, guiCoreSettings))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                if (backstageVisible)
                {
                    mainWindow.ToggleBackstageCommand.Execute(null);
                }

                // When
                mainWindow.OpenProjectCommand.Execute(null);

                // Then
                Assert.AreEqual(Visibility.Collapsed, mainWindow.BackstageDockPanel.Visibility);
                Assert.AreEqual(Visibility.Visible, mainWindow.MainDockPanel.Visibility);
            }

            projectStore.Received().LoadProject(filePath);
        }

        [Test]
        public void GivenMainWindowWithoutViewTabOpen_WhenCanExecuteCloseViewTabCommand_ThenFalse()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                // When
                bool canExecute = mainWindow.CloseViewTabCommand.CanExecute(null);

                // Then
                Assert.IsFalse(canExecute);
            }
        }

        [Test]
        public void GivenMainWindowWithViewTabOpen_WhenCanExecuteCloseViewTabCommand_ThenTrue()
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                gui.ViewHost.AddDocumentView(new TestView(), string.Empty, string.Empty, null);

                // When
                bool canExecute = mainWindow.CloseViewTabCommand.CanExecute(null);

                // Then
                Assert.IsTrue(canExecute);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenExecuteToggleBackstageCommand_ThenBackstageToggled(bool backstageVisible)
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                if (backstageVisible)
                {
                    mainWindow.ToggleBackstageCommand.Execute(null);
                }

                // Precondition
                AssertVisibility(mainWindow, backstageVisible);

                // When
                mainWindow.ToggleBackstageCommand.Execute(null);

                // Then
                AssertVisibility(mainWindow, !backstageVisible);
            }
        }

        private static void ToggleToolViewAndAssert(Func<MainWindow, IView> getToolViewFunc,
                                                    Func<MainWindow, ICommand> getCommandFunc,
                                                    bool initiallyAdded)
        {
            // Given
            var projectStore = Substitute.For<IStoreProject>();
            var projectMigrator = Substitute.For<IMigrateProject>();
            var projectFactory = Substitute.For<IProjectFactory>();
            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                ICommand command = getCommandFunc(mainWindow);

                if (!initiallyAdded)
                {
                    command.Execute(null);
                }

                // Precondition
                AssertToolWindowPresent(mainWindow.ViewHost.ToolViews, getToolViewFunc(mainWindow), initiallyAdded);

                // When
                command.Execute(null);

                // Then
                AssertToolWindowPresent(mainWindow.ViewHost.ToolViews, getToolViewFunc(mainWindow), !initiallyAdded);
            }
        }

        private static void AssertToolWindowPresent(IEnumerable<IView> toolViews, IView toolView, bool isPresent)
        {
            if (isPresent)
            {
                Assert.IsNotNull(toolView);
                CollectionAssert.Contains(toolViews, toolView);
            }
            else
            {
                Assert.IsNull(toolView);
                CollectionAssert.DoesNotContain(toolViews, toolView);
            }
        }

        private static void AssertVisibility(MainWindow mainWindow, bool backStageVisible)
        {
            if (backStageVisible)
            {
                Assert.AreEqual(Visibility.Collapsed, mainWindow.MainDockPanel.Visibility);
                Assert.AreEqual(Visibility.Visible, mainWindow.BackstageDockPanel.Visibility);
            }
            else
            {
                Assert.AreEqual(Visibility.Visible, mainWindow.MainDockPanel.Visibility);
                Assert.AreEqual(Visibility.Collapsed, mainWindow.BackstageDockPanel.Visibility);
            }
        }

        private static IMapControl GetMapControl(MapLegendView mapLegendView)
        {
            return TypeUtils.GetProperty<IMapControl>(mapLegendView, "MapControl");
        }

        private static IChartControl GetChartControl(ChartLegendView chartLegendView)
        {
            return TypeUtils.GetProperty<IChartControl>(chartLegendView, "ChartControl");
        }

        private static void AssertViewTitle(DockingManager dockingManager, IView view, string expectedTitle)
        {
            LayoutContent layoutContent = dockingManager.Layout.Descendents()
                                                        .OfType<LayoutContent>()
                                                        .First(d => ((WindowsFormsHost) d.Content).Child == view);

            Assert.AreEqual(expectedTitle, layoutContent.Title);
        }
    }
}