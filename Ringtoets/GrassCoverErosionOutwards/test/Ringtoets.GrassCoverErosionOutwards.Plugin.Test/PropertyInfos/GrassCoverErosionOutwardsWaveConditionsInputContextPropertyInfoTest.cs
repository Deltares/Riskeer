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

using System.Collections;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
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
        [TestCaseSource(nameof(DifferentCategoryTypes))]
        public void CreateInstance_WithContextThatHasInputWithSpecificCategoryType_ExpectedProperties(
            IAssessmentSection assessmentSection,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            FailureMechanismCategoryType categoryType,
            RoundedDouble expectedAssessmentLevel)
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = categoryType
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

                Assert.AreEqual(expectedAssessmentLevel, ((GrassCoverErosionOutwardsWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsInputContext));
        }

        private static IEnumerable DifferentCategoryTypes()
        {
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection,
                new[]
                {
                    hydraulicBoundaryLocation
                }, true);

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificFactorizedSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificSignalingNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificLowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.LowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("LowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("FactorizedLowerLimitNorm");
        }
    }
}