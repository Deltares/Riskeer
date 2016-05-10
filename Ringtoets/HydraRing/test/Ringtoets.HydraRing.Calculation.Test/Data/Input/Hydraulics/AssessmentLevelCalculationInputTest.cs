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
using MathNet.Numerics.Distributions;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class AssessmentLevelCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var norm = 10000;
            var expectedBeta = -Normal.InvCDF(0.0, 1.0, 1.0/norm);
            var assessmentLevelCalculationInput = new AssessmentLevelCalculationInput(1, norm);

            // Assert
            Assert.AreEqual(HydraRingFailureMechanismType.AssessmentLevel, assessmentLevelCalculationInput.FailureMechanismType);
            Assert.AreEqual(2, assessmentLevelCalculationInput.CalculationTypeId);
            Assert.AreEqual(26, assessmentLevelCalculationInput.VariableId);
            Assert.AreEqual(1, assessmentLevelCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(assessmentLevelCalculationInput.Section);
            Assert.AreEqual(1, assessmentLevelCalculationInput.Variables.Count());
            CollectionAssert.IsEmpty(assessmentLevelCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(assessmentLevelCalculationInput.ForelandsPoints);
            Assert.IsNull(assessmentLevelCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, assessmentLevelCalculationInput.Beta);

            var hydraRingSection = assessmentLevelCalculationInput.Section;
            Assert.AreEqual(1, hydraRingSection.SectionId);
            Assert.AreEqual("1", hydraRingSection.SectionName);
            Assert.IsNaN(hydraRingSection.SectionLength);
            Assert.IsNaN(hydraRingSection.CrossSectionNormal);

            var assessmentLevelVariable = assessmentLevelCalculationInput.Variables.First();
            Assert.AreEqual(26, assessmentLevelVariable.VariableId);
            Assert.AreEqual(HydraRingDistributionType.Deterministic, assessmentLevelVariable.DistributionType);
            Assert.AreEqual(0.0, assessmentLevelVariable.Value);
            Assert.AreEqual(HydraRingDeviationType.Standard, assessmentLevelVariable.DeviationType);
            Assert.IsNaN(assessmentLevelVariable.Mean);
            Assert.IsNaN(assessmentLevelVariable.Variability);
            Assert.IsNaN(assessmentLevelVariable.Shift);
        }

        [Test]
        public void GetSubMechanismModelId_ReturnsExpectedValues()
        {
            // Call
            var assessmentLevelCalculationInput = new AssessmentLevelCalculationInput(1, 2.2);

            // Assert
            Assert.IsNull(assessmentLevelCalculationInput.GetSubMechanismModelId(1));
        }
    }
}