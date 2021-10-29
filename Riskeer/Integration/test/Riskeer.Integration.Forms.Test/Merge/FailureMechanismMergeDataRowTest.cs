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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Forms.Merge;

namespace Riskeer.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class FailureMechanismMergeDataRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<TestCalculation> calculations = Enumerable.Repeat(new TestCalculation(), random.Next(0, 10));

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(calculations);
            mocks.ReplayAll();

            // Call
            var row = new FailureMechanismMergeDataRow(failureMechanism);

            // Assert
            Assert.IsInstanceOf<FailurePathMergeDataRow>(row);
            Assert.AreSame(failureMechanism, row.FailurePath);
            Assert.AreEqual(calculations.Count(), row.NumberOfCalculations);

            mocks.ReplayAll();
        }
    }
}