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
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Forms.Merge;

namespace Riskeer.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class CalculatableFailureMechanismMergeDataRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<TestCalculation> calculations = Enumerable.Repeat(new TestCalculation(), random.Next(0, 10));
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(calculations);
            // Call
            var row = new CalculatableFailureMechanismMergeDataRow(failureMechanism);

            // Assert
            Assert.IsInstanceOf<FailureMechanismMergeDataRow>(row);
            Assert.AreSame(failureMechanism, row.FailureMechanism);
            Assert.AreEqual(calculations.Count(), row.NumberOfCalculations);
        }
    }
}