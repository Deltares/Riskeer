// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
            var mockRepository = new MockRepository();
            var reader = mockRepository.Stub<IRowBasedDatabaseReader>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new PreconsolidationStressReadValues(reader, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("profileName", exception.ParamName);

            mockRepository.VerifyAll();
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

            var mockRepository = new MockRepository();
            var reader = mockRepository.StrictMock<IRowBasedDatabaseReader>();
            reader.Expect(r => r.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressXCoordinate)).Return(xCoordinate);
            reader.Expect(r => r.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressZCoordinate)).Return(zCoordinate);
            reader.Expect(r => r.ReadOrDefault<long?>(PreconsolidationStressTableDefinitions.PreconsolidationStressDistributionType)).Return(preconsolidationStressDistributionType);
            reader.Expect(r => r.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressMean)).Return(preconsolidationStressMean);
            reader.Expect(r => r.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressCoefficientOfVariation)).Return(preconsolidationStressCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(PreconsolidationStressTableDefinitions.PreconsolidationStressShift)).Return(preconsolidationStressShift);
            mockRepository.ReplayAll();

            // Call
            var properties = new PreconsolidationStressReadValues(reader, string.Empty);

            // Assert
            Assert.AreEqual(xCoordinate, properties.XCoordinate);
            Assert.AreEqual(zCoordinate, properties.ZCoordinate);
            Assert.AreEqual(preconsolidationStressDistributionType, properties.StressDistributionType);
            Assert.AreEqual(preconsolidationStressMean, properties.StressMean);
            Assert.AreEqual(preconsolidationStressCoefficientOfVariation, properties.StressCoefficientOfVariation);
            Assert.AreEqual(preconsolidationStressShift, properties.StressShift);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(PreconsolidationStressProperties))]
        public void PreconsolidationStressReadValues_ReaderThrowsInvalidCastException_ThrowsSoilProfileReadException(string columnName)
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

            reader.Stub(r => r.ReadOrDefault<double?>(Arg<string>.Matches(s => s != columnName)))
                  .Return(0);
            reader.Stub(r => r.ReadOrDefault<long?>(Arg<string>.Matches(s => s != columnName)))
                  .Return(0);
            reader.Stub(r => r.ReadOrDefault<string>(Arg<string>.Matches(s => s != columnName)))
                  .Return("");
            reader.Expect(r => r.Path).Return(path);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new PreconsolidationStressReadValues(reader, profileName);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}' (ondergrondschematisatie '{profileName}'): " +
                                     $"ondergrondschematisatie bevat geen geldige waarde in kolom '{columnName}'.";

            var exception = Assert.Throws<SoilProfileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreSame(invalidCastException, exception.InnerException);
            mockRepository.VerifyAll();
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