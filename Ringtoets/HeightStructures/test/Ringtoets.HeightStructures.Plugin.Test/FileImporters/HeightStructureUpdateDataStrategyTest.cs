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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Plugin.FileImporters;

namespace Ringtoets.HeightStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class HeightStructureUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructureUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<UpdateDataStrategyBase<HeightStructure, HeightStructuresFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IStructureUpdateStrategy<HeightStructure>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(null, Enumerable.Empty<HeightStructure>(),
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<HeightStructure>(),
                                                                                null,
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<HeightStructure>(),
                                                                                Enumerable.Empty<HeightStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }
    }
}