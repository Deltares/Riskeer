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

namespace Ringtoets.HeightStructures.Data.Test
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
            Assert.IsInstanceOf<IHasSectionResults<HeightStructuresFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Kunstwerken - Hoogte kunstwerk", failureMechanism.Name);
            Assert.AreEqual("HTKW", failureMechanism.Code);
            Assert.AreEqual(1, failureMechanism.Group);
            Assert.IsInstanceOf<GeneralHeightStructuresInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
        }

        [Test]
        public void AddSection_WithSection_AddedSectionResult()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            failureMechanism.AddSectionResult(section);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsCleared()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            failureMechanism.AddSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(2, 1)
            }));
            failureMechanism.AddSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
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