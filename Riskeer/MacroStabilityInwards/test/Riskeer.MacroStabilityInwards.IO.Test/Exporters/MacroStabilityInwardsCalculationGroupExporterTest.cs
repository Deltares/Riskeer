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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.MacroStabilityInwards.IO.Exporters;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), "ValidFolderPath");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("C:\\Not:Valid")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string folderPath)
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), folderPath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Export_Always_ReturnsFalse()
        {
            // Setup
            var exporter = new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), "ValidFolderPath");

            // Call
            bool exportResult = exporter.Export();

            // Assert
            Assert.IsFalse(exportResult);
        }
    }
}