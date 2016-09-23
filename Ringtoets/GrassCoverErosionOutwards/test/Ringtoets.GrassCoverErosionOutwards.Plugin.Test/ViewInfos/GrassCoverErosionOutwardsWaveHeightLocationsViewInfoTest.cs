﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsViewInfoTest 
    {
        [TestCase]
        public void Initialized_Always_DataTypeAndViewTypeAsExpected()
        {
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                var info = GetInfo(plugin);

                Assert.NotNull(info, "Expected a viewInfo definition for views with type {0}.", typeof(GrassCoverErosionOutwardsWaveHeightLocationsView));
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext), info.DataType);
                Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocation>), info.ViewDataType);
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveHeightLocationsView), info.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, info.Image);
                Assert.AreEqual(Resources.GrassCoverErosionOutwardsHydraulicBoundaryLocation_WaveHeight_DisplayName, info.GetViewName(null, null));
            }
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

                var data = new GrassCoverErosionOutwardsWaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>(), assessmentSectionStub);
                plugin.Gui = guiStub;
                plugin.Activate();

                using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
                {
                    info.AfterCreate(view, data);

                    // Assert
                    Assert.AreSame(assessmentSectionStub, view.AssessmentSection);
                    Assert.AreSame(guiStub, view.ApplicationSelection);
                    Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(view.CalculationGuiService);
                }
            }
            mockRepository.VerifyAll();
        }

        private ViewInfo GetInfo(PluginBase plugin)
        {
            return plugin.GetViewInfos().FirstOrDefault(vi => vi.ViewType == typeof(GrassCoverErosionOutwardsWaveHeightLocationsView));
        }
    }
}
