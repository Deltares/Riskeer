// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.IO.Readers;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class PreconsolidationStressReadValuesTest
    {
        [Test]
        public void PreconsolidationStressReadValues_ReaderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PreconsolidationStressReadValues(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("reader", exception.ParamName);
        }

        [Test]
        public void PreconsolidationStressReadValues_ProfileNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            // Call
            TestDelegate call = () => new PreconsolidationStressReadValues(reader, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("profileName", exception.ParamName);
        }

        [Test]
        public void PreconsolidationStressReadValues_WithReaderAndProfileName_SetProperties()
        {
            // Setup
            var random = new Random(42);
            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            long preconsolidationStressDistributionType = random.Next();
            double preconsolidationStressMean = random.Next();
            double preconsolidationStressCoefficientOfVariation = random.Next();
            double preconsolidationStressShift = random.Next();
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            reader.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressXCoordinate).Returns(xCoordinate);
            reader.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressZCoordinate).Returns(zCoordinate);
            reader.ReadOrDefault<long?>(PreconsolidationStressTableDefinitions.PreconsolidationStressDistributionType).Returns(preconsolidationStressDistributionType);
            reader.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressMean).Returns(preconsolidationStressMean);
            reader.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressCoefficientOfVariation).Returns(preconsolidationStressCoefficientOfVariation);
            reader.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressShift).Returns(preconsolidationStressShift);
            // Call
            var properties = new PreconsolidationStressReadValues(reader, string.Empty);

            // Assert
            Assert.AreEqual(xCoordinate, properties.XCoordinate);
            Assert.AreEqual(zCoordinate, properties.ZCoordinate);
            Assert.AreEqual(preconsolidationStressDistributionType, properties.StressDistributionType);
            Assert.AreEqual(preconsolidationStressMean, properties.StressMean);
            Assert.AreEqual(preconsolidationStressCoefficientOfVariation, properties.StressCoefficientOfVariation);
            Assert.AreEqual(preconsolidationStressShift, properties.StressShift);
        }

        [Test]
        [TestCaseSource(nameof(PreconsolidationStressProperties))]
        public void PreconsolidationStressReadValues_ReaderThrowsInvalidCastException_ThrowsSoilProfileReadException(string columnName)
        {
            // Setup
            const string path = "path";
            const string profileName = "SomeProfile";

            var invalidCastException = new InvalidCastException();
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            reader.ReadOrDefault<double?>(columnName).Returns(_ =>
            {
                throw invalidCastException;
            });
            reader.ReadOrDefault<long?>(columnName).Returns(_ =>
            {
                throw invalidCastException;
            });
            reader.ReadOrDefault<string>(columnName).Returns(_ =>
            {
                throw invalidCastException;
            });

            reader.ReadOrDefault<double?>(Arg.Is<string>(s => s != columnName))
                  .Returns(0);
            reader.ReadOrDefault<long?>(Arg.Is<string>(s => s != columnName))
                  .Returns(0);
            reader.ReadOrDefault<string>(Arg.Is<string>(s => s != columnName))
                  .Returns("");
            reader.Path.Returns(path);
            // Call
            TestDelegate test = () => new PreconsolidationStressReadValues(reader, profileName);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}' (ondergrondschematisatie '{profileName}'): " +
                                     $"ondergrondschematisatie bevat geen geldige waarde in kolom '{columnName}'.";

            var exception = Assert.Throws<SoilProfileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreSame(invalidCastException, exception.InnerException);
        }

        private static IEnumerable<string> PreconsolidationStressProperties()
        {
            yield return PreconsolidationStressTableDefinitions.PreconsolidationStressXCoordinate;
            yield return PreconsolidationStressTableDefinitions.PreconsolidationStressZCoordinate;
            yield return PreconsolidationStressTableDefinitions.PreconsolidationStressDistributionType;
            yield return PreconsolidationStressTableDefinitions.PreconsolidationStressMean;
            yield return PreconsolidationStressTableDefinitions.PreconsolidationStressCoefficientOfVariation;
            yield return PreconsolidationStressTableDefinitions.PreconsolidationStressShift;
        }
    }
}