﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Plugin.FileImporter;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSurfaceLineReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructure_NullArgument_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingSurfaceLineReplaceDataStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new PipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateDataStrategy<PipingSurfaceLine>>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<PipingSurfaceLine, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new PipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            void Call() => strategy.UpdateSurfaceLinesWithImportedData(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new PipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            void Call() => strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<PipingSurfaceLine>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfTargetCollection()
        {
            // Setup 
            var failureMechanism = new PipingFailureMechanism();
            PipingSurfaceLineCollection targetCollection = failureMechanism.SurfaceLines;

            var strategy = new PipingSurfaceLineReplaceDataStrategy(failureMechanism);
            const string newSourcePath = "some/other/path";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<PipingSurfaceLine>(),
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
                new PipingSurfaceLine(string.Empty)
            };

            var failureMechanism = new PipingFailureMechanism();
            var strategy = new PipingSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(importedSurfaceLines,
                                                                                                   sourceFilePath);

            // Assert
            PipingSurfaceLineCollection actualCollection = failureMechanism.SurfaceLines;
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
            var targetSurfaceLine = new PipingSurfaceLine("Name A");
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            var readSurfaceLine = new PipingSurfaceLine("Name B");

            var strategy = new PipingSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                readSurfaceLine
            }, sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.SurfaceLines
            }, affectedObjects);

            PipingSurfaceLine[] expectedSurfaceLines =
            {
                readSurfaceLine
            };
            CollectionAssert.AreEqual(expectedSurfaceLines, failureMechanism.SurfaceLines);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CalculationWithOutputSurfaceLine_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var existingSurfaceLine = new PipingSurfaceLine(string.Empty);
            existingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5)
            });

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = existingSurfaceLine
                },
                Output = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties())
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                existingSurfaceLine
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new PipingSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<PipingSurfaceLine>(),
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
        public void UpdateSurfaceLinesWithImportedData_ImportedDataContainsDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateName = "Duplicate name it is";
            PipingSurfaceLine[] importedSurfaceLines =
            {
                new PipingSurfaceLine(duplicateName),
                new PipingSurfaceLine(duplicateName)
            };

            var strategy = new PipingSurfaceLineReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = "Profielschematisaties moeten een unieke naam hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}