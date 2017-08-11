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
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.IO.Importers;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingStochasticSoilModelTransformerTest
    {
        [Test]
        public void Constructor_ValidProperties_ExpectedValues()
        {
            // Call
            var transformer = new PipingStochasticSoilModelTransformer();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelTransformer<PipingStochasticSoilModel>>(transformer);
        }

        [Test]
        [TestCaseSource(nameof(InvalidFailureMechanismTypes))]
        public void Transform_InvalidFailureMechanismType_ThrowsImportedDataTransformException(FailureMechanismType failureMechanismType)
        {
            // Setup
            var transformer = new PipingStochasticSoilModelTransformer();
            var soilModel = new StochasticSoilModel("some name", failureMechanismType);

            // Call
            TestDelegate test = () => transformer.Transform(soilModel);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Stochastic soil model of failure mechanism type '{failureMechanismType}' is not supported." +
                            $"Only stochastic soil model of failure mechanism type '{FailureMechanismType.Piping}' is supported.", exception.Message);
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
            Assert.AreEqual("Soil profile of type 'TestSoilProfile' is not supported." +
                            "Only soil profiles of type 'SoilProfile1D' or 'SoilProfile2D' are supported.", exception.Message);
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
                    new StochasticSoilProfile(1.0, new SoilProfile2D(0, name, Enumerable.Empty<SoilLayer2D>()))
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
            const double bottom = 0.5;
            const double intersectionX = 1.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new List<Segment2D[]>(),
                                                                         new List<Segment2D>
                                                                         {
                                                                             new Segment2D(new Point2D(1.0, bottom),
                                                                                           new Point2D(1.2, 1)),
                                                                             new Segment2D(new Point2D(1.2, 1),
                                                                                           new Point2D(1.0, bottom))
                                                                         });
            var profile = new SoilProfile2D(0, "SoilProfile2D", new[]
            {
                layer
            })
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
        }

        [Test]
        public void Transform_ValidTwoStochasticSoilModelWithSameProfile_ReturnsExpectedPipingStochasticSoilModel()
        {
            // Setup
            const string name = "name";
            const double bottom = 0.5;
            const double intersectionX = 1.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new List<Segment2D[]>(),
                                                                         new List<Segment2D>
                                                                         {
                                                                             new Segment2D(new Point2D(1.0, bottom),
                                                                                           new Point2D(1.2, 1)),
                                                                             new Segment2D(new Point2D(1.2, 1),
                                                                                           new Point2D(1.0, bottom))
                                                                         });
            var profile = new SoilProfile2D(0, "SoilProfile2D", new[]
            {
                layer
            })
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

        private class TestSoilProfile : ISoilProfile
        {
            public string Name { get; }
        }

        private static IEnumerable<FailureMechanismType> InvalidFailureMechanismTypes()
        {
            return Enum.GetValues(typeof(FailureMechanismType))
                       .Cast<FailureMechanismType>()
                       .Where(t => t != FailureMechanismType.Piping);
        }
    }
}