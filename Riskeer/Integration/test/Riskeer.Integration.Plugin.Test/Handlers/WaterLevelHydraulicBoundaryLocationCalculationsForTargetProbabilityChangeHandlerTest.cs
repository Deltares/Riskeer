// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var handler = new WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), assessmentSection);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationsForTargetProbabilityWithAndWithoutOutput_AllCalculationOutputClearedAndReturnsAllAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                                          .First();

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            waveConditionsCalculations.ForEachElementDo(c =>
            {
                c.InputParameters.WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability;
                c.InputParameters.CalculationsTargetProbability = calculationsForTargetProbability;
            });

            var expectedAffectedObjects = new List<IObservable>
            {
                calculationsForTargetProbability
            };
            expectedAffectedObjects.AddRange(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.Where(c => c.HasOutput));
            expectedAffectedObjects.AddRange(waveConditionsCalculations.Where(c => c.HasOutput));

            var handler = new WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(calculationsForTargetProbability, assessmentSection);

            IEnumerable<IObservable> affectedObjects = null;

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            var expectedMessages = new[]
            {
                "Alle bijbehorende hydraulische belastingen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 1);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
            CollectionAssert.IsEmpty(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.Where(c => c.HasOutput));
            CollectionAssert.IsEmpty(waveConditionsCalculations.Where(c => c.HasOutput));
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationsForTargetProbabilityWithoutOutput_ReturnsOnlyCalculationsForTargetProbability()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                                          .First();

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            waveConditionsCalculations.ForEachElementDo(c =>
            {
                c.InputParameters.WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability;
                c.InputParameters.CalculationsTargetProbability = calculationsForTargetProbability;
                c.ClearOutput();
            });

            calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.ForEachElementDo(c => c.Output = null);

            var handler = new WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(
                calculationsForTargetProbability, assessmentSection);

            HydraulicBoundaryLocationCalculationsForTargetProbability[] expectedAffectedObjects =
            {
                calculationsForTargetProbability
            };

            IEnumerable<IObservable> affectedObjects = null;

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            TestHelper.AssertLogMessagesCount(Call, 0);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }
    }
}