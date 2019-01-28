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

using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="BackgroundData"/>.
    /// </summary>
    public static class BackgroundDataTestHelper
    {
        /// <summary>
        /// Assert the background data.
        /// </summary>
        /// <param name="expectedBackgroundData">The background data with the expected properties.</param>
        /// <param name="actualBackgroundData">The background data to assert.</param>
        /// <exception cref="AssertionException">Thrown when the properties of the <paramref name="actualBackgroundData"/>
        /// are not equal to the properties of <paramref name="expectedBackgroundData"/> or when 
        /// the set <see cref="BackgroundData.Configuration"/> is not supported.</exception>
        public static void AssertBackgroundData(BackgroundData expectedBackgroundData, BackgroundData actualBackgroundData)
        {
            Assert.AreEqual(expectedBackgroundData.Name, actualBackgroundData.Name);
            Assert.AreEqual(expectedBackgroundData.IsVisible, actualBackgroundData.IsVisible);
            Assert.AreEqual(expectedBackgroundData.Transparency, actualBackgroundData.Transparency);

            IBackgroundDataConfiguration backgroundDataConfiguration = expectedBackgroundData.Configuration;
            var wmtsBackgroundDataConfiguration = backgroundDataConfiguration as WmtsBackgroundDataConfiguration;
            var wellKnownBackgroundDataConfiguration = backgroundDataConfiguration as WellKnownBackgroundDataConfiguration;

            if (wmtsBackgroundDataConfiguration != null)
            {
                var actualWmtsBackgroundDataConfiguration = (WmtsBackgroundDataConfiguration) actualBackgroundData.Configuration;
                AssertWmtsBackgroundConfiguration(wmtsBackgroundDataConfiguration, actualWmtsBackgroundDataConfiguration);
                return;
            }

            if (wellKnownBackgroundDataConfiguration != null)
            {
                var actualWellKnownBackgroundDataConfiguration = (WellKnownBackgroundDataConfiguration) actualBackgroundData.Configuration;
                AssertWellKnownBackgroundConfiguration(wellKnownBackgroundDataConfiguration, actualWellKnownBackgroundDataConfiguration);
                return;
            }

            Assert.Fail($"Unsupported type of {nameof(IBackgroundDataConfiguration)} in {expectedBackgroundData.Configuration}");
        }

        private static void AssertWellKnownBackgroundConfiguration(WellKnownBackgroundDataConfiguration wellKnownBackgroundDataConfiguration,
                                                                   WellKnownBackgroundDataConfiguration actualWellKnownBackgroundDataConfiguration)
        {
            Assert.AreEqual(wellKnownBackgroundDataConfiguration.WellKnownTileSource, actualWellKnownBackgroundDataConfiguration.WellKnownTileSource);
        }

        private static void AssertWmtsBackgroundConfiguration(WmtsBackgroundDataConfiguration expectedWmtsBackgroundDataConfiguration,
                                                              WmtsBackgroundDataConfiguration actualWmtsBackgroundDataConfiguration)
        {
            Assert.AreEqual(expectedWmtsBackgroundDataConfiguration.IsConfigured, actualWmtsBackgroundDataConfiguration.IsConfigured);
            Assert.AreEqual(expectedWmtsBackgroundDataConfiguration.SourceCapabilitiesUrl, actualWmtsBackgroundDataConfiguration.SourceCapabilitiesUrl);
            Assert.AreEqual(expectedWmtsBackgroundDataConfiguration.SelectedCapabilityIdentifier, actualWmtsBackgroundDataConfiguration.SelectedCapabilityIdentifier);
            Assert.AreEqual(expectedWmtsBackgroundDataConfiguration.PreferredFormat, actualWmtsBackgroundDataConfiguration.PreferredFormat);
        }
    }
}