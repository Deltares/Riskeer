// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidHydraulicBoundaryDatabaseFilePath_ThrowsArgumentNullException(string invalidHydraulicBoundaryDatabaseFilePath)
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryCalculationSettings(invalidHydraulicBoundaryDatabaseFilePath,
                                                                               "D:\\hlcdFilePath",
                                                                               false,
                                                                               null);

            // Assert
            const string expectedMessage = "hydraulicBoundaryDatabaseFilePath is null, empty or consist of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidHlcdFilePath_ThrowsArgumentNullException(string invalidHlcdFilePath)
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryCalculationSettings("D:\\HydraulicBoundaryDatabseFilePath",
                                                                               invalidHlcdFilePath,
                                                                               false,
                                                                               null);

            // Assert
            const string expectedMessage = "hlcdFilePath is null, empty or consist of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("D:\\PreprocessorDirectory")]
        public void Constructor_WithArguments_ExpectedValues(string preprocessorDirectory)
        {
            // Setup
            const string hydraulicBoundaryDatabaseFilePath = "D:\\HydraulicBoundaryDatabaseFilePath";
            const string hlcdFilePath = "D:\\hlcdFilePath";
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            // Call
            var settings = new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                                    hlcdFilePath,
                                                                    usePreprocessorClosure,
                                                                    preprocessorDirectory);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
            Assert.AreEqual(preprocessorDirectory, settings.PreprocessorDirectory);
        }
    }
}