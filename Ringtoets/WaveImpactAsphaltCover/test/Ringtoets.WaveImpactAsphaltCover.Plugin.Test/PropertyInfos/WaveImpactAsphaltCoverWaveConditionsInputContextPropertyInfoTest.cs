// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

        [Test]
        public void CreateInstance_WithContextAndNormTypeSignaling_ExpectedProperties()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.Signaling
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                               calculation,
                                                                               assessmentSection,
                                                                               new ForeshoreProfile[0]);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<WaveImpactAsphaltCoverWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                double expectedAssessmentLevel = assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(0).Output.Result;
                Assert.AreEqual(expectedAssessmentLevel, ((WaveImpactAsphaltCoverWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        [Test]
        public void CreateInstance_WithContextAndNormTypeLowerLimit_ExpectedProperties()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                               calculation,
                                                                               assessmentSection,
                                                                               new ForeshoreProfile[0]);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<WaveImpactAsphaltCoverWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                double expectedAssessmentLevel = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result;
                Assert.AreEqual(expectedAssessmentLevel, ((WaveImpactAsphaltCoverWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(WaveImpactAsphaltCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsInputContext));
        }
    }
}