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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Revetment.IO;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.IO.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsExporterTest
    {
        private readonly string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.WaveImpactAsphaltCover.IO, "test.csv");

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Call
            var waveConditionsExporter = new WaveImpactAsphaltCoverWaveConditionsExporter(new WaveImpactAsphaltCoverWaveConditionsCalculation[0], testFilePath);

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(waveConditionsExporter);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsExporter(null, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsExporter(new WaveImpactAsphaltCoverWaveConditionsCalculation[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }
    }
}