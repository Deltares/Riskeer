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
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileTransformerTest
    {
        [Test]
        public void Transform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Transform_InvalidSoilProfile_ThrowsImportedDataTransformException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileTransformer.Transform(soilProfile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string message = $"De ondergrondschematisatie van het type '{soilProfile.GetType().Name}' is niet ondersteund. " +
                             "Alleen ondergrondschematisaties van het type 'SoilProfile1D' of 'SoilProfile2D' zijn ondersteund.";
            Assert.AreEqual(message, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Transform_ValidSoilProfile1D_ReturnMacroStabilityInwardsSoilProfile1D()
        {
            // Setup
            var profile = new SoilProfile1D(1, "test", 3, new[]
            {
                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer()
            });

            // Call
            var transformedProfile = (MacroStabilityInwardsSoilProfile1D) MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profile.Name, transformedProfile.Name);
            Assert.AreEqual(profile.Bottom, transformedProfile.Bottom);
            Assert.AreEqual(profile.Layers.Count(), transformedProfile.Layers.Count());
            CollectionAssert.AllItemsAreInstancesOfType(transformedProfile.Layers, typeof(MacroStabilityInwardsSoilLayer1D));
        }

        [Test]
        public void Transform_ValidSoilProfile2D_ReturnMacroStabilityInwardsSoilProfile2D()
        {
            // Setup
            var profile = new SoilProfile2D(1, "test", new[]
            {
                SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer()
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            var transformedProfile = (MacroStabilityInwardsSoilProfile2D) MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profile.Name, transformedProfile.Name);
            Assert.AreEqual(profile.Layers.Count(), transformedProfile.Layers.Count());
            CollectionAssert.AllItemsAreInstancesOfType(transformedProfile.Layers, typeof(MacroStabilityInwardsSoilLayer2D));
            CollectionAssert.IsEmpty(transformedProfile.PreconsolidationStresses);
        }

        [Test]
        public void Transform_SoilProfile2DWithPreconsolidationStresses_ReturnMacroStabilityInwardsSoilProfile2DWithStresses()
        {
            // Setup
            var random = new Random(21);
            var preconsolidationStress = new PreconsolidationStress
            {
                XCoordinate = random.NextDouble(),
                ZCoordinate = random.NextDouble(),
                PreconsolidationStressDistributionType = 3,
                PreconsolidationStressMean = random.NextDouble(),
                PreconsolidationStressCoefficientOfVariation = random.NextDouble(),
                PreconsolidationStressShift = 0
            };

            var profile = new SoilProfile2D(1, "test", new[]
            {
                SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer()
            }, new[]
            {
                preconsolidationStress
            });

            // Call
            var transformedProfile = (MacroStabilityInwardsSoilProfile2D) MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            AssertPreconsolidationStress(preconsolidationStress, transformedProfile.PreconsolidationStresses.Single());
        }

        private static void AssertPreconsolidationStress(PreconsolidationStress preconsolidationStress,
                                                         MacroStabilityInwardsPreconsolidationStress transformedPreconsolidationStress)
        {
            Assert.AreEqual(preconsolidationStress.XCoordinate,
                            transformedPreconsolidationStress.XCoordinate);
            Assert.AreEqual(preconsolidationStress.ZCoordinate,
                            transformedPreconsolidationStress.ZCoordinate);

            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) preconsolidationStress.PreconsolidationStressMean,
                CoefficientOfVariation = (RoundedDouble) preconsolidationStress.PreconsolidationStressCoefficientOfVariation
            }, transformedPreconsolidationStress.PreconsolidationStress);
        }
    }
}