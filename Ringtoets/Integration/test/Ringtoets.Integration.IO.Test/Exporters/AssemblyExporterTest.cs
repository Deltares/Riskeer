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
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Exporters;

namespace Ringtoets.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class AssemblyExporterTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.gml"));

            // Call
            TestDelegate call = () => new AssemblyExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => new AssemblyExporter(assessmentSection, null);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.gml"));

            // Call
            var exporter = new AssemblyExporter(assessmentSection, filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }
    }
}