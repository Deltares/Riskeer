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
using Core.Common.IO.Readers;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class LayerPropertiesTest
    {
        [Test]
        public void LayerProperties_ReaderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new LayerProperties(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("reader", paramName);
        }

        [Test]
        public void LayerProperties_ProfileNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var reader = mockRepository.StrictMock<IRowBasedDatabaseReader>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new LayerProperties(reader, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("profileName", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void LayerProperties_WithReaderAndProfileName_SetProperties()
        {
            // Setup
            var random = new Random(21);

            const double isAquifer = 1.0;
            const string materialName = "materialName";
            double color = random.NextDouble();
            int belowPhreaticLevelDistribution = random.Next(0, 3);
            double belowPhreaticLevelShift = random.NextDouble();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();
            int diameterD70Distribution = random.Next(0, 3);
            double diameterD70Shift = random.NextDouble();
            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();
            int permeabilityDistribution = random.Next(0, 3);
            double permeabilityShift = random.NextDouble();
            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();

            var mockRepository = new MockRepository();
            var reader = mockRepository.StrictMock<IRowBasedDatabaseReader>();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.IsAquifer)).Return(isAquifer);
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileTableDefinitions.MaterialName)).Return(materialName);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.Color)).Return(color);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.BelowPhreaticLevelDistribution)).Return(belowPhreaticLevelDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelShift)).Return(belowPhreaticLevelShift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelMean)).Return(belowPhreaticLevelMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelDeviation)).Return(belowPhreaticLevelDeviation);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.DiameterD70Distribution)).Return(diameterD70Distribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70Shift)).Return(diameterD70Shift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70Mean)).Return(diameterD70Mean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation)).Return(diameterD70CoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.PermeabilityDistribution)).Return(permeabilityDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityShift)).Return(permeabilityShift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityMean)).Return(permeabilityMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation)).Return(permeabilityCoefficientOfVariation);
            mockRepository.ReplayAll();

            // Call
            var properties = new LayerProperties(reader, "");

            // Assert
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(belowPhreaticLevelDistribution, properties.BelowPhreaticLevelDistribution);
            Assert.AreEqual(belowPhreaticLevelShift, properties.BelowPhreaticLevelShift);
            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, properties.BelowPhreaticLevelDeviation);
            Assert.AreEqual(diameterD70Distribution, properties.DiameterD70Distribution);
            Assert.AreEqual(diameterD70Shift, properties.DiameterD70Shift);
            Assert.AreEqual(diameterD70Mean, properties.DiameterD70Mean);
            Assert.AreEqual(diameterD70CoefficientOfVariation, properties.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(permeabilityDistribution, properties.PermeabilityDistribution);
            Assert.AreEqual(permeabilityShift, properties.PermeabilityShift);
            Assert.AreEqual(permeabilityMean, properties.PermeabilityMean);
            Assert.AreEqual(permeabilityCoefficientOfVariation, properties.PermeabilityCoefficientOfVariation);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(LayerProperties))]
        public void LayerProperties_ReaderThrowsInvalidCastException_ThrowsSoilProfileReadException(string columnName)
        {
            // Setup
            const string path = "path";
            const string profileName = "SomeProfile";

            var invalidCastException = new InvalidCastException();

            var mockRepository = new MockRepository();
            var reader = mockRepository.Stub<IRowBasedDatabaseReader>();
            reader.Stub(r => r.ReadOrDefault<double?>(columnName)).Throw(invalidCastException);
            reader.Stub(r => r.ReadOrDefault<long?>(columnName)).Throw(invalidCastException);
            reader.Stub(r => r.ReadOrDefault<string>(columnName)).Throw(invalidCastException);

            reader.Stub(r => r.ReadOrDefault<double?>(columnName)).IgnoreArguments().Return(0);
            reader.Stub(r => r.ReadOrDefault<long?>(columnName)).IgnoreArguments().Return(0);
            reader.Stub(r => r.ReadOrDefault<string>(columnName)).IgnoreArguments().Return("");
            reader.Expect(r => r.Path).Return(path);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new LayerProperties(reader, profileName);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}' (ondergrondschematisatie '{profileName}'): " +
                                     $"ondergrondschematisatie bevat geen geldige waarde in kolom '{columnName}'.";

            var exception = Assert.Throws<SoilProfileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreSame(invalidCastException, exception.InnerException);
            mockRepository.VerifyAll();
        }

        private static IEnumerable<string> LayerProperties()
        {
            yield return SoilProfileTableDefinitions.IsAquifer;
            yield return SoilProfileTableDefinitions.MaterialName;
            yield return SoilProfileTableDefinitions.Color;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelDistribution;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelShift;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelMean;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelDeviation;
            yield return SoilProfileTableDefinitions.DiameterD70Distribution;
            yield return SoilProfileTableDefinitions.DiameterD70Shift;
            yield return SoilProfileTableDefinitions.DiameterD70Mean;
            yield return SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation;
            yield return SoilProfileTableDefinitions.PermeabilityDistribution;
            yield return SoilProfileTableDefinitions.PermeabilityShift;
            yield return SoilProfileTableDefinitions.PermeabilityMean;
            yield return SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation;
        }
    }
}