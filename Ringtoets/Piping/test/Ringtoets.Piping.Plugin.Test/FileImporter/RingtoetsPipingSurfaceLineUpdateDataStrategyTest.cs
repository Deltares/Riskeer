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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";


        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RingtoetsPipingSurfaceLineUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateSurfaceLineStrategy>(strategy);
        }

        [Test]
        public void Constructor_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(null, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>(),
                                                                                  null,
                                                                                  string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("readRingtoetsPipingSurfaceLines", paramName);
        }

        [Test]
        public void UpdatSurfaceLinesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>(),
                                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdatSurfaceLinesWithImportedData_ReadSurfaceLinesNotInTargetCollection_NewSurfaceLinesAdded()
        {
            // Setup
            var importedSurfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line A"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Name B"
                }
            };

            var targetCollection = new ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>();
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   importedSurfaceLines,
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedSurfaceLines, targetCollection);
            CollectionAssert.AreEqual(new []
            {
                targetCollection
            }, affectedObjects);
        }
        
        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentModelsAndImportedDataEmpty_SurfaceLinesRemoved()
        {
            // Setup
            var targetCollection = new ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>();
            targetCollection.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Name A"
                }
            }, sourceFilePath);

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.AreEqual(new []
            {
                targetCollection
            }, affectedObjects);
        }
    }
}