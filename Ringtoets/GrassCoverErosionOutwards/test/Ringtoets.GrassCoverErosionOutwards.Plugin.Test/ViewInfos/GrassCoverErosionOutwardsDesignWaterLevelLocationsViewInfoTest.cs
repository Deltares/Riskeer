﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewInfoTest
    {
        [Test]
        public void Initialized_Always_DataTypeAndViewTypeAsExpected()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                ViewInfo info = GetInfo(plugin);

                // Assert
                Assert.NotNull(info, "Expected a viewInfo definition for views with type {0}.", typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsView));
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext), info.DataType);
                Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocation>), info.ViewDataType);
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsView), info.ViewType);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnWrappedDataInContext()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var expectedLocations = new ObservableList<HydraulicBoundaryLocation>();

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                object locations = info.GetViewData(new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                                                        expectedLocations,
                                                        assessmentSection,
                                                        new GrassCoverErosionOutwardsFailureMechanism()));

                // Assert
                Assert.AreSame(locations, expectedLocations);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedProperties()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            var window = mockRepository.Stub<IMainWindow>();
            gui.Stub(gs => gs.MainWindow).Return(window);
            mockRepository.ReplayAll();
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

                var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                    new ObservableList<HydraulicBoundaryLocation>(),
                    assessmentSection,
                    grassCoverErosionOutwardsFailureMechanism);

                plugin.Gui = gui;
                plugin.Activate();

                // Call
                var view = info.CreateInstance(context) as GrassCoverErosionOutwardsDesignWaterLevelLocationsView;

                // Assert
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                string name = info.GetViewName(null, null);

                // Assert
                Assert.AreEqual(Resources.GrassCoverErosionOutwardsWaterLevelLocations_DisplayName, name);
            }
        }

        [Test]
        public void AfterCreate_Always_SetsExpectedProperties()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var gui = mockRepository.Stub<IGui>();
            var window = mockRepository.Stub<IMainWindow>();
            gui.Stub(gs => gs.MainWindow).Return(window);
            mockRepository.ReplayAll();
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

                var data = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                    new ObservableList<HydraulicBoundaryLocation>(),
                    assessmentSection,
                    grassCoverErosionOutwardsFailureMechanism);

                plugin.Gui = gui;
                plugin.Activate();

                using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(grassCoverErosionOutwardsFailureMechanism,
                                                                                             hbl => new HydraulicBoundaryLocationCalculation(),
                                                                                             assessmentSection,
                                                                                             () => 0.01))
                {
                    // Call
                    info.AfterCreate(view, data);

                    // Assert
                    Assert.AreSame(grassCoverErosionOutwardsFailureMechanism, view.FailureMechanism);
                    Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(view.CalculationGuiService);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingAssessmentSection_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(failureMechanism,
                                                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                                                         assessmentSection,
                                                                                         () => 0.01))
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForNonMatchingAssessmentSection_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            var assessmentSectionB = mocks.Stub<IAssessmentSection>();
            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            assessmentSectionA.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSectionA.Stub(a => a.Detach(null)).IgnoreArguments();
            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(failureMechanism,
                                                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                                                         assessmentSectionA,
                                                                                         () => 0.01))
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionB);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            var grassCoverErosionOutwardsFailureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                new GrassCoverErosionOutwardsFailureMechanism(),
                assessmentSection);

            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                                                         assessmentSection,
                                                                                         () => 0.01))
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, grassCoverErosionOutwardsFailureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForNonMatchingFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            var assessmentSectionB = mocks.Stub<IAssessmentSection>();
            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            assessmentSectionA.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSectionA.Stub(a => a.Detach(null)).IgnoreArguments();
            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            mocks.ReplayAll();

            var grassCoverErosionOutwardsFailureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                new GrassCoverErosionOutwardsFailureMechanism(),
                assessmentSectionB);

            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                                                         assessmentSectionA,
                                                                                         () => 0.01))
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, grassCoverErosionOutwardsFailureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForOtherObjectType_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                                                         assessmentSection,
                                                                                         () => 0.01))
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        private static ViewInfo GetInfo(PluginBase plugin)
        {
            return plugin.GetViewInfos().FirstOrDefault(vi => vi.ViewType == typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsView));
        }
    }
}