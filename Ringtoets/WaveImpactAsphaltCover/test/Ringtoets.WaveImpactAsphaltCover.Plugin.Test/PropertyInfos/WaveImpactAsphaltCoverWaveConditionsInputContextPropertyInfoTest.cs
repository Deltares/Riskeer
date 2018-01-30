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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsInputContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(WaveImpactAsphaltCoverWaveConditionsInputContext), info.DataType);
                Assert.AreEqual(typeof(WaveImpactAsphaltCoverWaveConditionsInputContextProperties), info.PropertyObjectType);
            }
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void CreateInstance_Always_ExpectedProperties(NormType normType)
        {
            // Setup
            const double designWaterLevelSignaling = 1.1;
            const double designWaterLevelLowerLimit = 2.2;

            var testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation2 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(designWaterLevelSignaling)
                },
                DesignWaterLevelCalculation3 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(designWaterLevelLowerLimit)
                }
            };
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = testHydraulicBoundaryLocation
                }
            };
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = normType
                }
            };

            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                               calculation,
                                                                               assessmentSection,
                                                                               new ForeshoreProfile[0]);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<WaveImpactAsphaltCoverWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                double expectedAssessmentLevel = normType == NormType.Signaling
                                                     ? designWaterLevelSignaling
                                                     : designWaterLevelLowerLimit;
                Assert.AreEqual(expectedAssessmentLevel, ((WaveImpactAsphaltCoverWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(WaveImpactAsphaltCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsInputContext));
        }
    }
}