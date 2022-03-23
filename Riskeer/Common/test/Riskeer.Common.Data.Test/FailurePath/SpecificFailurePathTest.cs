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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Common.Data.Test.FailurePath
{
    [TestFixture]
    public class SpecificFailurePathTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failurePath = new SpecificFailurePath();

            // Assert
            Assert.IsInstanceOf<Observable>(failurePath);
            Assert.IsInstanceOf<IFailureMechanism<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>>(failurePath);
            Assert.IsInstanceOf<IHasGeneralInput>(failurePath);
            Assert.AreEqual("Nieuw faalmechanisme", failurePath.Name);
            Assert.AreEqual("NIEUW", failurePath.Code);
            Assert.IsNotNull(failurePath.GeneralInput);
            Assert.IsNotNull(failurePath.InAssemblyInputComments);
            Assert.IsNotNull(failurePath.InAssemblyOutputComments);
            Assert.IsNotNull(failurePath.NotInAssemblyComments);
            Assert.IsNotNull(failurePath.AssemblyResult);
            Assert.IsTrue(failurePath.InAssembly);
            CollectionAssert.IsEmpty(failurePath.Sections);
            CollectionAssert.IsEmpty(failurePath.SectionResults);
        }

        [Test]
        public void SetSections_SectionsNull_ThrowArgumentNullException()
        {
            // Setup
            var failurePath = new SpecificFailurePath();

            // Call 
            void Call() => failurePath.SetSections(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void SetSections_SourcePathNull_ThrowArgumentNullException()
        {
            // Setup
            var failurePath = new SpecificFailurePath();

            // Call 
            void Call() => failurePath.SetSections(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourcePath", exception.ParamName);
        }

        [Test]
        public void GivenSpecificFailurePathWithoutSections_WhenSettingValidSections_ThenExpectedSectionsAndSourcePathAndSectionResultsSet()
        {
            // Given
            const string sourcePath = "some/Path";
            var failurePath = new SpecificFailurePath();

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
            failurePath.SetSections(sections, sourcePath);

            // Then
            Assert.AreEqual(sourcePath, failurePath.FailureMechanismSectionSourcePath);
            CollectionAssert.AreEqual(sections, failurePath.Sections);
            Assert.AreEqual(sections.Length, failurePath.SectionResults.Count());
            CollectionAssert.AreEqual(sections, failurePath.SectionResults.Select(sr => sr.Section));
        }

        [Test]
        public void GivenSpecificFailurePathWithSections_WhenSettingValidSections_ThenExpectedSectionsAndSourcePathAndSectionResultsSet()
        {
            // Given
            const string sourcePath = "some/Path";
            var failurePath = new SpecificFailurePath();

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

            failurePath.SetSections(new[]
            {
                new FailureMechanismSection("X", new[]
                {
                    new Point2D(matchingX, matchingY),
                    new Point2D(0, 0)
                })
            }, "");

            // When
            failurePath.SetSections(sections, sourcePath);

            // Then
            Assert.AreEqual(sourcePath, failurePath.FailureMechanismSectionSourcePath);
            CollectionAssert.AreEqual(sections, failurePath.Sections);
            Assert.AreEqual(sections.Length, failurePath.SectionResults.Count());
            CollectionAssert.AreEqual(sections, failurePath.SectionResults.Select(sr => sr.Section));
        }

        [Test]
        public void SetSections_SecondSectionEndConnectingToStartOfFirst_ThrowArgumentException()
        {
            // Setup
            var failurePath = new SpecificFailurePath();

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
            void Call() => failurePath.SetSections(new[]
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
            var failurePath = new SpecificFailurePath();

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
            void Call() => failurePath.SetSections(new[]
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

            var failurePath = new SpecificFailurePath();
            const string sourcePath = "some/Path";
            failurePath.SetSections(new[]
            {
                section
            }, sourcePath);

            // Precondition
            Assert.AreEqual(sourcePath, failurePath.FailureMechanismSectionSourcePath);
            CollectionAssert.IsNotEmpty(failurePath.Sections);
            CollectionAssert.IsNotEmpty(failurePath.SectionResults);

            // Call
            failurePath.ClearAllSections();

            // Assert
            Assert.IsNull(failurePath.FailureMechanismSectionSourcePath);
            CollectionAssert.IsEmpty(failurePath.Sections);
            CollectionAssert.IsEmpty(failurePath.SectionResults);
        }
    }
}