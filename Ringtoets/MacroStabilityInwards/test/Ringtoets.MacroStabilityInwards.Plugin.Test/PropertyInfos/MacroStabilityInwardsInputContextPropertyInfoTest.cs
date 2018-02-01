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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class MacroStabilityInwardsInputContextPropertyInfoTest
    {
        private MacroStabilityInwardsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(MacroStabilityInwardsInputContextProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
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

            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = normType
                }
            };

            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = testHydraulicBoundaryLocation
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsInputContext(
                scenario.InputParameters,
                scenario,
                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                failureMechanism, assessmentSection);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsInputContextProperties>(objectProperties);
            Assert.AreSame(context, objectProperties.Data);

            double expectedAssessmentLevel = normType == NormType.Signaling
                                                 ? designWaterLevelSignaling
                                                 : designWaterLevelLowerLimit;
            Assert.AreEqual(expectedAssessmentLevel, ((MacroStabilityInwardsInputContextProperties) objectProperties).AssessmentLevel);
        }
    }
}