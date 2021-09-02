// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base.IO;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporterTest
    {
        [Test]
        public void Constructor_LocationCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("locationCalculationsForTargetProbabilities", exception.ParamName);
        }
        
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("C:\\Not:Valid")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string folderPath)
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>(), folderPath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }
        
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>(), "test");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }
    }
}