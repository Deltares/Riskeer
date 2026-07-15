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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.IO.Readers;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class LayerPropertiesTest
    {
        [Test]
        public void LayerProperties_ReaderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new LayerProperties(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("reader", paramName);
        }

        [Test]
        public void LayerProperties_ProfileNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            // Call
            TestDelegate test = () => new LayerProperties(reader, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("profileName", paramName);
        }

        [Test]
        public void LayerProperties_WithReaderAndProfileName_SetProperties()
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
            // Call
            var properties = new LayerProperties(reader, "");

            // Assert
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(belowPhreaticLevelDistributionType, properties.BelowPhreaticLevelDistributionType);
            Assert.AreEqual(belowPhreaticLevelShift, properties.BelowPhreaticLevelShift);
            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, properties.BelowPhreaticLevelDeviation);
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, properties.BelowPhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(diameterD70DistributionType, properties.DiameterD70DistributionType);
            Assert.AreEqual(diameterD70Shift, properties.DiameterD70Shift);
            Assert.AreEqual(diameterD70Mean, properties.DiameterD70Mean);
            Assert.AreEqual(diameterD70CoefficientOfVariation, properties.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(permeabilityDistributionType, properties.PermeabilityDistributionType);
            Assert.AreEqual(permeabilityShift, properties.PermeabilityShift);
            Assert.AreEqual(permeabilityMean, properties.PermeabilityMean);
            Assert.AreEqual(permeabilityCoefficientOfVariation, properties.PermeabilityCoefficientOfVariation);
            Assert.AreEqual(usePop, properties.UsePop);
            Assert.AreEqual(shearStrengthModel, properties.ShearStrengthModel);
            Assert.AreEqual(abovePhreaticLevelDistributionType, properties.AbovePhreaticLevelDistributionType);
            Assert.AreEqual(abovePhreaticLevelMean, properties.AbovePhreaticLevelMean);
            Assert.AreEqual(abovePhreaticLevelCoefficientOfVariation, properties.AbovePhreaticLevelCoefficientOfVariation);
            Assert.AreEqual(abovePhreaticLevelShift, properties.AbovePhreaticLevelShift);
            Assert.AreEqual(cohesionDistributionType, properties.CohesionDistributionType);
            Assert.AreEqual(cohesionMean, properties.CohesionMean);
            Assert.AreEqual(cohesionCoefficientOfVariation, properties.CohesionCoefficientOfVariation);
            Assert.AreEqual(cohesionShift, properties.CohesionShift);
            Assert.AreEqual(frictionAngleDistributionType, properties.FrictionAngleDistributionType);
            Assert.AreEqual(frictionAngleMean, properties.FrictionAngleMean);
            Assert.AreEqual(frictionAngleCoefficientOfVariation, properties.FrictionAngleCoefficientOfVariation);
            Assert.AreEqual(frictionAngleShift, properties.FrictionAngleShift);
            Assert.AreEqual(shearStrengthRatioDistributionType, properties.ShearStrengthRatioDistributionType);
            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatioMean);
            Assert.AreEqual(shearStrengthRatioCoefficientOfVariation, properties.ShearStrengthRatioCoefficientOfVariation);
            Assert.AreEqual(shearStrengthRatioShift, properties.ShearStrengthRatioShift);
            Assert.AreEqual(strengthIncreaseExponentDistributionType, properties.StrengthIncreaseExponentDistributionType);
            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponentMean);
            Assert.AreEqual(strengthIncreaseExponentCoefficientOfVariation, properties.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.AreEqual(strengthIncreaseExponentShift, properties.StrengthIncreaseExponentShift);
            Assert.AreEqual(popDistributionType, properties.PopDistributionType);
            Assert.AreEqual(popMean, properties.PopMean);
            Assert.AreEqual(popCoefficientOfVariation, properties.PopCoefficientOfVariation);
            Assert.AreEqual(popShift, properties.PopShift);
        }

        [Test]
        [TestCaseSource(nameof(LayerProperties))]
        public void LayerProperties_ReaderThrowsInvalidCastException_ThrowsSoilProfileReadException(string columnName)
        {
            // Setup
            const string path = "path";
            const string profileName = "SomeProfile";

            var invalidCastException = new InvalidCastException();
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            reader.ReadOrDefault<double?>(columnName).Returns(_ =>  throw invalidCastException);
            reader.ReadOrDefault<long?>(columnName).Returns(_ =>  throw invalidCastException);
            reader.ReadOrDefault<string>(columnName).Returns(_ => throw invalidCastException);

            reader.ReadOrDefault<double?>(Arg.Is<string>(s => s != columnName))
                  .Returns(0);
            reader.ReadOrDefault<long?>(Arg.Is<string>(s => s != columnName))
                  .Returns(0);
            reader.ReadOrDefault<string>(Arg.Is<string>(s => s != columnName))
                  .Returns("");
            reader.Path.Returns(path);
            // Call
            TestDelegate test = () => new LayerProperties(reader, profileName);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}' (ondergrondschematisatie '{profileName}'): " +
                                     $"ondergrondschematisatie bevat geen geldige waarde in kolom '{columnName}'.";

            var exception = Assert.Throws<SoilProfileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreSame(invalidCastException, exception.InnerException);
        }

        private static IEnumerable<string> LayerProperties()
        {
            yield return SoilProfileTableDefinitions.IsAquifer;
            yield return SoilProfileTableDefinitions.MaterialName;
            yield return SoilProfileTableDefinitions.Color;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelShift;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelMean;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelDeviation;
            yield return SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.DiameterD70DistributionType;
            yield return SoilProfileTableDefinitions.DiameterD70Shift;
            yield return SoilProfileTableDefinitions.DiameterD70Mean;
            yield return SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation;
            yield return SoilProfileTableDefinitions.PermeabilityDistributionType;
            yield return SoilProfileTableDefinitions.PermeabilityShift;
            yield return SoilProfileTableDefinitions.PermeabilityMean;
            yield return SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.UsePop;
            yield return SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType;
            yield return SoilProfileTableDefinitions.AbovePhreaticLevelMean;
            yield return SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.AbovePhreaticLevelShift;
            yield return SoilProfileTableDefinitions.CohesionDistributionType;
            yield return SoilProfileTableDefinitions.CohesionMean;
            yield return SoilProfileTableDefinitions.CohesionCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.CohesionShift;
            yield return SoilProfileTableDefinitions.FrictionAngleDistributionType;
            yield return SoilProfileTableDefinitions.FrictionAngleMean;
            yield return SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.FrictionAngleShift;
            yield return SoilProfileTableDefinitions.ShearStrengthRatioDistributionType;
            yield return SoilProfileTableDefinitions.ShearStrengthRatioMean;
            yield return SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.ShearStrengthRatioShift;
            yield return SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType;
            yield return SoilProfileTableDefinitions.StrengthIncreaseExponentMean;
            yield return SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.StrengthIncreaseExponentShift;
            yield return SoilProfileTableDefinitions.PopDistributionType;
            yield return SoilProfileTableDefinitions.PopMean;
            yield return SoilProfileTableDefinitions.PopCoefficientOfVariation;
            yield return SoilProfileTableDefinitions.PopShift;
        }
    }
}