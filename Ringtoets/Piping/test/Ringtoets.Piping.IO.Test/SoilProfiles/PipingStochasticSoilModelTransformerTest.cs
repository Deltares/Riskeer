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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SoilProfile.Schema;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.IO.SoilProfiles;

namespace Ringtoets.Piping.IO.Test.SoilProfiles
{
    [TestFixture]
    public class PipingStochasticSoilModelTransformerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var transformer = new PipingStochasticSoilModelTransformer();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelTransformer<PipingStochasticSoilModel>>(transformer);
        }

        [Test]
        public void Transform_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var transformer = new PipingStochasticSoilModelTransformer();

            // Call
            TestDelegate test = () => transformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilModel", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidFailureMechanismTypes))]
        public void Transform_InvalidFailureMechanismType_ThrowsImportedDataTransformException(FailureMechanismType failureMechanismType)
        {
            // Setup
            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", failureMechanismType);

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Het stochastische ondergrondmodel met '{failureMechanismType}' als faalmechanisme type is niet ondersteund. " +
                            "Alleen stochastische ondergrondmodellen met 'Piping' als faalmechanisme type zijn ondersteund.", exception.Message);
        }

        [Test]
        public void Transform_StochasticSoilModelWithInvalidSoilProfile_ThrowsImportedDataTransformException()
        {
            // Setup
            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, new TestSoilProfile())
                }
            };

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            const string message = "De ondergrondschematisatie van het type 'TestSoilProfile' is niet ondersteund. " +
                                   "Alleen ondergrondschematisaties van het type 'SoilProfile1D' of 'SoilProfile2D' zijn ondersteund.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Transform_SoilProfile2DWithoutIntersection_ThrowsImportedDataTransformException()
        {
            // Setup
            const string name = "name";
            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel(name, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, new SoilProfile2D(0, name, new[]
                    {
                        new SoilLayer2D()
                    }, Enumerable.Empty<PreconsolidationStress>()))
                }
            };

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel '{name}'.",
                            exception.Message);
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSoilProfile2D_ReturnsExpectedPipingStochasticSoilModel()
        {
            // Setup
            const string name = "name";
            const double intersectionX = 1.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
            var profile = new SoilProfile2D(0, "SoilProfile2D", new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = intersectionX
            };

            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel(name, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, profile)
                },
                Geometry =
                {
                    new Point2D(1.0, 0.0),
                    new Point2D(0.0, 0.0)
                }
            };

            // Call
            PipingStochasticSoilModel transformed = transformer.Transform(soilModel);

            // Assert
            Assert.AreEqual(name, transformed.Name);
            Assert.AreEqual(1, transformed.StochasticSoilProfiles.Count);
            CollectionAssert.AreEqual(soilModel.Geometry, transformed.Geometry);

            var expectedPipingSoilProfile = new List<PipingStochasticSoilProfile>
            {
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTransformer.Transform(profile))
            };
            AssertPipingStochasticSoilProfiles(expectedPipingSoilProfile, transformed.StochasticSoilProfiles);
        }

        [Test]
        public void Transform_ValidTwoStochasticSoilModelWithSameProfile_ReturnsExpectedPipingStochasticSoilModel()
        {
            // Setup
            const string name = "name";
            const double intersectionX = 1.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
            var profile = new SoilProfile2D(0, "SoilProfile2D", new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = intersectionX
            };

            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel1 = new StochasticSoilModel(name, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, profile)
                }
            };

            var soilModel2 = new StochasticSoilModel(name, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, profile)
                }
            };

            // Call
            PipingStochasticSoilModel transformed1 = transformer.Transform(soilModel1);
            PipingStochasticSoilModel transformed2 = transformer.Transform(soilModel2);

            // Assert
            List<PipingStochasticSoilProfile> transformedStochasticSoilProfiles1 = transformed1.StochasticSoilProfiles;
            List<PipingStochasticSoilProfile> transformedStochasticSoilProfiles2 = transformed2.StochasticSoilProfiles;
            Assert.AreEqual(1, transformedStochasticSoilProfiles1.Count);
            Assert.AreEqual(1, transformedStochasticSoilProfiles2.Count);

            PipingStochasticSoilProfile pipingStochasticSoilProfile1 = transformedStochasticSoilProfiles1[0];
            PipingStochasticSoilProfile pipingStochasticSoilProfile2 = transformedStochasticSoilProfiles2[0];
            Assert.AreSame(pipingStochasticSoilProfile1.SoilProfile, pipingStochasticSoilProfile2.SoilProfile);
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSameProfileInTwoStochasticSoilProfiles_ReturnsExpectedPipingStochasticSoilModel()
        {
            // Setup
            const string soilModelName = "name";
            const string soilProfileName = "SoilProfile";
            const double intersectionX = 1.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer();
            var profile = new SoilProfile2D(0, soilProfileName, new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = intersectionX
            };

            const double originalProfileOneProbability = 0.2;
            const double originalProfileTwoProbability = 0.7;
            var soilModel = new StochasticSoilModel(soilModelName, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(originalProfileOneProbability, profile),
                    new StochasticSoilProfile(originalProfileTwoProbability, profile)
                }
            };

            var transformer = new PipingStochasticSoilModelTransformer();
            PipingStochasticSoilModel transformed = null;

            // Call
            Action call = () => transformed = transformer.Transform(soilModel);

            // Assert
            string expectedMessage = $"Ondergrondschematisatie '{soilProfileName}' is meerdere keren gevonden in ondergrondmodel '{soilModelName}'. " +
                                     "Kansen van voorkomen worden opgeteld.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn));

            List<PipingStochasticSoilProfile> transformedStochasticSoilProfiles = transformed.StochasticSoilProfiles;
            Assert.AreEqual(1, transformedStochasticSoilProfiles.Count);
            const double expectedProbability = originalProfileOneProbability + originalProfileTwoProbability;
            Assert.AreEqual(expectedProbability, transformedStochasticSoilProfiles[0].Probability, 1e-6);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Transform_ValidStochasticSoilModelWithSameProfileProbabilityExceedingValidRange_ThrowsImportedDataException()
        {
            // Setup
            const string soilModelName = "name";

            var mocks = new MockRepository();
            var profile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            var soilModel = new StochasticSoilModel(soilModelName, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.9, profile),
                    new StochasticSoilProfile(0.9, profile)
                }
            };

            var transformer = new PipingStochasticSoilModelTransformer();

            // Call
            TestDelegate call = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel " +
                                           "moet in het bereik [0,0, 1,0] liggen.";
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Transform_ValidStochasticSoilModelWithSimilarProfileInTwoStochasticSoilProfiles_ReturnsExpectedPipingStochasticSoilModel()
        {
            // Setup
            const string soilModelName = "name";
            const string soilProfileName = "SoilProfile";
            const double intersectionX = 1.0;

            var soilProfile2D = new SoilProfile2D(0, soilProfileName, new[]
            {
                SoilLayer2DTestFactory.CreateSoilLayer2DWithValidAquifer()
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = intersectionX
            };
            var stochasticSoilProfile2D = new StochasticSoilProfile(0.2, soilProfile2D);

            var soilProfile1D = new SoilProfile1D(0, soilProfileName, 0, new[]
            {
                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer()
            });
            var stochasticSoilProfile1D = new StochasticSoilProfile(0.2, soilProfile1D);

            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel(soilModelName, FailureMechanismType.Piping)
            {
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile2D,
                    stochasticSoilProfile1D
                }
            };

            // Call
            PipingStochasticSoilModel transformed = transformer.Transform(soilModel);

            // Assert
            List<PipingStochasticSoilProfile> transformedStochasticSoilProfiles = transformed.StochasticSoilProfiles;
            Assert.AreEqual(2, transformedStochasticSoilProfiles.Count);
            Assert.AreEqual(stochasticSoilProfile2D.Probability, transformedStochasticSoilProfiles[0].Probability, 1e-6);
            Assert.AreEqual(stochasticSoilProfile1D.Probability, transformedStochasticSoilProfiles[1].Probability, 1e-6);
        }

        private static void AssertPipingStochasticSoilProfiles(IList<PipingStochasticSoilProfile> expected,
                                                               IList<PipingStochasticSoilProfile> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                AssertPipingStochasticSoilProfile(expected[i], actual[i]);
            }
        }

        private static void AssertPipingStochasticSoilProfile(PipingStochasticSoilProfile expected,
                                                              PipingStochasticSoilProfile actual)
        {
            Assert.AreEqual(expected.Probability, actual.Probability);
            Assert.AreEqual(expected.SoilProfile, actual.SoilProfile);
        }

        private class TestSoilProfile : ISoilProfile
        {
            public string Name { get; }
        }

        private static IEnumerable<FailureMechanismType> GetInvalidFailureMechanismTypes()
        {
            return Enum.GetValues(typeof(FailureMechanismType))
                       .Cast<FailureMechanismType>()
                       .Where(t => t != FailureMechanismType.Piping);
        }
    }
}