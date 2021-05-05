﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using Core.Components.Chart.Forms;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Forms;
using Core.Gui.Commands;
using Core.Gui.Forms.Chart;
using Core.Gui.Forms.MainWindow;
using Core.Gui.Forms.Map;
using Core.Gui.Forms.MessageWindow;
using Core.Gui.Forms.PropertyGridView;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using Core.Gui.Settings;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.Map;
using DotSpatial.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Forms.MainWindow
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTest
    {
        private MessageWindowLogAppender originalValue;

        [SetUp]
        public void SetUp()
        {
            originalValue = MessageWindowLogAppender.Instance;
            MessageWindowLogAppender.Instance = new MessageWindowLogAppender();
        }

        [TearDown]
        public void TearDown()
        {
            MessageWindowLogAppender.Instance = originalValue;
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
                Assert.AreEqual("RiskeerMainWindow", mainWindow.Name);
            }
        }

        [Test]
        public void Dispose_SetIsWindowDisposedTrue()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
        public void InitPropertiesWindowOrBringToFront_GuiNotSet_ThrowInvalidOperationException()
        {
            // Setup
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                void Call() => mainWindow.InitPropertiesWindowOrBringToFront();

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }
        }

        [Test]
        public void InitPropertiesWindowOrBringToFront_GuiSet_InitializePropertyGrid()
        {
            // Setup
            var selectedObject = new object();

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();

            var viewHost = new AvalonDockViewHost();

            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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

            var mocks = new MockRepository();
            var selectedObjectProperties = mocks.Stub<IObjectProperties>();
            var viewHost = new AvalonDockViewHost();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(selectedObject))
                            .Return(selectedObjectProperties)
                            .Repeat.Once();

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(viewHost);
            gui.Selection = selectedObject;
            gui.Stub(g => g.PropertyResolver).Return(propertyResolver);
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
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
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                // Call
                void Call() => mainWindow.InitializeToolWindows();

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }
        }

        [Test]
        public void InitializeToolWindows_GuiSet_InitializeToolWindows()
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
            mocks.ReplayAll();
            
            gui.Selection = selectedObject;

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            {
                mainWindow.SetGui(gui);

                // Call
                mainWindow.InitializeToolWindows();

                // Assert
                Assert.IsInstanceOf<Gui.Forms.ProjectExplorer.ProjectExplorer>(mainWindow.ProjectExplorer);
                Assert.AreSame(gui.Project, mainWindow.ProjectExplorer.Data);
                
                Assert.IsInstanceOf<Gui.Forms.PropertyGridView.PropertyGridView>(mainWindow.PropertyGrid);
                Assert.AreEqual("Eigenschappen", mainWindow.PropertyGrid.Text);
                Assert.AreEqual(selectedObject, mainWindow.PropertyGrid.Data);

                Assert.IsInstanceOf<Gui.Forms.MessageWindow.MessageWindow>(mainWindow.MessageWindow);
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
        public void GivenGuiWithProjectExplorer_WhenClosingProjectExplorer_ThenProjectExplorerSetToNull()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();
            
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();
            
                // Precondition
                Assert.IsNotNull(mainWindow.ProjectExplorer);

                // When
                mainWindow.ViewHost.Remove(mainWindow.ProjectExplorer);

                // Then
                Assert.IsNull(mainWindow.ProjectExplorer);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithPropertyGrid_WhenClosingPropertyGrid_ThenPropertyGridSetToNull()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                // Precondition
                Assert.IsNotNull(mainWindow.PropertyGrid);

                // When
                mainWindow.ViewHost.Remove(mainWindow.PropertyGrid);

                // Then
                Assert.IsNull(mainWindow.PropertyGrid);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMessageWindow_WhenClosingMessageWindow_ThenMessageWindowSetToNull()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                // Precondition
                Assert.IsNotNull(mainWindow.MessageWindow);

                // When
                mainWindow.ViewHost.Remove(mainWindow.MessageWindow);

                // Then
                Assert.IsNull(mainWindow.MessageWindow);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithMapLegendView_WhenClosingMapLegendView_ThenMapLegendViewSetToNull()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                // Precondition
                Assert.IsNotNull(mainWindow.MapLegendView);

                // When
                mainWindow.ViewHost.Remove(mainWindow.MapLegendView);

                // Then
                Assert.IsNull(mainWindow.MapLegendView);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithChartLegendView_WhenClosingChartLegendView_ThenChartLegendViewSetToNull()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();

                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                // Precondition
                Assert.IsNotNull(mainWindow.ChartLegendView);

                // When
                mainWindow.ViewHost.Remove(mainWindow.ChartLegendView);

                // Then
                Assert.IsNull(mainWindow.ChartLegendView);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenGuiWithProjectExplorer_WhenUpdateProjectExplorer_ThenDataSetOnProjectExplorer()
        {
            // Given
            var mocks = new MockRepository();
            var project1 = mocks.Stub<IProject>();
            var project2 = mocks.Stub<IProject>();

            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(project1);
            mocks.ReplayAll();
            
            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();
                
                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

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
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
                gui.Run();
        
                mainWindow.SetGui(gui);
                mainWindow.InitializeToolWindows();

                var testMapView = new TestMapView();
                var map = (DotSpatialMap) ((MapControl) testMapView.Map).Controls[0].Controls[1];

                // Precondition
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
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
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
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
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
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
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
            projectFactory.Stub(pf => pf.CreateNewProject()).Return(mocks.Stub<IProject>());
            mocks.ReplayAll();

            using (var mainWindow = new Gui.Forms.MainWindow.MainWindow())
            using (var gui = new GuiCore(mainWindow, projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.Add(new TestPlugin());
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