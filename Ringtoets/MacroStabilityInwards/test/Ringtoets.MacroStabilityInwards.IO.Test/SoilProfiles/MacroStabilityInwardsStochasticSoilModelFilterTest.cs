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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelFilterTest
    {
        [Test]
        public void IsValidForFailureMechanism_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var filter = new MacroStabilityInwardsStochasticSoilModelFilter();

            // Call
            TestDelegate test = () => filter.IsValidForFailureMechanism(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilModel", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(StochasticSoilModelsOfInvalidType))]
        public void IsValidForFailureMechanism_StochasticSoilModelOfInvalidType_ReturnsFalse(StochasticSoilModel model)
        {
            // Setup
            var filter = new MacroStabilityInwardsStochasticSoilModelFilter();

            // Call
            bool isValid = filter.IsValidForFailureMechanism(model);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidForFailureMechanism_ValidStochasticSoilModelType_ReturnsFalse()
        {
            // Setup
            var filter = new MacroStabilityInwardsStochasticSoilModelFilter();
            var model = new StochasticSoilModel(nameof(FailureMechanismType.Stability), FailureMechanismType.Stability);

            // Call
            bool isValid = filter.IsValidForFailureMechanism(model);

            // Assert
            Assert.IsTrue(isValid);
        }

        private static IEnumerable<TestCaseData> StochasticSoilModelsOfInvalidType()
        {
            return Enum.GetValues(typeof(FailureMechanismType))
                       .Cast<FailureMechanismType>()
                       .Where(type => type != FailureMechanismType.Stability)
                       .Select(type => new TestCaseData(
                                       new StochasticSoilModel(type.ToString(), type))
                                   .SetName($"Constructor_InvalidType_ReturnsFalse({type.ToString()})")
                       ).ToArray();
        }
    }
}