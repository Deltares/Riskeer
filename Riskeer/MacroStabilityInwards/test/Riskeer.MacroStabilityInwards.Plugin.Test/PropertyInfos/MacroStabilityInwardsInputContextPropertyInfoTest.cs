﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Linq;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.PropertyInfos
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

        [Test]
        public void CreateInstance_WithContextAndNormativeProbabilityTypeSignalFloodingProbability_ExpectedProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = NormativeProbabilityType.SignalFloodingProbability
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsInputContext(
                scenario.InputParameters,
                scenario,
                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                failureMechanism,
                assessmentSection);

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsInputContextProperties>(objectProperties);
            Assert.AreSame(context, objectProperties.Data);

            double expectedAssessmentLevel = assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.ElementAt(0).Output.Result;
            Assert.AreEqual(expectedAssessmentLevel, ((MacroStabilityInwardsInputContextProperties) objectProperties).AssessmentLevel);
        }

        [Test]
        public void CreateInstance_WithContextAndNormativeProbabilityTypeMaximumAllowableFloodingProbability_ExpectedProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = NormativeProbabilityType.MaximumAllowableFloodingProbability
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsInputContext(
                scenario.InputParameters,
                scenario,
                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                failureMechanism,
                assessmentSection);

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsInputContextProperties>(objectProperties);
            Assert.AreSame(context, objectProperties.Data);

            double expectedAssessmentLevel = assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.ElementAt(0).Output.Result;
            Assert.AreEqual(expectedAssessmentLevel, ((MacroStabilityInwardsInputContextProperties) objectProperties).AssessmentLevel);
        }
    }
}