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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.SurfaceLines;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class SurfaceLinesCsvImporterConfigurationFactoryTest
    {
        [Test]
        public void CreateUpdateStrategyConfiguration_Always_ReturnsUpdateStrategyAndTransformerForPiping()
        {
            // Call
            SurfaceLinesCsvImporterConfiguration<PipingSurfaceLine> result = SurfaceLinesCsvImporterConfigurationFactory.CreateUpdateStrategyConfiguration(new PipingFailureMechanism(), new ReferenceLine());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PipingSurfaceLineTransformer>(result.Transformer);
            Assert.IsInstanceOf<PipingSurfaceLineUpdateDataStrategy>(result.UpdateStrategy);
        }

        [Test]
        public void CreateUpdateStrategyConfiguration_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SurfaceLinesCsvImporterConfigurationFactory.CreateUpdateStrategyConfiguration(null, new ReferenceLine());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateUpdateStrategyConfiguration_WithoutReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SurfaceLinesCsvImporterConfigurationFactory.CreateUpdateStrategyConfiguration(new PipingFailureMechanism(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateReplaceStrategyConfiguration_Always_ReturnsReplaceStrategyAndTransformerForPiping()
        {
            // Call
            SurfaceLinesCsvImporterConfiguration<PipingSurfaceLine> result = SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(new PipingFailureMechanism(), new ReferenceLine());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PipingSurfaceLineTransformer>(result.Transformer);
            Assert.IsInstanceOf<PipingSurfaceLineReplaceDataStrategy>(result.UpdateStrategy);
        }

        [Test]
        public void CreateReplaceStrategyConfiguration_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(null, new ReferenceLine());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateReplaceStrategyConfiguration_WithoutReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(new PipingFailureMechanism(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}