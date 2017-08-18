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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelTransformerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelTransformer<MacroStabilityInwardsStochasticSoilModel>>(transformer);
        }

        [Test]
        public void Transform_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            TestDelegate call = () => transformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilModel", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidFailureMechanismTypes))]
        public void Transform_InvalidFailureMechanismType_ThrowsImportedDataTransformException(FailureMechanismType failureMechanismType)
        {
            // Setup
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", failureMechanismType);

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Het stochastische ondergrondmodel met '{failureMechanismType}' als faalmechanisme type is niet ondersteund. " +
                            "Alleen stochastische ondergrondmodellen met 'Stability' als faalmechanisme type zijn ondersteund.", exception.Message);
        }
        private static IEnumerable<FailureMechanismType> GetInvalidFailureMechanismTypes()
        {
            return Enum.GetValues(typeof(FailureMechanismType))
                       .Cast<FailureMechanismType>()
                       .Where(t => t != FailureMechanismType.Stability);
        }
    }
}