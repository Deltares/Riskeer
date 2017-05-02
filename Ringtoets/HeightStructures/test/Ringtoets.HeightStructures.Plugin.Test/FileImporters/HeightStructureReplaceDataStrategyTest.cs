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
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Plugin.FileImporters;

namespace Ringtoets.HeightStructures.Plugin.Test.FileImporters
{
    public class HeightStructureReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructure_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructureReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_ValidFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<IStructureUpdateStrategy<HeightStructure>>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<HeightStructure, HeightStructuresFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(null, Enumerable.Empty<HeightStructure>(),
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadHeightStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<HeightStructure>(),
                                                                                null,
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<HeightStructure>(),
                                                                                Enumerable.Empty<HeightStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_DifferentSourcePath_UpdatesSourcePathOfTargetCollection()
        {
            // Setup 
            var targetCollection = new StructureCollection<HeightStructure>();

            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());
            const string newSourcePath = "some/other/path";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 Enumerable.Empty<HeightStructure>(),
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
        public void UpdateStructuresWithImportedData_NoCurrentStructuresWithImportedData_AddsNewStructure()
        {
            // Setup
            var importedHeightStructures = new[]
            {
                new TestHeightStructure()
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(failureMechanism.HeightStructures,
                                                                                                 importedHeightStructures,
                                                                                                 sourceFilePath);

            // Assert
            StructureCollection<HeightStructure> actualCollection = failureMechanism.HeightStructures;
            CollectionAssert.AreEqual(importedHeightStructures, actualCollection);
            CollectionAssert.AreEqual(new[]
            {
                actualCollection
            }, affectedObjects);
            Assert.AreEqual(sourceFilePath, actualCollection.SourcePath);
        }
    }
}