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

using System.IO;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Importers;
using Ringtoets.Revetment.IO.Readers;

namespace Ringtoets.Revetment.IO.Test.Importers
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationImporterTest
    {
        private readonly string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO, "WaveConditionsCalculationConfigurationImporter");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new WaveConditionsCalculationConfigurationImporter<SimpleWaveConditionsCalculation>(
                "",
                new CalculationGroup());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<WaveConditionsCalculationConfigurationReader, ReadWaveConditionsCalculation>>(importer);
        }

        [Test]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new WaveConditionsCalculationConfigurationImporter<SimpleWaveConditionsCalculation>(
                filePath,
                calculationGroup);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            Assert.AreEqual(1, calculationGroup.Children.Count);
            var calculation = (IWaveConditionsCalculation) calculationGroup.Children[0];
            Assert.AreEqual("Berekening 1", calculation.Name);
        }

        private class SimpleWaveConditionsCalculation : Observable, IWaveConditionsCalculation
        {
            public string Name { get; set; }
            public bool HasOutput { get; }
            public Comment Comments { get; }
            public void ClearOutput()
            {
                throw new System.NotImplementedException();
            }

            public WaveConditionsInput InputParameters { get; }
        }
    }
}