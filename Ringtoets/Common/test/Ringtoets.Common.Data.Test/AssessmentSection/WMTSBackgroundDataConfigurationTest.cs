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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class WmtsBackgroundDataConfigurationTest
    {
        [Test]
        public void DefaultConstructor_Always_ReturnsExpectedProperties()
        {
            // Call
            var configuration = new WmtsBackgroundDataConfiguration();

            // Assert
            Assert.IsInstanceOf<IBackgroundDataConfiguration>(configuration);

            Assert.IsFalse(configuration.IsConfigured);
            Assert.IsNull(configuration.SourceCapabilitiesUrl);
            Assert.IsNull(configuration.SelectedCapabilityIdentifier);
            Assert.IsNull(configuration.PreferredFormat);
        }

        [Test]
        [TestCase("", "", "")]
        [TestCase(" ", " ", " ")]
        [TestCase("Value1", "Value2", "Value3")]
        public void ParameteredConstructor_IsConfiguredTrueAndValidParameters_ReturnsExpectedProperties(string sourceCapabilitiesUrl,
                                                                                                        string selectedCapabilityIdentifier,
                                                                                                        string preferredFormat)
        {
            // Call
            var configuration = new WmtsBackgroundDataConfiguration(true,
                                                                    sourceCapabilitiesUrl,
                                                                    selectedCapabilityIdentifier,
                                                                    preferredFormat);

            // Assert
            Assert.IsInstanceOf<IBackgroundDataConfiguration>(configuration);

            Assert.IsTrue(configuration.IsConfigured);
            Assert.AreEqual(sourceCapabilitiesUrl, configuration.SourceCapabilitiesUrl);
            Assert.AreEqual(selectedCapabilityIdentifier, configuration.SelectedCapabilityIdentifier);
            Assert.AreEqual(preferredFormat, configuration.PreferredFormat);
        }

        [Test]
        public void ParameteredConstructor_IsConfiguredFalseAndValidParameters_ReturnsExpectedProperties()
        {
            // Call
            var configuration = new WmtsBackgroundDataConfiguration(false,
                                                                    null,
                                                                    null,
                                                                    null);

            // Assert
            Assert.IsInstanceOf<IBackgroundDataConfiguration>(configuration);

            Assert.IsFalse(configuration.IsConfigured);
            Assert.IsNull(configuration.SourceCapabilitiesUrl);
            Assert.IsNull(configuration.SelectedCapabilityIdentifier);
            Assert.IsNull(configuration.PreferredFormat);
        }

        [Test]
        public void Constructor_IsConfiguredTrueAndSourceCapabilitiesUrlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsBackgroundDataConfiguration(true,
                                                                          null,
                                                                          string.Empty,
                                                                          string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceCapabilitiesUrl", exception.ParamName);
        }

        [Test]
        public void Constructor_IsConfiguredTrueAndSelectedCapabilityIdentifierNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsBackgroundDataConfiguration(true,
                                                                          string.Empty,
                                                                          null,
                                                                          string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("selectedCapabilityIdentifier", exception.ParamName);
        }

        [Test]
        public void Constructor_IsConfiguredTrueAndPreferredFormatNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsBackgroundDataConfiguration(true,
                                                                          string.Empty,
                                                                          string.Empty,
                                                                          null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preferredFormat", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_IsConfiguredFalseAndSourceCapabilitiesUrlHasValue_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new WmtsBackgroundDataConfiguration(false,
                                                                          string.Empty,
                                                                          null,
                                                                          null);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                call, "Value must be null when instantiating an unconfigured configuration.");
            Assert.AreEqual("sourceCapabilitiesUrl", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_IsConfiguredFalseAndSelectedCapabilityIdentifierHasValue_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new WmtsBackgroundDataConfiguration(false,
                                                                          null,
                                                                          string.Empty,
                                                                          null);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                call, "Value must be null when instantiating an unconfigured configuration.");
            Assert.AreEqual("selectedCapabilityIdentifier", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_IsConfiguredFalseAndPreferredFormatHasValue_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new WmtsBackgroundDataConfiguration(false,
                                                                          null,
                                                                          null,
                                                                          string.Empty);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                call, "Value must be null when instantiating an unconfigured configuration.");
            Assert.AreEqual("preferredFormat", exception.ParamName);
        }
    }
}