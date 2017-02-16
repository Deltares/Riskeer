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
using Core.Common.Base.IO;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.IO.Exporters;

namespace Ringtoets.Piping.IO.Test.Exporters
{
    [TestFixture]
    public class PipingConfigurationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new PipingConfigurationExporter(new CalculationGroup(), "test.xml");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingConfigurationExporter(null, "test.xml");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => new PipingConfigurationExporter(new CalculationGroup(), filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }
    }
}