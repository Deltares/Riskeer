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
using NUnit.Framework;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.IO.SoilProfiles;
using Riskeer.Piping.Plugin.FileImporter;

namespace Riskeer.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingStochasticSoilModelImporterConfigurationFactoryTest
    {
        [Test]
        public void CreateUpdateStrategyConfiguration_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingStochasticSoilModelImporterConfigurationFactory.CreateUpdateStrategyConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateUpdateStrategyConfiguration_ValidArgument_ReturnsUpdateStrategyAndTransformerForPiping()
        {
            // Call
            StochasticSoilModelImporterConfiguration<PipingStochasticSoilModel> result =
                PipingStochasticSoilModelImporterConfigurationFactory.CreateUpdateStrategyConfiguration(new PipingFailureMechanism());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PipingStochasticSoilModelTransformer>(result.Transformer);
            Assert.IsInstanceOf<PipingStochasticSoilModelFilter>(result.MechanismFilter);
            Assert.IsInstanceOf<PipingStochasticSoilModelUpdateDataStrategy>(result.UpdateStrategy);
        }

        [Test]
        public void CreateReplaceStrategyConfiguration_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateReplaceStrategyConfiguration_ValidArgument_ReturnsReplaceStrategyAndTransformerForPiping()
        {
            // Call
            StochasticSoilModelImporterConfiguration<PipingStochasticSoilModel> result =
                PipingStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(new PipingFailureMechanism());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PipingStochasticSoilModelTransformer>(result.Transformer);
            Assert.IsInstanceOf<PipingStochasticSoilModelFilter>(result.MechanismFilter);
            Assert.IsInstanceOf<PipingStochasticSoilModelReplaceDataStrategy>(result.UpdateStrategy);
        }
    }
}