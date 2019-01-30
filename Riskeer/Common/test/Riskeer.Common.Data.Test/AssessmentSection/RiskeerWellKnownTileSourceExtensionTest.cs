// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class RiskeerWellKnownTileSourceExtensionTest
    {
        [Test]
        [TestCaseSource(nameof(GetValidRiskeerWellKnownTileSources))]
        public void GetDisplayName_ValidEnum_ReturnsExpectedDislayName(RiskeerWellKnownTileSource riskeerWellKnownTileSource,
                                                                       string expectedDisplayName)
        {
            // Call
            string displayName = riskeerWellKnownTileSource.GetDisplayName();

            // Assert
            Assert.AreEqual(expectedDisplayName, displayName);
        }

        [Test]
        public void GetDisplayName_InvalidEnum_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;
            const RiskeerWellKnownTileSource wellKnownTileSource = (RiskeerWellKnownTileSource) invalidValue;

            // Call
            TestDelegate call = () => wellKnownTileSource.GetDisplayName();

            // Assert
            string expectedMessage = $"The value of argument 'riskeerWellKnownTileSource' ({invalidValue}) is invalid for Enum type '{nameof(RiskeerWellKnownTileSource)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("riskeerWellKnownTileSource", parameterName);
        }

        private static IEnumerable<TestCaseData> GetValidRiskeerWellKnownTileSources()
        {
            yield return new TestCaseData(RiskeerWellKnownTileSource.OpenStreetMap, "OpenStreetMap");
            yield return new TestCaseData(RiskeerWellKnownTileSource.BingAerial, "Bing Maps - Satelliet");
            yield return new TestCaseData(RiskeerWellKnownTileSource.BingHybrid, "Bing Maps - Satelliet + Wegen");
            yield return new TestCaseData(RiskeerWellKnownTileSource.BingRoads, "Bing Maps - Wegen");
            yield return new TestCaseData(RiskeerWellKnownTileSource.EsriWorldTopo, "Esri World - Topografisch");
            yield return new TestCaseData(RiskeerWellKnownTileSource.EsriWorldShadedRelief, "Esri World - Reliëf");
        }
    }
}