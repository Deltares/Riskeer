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
using Core.Common.Base.Geometry;
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
        public void UpdateSurfaceLinesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
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
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNotInTargetCollection_NewSurfaceLinesAdded()
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
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedMultipleLinesWithSameNames_ThrowsRingtoetsPipingSurfaceLineUpdateException()
        {
            // Setup
            const string duplicateName = "Duplicate name it is";
            var targetCollection = new ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>();
            targetCollection.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateName
                }
            }, sourceFilePath);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines =
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

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<RingtoetsPipingSurfaceLineUpdateException>(call);
            Assert.AreEqual("Het bijwerken van de profielschematisaties is mislukt.", exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedDataEmpty_SurfaceLinesRemoved()
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
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedDataHasSameName_UpdatesTargetCollection()
        {
            // Setup
            var targetSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            var targetCollection = new ObservableCollectionWithSourcePath<RingtoetsPipingSurfaceLine>();
            targetCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            Point3D[] expectedGeometry =
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5),
                new Point3D(6, 7, 8)
            };
            var readSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            readSurfaceLine.SetGeometry(expectedGeometry);
            var readSurfacelines = new[]
            {
                readSurfaceLine
            };

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   readSurfacelines,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(targetSurfaceLine, targetCollection[0]);
            CollectionAssert.AreEqual(expectedGeometry, targetSurfaceLine.Points);
            CollectionAssert.AreEqual(new[]
            {
                targetSurfaceLine
            }, affectedObjects);
        }
    }
}