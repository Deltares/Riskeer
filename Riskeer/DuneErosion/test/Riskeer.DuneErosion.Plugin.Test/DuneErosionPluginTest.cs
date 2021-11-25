// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.Settings;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.Forms.PropertyClasses;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Plugin.Test
{
    [TestFixture]
    public class DuneErosionPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new DuneErosionPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void Activate_GuiNull_ThrowInvalidOperationException()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                void Call() => plugin.Activate();

                // Assert
                var exception = Assert.Throws<InvalidOperationException>(Call);
                Assert.AreEqual("Gui cannot be null", exception.Message);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(4, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneErosionHydraulicLoadsContext),
                    typeof(DuneErosionHydraulicLoadsProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneErosionFailurePathContext),
                    typeof(DuneErosionFailurePathProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext),
                    typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneLocationCalculation),
                    typeof(DuneLocationCalculationProperties));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(5, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneErosionHydraulicLoadsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneErosionFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            const string symbol = "<symbol>";
            var fontFamily = new FontFamily();

            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            gui.Stub(g => g.ViewHost).Return(mockRepository.Stub<IViewHost>());
            gui.Stub(g => g.ActiveStateInfo).Return(new StateInfo(string.Empty, symbol, fontFamily, p => p));
            mockRepository.ReplayAll();

            using (var plugin = new DuneErosionPlugin
            {
                Gui = gui
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(4, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<DuneErosionFailureMechanismSectionResult>),
                    typeof(DuneErosionFailureMechanismResultViewOld));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DuneErosionHydraulicLoadsContext),
                    typeof(DuneErosionFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DuneErosionFailurePathContext),
                    typeof(DuneErosionFailurePathView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext),
                    typeof(IObservableEnumerable<DuneLocationCalculation>),
                    typeof(DuneLocationCalculationsView));

                viewInfos.ForEachElementDo(vi =>
                {
                    Assert.AreEqual(symbol, vi.GetSymbol());
                    Assert.AreSame(fontFamily, vi.GetFontFamily());
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext)));
            }
        }

        [Test]
        public void GetUpdateInfos_ReturnsSupportedUpdateInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(1, updateInfos.Length);
                Assert.IsTrue(updateInfos.Any(ei => ei.DataType == typeof(DuneErosionFailureMechanismSectionsContext)));
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDuneLocationCalculationsView_WhenChangingCorrespondingUserDefinedTargetProbabilityAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var project = mocks.Stub<IProject>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.AddRange(new PluginBase[]
                {
                    new DuneErosionPlugin()
                });

                gui.Run();
                gui.SetProject(project, null);

                var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocationCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculationsForTargetProbability
                    }
                };

                gui.DocumentViewController.OpenViewForData(new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                                                              failureMechanism,
                                                                                                                              assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DuneLocationCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Hydraulische belastingen - 1/10"));

                // When
                calculationsForTargetProbability.TargetProbability = 0.01;
                calculationsForTargetProbability.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Hydraulische belastingen - 1/100"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDuneLocationCalculationsView_WhenItemInUserDefinedTargetProbabilityCollectionUpdatedAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var project = mocks.Stub<IProject>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.AddRange(new PluginBase[]
                {
                    new DuneErosionPlugin()
                });

                gui.Run();
                gui.SetProject(project, null);

                const double targetProbability = 0.1;
                var updatedCalculations = new DuneLocationCalculationsForTargetProbability(targetProbability);
                var affectedCalculations = new DuneLocationCalculationsForTargetProbability(targetProbability);
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocationCalculationsForUserDefinedTargetProbabilities =
                    {
                        updatedCalculations,
                        affectedCalculations
                    }
                };

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(affectedCalculations,
                                                                                                                              failureMechanism,
                                                                                                                              assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DuneLocationCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Hydraulische belastingen - 1/10 (1)"));

                // When
                updatedCalculations.TargetProbability = 0.01;
                updatedCalculations.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Hydraulische belastingen - 1/10"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDuneLocationCalculationsView_WhenUserDefinedTargetProbabilityRemovedFromCollectionAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var project = mocks.Stub<IProject>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.AddRange(new PluginBase[]
                {
                    new DuneErosionPlugin()
                });

                gui.Run();
                gui.SetProject(project, null);

                const double targetProbability = 0.1;
                var removedCalculations = new DuneLocationCalculationsForTargetProbability(targetProbability);
                var affectedCalculations = new DuneLocationCalculationsForTargetProbability(targetProbability);
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocationCalculationsForUserDefinedTargetProbabilities =
                    {
                        removedCalculations,
                        affectedCalculations
                    }
                };

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(affectedCalculations,
                                                                                                                              failureMechanism,
                                                                                                                              assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DuneLocationCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Hydraulische belastingen - 1/10 (1)"));

                // When
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Remove(removedCalculations);
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Hydraulische belastingen - 1/10"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDuneLocationCalculationsView_WhenRemovingDataForOpenedViewAndObserversNotified_ThenNoExceptionThrown()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            var projectFactory = mocks.Stub<IProjectFactory>();
            var project = mocks.Stub<IProject>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, projectFactory, new GuiCoreSettings()))
            {
                gui.Plugins.AddRange(new PluginBase[]
                {
                    new DuneErosionPlugin()
                });
                gui.Run();
                gui.SetProject(project, null);

                var calculations = new DuneLocationCalculationsForTargetProbability(0.1);
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocationCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculations
                    }
                };

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculations,
                                                                                                                              failureMechanism,
                                                                                                                              assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DuneLocationCalculationsView>(view);

                // When
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Remove(calculations);
                void Call() => failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                Assert.DoesNotThrow(Call);
                mocks.VerifyAll();
            }
        }
    }
}