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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Service.Converters;
using Ringtoets.MacroStabilityInwards.Service.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class UpliftVanSoilProfileConverterTest
    {
        [Test]
        public void Convert_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanSoilProfileConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Convert_WithSoilProfile_ReturnUpliftVanSoilProfile()
        {
            // Setup
            var random = new Random(22);

            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(CreateRing(21), new List<Point2D[]>
                {
                    CreateRing(11),
                    CreateRing(22)
                }, new MacroStabilityInwardsSoilLayerProperties
                {
                    UsePop = random.NextBoolean(),
                    IsAquifer = random.NextBoolean(),
                    MaterialName = "Test",
                    AbovePhreaticLevel =
                    {
                        Mean = (RoundedDouble) 10,
                        CoefficientOfVariation = (RoundedDouble) 0.3,
                        Shift = (RoundedDouble) 0.1
                    },
                    BelowPhreaticLevel =
                    {
                        Mean = (RoundedDouble) 5,
                        CoefficientOfVariation = (RoundedDouble) 0.8,
                        Shift = (RoundedDouble) 0.3
                    },
                    Cohesion =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    FrictionAngle =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    StrengthIncreaseExponent =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    ShearStrengthRatio =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    },
                    Pop =
                    {
                        Mean = random.NextRoundedDouble(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    }
                })
            }, new[]
            {
                new MacroStabilityInwardsPreconsolidationStress(random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble())
            });

            // Call
            UpliftVanSoilProfile upliftVanSoilProfile = UpliftVanSoilProfileConverter.Convert(profile);

            // Assert
            UpliftVanSoilProfileHelper.AssertSoilProfile(profile, upliftVanSoilProfile);
        }

        [Test]
        public void Convert_InvalidShearStrengthModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(CreateRing(21), new Point2D[0][],
                                                                   new MacroStabilityInwardsSoilLayerProperties
                                                                   {
                                                                       ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 99
                                                                   })
            }, new MacroStabilityInwardsPreconsolidationStress[0]);

            // Call
            TestDelegate test = () => UpliftVanSoilProfileConverter.Convert(profile);

            // Assert
            const string message = "The value of argument 'shearStrengthModel' (99) is invalid for Enum type 'MacroStabilityInwardsShearStrengthModel'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi, UpliftVanShearStrengthModel.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated, UpliftVanShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated, UpliftVanShearStrengthModel.SuCalculated)]
        public void Convert_ValidShearStrengthModel_ReturnExpectedUpliftVanShearStrengthModel(MacroStabilityInwardsShearStrengthModel originalShearStrengthModel,
                                                                                              UpliftVanShearStrengthModel expectedShearStrengthModel)
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(CreateRing(21), new Point2D[0][],
                                                                   new MacroStabilityInwardsSoilLayerProperties
                                                                   {
                                                                       ShearStrengthModel = originalShearStrengthModel
                                                                   })
            }, new MacroStabilityInwardsPreconsolidationStress[0]);

            // Call
            UpliftVanSoilProfile upliftVanSoilProfile = UpliftVanSoilProfileConverter.Convert(profile);

            // Assert
            Assert.AreEqual(expectedShearStrengthModel, upliftVanSoilProfile.Layers.First().ShearStrengthModel);
        }

        private static Point2D[] CreateRing(int seed)
        {
            var random = new Random(seed);
            int x1 = random.Next();
            int y1 = random.Next();
            int x2 = x1;
            int y2 = y1 + random.Next();
            int x3 = x2 + random.Next();
            int y3 = y2;
            double x4 = x1 + (x3 - x1) * random.NextDouble();
            int y4 = y1;

            return new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2),
                new Point2D(x3, y3),
                new Point2D(x4, y4)
            };
        }
    }
}