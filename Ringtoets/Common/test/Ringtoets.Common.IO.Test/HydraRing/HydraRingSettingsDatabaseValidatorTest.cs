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

using System.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraRingSettingsDatabaseValidatorTest
    {
        private const string testDataSubDirectory = "HydraRingSettingsDatabaseValidator";

        private static readonly string directoryPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO, testDataSubDirectory);

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string completeDatabasePath = Path.Combine(directoryPath, "withoutPreprocessor.config.sqlite");

            // Call
            using (var validator = new HydraRingSettingsDatabaseValidator(completeDatabasePath))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(validator);
            }
        }
    }
}