﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<HeightStructuresFailureMechanismSectionResultOld, AdoptableFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Hoogte kunstwerk", failureMechanism.Name);
            Assert.AreEqual("HTKW", failureMechanism.Code);
            Assert.IsInstanceOf<GeneralHeightStructuresInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            CollectionAssert.IsEmpty(failureMechanism.SectionResultsOld);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void SetSections_WithSection_SetsSectionResults()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResultsOld.Count());
            Assert.AreSame(section, failureMechanism.SectionResultsOld.First().Section);

            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionResults_SectionResultsCleared()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                })
            });

            // Precondition
            Assert.AreEqual(2, failureMechanism.SectionResultsOld.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.SectionResultsOld);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnHeightStructuresCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new StructuresCalculation<HeightStructuresInput>(),
                        new StructuresCalculation<HeightStructuresInput>()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            List<ICalculation> calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is StructuresCalculation<HeightStructuresInput>));
            mocks.VerifyAll();
        }
    }
}