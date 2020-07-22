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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Helpers;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Helpers
{
    [TestFixture]
    public class PersistableStateHelperTest
    {
        [Test]
        public void HasValidStatePoints_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStateHelper.HasValidStatePoints(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void HasValidStatePoints_SoilProfileWithMultiplePreconsolidationStressesOnOneLayer_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1)),
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 2))
            });

            MacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(stochasticSoilProfile.SoilProfile, CreateSurfaceLine());

            // Call
            bool hasValidStatePoints = PersistableStateHelper.HasValidStatePoints(soilProfileUnderSurfaceLine);

            // Assert
            Assert.IsFalse(hasValidStatePoints);
        }

        [Test]
        public void HasValidStatePoints_SoilProfileWithPOPAndPreconsolidationStressOnOneLayer_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1))
            });

            IMacroStabilityInwardsSoilLayer firstLayer = stochasticSoilProfile.SoilProfile.Layers.First();
            firstLayer.Data.UsePop = true;
            firstLayer.Data.Pop = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 2
            };

            MacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(stochasticSoilProfile.SoilProfile,
                                                                                                                                                          CreateSurfaceLine());

            // Call
            bool hasValidStatePoints = PersistableStateHelper.HasValidStatePoints(soilProfileUnderSurfaceLine);

            // Assert
            Assert.IsFalse(hasValidStatePoints);
        }

        [Test]
        public void HasValidStatePoints_WithValidData_ReturnsTrue()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D(new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(new Point2D(2, 1))
            });

            IMacroStabilityInwardsSoilLayer lastLayer = stochasticSoilProfile.SoilProfile.Layers.Last();
            lastLayer.Data.UsePop = true;
            lastLayer.Data.Pop = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 2
            };

            MacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(stochasticSoilProfile.SoilProfile,
                                                                                                                                                          CreateSurfaceLine());

            // Call
            bool hasValidStatePoints = PersistableStateHelper.HasValidStatePoints(soilProfileUnderSurfaceLine);

            // Assert
            Assert.IsTrue(hasValidStatePoints);
        }

        [Test]
        public void HasValidPop_PopNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStateHelper.HasValidPop(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("pop", exception.ParamName);
        }

        [Test]
        public void HasValidPop_MeanNaN_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var pop = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            pop.Mean = RoundedDouble.NaN;
            pop.CoefficientOfVariation = new RoundedDouble(2);

            // Call
            bool hasValidPop = PersistableStateHelper.HasValidPop(pop);

            // Assert
            Assert.IsFalse(hasValidPop);
            mocks.VerifyAll();
        }

        [Test]
        public void HasValidPop_CoefficientOfVariationNaN_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var pop = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            pop.Mean = new RoundedDouble(2);
            pop.CoefficientOfVariation = RoundedDouble.NaN;

            // Call
            bool hasValidPop = PersistableStateHelper.HasValidPop(pop);

            // Assert
            Assert.IsFalse(hasValidPop);
            mocks.VerifyAll();
        }

        [Test]
        public void HasValidPop_DistributionWithValidValues_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var pop = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            pop.Mean = new RoundedDouble(2);
            pop.CoefficientOfVariation = new RoundedDouble(2);

            // Call
            bool hasValidPop = PersistableStateHelper.HasValidPop(pop);

            // Assert
            Assert.IsTrue(hasValidPop);
            mocks.VerifyAll();
        }

        private static MacroStabilityInwardsSurfaceLine CreateSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.1, 0.0, 2),
                new Point3D(0.2, 0.0, 2),
                new Point3D(0.3, 0.0, 3),
                new Point3D(0.4, 0.0, 3),
                new Point3D(0.5, 0.0, 1),
                new Point3D(0.6, 0.0, 1)
            });

            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);

            return surfaceLine;
        }
    }
}