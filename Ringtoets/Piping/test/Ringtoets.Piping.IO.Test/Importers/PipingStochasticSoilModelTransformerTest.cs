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
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.IO.Importers;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingStochasticSoilModelTransformerTest
    {
        [Test]
        public void Constructor_ValidProperties_ExpectedValues()
        {
            // Call
            var transformer = new PipingStochasticSoilModelTransformer();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelTransformer<PipingStochasticSoilModel>>(transformer);
        }

        [Test]
        [TestCaseSource(nameof(InvalidFailureMechanismTypes))]
        public void Transform_InvalidFailureMechanismType_ThrowsImportedDataTransformException(FailureMechanismType failureMechanismType)
        {
            // Setup
            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", failureMechanismType);

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Stochastic soil model of failure mechanism type '{failureMechanismType}' is not supported." +
                            $"Only stochastic soil model of failure mechanism type '{FailureMechanismType.Piping}' is supported.", exception.Message);
        }

        private static IEnumerable<FailureMechanismType> InvalidFailureMechanismTypes()
        {
            return Enum.GetValues(typeof(FailureMechanismType))
                       .Cast<FailureMechanismType>()
                       .Where(t => t != FailureMechanismType.Piping);
        }
    }
}