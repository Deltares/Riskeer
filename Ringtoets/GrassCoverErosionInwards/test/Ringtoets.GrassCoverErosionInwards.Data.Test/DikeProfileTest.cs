// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.TestUtil;

using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class DikeProfileTest
    {
        [Test]
        public void Constructor_Valid()
        {
            // Setup
            var worldCoordinate = new Point2D(1.1, 2.2);

            // Call
            var dikeProfile = new DikeProfile(worldCoordinate);

            // Assert
            Assert.IsInstanceOf<double>(dikeProfile.Orientation);
            Assert.IsInstanceOf<double>(dikeProfile.X0);

            Assert.AreSame(worldCoordinate, dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0.0, dikeProfile.Orientation);
            Assert.AreEqual(0.0, dikeProfile.X0);
            Assert.AreEqual("Dijkprofiel", dikeProfile.Name);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.AreEqual(0.0, dikeProfile.CrestLevel);
            Assert.AreEqual(string.Empty, dikeProfile.Memo);
        }

        [Test]
        public void Constructor_WorldReferencePointIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DikeProfile(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("worldCoordinate", paramName);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(180.346)]
        [TestCase(360.0)]
        public void Orientation_SetNewValue_GetsNewValue(double newValue)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0,0));

            // Call
            dikeProfile.Orientation = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfile.Orientation);
        }

        [Test]
        [TestCase(-987.65)]
        [TestCase(-1e-6)]
        [TestCase(360 + 1e-6)]
        [TestCase(875.12)]
        public void Orientation_SetIllegalValue_ThrowsArgumentOutOfRangeException(double invalidNewValue)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            TestDelegate call = () => dikeProfile.Orientation = invalidNewValue;

            // Assert
            string expectedMessage = String.Format("De dijkprofiel oriëntatie waarde {0} moet in het interval [0, 360] liggen.",
                                                   invalidNewValue);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void X0_SetNewValue_GetsNewValue([Random(-9999.99, 9999.99, 1)] double newValue)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            dikeProfile.X0 = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfile.X0);
        }

        [Test]
        public void CrestLevel_SetNewValue_GetsNewValue([Random(-9999.99, 9999.99, 1)] double newValue)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            dikeProfile.CrestLevel = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfile.CrestLevel);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Cool new name!")]
        public void Name_SetNewValue_GetsNewValue(string name)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            dikeProfile.Name = name;

            // Assert
            Assert.AreEqual(name, dikeProfile.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Very informative memo")]
        public void Memo_SetNewValue_GetsNewValue(string memo)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            dikeProfile.Memo = memo;

            // Assert
            Assert.AreEqual(memo, dikeProfile.Memo);
        }

        [Test]
        public void BreakWater_SetToNull_GetsNewlySetNull()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            dikeProfile.BreakWater = null;

            // Assert
            Assert.IsNull(dikeProfile.BreakWater);
        }

        [Test]
        public void BreakWater_SetToNewInstance_GetsNewlySetInstance()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var newBreakWater = new BreakWater(BreakWaterType.Caisson, 1.1);

            // Call
            dikeProfile.BreakWater = newBreakWater;

            // Assert
            Assert.AreSame(newBreakWater, dikeProfile.BreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToNull_ReturnFalse()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0))
            {
                BreakWater = null
            };

            // Call
            bool hasBreakWater = dikeProfile.HasBreakWater;

            // Assert
            Assert.IsFalse(hasBreakWater);
        }

        [Test]
        public void HasBreakWater_BreakWaterSetToAnInstance_ReturnTrue()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0))
            {
                BreakWater = new BreakWater(BreakWaterType.Dam, 12.34)
            };

            // Call
            bool hasBreakWater = dikeProfile.HasBreakWater;

            // Assert
            Assert.IsTrue(hasBreakWater);
        }

        [Test]
        public void AddForshoreProfileSection_NoElementsYetInCollection_AddNewProfileSection()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var newSection = new ProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4));

            // Precondition
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);

            // Call
            dikeProfile.AddForshoreGeometrySection(newSection);

            // Assert
            CollectionAssert.Contains(dikeProfile.ForeshoreGeometry, newSection);
            Assert.AreEqual(1, dikeProfile.ForeshoreGeometry.Count());
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementsAndNewElementConnectsToStart_NewElementInsertedAtFront()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(1.1, 2.2);
            var existingSection = new ProfileSection(connectedPoint, new Point2D(3.3, 4.4));
            dikeProfile.AddForshoreGeometrySection(existingSection);

            var newSection = new ProfileSection(new Point2D(0.0, 0.0), connectedPoint);

            // Call
            dikeProfile.AddForshoreGeometrySection(newSection);

            // Assert
            ProfileSection[] expectedForshoreSections =
            {
                newSection,
                existingSection
            };
            CollectionAssert.AreEqual(expectedForshoreSections, dikeProfile.ForeshoreGeometry);
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementsAndNewElementNotProperlyConnectsToStart_ThrowsArgumentException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(1.1, 2.2);
            var existingSection = new ProfileSection(connectedPoint, new Point2D(3.3, 4.4));
            dikeProfile.AddForshoreGeometrySection(existingSection);

            var newSection = new ProfileSection(connectedPoint, new Point2D(0.0, 0.0));

            // Call
            TestDelegate call = () => dikeProfile.AddForshoreGeometrySection(newSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is wel verbonden, maar heeft een verkeerde oriëntatie (moet omgedraaid worden).");
            CollectionAssert.DoesNotContain(dikeProfile.ForeshoreGeometry, newSection);
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementAndNewElementConnectsToEnd_NewElementInsertedAtEnd()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(4.4, 5.5);
            var existingSection = new ProfileSection(new Point2D(1.1, 2.2), connectedPoint);
            dikeProfile.AddForshoreGeometrySection(existingSection);

            var newSection = new ProfileSection(connectedPoint, new Point2D(12.12, 13.13));

            // Call
            dikeProfile.AddForshoreGeometrySection(newSection);

            // Assert
            ProfileSection[] expectedForshoreSections =
            {
                existingSection,
                newSection
            };
            CollectionAssert.AreEqual(expectedForshoreSections, dikeProfile.ForeshoreGeometry);
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementsAndNewElementNotProperlyConnectsToEnd_ThrowsArgumentException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(4.4, 5.5);
            var existingSection = new ProfileSection(new Point2D(1.1, 2.2), connectedPoint);
            dikeProfile.AddForshoreGeometrySection(existingSection);

            var newSection = new ProfileSection(new Point2D(12.12, 13.13), connectedPoint);

            // Call
            TestDelegate call = () => dikeProfile.AddForshoreGeometrySection(newSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is wel verbonden, maar heeft een verkeerde oriëntatie (moet omgedraaid worden).");
            CollectionAssert.DoesNotContain(dikeProfile.ForeshoreGeometry, newSection);
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementsAndNewElementNotConnected_ThrowsArgumentException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-10.10, -11.11),
                new Point2D(-10.10, 5.5),
                new Point2D(7.7, 5.5),
                new Point2D(7.7, -4.4),
                new Point2D(13.13, -2.2)
            };
            IEnumerable<ProfileSection> existingSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                                 .Select(segment => new ProfileSection(segment.FirstPoint, segment.SecondPoint));
            foreach (ProfileSection existingSection in existingSections)
            {
                dikeProfile.AddForshoreGeometrySection(existingSection);
            }

            var random = new Random(123);
            var totallyDisconnectSection = new ProfileSection(new Point2D(random.NextDouble(), random.NextDouble()),
                                                              new Point2D(random.NextDouble(), random.NextDouble()));

            // Call
            TestDelegate call = () => dikeProfile.AddForshoreGeometrySection(totallyDisconnectSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            CollectionAssert.DoesNotContain(dikeProfile.ForeshoreGeometry, totallyDisconnectSection);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void AddForshoreProfileSection_CollectionHasElementsAndNewElementAlreadyPartOfCollection_ThrowsArgumentException(int index)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-10.10, -11.11),
                new Point2D(-10.10, 5.5),
                new Point2D(7.7, 5.5),
                new Point2D(7.7, -4.4),
                new Point2D(13.13, -2.2)
            };
            ProfileSection[] existingSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                 .Select(segment => new ProfileSection(segment.FirstPoint, segment.SecondPoint))
                                                 .ToArray();
            foreach (ProfileSection existingSection in existingSections)
            {
                dikeProfile.AddForshoreGeometrySection(existingSection);
            }

            ProfileSection totallyDisconnectSection = existingSections[index];

            // Call
            TestDelegate call = () => dikeProfile.AddForshoreGeometrySection(totallyDisconnectSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            Assert.AreEqual(existingSections.Length, dikeProfile.ForeshoreGeometry.Count());
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementsAndIsConnectedToElementInMiddleWithFirstPoint_ThrowsArgumentException(
            [Range(0, 2)] int index)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-2.2, -2.2),
                new Point2D(-1.1, -1.2),
                new Point2D(3.3, -1.1),
                new Point2D(4.4, 6.7),
                new Point2D(7.6, 3.2)
            };
            ProfileSection[] existingSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                .Select(segment => new ProfileSection(segment.FirstPoint, segment.SecondPoint))
                .ToArray();
            foreach (ProfileSection existingSection in existingSections)
            {
                dikeProfile.AddForshoreGeometrySection(existingSection);
            }

            var touchingSection = new ProfileSection(existingSections[index].EndingPoint, new Point2D(999.99, 999.9));

            // Call
            TestDelegate call = () => dikeProfile.AddForshoreGeometrySection(touchingSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            CollectionAssert.DoesNotContain(dikeProfile.ForeshoreGeometry, touchingSection);
        }

        [Test]
        public void AddForshoreProfileSection_CollectionHasElementsAndIsConnectedToElementInMiddleWithSecondPoint_ThrowsArgumentException(
            [Range(1, 3)] int index)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-2.2, -2.2),
                new Point2D(-1.1, -1.2),
                new Point2D(3.3, -1.1),
                new Point2D(4.4, 6.7),
                new Point2D(7.6, 3.2)
            };
            ProfileSection[] existingSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                      .Select(segment => new ProfileSection(segment.FirstPoint, segment.SecondPoint))
                                                      .ToArray();
            foreach (ProfileSection existingSection in existingSections)
            {
                dikeProfile.AddForshoreGeometrySection(existingSection);
            }

            var touchingSection = new ProfileSection(new Point2D(999.99, 999.9), existingSections[index].StartingPoint);

            // Call
            TestDelegate call = () => dikeProfile.AddForshoreGeometrySection(touchingSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            CollectionAssert.DoesNotContain(dikeProfile.ForeshoreGeometry, touchingSection);
        }

        [Test]
        public void AddDikeGeometrySection_NoElementsYetInCollection_AddNewRoughnessProfileSection()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var section = new RoughnessProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4), 5.5);

            // Precondition
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);

            // Call
            dikeProfile.AddDikeGeometrySection(section);

            // Assert
            CollectionAssert.Contains(dikeProfile.DikeGeometry, section);
            Assert.AreEqual(1, dikeProfile.DikeGeometry.Count());
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementsAndNewElementConnectsToStart_NewElementInsertedAtFront()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(1.1, 2.2);
            var existingSection = new RoughnessProfileSection(connectedPoint, new Point2D(3.3, 4.4), 1.0);
            dikeProfile.AddDikeGeometrySection(existingSection);

            var newSection = new RoughnessProfileSection(new Point2D(0.0, 0.0), connectedPoint, 1.0);

            // Call
            dikeProfile.AddDikeGeometrySection(newSection);

            // Assert
            RoughnessProfileSection[] expectedGeometrySections =
            {
                newSection,
                existingSection
            };
            CollectionAssert.AreEqual(expectedGeometrySections, dikeProfile.DikeGeometry);
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementsAndNewElementNotProperlyConnectsToStart_ThrowsArgumentException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(1.1, 2.2);
            var existingSection = new RoughnessProfileSection(connectedPoint, new Point2D(3.3, 4.4), 0.8);
            dikeProfile.AddDikeGeometrySection(existingSection);

            var newSection = new RoughnessProfileSection(connectedPoint, new Point2D(0.0, 0.0), 1.0);

            // Call
            TestDelegate call = () => dikeProfile.AddDikeGeometrySection(newSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is wel verbonden, maar heeft een verkeerde oriëntatie (moet omgedraaid worden).");
            CollectionAssert.DoesNotContain(dikeProfile.DikeGeometry, newSection);
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementAndNewElementConnectsToEnd_NewElementInsertedAtEnd()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(4.4, 5.5);
            var existingSection = new RoughnessProfileSection(new Point2D(1.1, 2.2), connectedPoint, 0.5);
            dikeProfile.AddDikeGeometrySection(existingSection);

            var newSection = new RoughnessProfileSection(connectedPoint, new Point2D(12.12, 13.13), 0.6);

            // Call
            dikeProfile.AddDikeGeometrySection(newSection);

            // Assert
            RoughnessProfileSection[] expectedGeometrySections =
            {
                existingSection,
                newSection
            };
            CollectionAssert.AreEqual(expectedGeometrySections, dikeProfile.DikeGeometry);
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementsAndNewElementNotProperlyConnectsToEnd_ThrowsArgumentException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var connectedPoint = new Point2D(4.4, 5.5);
            var existingSection = new RoughnessProfileSection(new Point2D(1.1, 2.2), connectedPoint, 0.7);
            dikeProfile.AddDikeGeometrySection(existingSection);

            var newSection = new RoughnessProfileSection(new Point2D(12.12, 13.13), connectedPoint, 0.6);

            // Call
            TestDelegate call = () => dikeProfile.AddDikeGeometrySection(newSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is wel verbonden, maar heeft een verkeerde oriëntatie (moet omgedraaid worden).");
            CollectionAssert.DoesNotContain(dikeProfile.DikeGeometry, newSection);
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementsAndNewElementNotConnected_ThrowsArgumentException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-10.10, -11.11),
                new Point2D(-10.10, 5.5),
                new Point2D(7.7, 5.5),
                new Point2D(7.7, -4.4),
                new Point2D(13.13, -2.2)
            };
            foreach (RoughnessProfileSection existingSection in Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                                      .Select(segment => ConvertSegmentToRoughnessSection(segment, 0.8)))
            {
                dikeProfile.AddDikeGeometrySection(existingSection);
            }

            var random = new Random(123);
            var totallyDisconnectSection = new RoughnessProfileSection(new Point2D(random.NextDouble(), random.NextDouble()),
                                                                       new Point2D(random.NextDouble(), random.NextDouble()),
                                                                       0.7);

            // Call
            TestDelegate call = () => dikeProfile.AddDikeGeometrySection(totallyDisconnectSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            CollectionAssert.DoesNotContain(dikeProfile.DikeGeometry, totallyDisconnectSection);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void AddDikeGeometrySection_CollectionHasElementsAndNewElementAlreadyPartOfCollection_ThrowsArgumentException(int index)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-10.10, -11.11),
                new Point2D(-10.10, 5.5),
                new Point2D(7.7, 5.5),
                new Point2D(7.7, -4.4),
                new Point2D(13.13, -2.2)
            };
            RoughnessProfileSection[] roughnessProfileSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                                       .Select(segment => ConvertSegmentToRoughnessSection(segment, 0.7))
                                                                       .ToArray();
            foreach (RoughnessProfileSection existingSection in roughnessProfileSections)
            {
                dikeProfile.AddDikeGeometrySection(existingSection);
            }

            RoughnessProfileSection alreadyAddedElement = dikeProfile.DikeGeometry.ElementAt(index);

            // Call
            TestDelegate call = () => dikeProfile.AddDikeGeometrySection(alreadyAddedElement);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            Assert.AreEqual(roughnessProfileSections.Length, dikeProfile.DikeGeometry.Count());
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementsAndIsConnectedToElementInMiddleWithFirstPoint_ThrowsArgumentException(
            [Range(0, 2)] int index)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-2.2, -2.2),
                new Point2D(-1.1, -1.2),
                new Point2D(3.3, -1.1),
                new Point2D(4.4, 6.7),
                new Point2D(7.6, 3.2)
            };
            RoughnessProfileSection[] existingSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                               .Select(segment => ConvertSegmentToRoughnessSection(segment, 0.9))
                                                               .ToArray();
            foreach (RoughnessProfileSection existingSection in existingSections)
            {
                dikeProfile.AddDikeGeometrySection(existingSection);
            }

            var touchingSection = new RoughnessProfileSection(existingSections[index].EndingPoint, new Point2D(999.99, 999.9), 0.6);

            // Call
            TestDelegate call = () => dikeProfile.AddDikeGeometrySection(touchingSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            CollectionAssert.DoesNotContain(dikeProfile.DikeGeometry, touchingSection);
        }

        [Test]
        public void AddDikeGeometrySection_CollectionHasElementsAndIsConnectedToElementInMiddleWithSecondPoint_ThrowsArgumentException(
            [Range(1, 3)] int index)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            var existingPoints = new[]
            {
                new Point2D(-2.2, -2.2),
                new Point2D(-1.1, -1.2),
                new Point2D(3.3, -1.1),
                new Point2D(4.4, 6.7),
                new Point2D(7.6, 3.2)
            };
            RoughnessProfileSection[] existingSections = Math2D.ConvertLinePointsToLineSegments(existingPoints)
                                                               .Select(segment => ConvertSegmentToRoughnessSection(segment, 0.9))
                                                               .ToArray();
            foreach (RoughnessProfileSection existingSection in existingSections)
            {
                dikeProfile.AddDikeGeometrySection(existingSection);
            }

            var touchingSection = new RoughnessProfileSection(new Point2D(999.99, 999.9), existingSections[index].StartingPoint, 0.8);

            // Call
            TestDelegate call = () => dikeProfile.AddDikeGeometrySection(touchingSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call,
                                                                                      "Het nieuwe segment is niet verbonden met de bestaande geometrie.");
            CollectionAssert.DoesNotContain(dikeProfile.DikeGeometry, touchingSection);
        }

        private RoughnessProfileSection ConvertSegmentToRoughnessSection(Segment2D segment, double roughness)
        {
            return new RoughnessProfileSection(segment.FirstPoint, segment.SecondPoint, roughness);
        }
    }
}