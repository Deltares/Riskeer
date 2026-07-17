// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Service;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Service;
using NSubstitute;

namespace Riskeer.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            // Call
            var activity = new ClosingStructuresCalculationActivity(calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<CalculatableActivity>(activity);
            Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}'", activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            // Call
            TestDelegate call = () => new ClosingStructuresCalculationActivity(calculation, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            // Call
            TestDelegate call = () => new ClosingStructuresCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}