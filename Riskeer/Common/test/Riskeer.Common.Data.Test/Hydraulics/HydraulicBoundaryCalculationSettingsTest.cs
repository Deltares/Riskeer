// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
        public void Constructor_InvalidHlcdFilePath_ThrowsArgumentNullException(string invalidHlcdFilePath)
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryCalculationSettings(invalidHlcdFilePath, "D:\\hrdFilePath", string.Empty, false);

            // Assert
            const string expectedMessage = "hlcdFilePath is null, empty or consists of whitespaces.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidHrdFilePath_ThrowsArgumentNullException(string invalidHrdFilePath)
        {
            // Call
            void Call() => new HydraulicBoundaryCalculationSettings("D:\\hlcdFilePath", invalidHrdFilePath, string.Empty, false);

            // Assert
            const string expectedMessage = "hrdFilePath is null, empty or consists of whitespaces.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            const string hlcdFilePath = "D:\\hlcdFilePath";
            const string hrdFilePath = "D:\\hrdFilePath";
            const string hrdFileVersion = "1 2 3 4 5";
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            // Call
            var settings = new HydraulicBoundaryCalculationSettings(hlcdFilePath, hrdFilePath, hrdFileVersion, usePreprocessorClosure);

            // Assert
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(hrdFilePath, settings.HrdFilePath);
            Assert.AreEqual(hrdFileVersion, settings.HrdFileVersion);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
        }
    }
}