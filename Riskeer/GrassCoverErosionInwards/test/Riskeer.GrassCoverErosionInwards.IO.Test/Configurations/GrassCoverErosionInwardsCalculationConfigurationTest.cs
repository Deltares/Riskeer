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

using System;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.GrassCoverErosionInwards.IO.Configurations;

namespace Riskeer.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Call);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var readCalculation = new GrassCoverErosionInwardsCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.AreEqual(name, readCalculation.Name);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocationName);
            Assert.IsNull(readCalculation.DikeProfileId);
            Assert.IsNull(readCalculation.Orientation);
            Assert.IsNull(readCalculation.DikeHeight);
            Assert.IsNull(readCalculation.DikeHeightCalculationType);
            Assert.IsNull(readCalculation.OvertoppingRateCalculationType);
            Assert.IsNull(readCalculation.WaveReduction);
            Assert.IsNull(readCalculation.CriticalFlowRate);
            Assert.IsNull(readCalculation.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsNull(readCalculation.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsNull(readCalculation.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            Assert.IsNull(readCalculation.Scenario);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new GrassCoverErosionInwardsCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}