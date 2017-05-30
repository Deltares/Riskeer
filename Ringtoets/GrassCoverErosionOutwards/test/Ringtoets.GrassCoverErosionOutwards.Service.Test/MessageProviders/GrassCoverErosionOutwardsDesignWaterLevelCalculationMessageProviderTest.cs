﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test.MessageProviders
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var provider = new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider();

            // Assert
            Assert.IsInstanceOf<ICalculationMessageProvider>(provider);
        }

        [Test]
        [TestCase(null, TestName = "GetCalculationName_ExpectedValue(null)")]
        [TestCase("", TestName = "GetCalculationName_ExpectedValue(empty)")]
        [TestCase("value", TestName = "GetCalculationName_ExpectedValue(value)")]
        public void GetCalculationName_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider();

            // Call
            string calculationName = provider.GetCalculationName(name);

            // Assert
            string expectedName = $"Waterstand bij doorsnede-eis voor locatie '{name}'";
            Assert.AreEqual(expectedName, calculationName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetActivityName_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider();

            // Call
            string activityName = provider.GetActivityName(name);

            // Assert
            string expectedName = $"Waterstand bij doorsnede-eis berekenen voor locatie '{name}'";
            Assert.AreEqual(expectedName, activityName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider();
            const string failureMessage = "It failed!";

            // Call
            string message = provider.GetCalculationFailedMessage(name, failureMessage);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de Waterstand bij doorsnede-eis berekening '{name}'. " +
                                     $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{failureMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculatedNotConvergedMessage_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider();

            // Call
            string message = provider.GetCalculatedNotConvergedMessage(name);

            // Assert
            string expectedMessage = $"Waterstand bij doorsnede-eis berekening voor locatie '{name}' is niet geconvergeerd.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null, TestName = "GetCalculationFailedUnexplainedMessage_ExpectedValues(null)")]
        [TestCase("", TestName = "GetCalculationFailedUnexplainedMessage_ExpectedValues(empty)")]
        [TestCase("value", TestName = "GetCalculationFailedUnexplainedMessage_ExpectedValues(value)")]
        public void GetCalculationFailedUnexplainedMessage_ValidNames_ExpectedValues(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider();

            // Call
            string message = provider.GetCalculationFailedUnexplainedMessage(name);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de Waterstand bij doorsnede-eis berekening '{name}'. " +
                                     "Er is geen foutrapport beschikbaar.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}