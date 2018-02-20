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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<StabilityPointStructuresFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Kunstwerken - Sterkte en stabiliteit puntconstructies", failureMechanism.Name);
            Assert.AreEqual("STKWp", failureMechanism.Code);

            Assert.IsInstanceOf<GeneralStabilityPointStructuresInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.StabilityPointStructures);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
        }

        [Test]
        public void AddSection_WithSection_AddedSectionResult()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            failureMechanism.AddSection(section);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsCleared()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(2, 1)
            }));
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(2, 1)
            }));

            // Precondition
            Assert.AreEqual(2, failureMechanism.Sections.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnStabilityPointStructuresCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new StructuresCalculation<StabilityPointStructuresInput>(),
                        new StructuresCalculation<StabilityPointStructuresInput>()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            List<ICalculation> calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculations, typeof(StructuresCalculation<StabilityPointStructuresInput>));
            mocks.VerifyAll();
        }
    }
}