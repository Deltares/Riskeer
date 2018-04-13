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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsInputContext), info.DataType);
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsInputContextProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_ExpectedProperties()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.Signaling
                }
            };

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism, assessmentSection, new[]
                {
                    hydraulicBoundaryLocation
                }, true);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(calculation.InputParameters,
                                                                                  calculation,
                                                                                  assessmentSection,
                                                                                  failureMechanism);

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                double expectedAssessmentLevel = failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ElementAt(0).Output.Result;
                Assert.AreEqual(expectedAssessmentLevel, ((GrassCoverErosionOutwardsWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsInputContext));
        }
    }
}