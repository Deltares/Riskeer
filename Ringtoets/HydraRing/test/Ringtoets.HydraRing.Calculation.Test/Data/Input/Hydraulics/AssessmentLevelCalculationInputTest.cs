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

using System.Linq;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class AssessmentLevelCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int returnPeriod = 10000;
            const int sectionId = 1;
            const long hydraulicBoundaryLocationId = 1234;

            // Call
            var assessmentLevelCalculationInput = new AssessmentLevelCalculationInput(sectionId, hydraulicBoundaryLocationId, returnPeriod);

            // Assert
            double expectedBeta = StatisticsConverter.ReturnPeriodToReliability(returnPeriod);
            Assert.IsInstanceOf<ReliabilityIndexCalculationInput>(assessmentLevelCalculationInput);
            Assert.AreEqual(HydraRingFailureMechanismType.AssessmentLevel, assessmentLevelCalculationInput.FailureMechanismType);
            Assert.AreEqual(9, assessmentLevelCalculationInput.CalculationTypeId);
            Assert.AreEqual(26, assessmentLevelCalculationInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, assessmentLevelCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(assessmentLevelCalculationInput.Section);
            Assert.AreEqual(1, assessmentLevelCalculationInput.Variables.Count());
            CollectionAssert.IsEmpty(assessmentLevelCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(assessmentLevelCalculationInput.ForelandsPoints);
            Assert.IsNull(assessmentLevelCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, assessmentLevelCalculationInput.Beta);

            var section = assessmentLevelCalculationInput.Section;
            Assert.AreEqual(sectionId, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.IsNaN(section.CrossSectionNormal);

            var waterLevelVariable = assessmentLevelCalculationInput.Variables.First();
            Assert.AreEqual(26, waterLevelVariable.VariableId);
            Assert.AreEqual(HydraRingDistributionType.Deterministic, waterLevelVariable.DistributionType);
            Assert.AreEqual(0.0, waterLevelVariable.Value);
            Assert.AreEqual(HydraRingDeviationType.Standard, waterLevelVariable.DeviationType);
            Assert.IsNaN(waterLevelVariable.Mean);
            Assert.IsNaN(waterLevelVariable.Variability);
            Assert.IsNaN(waterLevelVariable.Shift);
        }

        [Test]
        public void GetSubMechanismModelId_ReturnsExpectedValues()
        {
            // Call
            var assessmentLevelCalculationInput = new AssessmentLevelCalculationInput(1, 1, 2.2);

            // Assert
            Assert.IsNull(assessmentLevelCalculationInput.GetSubMechanismModelId(1));
        }
    }
}