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
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointsParserTest
    {
        private readonly string testDirectory = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Parsers"),
                                                             nameof(IllustrationPointsParser));

        [Test]
        public void DefaultConstructor_CreatesNewParserInstance()
        {
            // Call
            var parser = new IllustrationPointsParser();

            // Assert
            Assert.IsInstanceOf<IHydraRingFileParser>(parser);
        }

        [Test]
        public void Parse_WorkingDirectoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithoutExpectedFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "EmptyWorkingDirectory");
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithInvalidOutputFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "InvalidFile");
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_WithWorkingDirectoryWithEmptyFile_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "EmptyDatabase");
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 1);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_ValidDataForOtherSection_ThrowsHydraRingFileParserException()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidStructuresStabilityOutputSection1");
            var parser = new IllustrationPointsParser();

            // Call
            TestDelegate test = () => parser.Parse(path, 2);

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Parse_ValidStructuresStabilityData_SetsOutputAsExpected()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidStructuresStabilityOutputSection1");
            var parser = new IllustrationPointsParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            GeneralResult generalResult = parser.Output;
            Assert.NotNull(generalResult);
            Assert.NotNull(generalResult.GoverningWind);
            Assert.AreEqual(30, generalResult.GoverningWind.Angle);
            Assert.AreEqual(" 30,0", generalResult.GoverningWind.Name);
            Assert.AreEqual(1.19513, generalResult.Beta);
            Assert.AreEqual(46, generalResult.Stochasts.Count());

            Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> illustrationPointNodes = generalResult.IllustrationPoints;
            Assert.AreEqual(12, illustrationPointNodes.Count);
            CollectionAssert.AllItemsAreInstancesOfType(illustrationPointNodes.Values.Select(ip => ip.Data), typeof(FaultTreeIllustrationPoint));

            ICollection<FaultTreeIllustrationPoint> faultTrees = new List<FaultTreeIllustrationPoint>();
            ICollection<SubMechanismIllustrationPoint> subMechanisms = new List<SubMechanismIllustrationPoint>();
            GetAllNodes(illustrationPointNodes.Values.First(), faultTrees, subMechanisms);

            Assert.AreEqual(11, faultTrees.Count);
            Assert.AreEqual(12, subMechanisms.Count);
            SubMechanismIllustrationPoint subMechanismIllustrationPoint = subMechanisms.First();
            Assert.AreEqual(-7.94268, subMechanismIllustrationPoint.Beta);
            Assert.AreEqual(new[]
            {
                Tuple.Create("Faalkans kunstwerk gegeven erosie bodem", -1.0, 4383.0, -7.94268)
            }, subMechanismIllustrationPoint.Stochasts.Select(s => Tuple.Create(s.Name, s.Alpha, s.Duration, s.Realization)));
            Assert.IsEmpty(subMechanismIllustrationPoint.Results);

            FaultTreeIllustrationPoint faultTreeIllustrationPoint = faultTrees.First();
            Assert.AreEqual(2.23881, faultTreeIllustrationPoint.Beta);
            Assert.AreEqual(46, faultTreeIllustrationPoint.Stochasts.Count);
            Assert.AreEqual(new[]
            {
                Tuple.Create("Kerende hoogte van het kunstwerk", 0.0, 4383.0),
                Tuple.Create("Modelfactor voor onvolkomen stroming", 0.0, 4383.0),
                Tuple.Create("Drempelhoogte niet gesloten kering of Hoogte van de onderkant van de wand/drempel", 0.0, 4383.0),
                Tuple.Create("Afvoercoefficient", -0.000743268, 4383.0),
                Tuple.Create("Doorstroomoppervlak van doorstroomopeningen", -1.48258e-05, 4383.0),
                Tuple.Create("Constructieve sterkte lineair belastingmodel stabiliteit", 0.011086, 4383.0)
            }, faultTreeIllustrationPoint.Stochasts.Take(6).Select(s => Tuple.Create(s.Name, s.Alpha, s.Duration)));
        }

        [Test]
        public void Parse_ValidDesignWaterLevelData_SetsGeneralResultAndSubmechanismOutputForAWindDirectionAsExpected()
        {
            // Setup
            string path = Path.Combine(testDirectory, "ValidDesignWaterLevelOutputSection1");
            var parser = new IllustrationPointsParser();

            // Call
            parser.Parse(path, 1);

            // Assert
            GeneralResult generalResult = parser.Output;
            Assert.NotNull(generalResult);
            Assert.NotNull(generalResult.GoverningWind);
            Assert.AreEqual(112.5, generalResult.GoverningWind.Angle);
            Assert.AreEqual("OZO", generalResult.GoverningWind.Name);
            Assert.AreEqual(4.45304, generalResult.Beta);
            Assert.AreEqual(6, generalResult.Stochasts.Count());
            Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> illustrationPointNodes = generalResult.IllustrationPoints;
            Assert.AreEqual(16, illustrationPointNodes.Count);
            CollectionAssert.AllItemsAreInstancesOfType(illustrationPointNodes.Values.Select(ip => ip.Data), typeof(SubMechanismIllustrationPoint));

            ICollection<FaultTreeIllustrationPoint> faultTrees = new List<FaultTreeIllustrationPoint>();
            ICollection<SubMechanismIllustrationPoint> subMechanisms = new List<SubMechanismIllustrationPoint>();
            GetAllNodes(illustrationPointNodes.Values.First(), faultTrees, subMechanisms);

            Assert.IsEmpty(faultTrees);
            SubMechanismIllustrationPoint subMechanismIllustrationPoint = subMechanisms.Single();
            Assert.AreEqual(4.50094, subMechanismIllustrationPoint.Beta);
            Assert.AreEqual(new[]
            {
                Tuple.Create("Windrichting", 0.0, 12.0, 0.0),
                Tuple.Create("Waterstand IJsselmeer", -0.938446, 96.0, 0.905149),
                Tuple.Create("Windsnelheid Schiphol 16 richtingen", 0.0031547, 12.0, 5.19222),
                Tuple.Create("Onzekerheid waterstand IJsselmeer", -0.269114, 96.0, 0.200791),
                Tuple.Create("Onzekerheid windsnelheid Schiphol 16 richtingen", 0.000277653, 12.0, 0.999941),
                Tuple.Create("Modelonzekerheid lokale waterstand", -0.216534, 4383.0, 0.146191)
            }, subMechanismIllustrationPoint.Stochasts.Select(s => Tuple.Create(s.Name, s.Alpha, s.Duration, s.Realization)));
            Assert.AreEqual(new[]
            {
                Tuple.Create("Z", -5.48391e-05),
                Tuple.Create("Considered water level", 1.24846),
                Tuple.Create("Computed local water level", 1.24852)
            }, subMechanismIllustrationPoint.Results.Select(s => Tuple.Create(s.Description, s.Value)));
        }

        private static void GetAllNodes(IllustrationPointTreeNode tree, ICollection<FaultTreeIllustrationPoint> faultTrees, ICollection<SubMechanismIllustrationPoint> subMechanisms)
        {
            var subMechanism = tree.Data as SubMechanismIllustrationPoint;
            var faultTree = tree.Data as FaultTreeIllustrationPoint;
            if (subMechanism != null)
            {
                subMechanisms.Add(subMechanism);
            }
            else if (faultTree != null)
            {
                faultTrees.Add(faultTree);
            }

            foreach (IllustrationPointTreeNode child in tree.Children)
            {
                GetAllNodes(child, faultTrees, subMechanisms);
            }
        }
    }
}