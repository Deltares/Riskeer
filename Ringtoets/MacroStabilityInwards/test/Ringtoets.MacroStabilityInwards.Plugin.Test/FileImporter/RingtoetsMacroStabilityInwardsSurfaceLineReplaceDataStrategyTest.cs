﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructure_NullArgument_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateDataStrategy>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<RingtoetsMacroStabilityInwardsSurfaceLine, MacroStabilityInwardsFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(null, Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsMacroStabilityInwardsSurfaceLineCollection(),
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
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsMacroStabilityInwardsSurfaceLineCollection(),
                                                                                  Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfTargetCollection()
        {
            // Setup 
            var targetCollection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());
            const string newSourcePath = "some/other/path";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
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
                new RingtoetsMacroStabilityInwardsSurfaceLine()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   importedSurfaceLines,
                                                                                                   sourceFilePath);

            // Assert
            RingtoetsMacroStabilityInwardsSurfaceLineCollection actualCollection = failureMechanism.SurfaceLines;
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
            var targetSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Name A"
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            var readSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Name B"
            };

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(failureMechanism);

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
            var existingSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            existingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5)
            });

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = existingSurfaceLine
                },
                Output = new MacroStabilityInwardsOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                existingSurfaceLine
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(
                failureMechanism.SurfaceLines,
                Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
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
            var targetCollection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();

            const string duplicateName = "Duplicate name it is";
            RingtoetsMacroStabilityInwardsSurfaceLine[] importedSurfaceLines =
            {
                new RingtoetsMacroStabilityInwardsSurfaceLine
                {
                    Name = duplicateName
                },
                new RingtoetsMacroStabilityInwardsSurfaceLine
                {
                    Name = duplicateName
                }
            };

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = "Profielschematisaties moeten een unieke naam hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}