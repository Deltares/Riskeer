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
using System.Collections.Generic;
using System.IO;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.ClosingStructures.IO.Test
{
    [TestFixture]
    public class ClosingStructuresImporterTest
    {
        private readonly ObservableList<ClosingStructure> testImportTarget = new ObservableList<ClosingStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        [Test]
        public void Constructor_Always_ExcpectedValues()
        {
            // Call
            var importer = new ClosingStructuresImporter(testImportTarget, testReferenceLine, testFilePath);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<ObservableList<ClosingStructure>>>(importer);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClosingStructuresImporter(null, testReferenceLine, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndTrue()
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
            var structuresImporter = new ClosingStructuresImporter(new ObservableList<ClosingStructure>(), referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
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
                                                         Path.Combine("Structures", "CorrectShpIncompleteCsv", "Kunstwerken.shp"));

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
            var structuresImporter = new ClosingStructuresImporter(new ObservableList<ClosingStructure>(), referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                "De waarde op regel 13, kolom 18 valt buiten het bereik [0, 360].",
                "Parameter 'KW_BETSLUIT5' komt meermaals voor.",
                "Kunstwerk nummer 1 wordt overgeslagen.",
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
    }
}