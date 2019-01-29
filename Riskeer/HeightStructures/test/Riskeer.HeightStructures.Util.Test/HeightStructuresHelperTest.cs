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

using System;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;

namespace Riskeer.HeightStructures.Util.Test
{
    [TestFixture]
    public class HeightStructuresHelperTest
    {
        [Test]
        public void UpdateCalculationToSectionResultAssignments_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultWithCalculationNoRemainingCalculations_SectionResultCalculationIsNull()
        {
            // Setup
            var location = new Point2D(1, 1);
            var failureMechanism = new HeightStructuresFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection(location)
            });

            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = CreateCalculation(location);

            // Call
            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            // Assert
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultWithCalculationWithRemainingCalculations_SectionResultCalculationSetToRemainingCalculation()
        {
            // Setup
            var location = new Point2D(1, 1);
            StructuresCalculation<HeightStructuresInput> remainingCalculation = CreateCalculation(new Point2D(10, 10));
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        remainingCalculation
                    }
                }
            };
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                CreateFailureMechanismSection(location)
            });

            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = CreateCalculation(location);

            // Call
            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            // Assert
            Assert.AreSame(remainingCalculation, sectionResult.Calculation);
        }

        private static FailureMechanismSection CreateFailureMechanismSection(Point2D endPoint)
        {
            return FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                endPoint
            });
        }

        private static StructuresCalculation<HeightStructuresInput> CreateCalculation(Point2D location)
        {
            return new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestHeightStructure(location)
                }
            };
        }
    }
}