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

using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.Test.SoilProfile.Schema
{
    [TestFixture]
    public class SoilProfileTableDefinitionsTest
    {
        [Test]
        public void Constants_Always_ExpectedValues()
        {
            Assert.AreEqual("SoilProfileId", SoilProfileTableDefinitions.SoilProfileId);
            Assert.AreEqual("IsAquifer", SoilProfileTableDefinitions.IsAquifer);
            Assert.AreEqual("ProfileName", SoilProfileTableDefinitions.ProfileName);
            Assert.AreEqual("IntersectionX", SoilProfileTableDefinitions.IntersectionX);
            Assert.AreEqual("Bottom", SoilProfileTableDefinitions.Bottom);
            Assert.AreEqual("Top", SoilProfileTableDefinitions.Top);
            Assert.AreEqual("Color", SoilProfileTableDefinitions.Color);
            Assert.AreEqual("MaterialName", SoilProfileTableDefinitions.MaterialName);
            Assert.AreEqual("LayerGeometry", SoilProfileTableDefinitions.LayerGeometry);
            Assert.AreEqual("BelowPhreaticLevelDistributionType", SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType);
            Assert.AreEqual("BelowPhreaticLevelShift", SoilProfileTableDefinitions.BelowPhreaticLevelShift);
            Assert.AreEqual("BelowPhreaticLevelMean", SoilProfileTableDefinitions.BelowPhreaticLevelMean);
            Assert.AreEqual("BelowPhreaticLevelDeviation", SoilProfileTableDefinitions.BelowPhreaticLevelDeviation);
            Assert.AreEqual("PermeabKxDistributionType", SoilProfileTableDefinitions.PermeabilityDistributionType);
            Assert.AreEqual("PermeabKxShift", SoilProfileTableDefinitions.PermeabilityShift);
            Assert.AreEqual("PermeabKxMean", SoilProfileTableDefinitions.PermeabilityMean);
            Assert.AreEqual("PermeabKxCoefficientOfVariation", SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation);
            Assert.AreEqual("DiameterD70DistributionType", SoilProfileTableDefinitions.DiameterD70DistributionType);
            Assert.AreEqual("DiameterD70Shift", SoilProfileTableDefinitions.DiameterD70Shift);
            Assert.AreEqual("DiameterD70Mean", SoilProfileTableDefinitions.DiameterD70Mean);
            Assert.AreEqual("DiameterD70CoefficientOfVariation", SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation);
            Assert.AreEqual("LayerCount", SoilProfileTableDefinitions.LayerCount);
            Assert.AreEqual("UsePOP", SoilProfileTableDefinitions.UsePop);
            Assert.AreEqual("ShearStrengthModel", SoilProfileTableDefinitions.ShearStrengthModel);
            Assert.AreEqual("AbovePhreaticLevelDistributionType", SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType);
            Assert.AreEqual("AbovePhreaticLevelShift", SoilProfileTableDefinitions.AbovePhreaticLevelShift);
            Assert.AreEqual("AbovePhreaticLevelMean", SoilProfileTableDefinitions.AbovePhreaticLevelMean);
            Assert.AreEqual("AbovePhreaticLevelCoefficientOfVariation", SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual("CohesionDistributionType", SoilProfileTableDefinitions.CohesionDistributionType);
            Assert.AreEqual("CohesionShift", SoilProfileTableDefinitions.CohesionShift);
            Assert.AreEqual("CohesionMean", SoilProfileTableDefinitions.CohesionMean);
            Assert.AreEqual("CohesionCoefficientOfVariation", SoilProfileTableDefinitions.CohesionCoefficientOfVariation);
            Assert.AreEqual("FrictionAngleDistributionType", SoilProfileTableDefinitions.FrictionAngleDistributionType);
            Assert.AreEqual("FrictionAngleShift", SoilProfileTableDefinitions.FrictionAngleShift);
            Assert.AreEqual("FrictionAngleMean", SoilProfileTableDefinitions.FrictionAngleMean);
            Assert.AreEqual("FrictionAngleCoefficientOfVariation", SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation);
            Assert.AreEqual("ShearStrengthRatioDistributionType", SoilProfileTableDefinitions.ShearStrengthRatioDistributionType);
            Assert.AreEqual("ShearStrengthRatioShift", SoilProfileTableDefinitions.ShearStrengthRatioShift);
            Assert.AreEqual("ShearStrengthRatioMean", SoilProfileTableDefinitions.ShearStrengthRatioMean);
            Assert.AreEqual("ShearStrengthRatioCoefficientOfVariation", SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation);
            Assert.AreEqual("StrengthIncreaseExponentDistributionType", SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType);
            Assert.AreEqual("StrengthIncreaseExponentShift", SoilProfileTableDefinitions.StrengthIncreaseExponentShift);
            Assert.AreEqual("StrengthIncreaseExponentMean", SoilProfileTableDefinitions.StrengthIncreaseExponentMean);
            Assert.AreEqual("StrengthIncreaseExponentCoefficientOfVariation", SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.AreEqual("PopDistributionType", SoilProfileTableDefinitions.PopDistributionType);
            Assert.AreEqual("PopShift", SoilProfileTableDefinitions.PopShift);
            Assert.AreEqual("PopMean", SoilProfileTableDefinitions.PopMean);
            Assert.AreEqual("PopCoefficientOfVariation", SoilProfileTableDefinitions.PopCoefficientOfVariation);
        }
    }
}