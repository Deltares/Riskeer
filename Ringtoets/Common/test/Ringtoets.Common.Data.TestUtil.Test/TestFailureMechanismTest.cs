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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_InitializeProperties()
        {
            // Call
            var failureMechanism = new TestFailureMechanism();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual("Test failure mechanism", failureMechanism.Name);
            Assert.AreEqual("TFM", failureMechanism.Code);
            Assert.AreEqual(1, failureMechanism.Group);
        }

        [Test]
        public void Constructor_WithNameAndCode_SetNameAndCodeProperties()
        {
            // Setup
            const string testName = "Other name";
            const string testCode = "ON";

            // Call
            var failureMechanism = new TestFailureMechanism(testName, testCode);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual(testName, failureMechanism.Name);
            Assert.AreEqual(testCode, failureMechanism.Code);
            Assert.AreEqual(1, failureMechanism.Group);
        }

        [Test]
        public void Constructor_WithCalculations_InitializeCalculationsProperties()
        {
            // Setup
            var testCalculations = new List<ICalculation>();

            // Call
            var failureMechanism = new TestFailureMechanism(testCalculations);

            // Assert
            Assert.AreSame(testCalculations, failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual("Test failure mechanism", failureMechanism.Name);
            Assert.AreEqual("TFM", failureMechanism.Code);
            Assert.AreEqual(1, failureMechanism.Group);
        }

        [Test]
        public void SetSections_WithSection_AddedSectionResult()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();

            var failureMechanism = new TestFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            }, sourcePath);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsCleared()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();

            var failureMechanism = new TestFailureMechanism();

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
            }, sourcePath);

            // Precondition
            Assert.AreEqual(2, failureMechanism.Sections.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
        }
    }
}