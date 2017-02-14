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
using Core.Common.IO.Readers;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.IO.SoilProfile.Schema;

namespace Ringtoets.Piping.IO.Test.SoilProfile
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
            var mocks = new MockRepository();
            IRowBasedDatabaseReader reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new LayerProperties(reader, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("profileName", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void LayerProperties_WithReaderAndProfileName_SetProperties()
        {
            // Setup
            var random = new Random(21);

            var color = random.NextDouble();
            var belowPhreaticLevelDistribution = random.Next(0, 3);
            var belowPhreaticLevelShift = random.NextDouble();
            var belowPhreaticLevelMean = random.NextDouble();
            var belowPhreaticLevelDeviation = random.NextDouble();
            var diameterD70Distribution = random.Next(0, 3);
            var diameterD70Shift = random.NextDouble();
            var diameterD70Mean = random.NextDouble();
            var diameterD70Deviation = random.NextDouble();
            var permeabilityDistribution = random.Next(0, 3);
            var permeabilityShift = random.NextDouble();
            var permeabilityMean = random.NextDouble();
            var permeabilityDeviation = random.NextDouble();

            var mocks = new MockRepository();
            IRowBasedDatabaseReader reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.IsAquifer)).Return(1.0);
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileTableColumns.MaterialName)).Return("");
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.Color)).Return(color);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.BelowPhreaticLevelDistribution)).Return(belowPhreaticLevelDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelShift)).Return(belowPhreaticLevelShift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelMean)).Return(belowPhreaticLevelMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.BelowPhreaticLevelDeviation)).Return(belowPhreaticLevelDeviation);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.DiameterD70Distribution)).Return(diameterD70Distribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Shift)).Return(diameterD70Shift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Mean)).Return(diameterD70Mean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.DiameterD70Deviation)).Return(diameterD70Deviation);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableColumns.PermeabilityDistribution)).Return(permeabilityDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityShift)).Return(permeabilityShift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityMean)).Return(permeabilityMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.PermeabilityDeviation)).Return(permeabilityDeviation);
            mocks.ReplayAll();

            // Call
            var properties = new LayerProperties(reader, "");

            // Assert
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(belowPhreaticLevelDistribution, properties.BelowPhreaticLevelDistribution);
            Assert.AreEqual(belowPhreaticLevelShift, properties.BelowPhreaticLevelShift);
            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, properties.BelowPhreaticLevelDeviation);
            Assert.AreEqual(diameterD70Distribution, properties.DiameterD70Distribution);
            Assert.AreEqual(diameterD70Shift, properties.DiameterD70Shift);
            Assert.AreEqual(diameterD70Mean, properties.DiameterD70Mean);
            Assert.AreEqual(diameterD70Deviation, properties.DiameterD70Deviation);
            Assert.AreEqual(permeabilityDistribution, properties.PermeabilityDistribution);
            Assert.AreEqual(permeabilityShift, properties.PermeabilityShift);
            Assert.AreEqual(permeabilityMean, properties.PermeabilityMean);
            Assert.AreEqual(permeabilityDeviation, properties.PermeabilityDeviation);
            mocks.VerifyAll();
        }

        [Test]
        public void LayerProperties_ReaderThrowsInvalidCastException_ThrowsPipingSoilProfileReadException()
        {
            // Setup
            var path = "path";
            var profileName = "SomeProfile";

            var mocks = new MockRepository();
            IRowBasedDatabaseReader reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableColumns.IsAquifer)).Throw(new InvalidCastException());
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new LayerProperties(reader, profileName);

            // Assert
            var expectedMessage = string.Format(
                "Fout bij het lezen van bestand '{0}' (ondergrondschematisatie '{1}'): ondergrondschematisatie bevat geen geldige waarde in kolom 'IsAquifer'.",
                path,
                profileName);

            PipingSoilProfileReadException exception = Assert.Throws<PipingSoilProfileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(profileName, exception.ProfileName);
            mocks.VerifyAll();
        }
    }
}