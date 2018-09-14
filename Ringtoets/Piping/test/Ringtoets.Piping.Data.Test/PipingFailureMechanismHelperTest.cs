// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismHelperTest
    {
        [Test]
        public void HasManualAssemblyResults_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingFailureMechanismHelper.HasManualAssemblyResults(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void HasManualAssemblyResults_FailureMechanismWithManualAssemblyResults_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0)
                })
            });
            failureMechanism.SectionResults.First().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = PipingFailureMechanismHelper.HasManualAssemblyResults(failureMechanism);

            // Assert
            Assert.IsTrue(hasManualAssemblyResults);
        }

        [Test]
        public void HasManualAssemblyResults_FailureMechanismWithoutManualAssemblyResults_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0)
                })
            });

            // Call
            bool hasManualAssemblyResults = PipingFailureMechanismHelper.HasManualAssemblyResults(failureMechanism);

            // Assert
            Assert.IsFalse(hasManualAssemblyResults);
        }
    }
}