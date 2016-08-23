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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.IO;

namespace Ringtoets.HydraRing.Calculation.Test.IO
{
    [TestFixture]
    public class HydraRingSettingsVariableCsvReaderTest
    {
        [Test]
        public void Constructor_FileContentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestVariableCsvReader(null, new object());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "File contents must be set.");
        }

        [Test]
        public void Constructor_SettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestVariableCsvReader("path.csv", null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "The settings object must be provided.");
        }

        [Test]
        [TestCase("Toetspeil", HydraRingFailureMechanismType.AssessmentLevel)]
        [TestCase("Hs", HydraRingFailureMechanismType.WaveHeight)]
        [TestCase("Tp", HydraRingFailureMechanismType.WavePeakPeriod)]
        [TestCase("Tm-1,0", HydraRingFailureMechanismType.WaveSpectralPeriod)]
        [TestCase("Q", HydraRingFailureMechanismType.QVariant)]
        [TestCase("HBN", HydraRingFailureMechanismType.DikesHeight)]
        [TestCase("Gras", HydraRingFailureMechanismType.DikesOvertopping)]
        [TestCase("KwHoogte", HydraRingFailureMechanismType.StructuresOvertopping)]
        [TestCase("KwSluiten", HydraRingFailureMechanismType.StructuresClosure)]
        [TestCase("KwPuntconstructies", HydraRingFailureMechanismType.StructuresStructuralFailure)]
        public void GetFailureMechanismType_Always_ReturnsFailureMechanismType(string variable, HydraRingFailureMechanismType expectedType)
        {
            // Setup
            var reader = new TestVariableCsvReader("path.csv", new object());

            // Call
            HydraRingFailureMechanismType actualType = reader.TestGetFailureMechanismType(variable);

            // Assert
            Assert.AreEqual(expectedType, actualType);
        }

        private class TestVariableCsvReader : HydraRingSettingsVariableCsvReader<object>
        {
            public TestVariableCsvReader(string fileContents, object settings) 
                : base(fileContents, settings) {}
            
            protected override void CreateSetting(IList<string> line)
            {
                throw new NotImplementedException();
            }

            public HydraRingFailureMechanismType TestGetFailureMechanismType(string variable)
            {
                return GetFailureMechanismType(variable);
            }
        }
    }
}