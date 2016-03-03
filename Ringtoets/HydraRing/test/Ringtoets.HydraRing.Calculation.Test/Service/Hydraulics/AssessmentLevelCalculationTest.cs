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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Service.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Test.Service.Hydraulics
{
    [TestFixture]
    public class AssessmentLevelCalculationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var assessmentLevelCalculation = new AssessmentLevelCalculationInput(1, 2.2);

            // Assert
            Assert.AreEqual(1, assessmentLevelCalculation.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.AssessmentLevel, assessmentLevelCalculation.FailureMechanismType);
            Assert.AreEqual(2, assessmentLevelCalculation.CalculationTypeId);
            CollectionAssert.IsEmpty(assessmentLevelCalculation.ProfilePoints);
            Assert.AreEqual(2.2, assessmentLevelCalculation.Beta);
            Assert.AreEqual(1, assessmentLevelCalculation.Variables.Count());

            var assessmentLevelVariable = assessmentLevelCalculation.Variables.First();
            Assert.AreEqual(26, assessmentLevelVariable.VariableId);
            Assert.AreEqual(HydraRingDistributionType.Deterministic, assessmentLevelVariable.DistributionType);
            Assert.AreEqual(0.0, assessmentLevelVariable.Value);
            Assert.AreEqual(HydraRingDeviationType.Standard, assessmentLevelVariable.DeviationType);
            Assert.IsNaN(assessmentLevelVariable.Mean);
            Assert.IsNaN(assessmentLevelVariable.Variability);
            Assert.IsNaN(assessmentLevelVariable.Shift);
        }
    }
}