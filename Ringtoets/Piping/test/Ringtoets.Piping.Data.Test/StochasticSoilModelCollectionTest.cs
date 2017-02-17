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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelCollectionTest
    {
        [Test]
        public void DefaultConstructor_ReturnsCollectionWithPath()
        {
            // Call
            var collection = new StochasticSoilModelCollection();

            // Assert
            Assert.IsInstanceOf<ObservableCollectionWithSourcePath<StochasticSoilModel>>(collection);
        }

        [Test]
        public void AddRange_SurfaceLinesWithDifferentNames_AddsSurfaceLines()
        {
            // Setup
            var stochasticSoilModelsToAdd = new[]
            {
                new StochasticSoilModel(5, "Model A", "segmentA"),
                new StochasticSoilModel(6, "Model B", "segmentA")
            };

            var collection = new StochasticSoilModelCollection();
            const string expectedFilePath = "other/path";

            // Call
            collection.AddRange(stochasticSoilModelsToAdd, expectedFilePath);

            // Assert
            Assert.AreEqual(expectedFilePath, collection.SourcePath);
            CollectionAssert.AreEqual(stochasticSoilModelsToAdd, collection);
        }

        [Test]
        public void AddRange_WithStochasticSoilModelsWithEqualNames_ThrowsArgumentException()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            const string someName = "Soil model";
            var modelsToAdd = new[]
            {
                new StochasticSoilModel(5, someName, "segmentA"),
                new StochasticSoilModel(6, someName, "segmentB")
            };

            // Call
            TestDelegate call = () => collection.AddRange(modelsToAdd, "valid/file/path");

            // Assert
            string message = $"Ondergrondmodellen moeten een unieke naam hebben. Gevonden dubbele namen: {someName}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void AddRange_WithMultipleStochasticSoilModelsWithEqualNames_ThrowsArgumentException()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            const string someName = "Soil model";
            const string someOtherName = "Other soil model";
            var modelsToAdd = new[]
            {
                new StochasticSoilModel(5, someName, "segmentA"),
                new StochasticSoilModel(6, someName, "segmentB"),
                new StochasticSoilModel(7, someOtherName, "segmentC"),
                new StochasticSoilModel(8, someOtherName, "segmentD"),
                new StochasticSoilModel(9, someOtherName, "segmentE")
            };

            // Call
            TestDelegate call = () => collection.AddRange(modelsToAdd, "valid/file/path");

            // Assert
            string message = $"Ondergrondmodellen moeten een unieke naam hebben. Gevonden dubbele namen: {someName}, {someOtherName}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }
    }
}