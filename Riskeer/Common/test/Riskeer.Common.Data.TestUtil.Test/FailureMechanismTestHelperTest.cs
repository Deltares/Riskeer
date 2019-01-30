// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class FailureMechanismTestHelperTest
    {
        [Test]
        public void SetSections_WithSections_SetsSectionsToFailureMechanism()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            IEnumerable<FailureMechanismSection> sections = new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, sections);

            // Assert
            Assert.IsEmpty(failureMechanism.FailureMechanismSectionSourcePath);
            Assert.AreSame(sections.Single(), failureMechanism.Sections.Single());
        }

        [Test]
        public void AddSections_WithArguments_AddsSectionsToFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            int nrOfSections = random.Next(1, 10);
            var failureMechanism = new TestFailureMechanism();

            // Call
            FailureMechanismTestHelper.AddSections(failureMechanism, nrOfSections);

            // Assert
            Assert.IsEmpty(failureMechanism.FailureMechanismSectionSourcePath);
            Assert.AreEqual(nrOfSections, failureMechanism.Sections.Count());
        }

        [Test]
        public void AddSectionsBasedOnReferenceLine_WithArguments_AddsSectionsToFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            int nrOfSections = random.Next(1, 10);
            var failureMechanism = new TestFailureMechanism();
            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();

            // Call
            FailureMechanismTestHelper.AddSectionsBasedOnReferenceLine(referenceLine, failureMechanism, nrOfSections);

            // Assert
            Assert.IsEmpty(failureMechanism.FailureMechanismSectionSourcePath);
            Assert.AreEqual(nrOfSections, failureMechanism.Sections.Count());
            Assert.AreEqual(referenceLine.Points.First(), failureMechanism.Sections.First().StartPoint);
            Assert.AreEqual(referenceLine.Points.Last().X, failureMechanism.Sections.Last().EndPoint.X, 1e-6);
            Assert.AreEqual(referenceLine.Points.Last().Y, failureMechanism.Sections.Last().EndPoint.Y, 1e-6);
        }
    }
}