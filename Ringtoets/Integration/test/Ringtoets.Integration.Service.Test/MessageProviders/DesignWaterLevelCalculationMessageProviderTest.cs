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

using NUnit.Framework;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Properties;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Service.Test.MessageProviders
{
    [TestFixture]
    public class DesignWaterLevelCalculationMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var provider = new DesignWaterLevelCalculationMessageProvider();

            // Assert
            Assert.IsInstanceOf<ICalculationMessageProvider>(provider);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationName_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new DesignWaterLevelCalculationMessageProvider();

            // Call
            var calculationName = provider.GetCalculationName(name);

            // Assert
            var expectedName = string.Format(Resources.DesignWaterLevelCalculationService_Name_Assessment_level_for_location_0_, name);
            Assert.AreEqual(expectedName, calculationName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetActivityName_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new DesignWaterLevelCalculationMessageProvider();

            // Call
            var activityName = provider.GetActivityName(name);

            // Assert
            var expectedName = string.Format(Resources.DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_location_0_, name);
            Assert.AreEqual(expectedName, activityName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new DesignWaterLevelCalculationMessageProvider();

            // Call
            var message = provider.GetCalculationFailedMessage(name);

            // Assert
            var expectedMessage = string.Format(Resources.DesignWaterLevelCalculationService_Calculate_Error_in_design_water_level_0_calculation, name);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculatedNotConvergedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new DesignWaterLevelCalculationMessageProvider();

            // Call
            var message = provider.GetCalculatedNotConvergedMessage(name);

            // Assert
            var expectedMessage = string.Format(Resources.DesignWaterLevelCalculationActivity_DesignWaterLevel_calculation_for_location_0_not_converged, name);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}