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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionCollectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var sectionCollection = new FailureMechanismSectionCollection();

            // Assert
            Assert.IsInstanceOf<IEnumerable<FailureMechanismSection>>(sectionCollection);
            Assert.IsInstanceOf<Observable>(sectionCollection);
            Assert.IsNull(sectionCollection.SourcePath);
            CollectionAssert.IsEmpty(sectionCollection);
        }

        [Test]
        public void Clear_Always_ClearsSectionsAndSourcePath()
        {
            // Setup
            var sectionCollection = new FailureMechanismSectionCollection();
            sectionCollection.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, TestHelper.GetScratchPadPath());

            // Call
            sectionCollection.Clear();

            // Assert
            CollectionAssert.IsEmpty(sectionCollection);
            Assert.IsNull(sectionCollection.SourcePath);
        }

        [Test]
        public void SetSections_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionCollection = new FailureMechanismSectionCollection();

            // Call
            TestDelegate call = () => sectionCollection.SetSections(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void SetSections_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionCollection = new FailureMechanismSectionCollection();

            // Call
            TestDelegate call = () => sectionCollection.SetSections(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourcePath", exception.ParamName);
        }

        [Test]
        public void SetSections_InvalidPath_ThrowsArgumentException()
        {
            // Setup
            const string sourcePath = "<invalid>";
            var sectionCollection = new FailureMechanismSectionCollection();

            // Call
            TestDelegate call = () => sectionCollection.SetSections(Enumerable.Empty<FailureMechanismSection>(), sourcePath);

            // Assert
            string expectedMessage = $@"'{sourcePath}' is not a valid file path.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void SetSections_SecondSectionEndConnectingToStartOfFirst_ThrowsArgumentExceptionAndDoesNotSetSections()
        {
            // Setup
            var sectionCollection = new FailureMechanismSectionCollection();

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
            TestDelegate call = () => sectionCollection.SetSections(new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            CollectionAssert.IsEmpty(sectionCollection);
            Assert.IsNull(sectionCollection.SourcePath);
        }

        [Test]
        public void SetSections_SecondSectionDoesNotConnectToFirst_ThrowsArgumentException()
        {
            // Setup
            var sectionCollection = new FailureMechanismSectionCollection();

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
            TestDelegate call = () => sectionCollection.SetSections(new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            CollectionAssert.IsEmpty(sectionCollection);
            Assert.IsNull(sectionCollection.SourcePath);
        }

        [Test]
        public void GivenCollectionWithSections_WhenArgumentExceptionThrown_ThenOldDataRemains()
        {
            // Given
            var sectionCollection = new FailureMechanismSectionCollection();
            string oldPath = TestHelper.GetScratchPadPath();
            FailureMechanismSection oldSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            sectionCollection.SetSections(new[]
            {
                oldSection
            }, oldPath);

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

            // When
            TestDelegate call = () => sectionCollection.SetSections(new[]
            {
                section1,
                section2
            }, string.Empty);

            // Then
            Assert.Throws<ArgumentException>(call);
            Assert.AreSame(oldSection, sectionCollection.Single());
            Assert.AreEqual(oldPath, sectionCollection.SourcePath);
        }

        [Test]
        public void SetSections_ValidSections_SectionsAndSourcePathSet()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();
            var sectionCollection = new FailureMechanismSectionCollection();

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

            // Call
            sectionCollection.SetSections(new[]
            {
                section1,
                section2
            }, sourcePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                section1,
                section2
            }, sectionCollection);
            Assert.AreEqual(sourcePath, sectionCollection.SourcePath);
        }

        [Test]
        public void SetSections_WithEmptySectionCollection_SourcePathSet()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();
            var sectionCollection = new FailureMechanismSectionCollection();

            // Call
            sectionCollection.SetSections(Enumerable.Empty<FailureMechanismSection>(), sourcePath);

            // Assert
            CollectionAssert.IsEmpty(sectionCollection);
            Assert.AreEqual(sourcePath, sectionCollection.SourcePath);
        }
    }
}