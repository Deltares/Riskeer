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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationHelperTest
    {
        [Test]
        public void GenerateCalculations_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsCalculationConfigurationHelper.GenerateCalculations(null, Enumerable.Empty<DikeProfile>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        public void GenerateCalculations_StructuresNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsCalculationConfigurationHelper.GenerateCalculations(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dikeProfiles", exception.ParamName);
        }

        [Test]
        public void GenerateCalculations_Always_SetsCorrectCalculations()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0), "profiel 1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0), "profiel 2");

            // Call
            GrassCoverErosionInwardsCalculationConfigurationHelper.GenerateCalculations(calculationGroup, new[]
            {
                dikeProfile1,
                dikeProfile2
            });

            // Assert
            Assert.AreEqual(2, calculationGroup.Children.Count);

            var calculation1 = (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children.First();
            Assert.AreEqual("name", calculation1.Name);
            Assert.AreEqual(dikeProfile1, calculation1.InputParameters.DikeProfile);

            var calculation2 = (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children.Last();
            Assert.AreEqual("name (1)", calculation2.Name);
            Assert.AreEqual(dikeProfile2, calculation2.InputParameters.DikeProfile);
        }
    }
}