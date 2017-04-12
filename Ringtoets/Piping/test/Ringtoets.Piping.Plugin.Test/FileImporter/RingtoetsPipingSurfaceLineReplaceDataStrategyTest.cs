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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructure_NullArgument_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RingtoetsPipingSurfaceLineReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateDataStrategy>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<RingtoetsPipingSurfaceLine, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void Constructor_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(null, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsPipingSurfaceLineCollection(),
                                                                                  null,
                                                                                  string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsPipingSurfaceLineCollection(),
                                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfTargetCollection()
        {
            // Setup 
            var targetCollection = new RingtoetsPipingSurfaceLineCollection();

            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());
            const string newSourcePath = "some/other/path";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   newSourcePath);

            // Assert
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetCollection
            }, affectedObjects);
            CollectionAssert.IsEmpty(targetCollection);
            Assert.AreEqual(newSourcePath, targetCollection.SourcePath);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_NoCurrentLinesWithImportedData_AddsNewSurfaceLines()
        {
            // Setup
            var importedSurfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };

            var failureMechanism = new PipingFailureMechanism();
            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   importedSurfaceLines,
                                                                                                   sourceFilePath);

            // Assert
            RingtoetsPipingSurfaceLineCollection actualCollection = failureMechanism.SurfaceLines;
            CollectionAssert.AreEqual(importedSurfaceLines, actualCollection);
            CollectionAssert.AreEqual(new[]
            {
                actualCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentAndImportedDataAreDifferent_ReplacesCurrentWithImportedData()
        {
            // Setup
            var targetSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            var readSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name B"
            };

            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(
                failureMechanism.SurfaceLines,
                new[]
                {
                    readSurfaceLine
                }, sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.SurfaceLines
            }, affectedObjects);

            var expectedSurfaceLines = new[]
            {
                readSurfaceLine
            };
            CollectionAssert.AreEqual(expectedSurfaceLines, failureMechanism.SurfaceLines);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CalculationWithOutputSurfaceLine_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var existingSurfaceLine = new RingtoetsPipingSurfaceLine();
            existingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = existingSurfaceLine
                },
                Output = new PipingOutput(new PipingOutput.ConstructionProperties())
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                existingSurfaceLine
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(
                failureMechanism.SurfaceLines,
                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.SurfaceLine);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                failureMechanism.SurfaceLines
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ImportedDataContainsDuplicateNames_ThrowsUpdateException()
        {
            // Setup
            var targetCollection = new RingtoetsPipingSurfaceLineCollection();

            const string duplicateName = "Duplicate name it is";
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

            var strategy = new RingtoetsPipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<RingtoetsPipingSurfaceLineUpdateException>(call);
            string expectedMessage = "Het importeren van profielschematisaties is mislukt: " +
                                     $"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<UpdateDataException>(exception.InnerException);
        }
    }
}