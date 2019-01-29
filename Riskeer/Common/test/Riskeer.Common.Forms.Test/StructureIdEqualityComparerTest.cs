// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Forms.Test
{
    [TestFixture]
    public class StructureIdEqualityComparerTest
    {
        private static IEnumerable<TestCaseData> SameIdDifferentProperties
        {
            get
            {
                var reference = new TestStructure();
                yield return new TestCaseData(new TestStructure(reference.Id,
                                                                new Point2D(1, 1)));
                yield return new TestCaseData(new TestStructure(reference.Id,
                                                                "different name"));
                yield return new TestCaseData(new TestStructure(reference.Id,
                                                                reference.Name,
                                                                reference.Location,
                                                                RoundedDouble.NaN));
            }
        }

        [Test]
        public void Equals_SameInstance_ReturnTrue()
        {
            // Setup
            var comparer = new StructureIdEqualityComparer();
            var structure = new TestStructure();

            // Call
            bool areEqual = comparer.Equals(structure, structure);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Equals_OtherEqualInstance_ReturnTrue()
        {
            // Setup
            var comparer = new StructureIdEqualityComparer();
            var firstStructure = new TestStructure();
            var secondStructure = new TestStructure();

            // Call 
            bool firstEqualsSecond = comparer.Equals(firstStructure, secondStructure);
            bool secondEqualsFirst = comparer.Equals(secondStructure, firstStructure);

            // Assert
            Assert.IsTrue(firstEqualsSecond);
            Assert.IsTrue(secondEqualsFirst);
        }

        [Test]
        [TestCaseSource(nameof(SameIdDifferentProperties))]
        public void Equals_SameId_ReturnTrue(TestStructure secondStructure)
        {
            // Setup
            var comparer = new StructureIdEqualityComparer();
            var firstStructure = new TestStructure();

            // Call 
            bool firstEqualsSecond = comparer.Equals(firstStructure, secondStructure);
            bool secondEqualsFirst = comparer.Equals(secondStructure, firstStructure);

            // Assert
            Assert.IsTrue(firstEqualsSecond);
            Assert.IsTrue(secondEqualsFirst);
        }

        [Test]
        public void Equals_UnequalInstance_ReturnFalse()
        {
            // Setup
            var comparer = new StructureIdEqualityComparer();
            var firstStructure = new TestStructure("id");
            var secondStructure = new TestStructure("other id");

            // Call 
            bool firstEqualsSecond = comparer.Equals(firstStructure, secondStructure);
            bool secondEqualsFirst = comparer.Equals(secondStructure, firstStructure);

            // Assert
            Assert.IsFalse(firstEqualsSecond);
            Assert.IsFalse(secondEqualsFirst);
        }

        [Test]
        public void GetHashCode_EqualStructures_ReturnsSameHashCode()
        {
            // Setup
            var comparer = new StructureIdEqualityComparer();
            var structureOne = new TestStructure();
            var structureTwo = new TestStructure();

            // Call
            int hashCodeOne = comparer.GetHashCode(structureOne);
            int hashCodeTwo = comparer.GetHashCode(structureTwo);

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }
    }
}