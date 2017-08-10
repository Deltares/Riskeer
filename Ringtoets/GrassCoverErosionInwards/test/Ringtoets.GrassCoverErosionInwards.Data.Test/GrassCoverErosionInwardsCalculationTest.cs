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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using CloneAssert = Core.Common.Data.TestUtil.CloneAssert;
using CommonCloneAssert = Ringtoets.Common.Data.TestUtil.CloneAssert;
using CustomCloneAssert = Ringtoets.GrassCoverErosionInwards.Data.TestUtil.CloneAssert;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.IsInstanceOf<ICloneable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.InputParameters.DikeProfile);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Properties_Name_ReturnsExpectedValues(string name)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            calculation.Name = name;

            // Assert
            Assert.AreEqual(name, calculation.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Property_Comments_ReturnsExpectedValues(string comments)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            calculation.Comments.Body = comments;

            // Assert
            Assert.AreEqual(comments, calculation.Comments.Body);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = null
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(calculationHasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(calculationHasOutput);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = new TestDikeProfile(),
                    Orientation = random.NextRoundedDouble(),
                    DikeHeight = random.NextRoundedDouble(),
                    CriticalFlowRate = new LogNormalDistribution
                    {
                        Mean = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    },
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>(),
                    OvertoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>(),
                    ShouldDikeHeightIllustrationPointsBeCalculated = random.NextBoolean(),
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = random.NextBoolean(),
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = random.NextBoolean(),
                    UseBreakWater = random.NextBoolean(),
                    BreakWater =
                    {
                        Type = random.NextEnumValue<BreakWaterType>(),
                        Height = random.NextRoundedDouble()
                    },
                    UseForeshore = random.NextBoolean()
                },
                Comments =
                {
                    Body = "Random body"
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CloneAssert.AreClones(original, clone, (o, c) =>
            {
                Assert.AreEqual(o.Name, c.Name);
                CloneAssert.AreClones(o.Comments, c.Comments, CommonCloneAssert.AreClones);
                CloneAssert.AreClones(o.InputParameters, c.InputParameters, CustomCloneAssert.AreClones);
                CloneAssert.AreClones(o.Output, c.Output, CustomCloneAssert.AreClones);
            });
        }
    }
}