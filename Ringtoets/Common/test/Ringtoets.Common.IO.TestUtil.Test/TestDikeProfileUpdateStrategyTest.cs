﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Common.IO.TestUtil.Test
{
    [TestFixture]
    public class TestDikeProfileUpdateStrategyTest
    {
        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new TestDikeProfileUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<IDikeProfileUpdateDataStrategy>(strategy);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithoutSettingUpdatedInstances_SetPropertiesAndReturnSetUpdatedInstances()
        {
            // Setup
            var strategy = new TestDikeProfileUpdateStrategy();

            const string filePath = "path/to/dikeprofiles";
            var readDikeProfiles = new[]
            {
                new TestDikeProfile("Dike One"),
                new TestDikeProfile("Dike Two"),
                new TestDikeProfile("Dike Three")
            };

            // Call
            strategy.UpdateDikeProfilesWithImportedData(null, readDikeProfiles, filePath);

            // Assert
            Assert.AreEqual(filePath, strategy.FilePath);
            Assert.IsTrue(strategy.Updated);
            CollectionAssert.AreEqual(readDikeProfiles, strategy.ReadDikeProfiles);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_UpdatedInstancesSet_ReturnsSetUpdatedInstances()
        {
            // Setup
            IEnumerable<IObservable> updatedInstances = Enumerable.Empty<IObservable>();

            var strategy = new TestDikeProfileUpdateStrategy();
            strategy.UpdatedInstances = updatedInstances;

            // Call
            strategy.UpdateDikeProfilesWithImportedData(null, Enumerable.Empty<DikeProfile>(), string.Empty);

            // Assert
            Assert.AreSame(updatedInstances, strategy.UpdatedInstances);
        }
    }
}
