﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class SpecificFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new SpecificFailureMechanism();

            // Assert
            Assert.IsInstanceOf<Observable>(failureMechanism);
            Assert.IsInstanceOf<IFailureMechanism<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>>(failureMechanism);
            Assert.IsInstanceOf<IHasGeneralInput>(failureMechanism);
            Assert.AreEqual("Nieuw faalmechanisme", failureMechanism.Name);
            Assert.AreEqual("NIEUW", failureMechanism.Code);
            Assert.IsNotNull(failureMechanism.InAssemblyInputComments);
            Assert.IsNotNull(failureMechanism.InAssemblyOutputComments);
            Assert.IsNotNull(failureMechanism.NotInAssemblyComments);
            Assert.IsNotNull(failureMechanism.AssemblyResult);
            Assert.IsTrue(failureMechanism.InAssembly);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void SetSections_SectionsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new SpecificFailureMechanism();

            // Call 
            void Call() => failureMechanism.SetSections(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void SetSections_SourcePathNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new SpecificFailureMechanism();

            // Call 
            void Call() => failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourcePath", exception.ParamName);
        }

        [Test]
        public void GivenSpecificFailureMechanismWithoutSections_WhenSettingValidSections_ThenExpectedSectionsAndSourcePathAndSectionResultsSet()
        {
            // Given
            const string sourcePath = "some/Path";
            var failureMechanism = new SpecificFailureMechanism();

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(3, 4),
                new Point2D(matchingX, matchingY)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(-2, -1)
            });

            FailureMechanismSection[] sections =
            {
                section1,
                section2
            };

            // When
            failureMechanism.SetSections(sections, sourcePath);

            // Then
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            CollectionAssert.AreEqual(sections, failureMechanism.Sections);
            Assert.AreEqual(sections.Length, failureMechanism.SectionResults.Count());
            CollectionAssert.AreEqual(sections, failureMechanism.SectionResults.Select(sr => sr.Section));
        }

        [Test]
        public void GivenSpecificFailureMechanismWithSections_WhenSettingValidSections_ThenExpectedSectionsAndSourcePathAndSectionResultsSet()
        {
            // Given
            const string sourcePath = "some/Path";
            var failureMechanism = new SpecificFailureMechanism();

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(3, 4),
                new Point2D(matchingX, matchingY)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(-2, -1)
            });

            FailureMechanismSection[] sections =
            {
                section1,
                section2
            };

            failureMechanism.SetSections(new[]
            {
                new FailureMechanismSection("X", new[]
                {
                    new Point2D(matchingX, matchingY),
                    new Point2D(0, 0)
                })
            }, "");

            // When
            failureMechanism.SetSections(sections, sourcePath);

            // Then
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            CollectionAssert.AreEqual(sections, failureMechanism.Sections);
            Assert.AreEqual(sections.Length, failureMechanism.SectionResults.Count());
            CollectionAssert.AreEqual(sections, failureMechanism.SectionResults.Select(sr => sr.Section));
        }

        [Test]
        public void SetSections_SecondSectionEndConnectingToStartOfFirst_ThrowArgumentException()
        {
            // Setup
            var failureMechanism = new SpecificFailureMechanism();

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(3, 4)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(-2, -1),
                new Point2D(matchingX, matchingY)
            });

            // Call
            void Call() => failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het faalmechanisme.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void SetSections_SecondSectionDoesNotConnectToFirst_ThrowArgumentException()
        {
            // Setup
            var failureMechanism = new SpecificFailureMechanism();

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(5, 6),
                new Point2D(7, 8)
            });

            // Call
            void Call() => failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het faalmechanisme.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void ClearAllSections_HasSections_ClearSectionsAndSectionResultsAndSourcePath()
        {
            // Setup
            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var failureMechanism = new SpecificFailureMechanism();
            const string sourcePath = "some/Path";
            failureMechanism.SetSections(new[]
            {
                section
            }, sourcePath);

            // Precondition
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            CollectionAssert.IsNotEmpty(failureMechanism.Sections);
            CollectionAssert.IsNotEmpty(failureMechanism.SectionResults);

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }
    }
}