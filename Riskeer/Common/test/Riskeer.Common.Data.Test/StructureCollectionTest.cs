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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test
{
    [TestFixture]
    public class StructureCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<TestStructure>, TestStructure>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<TestStructure> CreateCollection()
        {
            return new StructureCollection<TestStructure>();
        }

        protected override IEnumerable<TestStructure> UniqueElements()
        {
            yield return new TestStructure("Structure Id");
            yield return new TestStructure("Other structure Id");
        }

        protected override IEnumerable<TestStructure> SingleNonUniqueElements()
        {
            yield return new TestStructure("Structure Id");
            yield return new TestStructure("Structure Id",
                                           "Other structure name",
                                           new Point2D(1, 1));
        }

        protected override IEnumerable<TestStructure> MultipleNonUniqueElements()
        {
            const string id = "Structure Id";
            const string otherId = "Other structure Id";
            yield return new TestStructure(id,
                                           "Structure name A",
                                           new Point2D(1, 0));
            yield return new TestStructure(id,
                                           "Structure name B",
                                           new Point2D(2, 0));
            yield return new TestStructure(otherId,
                                           "Structure name C",
                                           new Point2D(3, 0));
            yield return new TestStructure(otherId,
                                           "Structure name D",
                                           new Point2D(4, 0));
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception,
                                                              IEnumerable<TestStructure> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            Assert.AreEqual($"Kunstwerken moeten een unieke id hebben. Gevonden dubbele elementen: {someId}.",
                            exception.Message);
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception,
                                                                IEnumerable<TestStructure> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            string someOtherId = itemsToAdd.First(i => i.Id != someId).Id;
            Assert.AreEqual($"Kunstwerken moeten een unieke id hebben. Gevonden dubbele elementen: {someId}, {someOtherId}.",
                            exception.Message);
        }
    }
}