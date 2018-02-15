﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.ClosingStructures.Util.Test
{
    [TestFixture]
    public class ClosingStructuresHelperTest
    {
        [Test]
        public void UpdateCalculationToSectionResultAssignments_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultWithCalculationNoRemainingCalculations_SectionResultCalculationIsNull()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(failureMechanismSection);

            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults2.First();
            sectionResult.Calculation = calculationInSectionA;

            // Call
            ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            // Assert
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultWithCalculationWithRemainingCalculations_SectionResultCalculationSetToRemainingCalculation()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationInSectionB
                    }
                }
            };
            failureMechanism.AddSection(failureMechanismSection);

            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults2.First();
            sectionResult.Calculation = calculationInSectionA;

            // Call
            ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            // Assert
            Assert.AreSame(calculationInSectionB, sectionResult.Calculation);
        }

        #region Prepared data

        private static readonly FailureMechanismSection failureMechanismSection = new FailureMechanismSection("A", new List<Point2D>
        {
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 10.0)
        });

        private static readonly StructuresCalculation<ClosingStructuresInput> calculationInSectionA = new StructuresCalculation<ClosingStructuresInput>
        {
            InputParameters =
            {
                Structure = new TestClosingStructure(new Point2D(1.1, 2.2))
            }
        };

        private static readonly StructuresCalculation<ClosingStructuresInput> calculationInSectionB = new StructuresCalculation<ClosingStructuresInput>
        {
            InputParameters =
            {
                Structure = new TestClosingStructure(new Point2D(50.0, 66.0))
            }
        };

        #endregion
    }
}