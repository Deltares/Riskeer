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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelTransformerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelTransformer<MacroStabilityInwardsStochasticSoilModel>>(transformer);
        }

        [Test]
        public void Transform_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            TestDelegate call = () => transformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilModel", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidFailureMechanismTypes))]
        public void Transform_InvalidFailureMechanismType_ThrowsImportedDataTransformException(FailureMechanismType failureMechanismType)
        {
            // Setup
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", failureMechanismType);

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Het stochastische ondergrondmodel met '{failureMechanismType}' als faalmechanisme type is niet ondersteund. " +
                            "Alleen stochastische ondergrondmodellen met 'Stability' als faalmechanisme type zijn ondersteund.", exception.Message);
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSoilProfile1D_ReturnsExpectedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", FailureMechanismType.Stability)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1, new SoilProfile1D(2, "test", 3, new[]
                    {
                        new SoilLayer1D(4)
                    }))
                }
            };

            // Call
            MacroStabilityInwardsStochasticSoilModel transformedModel = transformer.Transform(soilModel);

            // Assert
            Assert.AreEqual(soilModel.Name, transformedModel.Name);
            CollectionAssert.AreEqual(soilModel.Geometry, transformedModel.Geometry);
            Assert.AreEqual(1, transformedModel.StochasticSoilProfiles.Count);

            var expectedStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1, new MacroStabilityInwardsSoilProfile1D("test", 3, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(4)
            }, SoilProfileType.SoilProfile1D, 1));
            AssertStochasticSoilProfile(expectedStochasticSoilProfile, transformedModel.StochasticSoilProfiles.First());
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSoilProfile2D_ReturnsExpectedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", FailureMechanismType.Stability)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1, new SoilProfile2D(2, "test", new[]
                    {
                        SoilLayer2DTestFactory.CreateSoilLayer2D()
                    }))
                }
            };

            // Call
            MacroStabilityInwardsStochasticSoilModel transformedModel = transformer.Transform(soilModel);

            // Assert
            Assert.AreEqual(soilModel.Name, transformedModel.Name);
            CollectionAssert.AreEqual(soilModel.Geometry, transformedModel.Geometry);
            Assert.AreEqual(1, transformedModel.StochasticSoilProfiles.Count);

            var expectedStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1, new MacroStabilityInwardsSoilProfile2D("test", new[]
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(1.0, 0.0),
                    new Point2D(1.0, 0.0),
                    new Point2D(0.0, 0.0)
                }), new[]
                {
                    new Ring(new[]
                    {
                        new Point2D(1.0, 1.0),
                        new Point2D(2.0, 1.0),
                        new Point2D(2.0, 1.0),
                        new Point2D(1.0, 1.0)
                    }),
                })
            }, SoilProfileType.SoilProfile2D, 1));
            AssertStochasticSoilProfile(expectedStochasticSoilProfile, transformedModel.StochasticSoilProfiles.First());
        }

        [Test]
        public void Transform_TwoStochasticSoilModelsWithSameProfile_ReturnExepctedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            var profile = new SoilProfile2D(2, "test", new[]
            {
                layer
            });

            var soilModel1 = new StochasticSoilModel("some name", FailureMechanismType.Stability)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, profile)
                }
            };

            var soilModel2 = new StochasticSoilModel("some name", FailureMechanismType.Stability)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, profile)
                }
            };

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            MacroStabilityInwardsStochasticSoilModel transformedModel1 = transformer.Transform(soilModel1);
            MacroStabilityInwardsStochasticSoilModel transformedModel2 = transformer.Transform(soilModel2);

            // Assert
            List<MacroStabilityInwardsStochasticSoilProfile> transformedStochasticSoilProfiles1 = transformedModel1.StochasticSoilProfiles;
            List<MacroStabilityInwardsStochasticSoilProfile> transformedStochasticSoilProfiles2 = transformedModel2.StochasticSoilProfiles;
            Assert.AreEqual(1, transformedStochasticSoilProfiles1.Count);
            Assert.AreEqual(1, transformedStochasticSoilProfiles2.Count);

            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile1 = transformedStochasticSoilProfiles1[0];
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile2 = transformedStochasticSoilProfiles2[0];
            Assert.AreSame(stochasticSoilProfile1.SoilProfile, stochasticSoilProfile2.SoilProfile);
        }

        private static void AssertStochasticSoilProfile(MacroStabilityInwardsStochasticSoilProfile expected,
                                                        MacroStabilityInwardsStochasticSoilProfile actual)
        {
            Assert.AreEqual(expected.Probability, actual.Probability);
            Assert.AreEqual(expected.SoilProfile, actual.SoilProfile);
        }

        private static IEnumerable<FailureMechanismType> GetInvalidFailureMechanismTypes()
        {
            return Enum.GetValues(typeof(FailureMechanismType))
                       .Cast<FailureMechanismType>()
                       .Where(t => t != FailureMechanismType.Stability);
        }
    }
}