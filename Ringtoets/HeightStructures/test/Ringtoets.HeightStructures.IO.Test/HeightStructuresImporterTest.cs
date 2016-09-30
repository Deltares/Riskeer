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
using System.IO;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.IO.Test
{
    [TestFixture]
    public class HeightStructuresImporterTest
    {
        private readonly ObservableList<HeightStructure> testImportTarget = new ObservableList<HeightStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        [Test]
        public void Constructor_Always_ExcpectedValues()
        {
            // Call
            var importer = new HeightStructuresImporter(testImportTarget, testReferenceLine, testFilePath);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<ObservableList<HeightStructure>>>(importer);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructuresImporter(null, testReferenceLine, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void Import_ValidImcompleteFile_LogAndTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var structuresImporter = new HeightStructuresImporter(new ObservableList<HeightStructure>(), referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST2'.",
                "Kunstwerk nummer 2 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST3'.",
                "Kunstwerk nummer 3 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST4'.",
                "Kunstwerk nummer 4 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST5'.",
                "Kunstwerk nummer 5 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST6'.",
                "Kunstwerk nummer 6 wordt overgeslagen."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_InvalidCsvFile_LogAndTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectShpIcompleteCsv", "Kunstwerken.shp"));

            string csvFilePath = Path.ChangeExtension(filePath, ".csv");

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var structuresImporter = new HeightStructuresImporter(new ObservableList<HeightStructure>(), referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                "Parameter 'KW_HOOGTE2' komt meermaals voor. De eerste specificatie wordt gebruikt.",
                "De variantie van de stroomvoerende breedte bij bodembescherming lognormaalverdeling heeft een ongeldige waarde.",
                "De variantie van de toegestane peilverhoging op het kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde.",
                "Kunstwerk nummer 1 wordt overgeslagen."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
        }
    }
}