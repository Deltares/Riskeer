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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;
using Riskeer.Common.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.SoilProfiles;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.SoilProfiles
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
        public void Transform_InvalidStochasticSoilModel_ThrowsImportedDataTransformException()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel("name", FailureMechanismType.Stability);

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            TestDelegate test = () => transformer.Transform(stochasticSoilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);

            Exception innerException = exception.InnerException;
            Assert.IsNotNull(innerException);
            Assert.IsInstanceOf<ArgumentException>(innerException);
            Assert.AreEqual(innerException.Message, exception.Message);
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSoilProfile1D_ReturnsExpectedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            const double top = 4;

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            StochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry("some name", new[]
                {
                    new StochasticSoilProfile(probability, new SoilProfile1D(2, "test", 3, new[]
                    {
                        SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(top)
                    }))
                });

            // Call
            MacroStabilityInwardsStochasticSoilModel transformedModel = transformer.Transform(soilModel);

            // Assert
            Assert.AreEqual(soilModel.Name, transformedModel.Name);
            CollectionAssert.AreEqual(soilModel.Geometry, transformedModel.Geometry);
            Assert.AreEqual(1, transformedModel.StochasticSoilProfiles.Count());

            var expectedStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, new MacroStabilityInwardsSoilProfile1D("test", 3, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(top)
                {
                    Data =
                    {
                        UsePop = true,
                        ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhi,
                        IsAquifer = false
                    }
                }
            }));
            AssertStochasticSoilProfile(expectedStochasticSoilProfile, transformedModel.StochasticSoilProfiles.First());
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSoilProfile2D_ReturnsExpectedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            StochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry("some name", new[]
                {
                    new StochasticSoilProfile(probability, new SoilProfile2D(2, "test", new[]
                    {
                        SoilLayer2DTestFactory.CreateSoilLayer2D()
                    }, Enumerable.Empty<PreconsolidationStress>()))
                });

            // Call
            MacroStabilityInwardsStochasticSoilModel transformedModel = transformer.Transform(soilModel);

            // Assert
            Assert.AreEqual(soilModel.Name, transformedModel.Name);
            CollectionAssert.AreEqual(soilModel.Geometry, transformedModel.Geometry);
            Assert.AreEqual(1, transformedModel.StochasticSoilProfiles.Count());

            var expectedStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, new MacroStabilityInwardsSoilProfile2D("test", new[]
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                     {
                                                         new Point2D(1.0, 1.0),
                                                         new Point2D(2.0, 1.0)
                                                     }),
                                                     new MacroStabilityInwardsSoilLayerData
                                                     {
                                                         UsePop = true
                                                     },
                                                     new[]
                                                     {
                                                         new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                                                              {
                                                                                                  new Point2D(0.0, 0.0),
                                                                                                  new Point2D(1.0, 0.0)
                                                                                              }),
                                                                                              new MacroStabilityInwardsSoilLayerData
                                                                                              {
                                                                                                  UsePop = true
                                                                                              },
                                                                                              Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>())
                                                     })
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>()));
            AssertStochasticSoilProfile(expectedStochasticSoilProfile, transformedModel.StochasticSoilProfiles.First());
        }

        [Test]
        public void Transform_TwoStochasticSoilModelsWithSameProfile_ReturnExpectedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();
            var profile = new SoilProfile2D(2, "test", new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>());

            StochasticSoilModel soilModel1 = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(new[]
            {
                StochasticSoilProfileTestFactory.CreateStochasticSoilProfileWithValidProbability(profile)
            });

            StochasticSoilModel soilModel2 = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(new[]
            {
                StochasticSoilProfileTestFactory.CreateStochasticSoilProfileWithValidProbability(profile)
            });

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            MacroStabilityInwardsStochasticSoilModel transformedModel1 = transformer.Transform(soilModel1);
            MacroStabilityInwardsStochasticSoilModel transformedModel2 = transformer.Transform(soilModel2);

            // Assert
            MacroStabilityInwardsStochasticSoilProfile[] transformedStochasticSoilProfiles1 = transformedModel1.StochasticSoilProfiles.ToArray();
            MacroStabilityInwardsStochasticSoilProfile[] transformedStochasticSoilProfiles2 = transformedModel2.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(1, transformedStochasticSoilProfiles1.Length);
            Assert.AreEqual(1, transformedStochasticSoilProfiles2.Length);

            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile1 = transformedStochasticSoilProfiles1[0];
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile2 = transformedStochasticSoilProfiles2[0];
            Assert.AreSame(stochasticSoilProfile1.SoilProfile, stochasticSoilProfile2.SoilProfile);
        }

        [Test]
        [TestCaseSource(nameof(GetValidConfiguredAndSupportedSoilProfiles))]
        public void Transform_ValidStochasticSoilModelWithSameProfileInTwoStochasticSoilProfiles_ReturnsExpectedMacroStabilityInwardsStochasticSoilModel(ISoilProfile soilProfile)
        {
            // Setup
            const string soilModelName = "name";
            const double originalProfileOneProbability = 0.2;
            const double originalProfileTwoProbability = 0.7;
            StochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(soilModelName, new[]
                {
                    new StochasticSoilProfile(originalProfileOneProbability, soilProfile),
                    new StochasticSoilProfile(originalProfileTwoProbability, soilProfile)
                });

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();
            MacroStabilityInwardsStochasticSoilModel transformed = null;

            // Call
            Action call = () => transformed = transformer.Transform(soilModel);

            // Assert
            string expectedMessage = $"Ondergrondschematisatie '{soilProfile.Name}' is meerdere keren gevonden in ondergrondmodel '{soilModelName}'. " +
                                     "Kansen van voorkomen worden opgeteld.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn));

            MacroStabilityInwardsStochasticSoilProfile[] transformedStochasticSoilProfiles = transformed.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(1, transformedStochasticSoilProfiles.Length);
            const double expectedProbability = originalProfileOneProbability + originalProfileTwoProbability;
            Assert.AreEqual(expectedProbability, transformedStochasticSoilProfiles[0].Probability, 1e-6);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetValidConfiguredAndSupportedSoilProfiles))]
        public void Transform_ValidStochasticSoilModelWithSameProfileInTwoStochasticSoilProfilesAndSumOfProbabilitiesInvalid_ThrowsImportedDataException(ISoilProfile soilProfile)
        {
            // Setup
            var stochasticSoilProfiles = new[]
            {
                new StochasticSoilProfile(0.9, soilProfile),
                new StochasticSoilProfile(0.9, soilProfile)
            };
            StochasticSoilModel soilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(stochasticSoilProfiles);

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            TestDelegate call = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel " +
                                           "moet in het bereik [0,0, 1,0] liggen.";
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetSupportedStochasticSoilProfilesWithInvalidProbabilities))]
        public void Transform_ValidStochasticSoilModelWithProfileInvalidProbability_ThrowsImportedDataException(StochasticSoilProfile profile)
        {
            // Setup
            StochasticSoilModel soilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(new[]
                {
                    profile
                });

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            TestDelegate call = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel " +
                                           "moet in het bereik [0,0, 1,0] liggen.";
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSimilarProfileInTwoStochasticSoilProfiles_ReturnsExpectedMacroStabilityInwardsStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            const string soilProfileName = "SoilProfile";
            const double intersectionX = 1.0;

            var soilProfile2D = new SoilProfile2D(0, soilProfileName, new[]
            {
                SoilLayer2DTestFactory.CreateSoilLayer2D()
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = intersectionX
            };
            var stochasticSoilProfile2D = new StochasticSoilProfile(random.NextDouble(), soilProfile2D);

            var soilProfile1D = new SoilProfile1D(0, soilProfileName, 0, new[]
            {
                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer()
            });
            var stochasticSoilProfile1D = new StochasticSoilProfile(random.NextDouble(), soilProfile1D);

            StochasticSoilModel soilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(new[]
            {
                stochasticSoilProfile2D,
                stochasticSoilProfile1D
            });

            var transformer = new MacroStabilityInwardsStochasticSoilModelTransformer();

            // Call
            MacroStabilityInwardsStochasticSoilModel transformed = transformer.Transform(soilModel);

            // Assert
            MacroStabilityInwardsStochasticSoilProfile[] transformedStochasticSoilProfiles = transformed.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(2, transformedStochasticSoilProfiles.Length);
            Assert.AreEqual(stochasticSoilProfile2D.Probability, transformedStochasticSoilProfiles[0].Probability, 1e-6);
            Assert.AreEqual(stochasticSoilProfile1D.Probability, transformedStochasticSoilProfiles[1].Probability, 1e-6);
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

        private static IEnumerable<TestCaseData> GetSupportedStochasticSoilProfilesWithInvalidProbabilities()
        {
            double[] invalidProbabilities =
            {
                double.NaN,
                double.NegativeInfinity,
                double.PositiveInfinity,
                1.1,
                -0.1,
                5,
                -5
            };

            const long id = 1;
            const string name = "test";
            foreach (double invalidProbability in invalidProbabilities)
            {
                yield return new TestCaseData(new StochasticSoilProfile(invalidProbability,
                                                                        new SoilProfile2D(id,
                                                                                          name,
                                                                                          new[]
                                                                                          {
                                                                                              SoilLayer2DTestFactory.CreateSoilLayer2D()
                                                                                          }, Enumerable.Empty<PreconsolidationStress>())))
                    .SetName($"2D Soil Profile - {invalidProbability}");
                yield return new TestCaseData(new StochasticSoilProfile(invalidProbability,
                                                                        new SoilProfile1D(id,
                                                                                          name,
                                                                                          1,
                                                                                          new[]
                                                                                          {
                                                                                              SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer()
                                                                                          })))
                    .SetName($"1D Soil Profile - {invalidProbability}");
            }
        }

        private static IEnumerable<TestCaseData> GetValidConfiguredAndSupportedSoilProfiles()
        {
            var random = new Random(21);

            const long id = 1;
            const string name = "test";

            yield return new TestCaseData(new SoilProfile2D(id,
                                                            name,
                                                            new[]
                                                            {
                                                                SoilLayer2DTestFactory.CreateSoilLayer2D()
                                                            },
                                                            Enumerable.Empty<PreconsolidationStress>()))
                .SetName("2D Soil Profile");

            yield return new TestCaseData(new SoilProfile1D(id,
                                                            name,
                                                            random.NextDouble(),
                                                            new[]
                                                            {
                                                                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer()
                                                            }))
                .SetName("1D Soil Profile");
        }
    }
}