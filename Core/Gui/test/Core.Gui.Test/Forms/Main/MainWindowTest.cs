// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Chart.Forms;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Forms;
using Core.Gui.Commands;
using Core.Gui.Forms.Chart;
using Core.Gui.Forms.Log;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.Map;
using Core.Gui.Forms.PropertyGridView;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using Core.Gui.Settings;
using Core.Gui.Test.Forms.ViewHost;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.Map;
using DotSpatial.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using CoreGuiTestUtilResources = Core.Gui.TestUtil.Properties.Resources;

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

                Assert.AreEqual("MainWindow", mainWindow.Title);
                Assert.AreEqual(ResizeMode.CanResizeWithGrip, mainWindow.ResizeMode);
                Assert.AreEqual(FlowDirection.LeftToRight, mainWindow.FlowDirection);
                Assert.AreEqual("MainWindow", mainWindow.Name);

                Assert.IsNull(mainWindow.NewProjectCommand);
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
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                // Call
                mainWindow.SetGui(gui);

                // Assert
                Assert.IsNotNull(mainWindow.BackstageViewModel);
                Assert.IsNotNull(mainWindow.NewProjectCommand);
                mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            gui.Stub(g => g.Plugins).Return(Enumerable.Empty<PluginBase>().ToList());
            gui.Stub(g => g.ViewHost).Return(viewHost);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Visible_SetToFalse_HideMainForm()
        {
            // Setup
            var mocks = new MockRepository();
            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Stub(vm => vm.ToolViews).Return(new IView[0]);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            gui.Stub(g => g.Plugins).Return(Enumerable.Empty<PluginBase>().ToList());
            gui.Stub(g => g.ViewHost).Return(viewHost);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Expect(vh => vh.ViewOpened += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewBroughtToFront += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ActiveDocumentViewChanged += null).IgnoreArguments();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.SubscribeToGui();
            }

            // Assert
            mocks.VerifyAll(); // Expect event subscription
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
            var mocks = new MockRepository();
            var viewHost = mocks.Stub<IViewHost>();
            viewHost.Expect(vh => vh.ViewOpened += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewOpened -= null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewBroughtToFront += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewBroughtToFront -= null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Expect(vh => vh.ActiveDocumentViewChanged += null).IgnoreArguments();
            viewHost.Expect(vh => vh.ActiveDocumentViewChanged -= null).IgnoreArguments();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);
                mainWindow.SubscribeToGui();

                // Call
                mainWindow.UnsubscribeFromGui();
            }

            // Assert
            mocks.VerifyAll(); // Expect event subscription and desubscription
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

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitPropertiesWindowOrBringToFront();

                // Assert
                Assert.IsNull(viewHost.ActiveDocumentView);
                Assert.AreSame(viewHost.ToolViews.ElementAt(0), mainWindow.PropertyGrid, "PropertyGrid instance should remain the same.");
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreEqual(selectedObject, mainWindow.PropertyGrid.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void InitPropertiesWindowOrBringToFront_GuiSetAndCalledTwice_PropertyGridViewInstanceNotUpdatedRedundantly()
        {
            // Setup
            var selectedObject = new object();
            var viewHost = new AvalonDockViewHost();

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties)
                            .Repeat.Once();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            mocks.ReplayAll();

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
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreSame(selectedObject, mainWindow.PropertyGrid.Data);
            }

            mocks.VerifyAll();
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

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();

            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties);

            var viewCommands = mocks.Stub<IViewCommands>();
            var project = mocks.Stub<IProject>();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            gui.Stub(g => g.Project).Return(project);
            gui.Stub(g => g.GetTreeNodeInfos()).Return(treeNodeInfos);
            gui.Stub(g => g.FixedSettings).Return(new GuiCoreSettings());
            mocks.ReplayAll();

            gui.Selection = selectedObject;

            using (var mainWindow = new MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitializeToolWindows();

                // Assert
                Assert.IsInstanceOf<Gui.Forms.ProjectExplorer.ProjectExplorer>(mainWindow.ProjectExplorer);
                Assert.IsNull(mainWindow.ProjectExplorer.Data);

                Assert.IsInstanceOf<Gui.Forms.PropertyGridView.PropertyGridView>(mainWindow.PropertyGrid);
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreEqual(selectedObject, mainWindow.PropertyGrid.Data);

                Assert.IsInstanceOf<MessageWindow>(mainWindow.MessageWindow);
                Assert.AreEqual("Berichten", mainWindow.MessageWindow.Text);

                Assert.IsInstanceOf<MapLegendView>(mainWindow.MapLegendView);
                Assert.IsNull(GetMapControl(mainWindow.MapLegendView));

                Assert.IsInstanceOf<ChartLegendView>(mainWindow.ChartLegendView);
                Assert.IsNull(GetChartControl(mainWindow.ChartLegendView));

                Assert.IsNull(viewHost.ActiveDocumentView);
            }

            mocks.VerifyAll();
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
        public void GivenGuiWithProjectExplorerAndNoStateInfos_WhenInitializeToolWindows_ThenNoDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                // When
                mainWindow.InitializeToolWindows();

                // Then
                Assert.IsNull(mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndSingleStateInfo_WhenInitializeToolWindows_ThenExpectedDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin(new[]
                {
                    new StateInfo("Name", "Symbol", new FontFamily(), p => p)
                }));

                gui.Run();

                mainWindow.SetGui(gui);
                gui.SetProject(project, null);

                // When
                mainWindow.InitializeToolWindows();

                // Then
                Assert.AreSame(project, mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndMultipleStateInfos_WhenInitializeToolWindows_ThenExpectedDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

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
                gui.SetProject(project, null);

                // When
                mainWindow.InitializeToolWindows();

                // Then
                Assert.AreSame(project, mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndNoStateInfos_WhenUpdateProjectExplorer_ThenNoDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project1 = mocks.Stub<IProject>();
            var project2 = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project1);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());

                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                // Precondition
                Assert.IsNotNull(mainWindow.ProjectExplorer);
                Assert.IsNull(mainWindow.ProjectExplorer.Data);

                gui.SetProject(project2, string.Empty);

                // When
                mainWindow.UpdateProjectExplorer();

                // Then
                Assert.IsNull(mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndSingleStateInfo_WhenUpdateProjectExplorer_ThenExpectedDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project1 = mocks.Stub<IProject>();
            var project2 = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project1);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin(new[]
                {
                    new StateInfo("Name", "Symbol", new FontFamily(), p => p)
                }));

                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();
                gui.SetProject(project1, null);

                // Precondition
                Assert.IsNotNull(mainWindow.ProjectExplorer);
                Assert.AreSame(project1, mainWindow.ProjectExplorer.Data);

                gui.SetProject(project2, string.Empty);

                // When
                mainWindow.UpdateProjectExplorer();

                // Then
                Assert.AreSame(project2, mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithProjectExplorerAndMultipleStateInfos_WhenUpdateProjectExplorer_ThenExpectedDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project1 = mocks.Stub<IProject>();
            var project2 = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project1);
            mocks.ReplayAll();

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
                mainWindow.InitializeToolWindows();
                gui.SetProject(project1, null);

                // Precondition
                Assert.IsNotNull(mainWindow.ProjectExplorer);
                Assert.AreSame(project1, mainWindow.ProjectExplorer.Data);

                gui.SetProject(project2, string.Empty);

                // When
                mainWindow.UpdateProjectExplorer();

                // Then
                Assert.AreSame(project2, mainWindow.ProjectExplorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewOpened_ThenMapZoomedToExtents()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var testMapView = new TestMapView();
                var map = (DotSpatialMap) ((MapControl) testMapView.Map).Controls[0].Controls[1];

                Extent initialExtents = map.ViewExtents;

                // When
                gui.ViewHost.AddDocumentView(testMapView);

                // Then
                Assert.AreNotEqual(initialExtents, map.ViewExtents);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewAdded_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

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
                gui.ViewHost.AddDocumentView(view);

                // Then
                Assert.AreSame(view.Map, GetMapControl(mapLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewBroughtToFront_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestMapView();
                var view2 = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                gui.ViewHost.AddDocumentView(view1);
                gui.ViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));

                // When
                gui.ViewHost.BringToFront(view1);

                // Then
                Assert.AreSame(view1.Map, GetMapControl(mapLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenMapViewRemoved_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                gui.ViewHost.AddDocumentView(view);

                // Precondition
                Assert.AreSame(view.Map, GetMapControl(mapLegendView));

                // When
                gui.ViewHost.Remove(view);

                // Then
                Assert.IsNull(GetMapControl(mapLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenOtherMapViewRemoved_ThenComponentsNotUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestMapView();
                var view2 = new TestMapView();
                MapLegendView mapLegendView = mainWindow.MapLegendView;

                gui.ViewHost.AddDocumentView(view1);
                gui.ViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));

                // When
                gui.ViewHost.Remove(view1);

                // Then
                Assert.AreSame(view2.Map, GetMapControl(mapLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenChartViewAdded_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

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
                gui.ViewHost.AddDocumentView(view);

                // Then
                Assert.AreSame(view.Chart, GetChartControl(chartLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenChartViewBroughtToFront_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestChartView();
                var view2 = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                gui.ViewHost.AddDocumentView(view1);
                gui.ViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));

                // When
                gui.ViewHost.BringToFront(view1);

                // Then
                Assert.AreSame(view1.Chart, GetChartControl(chartLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenChartViewRemoved_ThenComponentsUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                gui.ViewHost.AddDocumentView(view);

                // Precondition
                Assert.AreSame(view.Chart, GetChartControl(chartLegendView));

                // When
                gui.ViewHost.Remove(view);

                // Then
                Assert.IsNull(GetChartControl(chartLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenOtherChartViewRemoved_ThenComponentsNotUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var view1 = new TestChartView();
                var view2 = new TestChartView();
                ChartLegendView chartLegendView = mainWindow.ChartLegendView;

                gui.ViewHost.AddDocumentView(view1);
                gui.ViewHost.AddDocumentView(view2);

                // Precondition
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));

                // When
                gui.ViewHost.Remove(view1);

                // Then
                Assert.AreSame(view2.Chart, GetChartControl(chartLegendView));
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenNewProjectIsCalled_ThenCreateNewProject(bool backstageVisible)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.StrictMock<IProjectFactory>();
            projectFactory.Expect(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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
                var mocks = new MockRepository();
                var project = mocks.Stub<IProject>();

                var projectStore = mocks.StrictMock<IStoreProject>();
                projectStore.Expect(ps => ps.SaveProjectAs(someValidFilePath));
                projectStore.Expect(ps => ps.SaveProjectFileFilter).Return(string.Empty);
                projectStore.Expect(ps => ps.HasStagedProject).Return(false);
                projectStore.Expect(ps => ps.StageProject(project));

                var projectMigrator = mocks.Stub<IMigrateProject>();
                var projectFactory = mocks.Stub<IProjectFactory>();
                projectFactory.Stub(pf => pf.CreateNewProject(null))
                              .IgnoreArguments()
                              .Return(project);
                mocks.ReplayAll();

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
                    ApplicationIcon = CoreGuiTestUtilResources.TestIcon
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

                mocks.VerifyAll();
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
                var mocks = new MockRepository();
                var project = mocks.Stub<IProject>();

                var projectStore = mocks.StrictMock<IStoreProject>();
                projectStore.Expect(ps => ps.SaveProjectAs(someValidFilePath));
                projectStore.Expect(ps => ps.SaveProjectFileFilter).Return(string.Empty);
                projectStore.Expect(ps => ps.HasStagedProject).Return(false);
                projectStore.Expect(ps => ps.StageProject(project));

                var projectMigrator = mocks.Stub<IMigrateProject>();
                var projectFactory = mocks.Stub<IProjectFactory>();
                projectFactory.Stub(pf => pf.CreateNewProject(null))
                              .IgnoreArguments()
                              .Return(project);
                mocks.ReplayAll();

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
                    ApplicationIcon = CoreGuiTestUtilResources.TestIcon
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

                mocks.VerifyAll();
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

            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();

            var projectStore = mocks.StrictMock<IStoreProject>();
            projectStore.Expect(ps => ps.LoadProject(filePath));
            projectStore.Expect(ps => ps.OpenProjectFileFilter).Return(string.Empty);

            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

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
                ApplicationIcon = CoreGuiTestUtilResources.TestIcon
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

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMainWindowWithoutViewTabOpen_WhenCanExecuteCloseViewTabCommand_ThenFalse()
        {
            // Given
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMainWindowWithViewTabOpen_WhenCanExecuteCloseViewTabCommand_ThenTrue()
        {
            // Given
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

            using (var mainWindow = new MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Run();

                mainWindow.SetGui(gui);

                gui.ViewHost.AddDocumentView(new TestView());

                // When
                bool canExecute = mainWindow.CloseViewTabCommand.CanExecute(null);

                // Then
                Assert.IsTrue(canExecute);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMainWindow_WhenExecuteToggleBackstageCommand_ThenBackstageToggled(bool backstageVisible)
        {
            // Given
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(project);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        private static void ToggleToolViewAndAssert(Func<MainWindow, IView> getToolViewFunc,
                                                    Func<MainWindow, ICommand> getCommandFunc,
                                                    bool initiallyAdded)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject(null))
                          .IgnoreArguments()
                          .Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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
    }
}