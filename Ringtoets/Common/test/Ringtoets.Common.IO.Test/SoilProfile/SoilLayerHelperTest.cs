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
using Core.Common.IO.Readers;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerHelperTest
    {
        [Test]
        public void SetSoilLayerBaseProperties_SoilLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var reader = mockRepository.Stub<IRowBasedDatabaseReader>();
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
            int belowPhreaticLevelDistribution = random.Next();
            double belowPhreaticLevelShift = random.NextDouble();
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            int diameterD70Distribution = random.Next();
            double diameterD70Shift = random.NextDouble();
            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();
            int permeabilityDistribution = random.Next();
            double permeabilityShift = random.NextDouble();
            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();
            double usePop = random.NextDouble();
            double shearStrengthModel = random.NextDouble();
            int abovePhreaticLevelDistribution = random.Next();
            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            double abovePhreaticLevelShift = random.NextDouble();
            int cohesionDistribution = random.Next();
            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            double cohesionShift = random.NextDouble();
            double frictionAngleMean = random.NextDouble();
            int frictionAngleDistribution = random.Next();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            double frictionAngleShift = random.NextDouble();
            int shearStrengthRatioDistribution = random.Next();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            double shearStrengthRatioShift = random.NextDouble();
            int strengthIncreaseExponentDistribution = random.Next();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            double strengthIncreaseExponentShift = random.NextDouble();
            int popDistribution = random.Next();
            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();
            double popShift = random.NextDouble();

            var mockRepository = new MockRepository();
            var reader = mockRepository.StrictMock<IRowBasedDatabaseReader>();
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.IsAquifer)).Return(isAquifer);
            reader.Expect(r => r.ReadOrDefault<string>(SoilProfileTableDefinitions.MaterialName)).Return(materialName);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.Color)).Return(color);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.BelowPhreaticLevelDistribution)).Return(belowPhreaticLevelDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelShift)).Return(belowPhreaticLevelShift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelMean)).Return(belowPhreaticLevelMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelDeviation)).Return(belowPhreaticLevelDeviation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation)).Return(belowPhreaticLevelCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.DiameterD70Distribution)).Return(diameterD70Distribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70Shift)).Return(diameterD70Shift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70Mean)).Return(diameterD70Mean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation)).Return(diameterD70CoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.PermeabilityDistribution)).Return(permeabilityDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityShift)).Return(permeabilityShift);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityMean)).Return(permeabilityMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation)).Return(permeabilityCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.UsePop)).Return(usePop);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthModel)).Return(shearStrengthModel);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.AbovePhreaticLevelDistribution)).Return(abovePhreaticLevelDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.AbovePhreaticLevelMean)).Return(abovePhreaticLevelMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation)).Return(abovePhreaticLevelCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.AbovePhreaticLevelShift)).Return(abovePhreaticLevelShift);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.CohesionDistribution)).Return(cohesionDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.CohesionMean)).Return(cohesionMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.CohesionCoefficientOfVariation)).Return(cohesionCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.CohesionShift)).Return(cohesionShift);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.FrictionAngleDistribution)).Return(frictionAngleDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.FrictionAngleMean)).Return(frictionAngleMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation)).Return(frictionAngleCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.FrictionAngleShift)).Return(frictionAngleShift);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.ShearStrengthRatioDistribution)).Return(shearStrengthRatioDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthRatioMean)).Return(shearStrengthRatioMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation)).Return(shearStrengthRatioCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.ShearStrengthRatioShift)).Return(shearStrengthRatioShift);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.StrengthIncreaseExponentDistribution)).Return(strengthIncreaseExponentDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.StrengthIncreaseExponentMean)).Return(strengthIncreaseExponentMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation)).Return(strengthIncreaseExponentCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.StrengthIncreaseExponentShift)).Return(strengthIncreaseExponentShift);
            reader.Expect(r => r.ReadOrDefault<long?>(SoilProfileTableDefinitions.PopDistribution)).Return(popDistribution);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PopMean)).Return(popMean);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PopCoefficientOfVariation)).Return(popCoefficientOfVariation);
            reader.Expect(r => r.ReadOrDefault<double?>(SoilProfileTableDefinitions.PopShift)).Return(popShift);
            mockRepository.ReplayAll();

            var properties = new LayerProperties(reader, "");
            var soilLayer = new TestSoilLayerBase();

            // Call
            SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, properties);

            // Assert
            Assert.AreEqual(properties.IsAquifer, soilLayer.IsAquifer);
            Assert.AreEqual(properties.MaterialName, soilLayer.MaterialName);
            Assert.AreEqual(properties.Color, soilLayer.Color);

            Assert.AreEqual(properties.BelowPhreaticLevelDistribution, soilLayer.BelowPhreaticLevelDistribution);
            Assert.AreEqual(properties.BelowPhreaticLevelShift, soilLayer.BelowPhreaticLevelShift);
            Assert.AreEqual(properties.BelowPhreaticLevelMean, soilLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(properties.BelowPhreaticLevelDeviation, soilLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(properties.BelowPhreaticLevelCoefficientOfVariation, soilLayer.BelowPhreaticLevelCoefficientOfVariation);

            Assert.AreEqual(properties.DiameterD70Distribution, soilLayer.DiameterD70Distribution);
            Assert.AreEqual(properties.DiameterD70Shift, soilLayer.DiameterD70Shift);
            Assert.AreEqual(properties.DiameterD70Mean, soilLayer.DiameterD70Mean);
            Assert.AreEqual(properties.DiameterD70CoefficientOfVariation, soilLayer.DiameterD70CoefficientOfVariation);

            Assert.AreEqual(properties.PermeabilityDistribution, soilLayer.PermeabilityDistribution);
            Assert.AreEqual(properties.PermeabilityShift, soilLayer.PermeabilityShift);
            Assert.AreEqual(properties.PermeabilityMean, soilLayer.PermeabilityMean);
            Assert.AreEqual(properties.PermeabilityCoefficientOfVariation, soilLayer.PermeabilityCoefficientOfVariation);

            Assert.AreEqual(properties.UsePop, soilLayer.UsePop);
            Assert.AreEqual(properties.ShearStrengthModel, soilLayer.ShearStrengthModel);
            Assert.AreEqual(properties.AbovePhreaticLevelDistribution, soilLayer.AbovePhreaticLevelDistribution);
            Assert.AreEqual(properties.AbovePhreaticLevelMean, soilLayer.AbovePhreaticLevelMean);
            Assert.AreEqual(properties.AbovePhreaticLevelCoefficientOfVariation, soilLayer.AbovePhreaticLevelCoefficientOfVariation);

            Assert.AreEqual(properties.AbovePhreaticLevelShift, soilLayer.AbovePhreaticLevelShift);
            Assert.AreEqual(properties.CohesionDistribution, soilLayer.CohesionDistribution);
            Assert.AreEqual(properties.CohesionMean, soilLayer.CohesionMean);
            Assert.AreEqual(properties.CohesionCoefficientOfVariation, soilLayer.CohesionCoefficientOfVariation);

            Assert.AreEqual(properties.CohesionShift, soilLayer.CohesionShift);
            Assert.AreEqual(properties.FrictionAngleDistribution, soilLayer.FrictionAngleDistribution);
            Assert.AreEqual(properties.FrictionAngleMean, soilLayer.FrictionAngleMean);
            Assert.AreEqual(properties.FrictionAngleCoefficientOfVariation, soilLayer.FrictionAngleCoefficientOfVariation);

            Assert.AreEqual(properties.FrictionAngleShift, soilLayer.FrictionAngleShift);
            Assert.AreEqual(properties.ShearStrengthRatioDistribution, soilLayer.ShearStrengthRatioDistribution);
            Assert.AreEqual(properties.ShearStrengthRatioMean, soilLayer.ShearStrengthRatioMean);
            Assert.AreEqual(properties.ShearStrengthRatioCoefficientOfVariation, soilLayer.ShearStrengthRatioCoefficientOfVariation);

            Assert.AreEqual(properties.ShearStrengthRatioShift, soilLayer.ShearStrengthRatioShift);
            Assert.AreEqual(properties.StrengthIncreaseExponentDistribution, soilLayer.StrengthIncreaseExponentDistribution);
            Assert.AreEqual(properties.StrengthIncreaseExponentMean, soilLayer.StrengthIncreaseExponentMean);
            Assert.AreEqual(properties.StrengthIncreaseExponentCoefficientOfVariation, soilLayer.StrengthIncreaseExponentCoefficientOfVariation);

            Assert.AreEqual(properties.StrengthIncreaseExponentShift, soilLayer.StrengthIncreaseExponentShift);
            Assert.AreEqual(properties.PopDistribution, soilLayer.PopDistribution);
            Assert.AreEqual(properties.PopMean, soilLayer.PopMean);
            Assert.AreEqual(properties.PopCoefficientOfVariation, soilLayer.PopCoefficientOfVariation);
            Assert.AreEqual(properties.PopShift, soilLayer.PopShift);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetSoilLayerBaseProperties_LayerPropertiesNullValues_ReturnsExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var reader = mockRepository.Stub<IRowBasedDatabaseReader>();
            reader.Stub(r => r.ReadOrDefault<double?>(null)).IgnoreArguments().Return(null);
            reader.Stub(r => r.ReadOrDefault<long?>(null)).IgnoreArguments().Return(null);
            reader.Stub(r => r.ReadOrDefault<string>(null)).IgnoreArguments().Return(null);
            mockRepository.ReplayAll();

            var soilLayer = new TestSoilLayerBase();
            var properties = new LayerProperties(reader, string.Empty);

            // Call
            SoilLayerHelper.SetSoilLayerBaseProperties(soilLayer, properties);

            // Assert
            Assert.IsEmpty(soilLayer.MaterialName);
            Assert.IsNull(soilLayer.IsAquifer);
            Assert.IsNull(soilLayer.Color);

            Assert.IsNull(soilLayer.BelowPhreaticLevelDistribution);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelMean);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelDeviation);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(soilLayer.BelowPhreaticLevelShift);

            Assert.IsNull(soilLayer.DiameterD70Distribution);
            Assert.IsNaN(soilLayer.DiameterD70Mean);
            Assert.IsNaN(soilLayer.DiameterD70CoefficientOfVariation);
            Assert.IsNaN(soilLayer.DiameterD70Shift);

            Assert.IsNull(soilLayer.PermeabilityDistribution);
            Assert.IsNaN(soilLayer.PermeabilityMean);
            Assert.IsNaN(soilLayer.PermeabilityCoefficientOfVariation);
            Assert.IsNaN(soilLayer.PermeabilityShift);

            Assert.IsNull(soilLayer.UsePop);
            Assert.IsNull(soilLayer.ShearStrengthModel);

            Assert.IsNull(soilLayer.AbovePhreaticLevelDistribution);
            Assert.IsNaN(soilLayer.AbovePhreaticLevelMean);
            Assert.IsNaN(soilLayer.AbovePhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(soilLayer.AbovePhreaticLevelShift);

            Assert.IsNull(soilLayer.CohesionDistribution);
            Assert.IsNaN(soilLayer.CohesionMean);
            Assert.IsNaN(soilLayer.CohesionCoefficientOfVariation);
            Assert.IsNaN(soilLayer.CohesionShift);

            Assert.IsNull(soilLayer.FrictionAngleDistribution);
            Assert.IsNaN(soilLayer.FrictionAngleMean);
            Assert.IsNaN(soilLayer.FrictionAngleCoefficientOfVariation);
            Assert.IsNaN(soilLayer.FrictionAngleShift);

            Assert.IsNull(soilLayer.ShearStrengthRatioDistribution);
            Assert.IsNaN(soilLayer.ShearStrengthRatioMean);
            Assert.IsNaN(soilLayer.ShearStrengthRatioCoefficientOfVariation);
            Assert.IsNaN(soilLayer.ShearStrengthRatioShift);

            Assert.IsNull(soilLayer.StrengthIncreaseExponentDistribution);
            Assert.IsNaN(soilLayer.StrengthIncreaseExponentMean);
            Assert.IsNaN(soilLayer.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.IsNaN(soilLayer.StrengthIncreaseExponentShift);

            Assert.IsNull(soilLayer.PopDistribution);
            Assert.IsNaN(soilLayer.PopMean);
            Assert.IsNaN(soilLayer.PopCoefficientOfVariation);
            Assert.IsNaN(soilLayer.PopShift);

            mockRepository.VerifyAll();
        }

        private class TestSoilLayerBase : SoilLayerBase {}
    }
}