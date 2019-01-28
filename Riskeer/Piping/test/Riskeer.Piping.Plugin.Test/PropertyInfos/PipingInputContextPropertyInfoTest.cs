// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class PipingInputContextPropertyInfoTest
    {
        private PipingPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(PipingInputContextProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingInputContext), info.DataType);
            Assert.AreEqual(typeof(PipingInputContextProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_WithContextAndNormTypeSignaling_ExpectedProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.Signaling
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingInputContext(
                scenario.InputParameters,
                scenario,
                Enumerable.Empty<PipingSurfaceLine>(),
                Enumerable.Empty<PipingStochasticSoilModel>(),
                failureMechanism,
                assessmentSection);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<PipingInputContextProperties>(objectProperties);
            Assert.AreSame(context, objectProperties.Data);

            double expectedAssessmentLevel = assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(0).Output.Result;
            Assert.AreEqual(expectedAssessmentLevel, ((PipingInputContextProperties) objectProperties).AssessmentLevel);
        }

        [Test]
        public void CreateInstance_WithContextAndNormTypeLowerLimit_ExpectedProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingInputContext(
                scenario.InputParameters,
                scenario,
                Enumerable.Empty<PipingSurfaceLine>(),
                Enumerable.Empty<PipingStochasticSoilModel>(),
                failureMechanism,
                assessmentSection);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<PipingInputContextProperties>(objectProperties);
            Assert.AreSame(context, objectProperties.Data);

            double expectedAssessmentLevel = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result;
            Assert.AreEqual(expectedAssessmentLevel, ((PipingInputContextProperties) objectProperties).AssessmentLevel);
        }
    }
}