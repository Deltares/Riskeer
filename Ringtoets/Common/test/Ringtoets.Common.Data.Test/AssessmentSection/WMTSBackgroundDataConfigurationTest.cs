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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class WmtsBackgroundDataConfigurationTest
    {
        [Test]
        [TestCase(null, null, null)]
        [TestCase("", "", "")]
        [TestCase(" ", " ", " ")]
        [TestCase("Value1", "Value2", "Value3")]
        public void DefaultConstructor_Always_ReturnsExpectedProperties(string sourceCapabilitiesUrl,
                                                                        string selectedCapabilityIdentifier,
                                                                        string preferredFormat)
        {
            // Setup
            var random = new Random(21);
            bool isConfigured = random.NextBoolean();

            // Call
            var configuration = new WmtsBackgroundDataConfiguration(isConfigured,
                                                                    sourceCapabilitiesUrl,
                                                                    selectedCapabilityIdentifier,
                                                                    preferredFormat);

            // Assert
            Assert.IsInstanceOf<IBackgroundDataConfiguration>(configuration);

            Assert.AreEqual(isConfigured, configuration.IsConfigured);
            Assert.AreEqual(sourceCapabilitiesUrl, configuration.SourceCapabilitiesUrl);
            Assert.AreEqual(selectedCapabilityIdentifier, configuration.SelectedCapabilityIdentifier);
            Assert.AreEqual(preferredFormat, configuration.PreferredFormat);
        }
    }
}