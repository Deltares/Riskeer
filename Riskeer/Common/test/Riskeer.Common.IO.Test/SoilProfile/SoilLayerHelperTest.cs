// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.IO.Readers;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerHelperTest
    {
        [Test]
        public void SetSoilLayerBaseProperties_SoilLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            var properties = new LayerProperties(reader, "");

            // Call
            TestDelegate call = () => SoilLayerHelper.SetSoilLayerBaseProperties(null, properties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void SetSoilLayerBaseProperties_LayerPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var soilLayer = new TestSoilLayerBase();

            // Call
            TestDelegate call = () => SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void SetSoilLayerBaseProperties_LayerPropertiesWithValues_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            double isAquifer = random.NextDouble();
            const string materialName = "materialName";
            double color = random.NextDouble();

            int belowPhreaticLevelDistributionType = random.Next();
            double belowPhreaticLevelShift = random.NextDouble();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();

            int diameterD70DistributionType = random.Next();
            double diameterD70Shift = random.NextDouble();
            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();

            int permeabilityDistributionType = random.Next();
            double permeabilityShift = random.NextDouble();
            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();

            double usePop = random.NextDouble();
            double shearStrengthModel = random.NextDouble();

            int abovePhreaticLevelDistributionType = random.Next();
            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            double abovePhreaticLevelShift = random.NextDouble();

            int cohesionDistributionType = random.Next();
            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            double cohesionShift = random.NextDouble();

            int frictionAngleDistributionType = random.Next();
            double frictionAngleMean = random.NextDouble();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            double frictionAngleShift = random.NextDouble();

            int shearStrengthRatioDistributionType = random.Next();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            double shearStrengthRatioShift = random.NextDouble();

            int strengthIncreaseExponentDistributionType = random.Next();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            double strengthIncreaseExponentShift = random.NextDouble();

            int popDistributionType = random.Next();
            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();
            double popShift = random.NextDouble();
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.IsAquifer).Returns(isAquifer);
            reader.ReadOrDefault<string>(SoilProfileTableDefinitions.MaterialName).Returns(materialName);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.Color).Returns(color);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType).Returns(belowPhreaticLevelDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelShift).Returns(belowPhreaticLevelShift);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelMean).Returns(belowPhreaticLevelMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelDeviation).Returns(belowPhreaticLevelDeviation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation).Returns(belowPhreaticLevelCoefficientOfVariation);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.DiameterD70DistributionType).Returns(diameterD70DistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70Shift).Returns(diameterD70Shift);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70Mean).Returns(diameterD70Mean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation).Returns(diameterD70CoefficientOfVariation);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.PermeabilityDistributionType).Returns(permeabilityDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityShift).Returns(permeabilityShift);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityMean).Returns(permeabilityMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation).Returns(permeabilityCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.UsePop).Returns(usePop);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthModel).Returns(shearStrengthModel);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType).Returns(abovePhreaticLevelDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.AbovePhreaticLevelMean).Returns(abovePhreaticLevelMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation).Returns(abovePhreaticLevelCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.AbovePhreaticLevelShift).Returns(abovePhreaticLevelShift);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.CohesionDistributionType).Returns(cohesionDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.CohesionMean).Returns(cohesionMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.CohesionCoefficientOfVariation).Returns(cohesionCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.CohesionShift).Returns(cohesionShift);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.FrictionAngleDistributionType).Returns(frictionAngleDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.FrictionAngleMean).Returns(frictionAngleMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation).Returns(frictionAngleCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.FrictionAngleShift).Returns(frictionAngleShift);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.ShearStrengthRatioDistributionType).Returns(shearStrengthRatioDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthRatioMean).Returns(shearStrengthRatioMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation).Returns(shearStrengthRatioCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthRatioShift).Returns(shearStrengthRatioShift);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType).Returns(strengthIncreaseExponentDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.StrengthIncreaseExponentMean).Returns(strengthIncreaseExponentMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation).Returns(strengthIncreaseExponentCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.StrengthIncreaseExponentShift).Returns(strengthIncreaseExponentShift);
            reader.ReadOrDefault<long?>(SoilProfileTableDefinitions.PopDistributionType).Returns(popDistributionType);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.PopMean).Returns(popMean);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.PopCoefficientOfVariation).Returns(popCoefficientOfVariation);
            reader.ReadOrDefault<double?>(SoilProfileTableDefinitions.PopShift).Returns(popShift);
            var properties = new LayerProperties(reader, "");
            var soilLayer = new TestSoilLayerBase();

            // Call
            SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, properties);

            // Assert
            Assert.AreEqual(properties.IsAquifer, soilLayer.IsAquifer);
            Assert.AreEqual(properties.MaterialName, soilLayer.MaterialName);
            Assert.AreEqual(properties.Color, soilLayer.Color);

            Assert.AreEqual(properties.BelowPhreaticLevelDistributionType, soilLayer.BelowPhreaticLevelDistributionType);
            Assert.AreEqual(properties.BelowPhreaticLevelShift, soilLayer.BelowPhreaticLevelShift);
            Assert.AreEqual(properties.BelowPhreaticLevelMean, soilLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(properties.BelowPhreaticLevelDeviation, soilLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(properties.BelowPhreaticLevelCoefficientOfVariation, soilLayer.BelowPhreaticLevelCoefficientOfVariation);

            Assert.AreEqual(properties.DiameterD70DistributionType, soilLayer.DiameterD70DistributionType);
            Assert.AreEqual(properties.DiameterD70Shift, soilLayer.DiameterD70Shift);
            Assert.AreEqual(properties.DiameterD70Mean, soilLayer.DiameterD70Mean);
            Assert.AreEqual(properties.DiameterD70CoefficientOfVariation, soilLayer.DiameterD70CoefficientOfVariation);

            Assert.AreEqual(properties.PermeabilityDistributionType, soilLayer.PermeabilityDistributionType);
            Assert.AreEqual(properties.PermeabilityShift, soilLayer.PermeabilityShift);
            Assert.AreEqual(properties.PermeabilityMean, soilLayer.PermeabilityMean);
            Assert.AreEqual(properties.PermeabilityCoefficientOfVariation, soilLayer.PermeabilityCoefficientOfVariation);

            Assert.AreEqual(properties.UsePop, soilLayer.UsePop);
            Assert.AreEqual(properties.ShearStrengthModel, soilLayer.ShearStrengthModel);
            Assert.AreEqual(properties.AbovePhreaticLevelDistributionType, soilLayer.AbovePhreaticLevelDistributionType);
            Assert.AreEqual(properties.AbovePhreaticLevelMean, soilLayer.AbovePhreaticLevelMean);
            Assert.AreEqual(properties.AbovePhreaticLevelCoefficientOfVariation, soilLayer.AbovePhreaticLevelCoefficientOfVariation);

            Assert.AreEqual(properties.AbovePhreaticLevelShift, soilLayer.AbovePhreaticLevelShift);
            Assert.AreEqual(properties.CohesionDistributionType, soilLayer.CohesionDistributionType);
            Assert.AreEqual(properties.CohesionMean, soilLayer.CohesionMean);
            Assert.AreEqual(properties.CohesionCoefficientOfVariation, soilLayer.CohesionCoefficientOfVariation);

            Assert.AreEqual(properties.CohesionShift, soilLayer.CohesionShift);
            Assert.AreEqual(properties.FrictionAngleDistributionType, soilLayer.FrictionAngleDistributionType);
            Assert.AreEqual(properties.FrictionAngleMean, soilLayer.FrictionAngleMean);
            Assert.AreEqual(properties.FrictionAngleCoefficientOfVariation, soilLayer.FrictionAngleCoefficientOfVariation);

            Assert.AreEqual(properties.FrictionAngleShift, soilLayer.FrictionAngleShift);
            Assert.AreEqual(properties.ShearStrengthRatioDistributionType, soilLayer.ShearStrengthRatioDistributionType);
            Assert.AreEqual(properties.ShearStrengthRatioMean, soilLayer.ShearStrengthRatioMean);
            Assert.AreEqual(properties.ShearStrengthRatioCoefficientOfVariation, soilLayer.ShearStrengthRatioCoefficientOfVariation);

            Assert.AreEqual(properties.ShearStrengthRatioShift, soilLayer.ShearStrengthRatioShift);
            Assert.AreEqual(properties.StrengthIncreaseExponentDistributionType, soilLayer.StrengthIncreaseExponentDistributionType);
            Assert.AreEqual(properties.StrengthIncreaseExponentMean, soilLayer.StrengthIncreaseExponentMean);
            Assert.AreEqual(properties.StrengthIncreaseExponentCoefficientOfVariation, soilLayer.StrengthIncreaseExponentCoefficientOfVariation);

            Assert.AreEqual(properties.StrengthIncreaseExponentShift, soilLayer.StrengthIncreaseExponentShift);
            Assert.AreEqual(properties.PopDistributionType, soilLayer.PopDistributionType);
            Assert.AreEqual(properties.PopMean, soilLayer.PopMean);
            Assert.AreEqual(properties.PopCoefficientOfVariation, soilLayer.PopCoefficientOfVariation);
            Assert.AreEqual(properties.PopShift, soilLayer.PopShift);
        }

        [Test]
        public void SetSoilLayerBaseProperties_LayerPropertiesNullValues_ReturnsExpectedValues()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            reader.ReadOrDefault<double?>(null).Returns((double?)null);
            reader.ReadOrDefault<long?>(null).Returns((long?)null);
            reader.ReadOrDefault<string>(null).Returns((string)null);
            var soilLayer = new TestSoilLayerBase();
            var properties = new LayerProperties(reader, string.Empty);

            // Call
            SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, properties);

            // Assert
            Assert.IsEmpty(soilLayer.MaterialName);
            Assert.IsNull(soilLayer.IsAquifer);
            Assert.IsNull(soilLayer.Color);

            Assert.IsNull(soilLayer.BelowPhreaticLevelDistributionType);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelMean);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelDeviation);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelShift);

            Assert.IsNull(soilLayer.DiameterD70DistributionType);
            Assert.IsNaN(soilLayer.DiameterD70Mean);
            Assert.IsNaN(soilLayer.DiameterD70CoefficientOfVariation);
            Assert.IsNaN(soilLayer.DiameterD70Shift);

            Assert.IsNull(soilLayer.PermeabilityDistributionType);
            Assert.IsNaN(soilLayer.PermeabilityMean);
            Assert.IsNaN(soilLayer.PermeabilityCoefficientOfVariation);
            Assert.IsNaN(soilLayer.PermeabilityShift);

            Assert.IsNull(soilLayer.UsePop);
            Assert.IsNull(soilLayer.ShearStrengthModel);

            Assert.IsNull(soilLayer.AbovePhreaticLevelDistributionType);
            Assert.IsNaN(soilLayer.AbovePhreaticLevelMean);
            Assert.IsNaN(soilLayer.AbovePhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(soilLayer.AbovePhreaticLevelShift);

            Assert.IsNull(soilLayer.CohesionDistributionType);
            Assert.IsNaN(soilLayer.CohesionMean);
            Assert.IsNaN(soilLayer.CohesionCoefficientOfVariation);
            Assert.IsNaN(soilLayer.CohesionShift);

            Assert.IsNull(soilLayer.FrictionAngleDistributionType);
            Assert.IsNaN(soilLayer.FrictionAngleMean);
            Assert.IsNaN(soilLayer.FrictionAngleCoefficientOfVariation);
            Assert.IsNaN(soilLayer.FrictionAngleShift);

            Assert.IsNull(soilLayer.ShearStrengthRatioDistributionType);
            Assert.IsNaN(soilLayer.ShearStrengthRatioMean);
            Assert.IsNaN(soilLayer.ShearStrengthRatioCoefficientOfVariation);
            Assert.IsNaN(soilLayer.ShearStrengthRatioShift);

            Assert.IsNull(soilLayer.StrengthIncreaseExponentDistributionType);
            Assert.IsNaN(soilLayer.StrengthIncreaseExponentMean);
            Assert.IsNaN(soilLayer.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.IsNaN(soilLayer.StrengthIncreaseExponentShift);

            Assert.IsNull(soilLayer.PopDistributionType);
            Assert.IsNaN(soilLayer.PopMean);
            Assert.IsNaN(soilLayer.PopCoefficientOfVariation);
            Assert.IsNaN(soilLayer.PopShift);
        }

        private class TestSoilLayerBase : SoilLayerBase {}
    }
}