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

using System.Collections.Generic;
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
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsViewInfoTest
    {
        [Test]
        public void Initialized_Always_DataTypeAndViewTypeAsExpected()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                var info = GetInfo(plugin);

                // Assert
                Assert.NotNull(info, "Expected a viewInfo definition for views with type {0}.", typeof(GrassCoverErosionOutwardsWaveHeightLocationsView));
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext), info.DataType);
                Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocation>), info.ViewDataType);
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveHeightLocationsView), info.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, info.Image);
                Assert.AreEqual(Resources.GrassCoverErosionOutwardsWaveHeightLocationsContext_DisplayName, info.GetViewName(null, null));
            }
        }

        [Test]
        public void GetViewData_Always_ReturnWrappedDataInContext()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var expectedLocations = new ObservableList<HydraulicBoundaryLocation>();

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var locations = info.GetViewData(new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                                                     expectedLocations,
                                                     assessmentSectionStub,
                                                     new GrassCoverErosionOutwardsFailureMechanism()));

                // Assert
                Assert.AreSame(locations, expectedLocations);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsExpectedProperties()
        {
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            IGui guiStub = mockRepository.Stub<IGui>();
            IMainWindow windowsStub = mockRepository.Stub<IMainWindow>();
            guiStub.Stub(gs => gs.MainWindow).Return(windowsStub);
            mockRepository.ReplayAll();
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var data = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                    new ObservableList<HydraulicBoundaryLocation>(),
                    assessmentSectionStub,
                    failureMechanism);
                plugin.Gui = guiStub;
                plugin.Activate();

                using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
                {
                    info.AfterCreate(view, data);

                    // Assert
                    Assert.AreSame(assessmentSectionStub, view.AssessmentSection);
                    Assert.AreSame(failureMechanism, view.FailureMechanism);
                    Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(view.CalculationGuiService);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);
                view.AssessmentSection = assessmentSection;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForNonMatchingAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            var assessmentSectionB = mocks.Stub<IAssessmentSection>();

            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });

            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);
                view.AssessmentSection = assessmentSectionA;

                // Call
                var closeForData = info.CloseForData(view, assessmentSectionB);

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
            mocks.ReplayAll();

            var grassCoverErosionOutwardsFailureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                new GrassCoverErosionOutwardsFailureMechanism(),
                assessmentSection);

            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);
                view.AssessmentSection = assessmentSection;

                // Call
                var closeForData = info.CloseForData(view, grassCoverErosionOutwardsFailureMechanismContext);

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
            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            mocks.ReplayAll();

            var grassCoverErosionOutwardsFailureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(
                new GrassCoverErosionOutwardsFailureMechanism(),
                assessmentSectionB);

            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);
                view.AssessmentSection = assessmentSectionA;

                // Call
                var closeForData = info.CloseForData(view, grassCoverErosionOutwardsFailureMechanismContext);

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
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);
                view.AssessmentSection = assessmentSectionA;

                // Call
                var closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ViewDataNull_ReturnsFalse()
        {
            // Setup
            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        private ViewInfo GetInfo(PluginBase plugin)
        {
            return plugin.GetViewInfos().FirstOrDefault(vi => vi.ViewType == typeof(GrassCoverErosionOutwardsWaveHeightLocationsView));
        }
    }
}