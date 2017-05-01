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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class StructureCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<SimpleStructure>, SimpleStructure>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<SimpleStructure> CreateCollection()
        {
            return new StructureCollection<SimpleStructure>();
        }

        protected override IEnumerable<SimpleStructure> UniqueElements()
        {
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = "Structure Id",
                Name = "Structure with name",
                Location = new Point2D(0, 0)
            });
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = "Other structure Id",
                Name = "Structure with name",
                Location = new Point2D(0, 0)
            });
        }

        protected override IEnumerable<SimpleStructure> SingleNonUniqueElements()
        {
            const string id = "Structure Id";
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = id,
                Name = "Structure name",
                Location = new Point2D(0, 0)
            });
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = id,
                Name = "Other structure name",
                Location = new Point2D(1, 1)
            });
        }

        protected override IEnumerable<SimpleStructure> MultipleNonUniqueElements()
        {
            const string id = "Structure Id";
            const string otherId = "Other structure Id";
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = id,
                Name = "Structure name A",
                Location = new Point2D(1, 0)
            });
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = id,
                Name = "Structure name B",
                Location = new Point2D(2, 0)
            });
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = otherId,
                Name = "Structure name C",
                Location = new Point2D(3, 0)
            });
            yield return new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Id = otherId,
                Name = "Structure name D",
                Location = new Point2D(4, 0)
            });
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<SimpleStructure> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            Assert.AreEqual($"Kunstwerken moeten een unieke id hebben. Gevonden dubbele elementen: {someId}.", exception.Message);
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<SimpleStructure> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            string someOtherId = itemsToAdd.First(i => i.Id != someId).Id;
            Assert.AreEqual($"Kunstwerken moeten een unieke id hebben. Gevonden dubbele elementen: {someId}, {someOtherId}.", exception.Message);
        }
    }

    public class SimpleStructure : StructureBase
    {
        public SimpleStructure(ConstructionProperties constructionProperties) : base(constructionProperties) {}
    }
}