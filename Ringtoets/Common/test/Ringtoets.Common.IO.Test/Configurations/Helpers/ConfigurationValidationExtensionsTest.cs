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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Ringtoets.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationValidationExtensionsTest
    {
        [Test]
        public void ValidateWaveReduction_NoLog_ThrowsArgumentNullException()
        {
            // Setup
            const string calculationName = "calculation";

            // Call
            TestDelegate test = () => ((WaveReductionConfiguration) null).ValidateWaveReduction(null, calculationName, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("log", exception.ParamName);
        }

        [Test]
        public void ValidateWaveReduction_NoCalculationName_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => ((WaveReductionConfiguration) null).ValidateWaveReduction(null, null, log);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationName", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateWaveReduction_NoForeshoreProfileNoParameters_ReturnsTrue()
        {
            // Setup
            const string calculationName = "calculation";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            // Call
            bool valid = ((WaveReductionConfiguration) null).ValidateWaveReduction(null, calculationName, log);

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateWaveReduction_NoForeshoreProfileWaveReductionWithoutParameters_ReturnsTrue()
        {
            // Setup
            const string calculationName = "calculation";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            // Call
            bool valid = new WaveReductionConfiguration().ValidateWaveReduction(null, calculationName, log);

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateWaveReduction_NoForeshoreProfileWaveReductionWithParameter_LogsErrorReturnsFalse([Values(0, 1, 2, 3)] int propertyToSet)
        {
            // Setup
            const string calculationName = "calculation";
            const string error = "Er is geen voorlandprofiel opgegeven om golfreductie parameters aan toe te voegen.";
            const string expectedMessage = "{0} Berekening '{1}' is overgeslagen.";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            log.Expect(l => l.ErrorFormat(expectedMessage, error, calculationName));
            mocks.ReplayAll();

            var waveReductionConfiguration = new WaveReductionConfiguration();
            var random = new Random(21);

            switch (propertyToSet)
            {
                case 0:
                    waveReductionConfiguration.BreakWaterType = random.NextEnumValue<ConfigurationBreakWaterType>();
                    break;
                case 1:
                    waveReductionConfiguration.BreakWaterHeight = random.NextDouble();
                    break;
                case 2:
                    waveReductionConfiguration.UseBreakWater = random.NextBoolean();
                    break;
                case 3:
                    waveReductionConfiguration.UseForeshoreProfile = random.NextBoolean();
                    break;
            }

            // Call
            bool valid = waveReductionConfiguration.ValidateWaveReduction(null, calculationName, log);

            // Assert
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateWaveReduction_ForeshoreProfileWithoutGeometryNoWaveReduction_ReturnsTrue()
        {
            // Setup
            const string calculationName = "calculation";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            // Call
            bool valid = ((WaveReductionConfiguration) null).ValidateWaveReduction(
                new TestForeshoreProfile("voorland", Enumerable.Empty<Point2D>()),
                calculationName,
                log);

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateWaveReduction_ForeshoreProfileWithGeometryForeshoreProfileUsed_ReturnsTrue()
        {
            // Setup
            const string calculationName = "calculation";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            var waveReductionConfiguration = new WaveReductionConfiguration
            {
                UseForeshoreProfile = true
            };

            // Call
            bool valid = waveReductionConfiguration.ValidateWaveReduction(new TestForeshoreProfile("voorland", new[]
            {
                new Point2D(0, 2)
            }), calculationName, log);

            // Assert
            Assert.IsTrue(valid);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateWaveReduction_ForeshoreProfileWithoutGeometryForeshoreProfileUsed_LogsErrorReturnsFalse()
        {
            // Setup
            const string profileName = "voorland";
            const string calculationName = "calculation";
            const string expectedMessage = "{0} Berekening '{1}' is overgeslagen.";

            string error = $"Het opgegeven voorlandprofiel '{profileName}' heeft geen voorlandgeometrie en kan daarom niet gebruikt worden.";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            log.Expect(l => l.ErrorFormat(expectedMessage, error, calculationName));
            mocks.ReplayAll();

            var waveReductionConfiguration = new WaveReductionConfiguration
            {
                UseForeshoreProfile = true
            };

            // Call
            bool valid = waveReductionConfiguration.ValidateWaveReduction(new TestForeshoreProfile(profileName), calculationName, log);

            // Assert
            Assert.IsFalse(valid);
            mocks.VerifyAll();
        }
    }
}