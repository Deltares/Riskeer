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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<RingtoetsPipingSurfaceLine>, RingtoetsPipingSurfaceLine>
    {
        [Test]
        public void AddRange_MultipleSurfaceLinesWithSameNames_ThrowsArgumentException()
        {
            // Setup
            const string duplicateNameOne = "Duplicate name it is";
            const string duplicateNameTwo = "Duplicate name again";
            var surfaceLinesToAdd = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateNameOne
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateNameOne
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateNameTwo
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateNameTwo
                }
            };

            var collection = new RingtoetsPipingSurfaceLineCollection();

            // Call
            TestDelegate call = () => collection.AddRange(surfaceLinesToAdd, "path");

            // Assert
            string message = $"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateNameOne}, " +
                             $"{duplicateNameTwo}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        protected override ObservableUniqueItemCollectionWithSourcePath<RingtoetsPipingSurfaceLine> CreateCollection()
        {
            return new RingtoetsPipingSurfaceLineCollection();
        }

        protected override IEnumerable<RingtoetsPipingSurfaceLine> UniqueElements()
        {
            yield return new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            yield return new RingtoetsPipingSurfaceLine
            {
                Name = "Name B"
            };
        }

        protected override IEnumerable<RingtoetsPipingSurfaceLine> SingleNonUniqueElements()
        {
            const string duplicateName = "Duplicate name it is";

            yield return new RingtoetsPipingSurfaceLine
            {
                Name = duplicateName
            };
            yield return new RingtoetsPipingSurfaceLine
            {
                Name = duplicateName
            };
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<RingtoetsPipingSurfaceLine> itemsToAdd)
        {
            string duplicateName = itemsToAdd.First().Name;
            Assert.AreEqual($"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.",
                            exception.Message);
        }

        protected override IEnumerable<RingtoetsPipingSurfaceLine> MultipleNonUniqueElements()
        {
            const string duplicateNameOne = "Duplicate name it is";
            const string duplicateNameTwo = "Duplicate name again";
            yield return
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateNameOne
                };
            yield return new RingtoetsPipingSurfaceLine
            {
                Name = duplicateNameOne
            };
            yield return new RingtoetsPipingSurfaceLine
            {
                Name = duplicateNameTwo
            };
            yield return new RingtoetsPipingSurfaceLine
            {
                Name = duplicateNameTwo
            };
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<RingtoetsPipingSurfaceLine> itemsToAdd)
        {
            string duplicateNameOne = itemsToAdd.First().Name;
            string duplicateNameTwo = itemsToAdd.First(i => i.Name != duplicateNameOne).Name;
            Assert.AreEqual("Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: " +
                            $"{duplicateNameOne}, {duplicateNameTwo}.", exception.Message);
        }
    }
}