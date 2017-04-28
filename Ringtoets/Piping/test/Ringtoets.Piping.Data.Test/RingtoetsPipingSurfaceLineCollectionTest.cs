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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineCollectionTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var collection = new RingtoetsPipingSurfaceLineCollection();

            // Assert
            Assert.IsInstanceOf<ObservableUniqueItemCollectionWithSourcePath<RingtoetsPipingSurfaceLine>>(collection);
        }

        [Test]
        public void AddRange_SurfaceLinesWithDifferentNames_AddsSurfaceLines()
        {
            // Setup
            var surfaceLinesToAdd = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Name A"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Name B"
                }
            };

            var collection = new RingtoetsPipingSurfaceLineCollection();
            const string expectedFilePath = "other/path";

            // Call
            collection.AddRange(surfaceLinesToAdd, expectedFilePath);

            // Assert
            Assert.AreEqual(expectedFilePath, collection.SourcePath);
            CollectionAssert.AreEqual(surfaceLinesToAdd, collection);
        }

        [Test]
        public void AddRange_SurfaceLinesWithSameNames_ThrowsArgumentException()
        {
            // Setup
            const string duplicateName = "Duplicate name it is";
            var surfaceLinesToAdd = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateName
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateName
                }
            };

            var collection = new RingtoetsPipingSurfaceLineCollection();

            // Call
            TestDelegate call = () => collection.AddRange(surfaceLinesToAdd, "path");

            // Assert
            string message = $"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

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
            string message = $"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateNameOne}, {duplicateNameTwo}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }
    }
}