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

using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new PipingFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<PipingFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Piping", failureMechanism.Name);
            Assert.AreEqual("STPH", failureMechanism.Code);
            Assert.AreEqual(2, failureMechanism.Group);

            Assert.IsNotNull(failureMechanism.GeneralInput);
            Assert.IsNotNull(failureMechanism.PipingProbabilityAssessmentInput);

            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Calculations_AddPipingCalculation_ItemIsAddedToCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_RemovePipingCalculation_ItemIsRemovedFromCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(calculation);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_AddCalculationGroup_ItemIsAddedToCollection()
        {
            // Setup
            var folder = new CalculationGroup();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, folder);
        }

        [Test]
        public void Calculations_RemoveCalculationGroup_ItemIsRemovedFromCollection()
        {
            // Setup
            var folder = new CalculationGroup();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(folder);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, folder);
        }

        [Test]
        public void AddSection_WithSection_AddedSectionResult()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            failureMechanism.SetSections(new[]
            {
                section
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsCleared()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            failureMechanism.SetSections(new[]
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
            Assert.AreEqual(2, failureMechanism.Sections.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }
    }
}