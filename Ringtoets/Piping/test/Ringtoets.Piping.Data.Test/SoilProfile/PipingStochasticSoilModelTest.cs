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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;

namespace Ringtoets.Piping.Data.Test.SoilProfile
{
    [TestFixture]
    public class PipingStochasticSoilModelTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingStochasticSoilModel(null, new[]
                                                                    {
                                                                        new Point2D(1, 1)
                                                                    },
                                                                    new[]
                                                                    {
                                                                        CreateStochasticSoilProfile()
                                                                    });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            const string name = "name";
            var geometry = new[]
            {
                new Point2D(1, 1)
            };
            PipingStochasticSoilProfile[] stochasticSoilProfiles =
            {
                CreateStochasticSoilProfile()
            };

            // Call
            var model = new PipingStochasticSoilModel(name, geometry, stochasticSoilProfiles);

            // Assert
            Assert.IsInstanceOf<Observable>(model);
            Assert.IsInstanceOf<IMechanismStochasticSoilModel>(model);

            Assert.AreEqual(name, model.Name);
            Assert.AreSame(geometry, model.Geometry);
            CollectionAssert.AreEqual(stochasticSoilProfiles, model.StochasticSoilProfiles);
            Assert.AreEqual(name, model.ToString());
        }

        [Test]
        public void Constructor_WithGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingStochasticSoilModel(string.Empty, null, new[]
            {
                CreateStochasticSoilProfile()
            });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_WithGeometryEmpty_ThrowsArgumentException()
        {
            // Setup
            const string name = "modelName";

            // Call
            TestDelegate call = () => new PipingStochasticSoilModel(name, Enumerable.Empty<Point2D>(), new[]
            {
                CreateStochasticSoilProfile()
            });

            // Assert
            string expectedMessage = $"Het stochastische ondergrondmodel '{name}' moet een geometrie bevatten.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_StochasticSoilProfilesNull_ThrowsArgumentNullException()
        {
            // Setup
            const string name = "name";
            var geometry = new[]
            {
                new Point2D(1, 1)
            };

            // Call
            TestDelegate call = () => new PipingStochasticSoilModel(name, geometry, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilProfiles", exception.ParamName);
        }

        [Test]
        public void Constructor_StochasticSoilProfilesEmpty_ThrowsArgumentException()
        {
            // Setup
            var random = new Random(21);
            const string name = "name";
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            TestDelegate call = () => new PipingStochasticSoilModel(name,
                                                                    geometry,
                                                                    Enumerable.Empty<PipingStochasticSoilProfile>());

            // Assert
            string expectedMessage = $"Er zijn geen ondergrondschematisaties gevonden in het stochastische ondergrondmodel '{name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var model = new PipingStochasticSoilModel("name", new[]
                                                      {
                                                          new Point2D(1, 1)
                                                      },
                                                      new[]
                                                      {
                                                          CreateStochasticSoilProfile()
                                                      });

            // Call
            TestDelegate test = () => model.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromModel", paramName);
        }

        [Test]
        public void Update_ModelWithUpdatedProperties_PropertiesUpdated()
        {
            // Setup
            var model = new PipingStochasticSoilModel("name", new[]
            {
                new Point2D(1, 2),
                new Point2D(4, 5)
            }, new[]
            {
                CreateStochasticSoilProfile()
            });

            const string expectedName = "otherName";
            var expectedGeometry = new[]
            {
                new Point2D(4, 2)
            };

            var otherModel = new PipingStochasticSoilModel(expectedName, expectedGeometry, new[]
            {
                CreateStochasticSoilProfile()
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(expectedName, model.Name);
            CollectionAssert.AreEqual(expectedGeometry, model.Geometry);

            CollectionAssert.IsEmpty(difference.AddedProfiles);
            CollectionAssert.IsEmpty(difference.UpdatedProfiles);
            CollectionAssert.IsEmpty(difference.RemovedProfiles);
        }

        [Test]
        public void Update_ModelWithAddedProfile_ProfileAdded()
        {
            // Setup
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile()
            });

            var expectedAddedProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            PipingStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                expectedAddedProfile,
                CreateStochasticSoilProfile()
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(2, otherModel.StochasticSoilProfiles.Count());
            Assert.AreEqual(expectedAddedProfile, otherModel.StochasticSoilProfiles.First());

            CollectionAssert.AreEqual(new[]
            {
                expectedAddedProfile
            }, difference.AddedProfiles);
            CollectionAssert.IsEmpty(difference.UpdatedProfiles);
            CollectionAssert.IsEmpty(difference.RemovedProfiles);
        }

        [Test]
        public void Update_ModelWithUpdatedProfile_ProfileUpdated()
        {
            // Setup
            const string profileName = "A";
            var expectedUpdatedProfile = new PipingStochasticSoilProfile(
                0.2,
                new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D));
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                expectedUpdatedProfile
            });
            PipingStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                new PipingStochasticSoilProfile(0.2,
                                                new PipingSoilProfile(profileName, -1, CreateLayers(), SoilProfileType.SoilProfile1D))
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(1, otherModel.StochasticSoilProfiles.Count());
            Assert.AreEqual(expectedUpdatedProfile, otherModel.StochasticSoilProfiles.First());

            CollectionAssert.IsEmpty(difference.AddedProfiles);
            CollectionAssert.AreEqual(new[]
            {
                expectedUpdatedProfile
            }, difference.UpdatedProfiles);
            CollectionAssert.IsEmpty(difference.RemovedProfiles);
        }

        [Test]
        public void Update_ModelWithUpdatedStochasticSoilProfile_ProfileUpdated()
        {
            // Setup
            const string profileName = "A";
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D);
            var expectedUpdatedProfile = new PipingStochasticSoilProfile(0.2, soilProfile);
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                expectedUpdatedProfile
            });

            PipingStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                new PipingStochasticSoilProfile(0.5, soilProfile)
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            CollectionAssert.IsEmpty(difference.AddedProfiles);
            CollectionAssert.AreEqual(new[]
            {
                expectedUpdatedProfile
            }, difference.UpdatedProfiles);
            CollectionAssert.IsEmpty(difference.RemovedProfiles);
        }

        [Test]
        public void Update_ModelWithRemovedProfile_ProfileRemoved()
        {
            // Setup
            const string profileName = "A";
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D);
            var expectedRemovedProfile = new PipingStochasticSoilProfile(0.2, soilProfile);
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile(),
                expectedRemovedProfile
            });

            PipingStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile()
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            CollectionAssert.IsEmpty(difference.AddedProfiles);
            CollectionAssert.IsEmpty(difference.UpdatedProfiles);
            CollectionAssert.AreEqual(new[]
            {
                expectedRemovedProfile
            }, difference.RemovedProfiles);
        }

        [Test]
        public void Update_ModelWithRemovedProfileSameNameOtherType_ProfileRemoved()
        {
            // Setup
            const string profileName = "A";
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D);
            var expectedRemovedProfile = new PipingStochasticSoilProfile(0.2, soilProfile);
            var newProfile = new PipingStochasticSoilProfile(
                0.2,
                new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile2D));
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile(),
                expectedRemovedProfile
            });

            PipingStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile(),
                newProfile
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                newProfile
            }, difference.AddedProfiles);
            CollectionAssert.IsEmpty(difference.UpdatedProfiles);
            CollectionAssert.AreEqual(new[]
            {
                expectedRemovedProfile
            }, difference.RemovedProfiles);
        }

        [Test]
        public void Update_WithOtherModel_PropertiesUpdated()
        {
            // Setup
            const string equalProfileName = "nameA";
            var stochasticProfileA = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile(equalProfileName));
            var stochasticProfileB = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("nameB"));
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                stochasticProfileA,
                stochasticProfileB
            });

            const string otherName = "other name";
            var otherGeometry = new[]
            {
                new Point2D(2, 0),
                new Point2D(3, 0)
            };
            var otherStochasticProfileA = new PipingStochasticSoilProfile(
                0.7, new PipingSoilProfile(equalProfileName, -1, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D));
            var otherStochasticProfileB = new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile("other profile name"));
            var otherModel = new PipingStochasticSoilModel(otherName, otherGeometry, new[]
            {
                otherStochasticProfileA,
                otherStochasticProfileB
            });

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(otherName, model.Name);
            Assert.AreSame(otherGeometry, model.Geometry);

            PipingStochasticSoilProfile[] stochasticSoilProfiles = model.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(2, stochasticSoilProfiles.Length);
            Assert.AreSame(stochasticProfileA, stochasticSoilProfiles[0]);
            Assert.AreSame(otherStochasticProfileA.SoilProfile, stochasticSoilProfiles[0].SoilProfile);
            Assert.AreNotSame(stochasticProfileB, stochasticSoilProfiles[1]);
            Assert.AreSame(otherStochasticProfileB.SoilProfile, stochasticSoilProfiles[1].SoilProfile);

            CollectionAssert.AreEqual(new[]
            {
                stochasticProfileA
            }, difference.UpdatedProfiles);
            CollectionAssert.AreEqual(new[]
            {
                stochasticProfileB
            }, difference.RemovedProfiles);
            CollectionAssert.AreEqual(new[]
            {
                otherStochasticProfileB
            }, difference.AddedProfiles);
        }

        [Test]
        public void Update_ModelsWithAddedProfilesWithSameNames_ThrowsInvalidOperationException()
        {
            // Setup 
            const string profileName = "Name of the profile";
            var addedProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName));
            PipingStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                addedProfile
            });

            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName);
            var existingStochasticSoilProfileOne = new PipingStochasticSoilProfile(0.2, soilProfile);
            var existingStochasticSoilProfileTwo = new PipingStochasticSoilProfile(0.3, soilProfile);
            PipingStochasticSoilModel model = CreateValidModel(new[]
            {
                existingStochasticSoilProfileOne,
                existingStochasticSoilProfileTwo
            });

            // Call 
            TestDelegate call = () => model.Update(otherModel);

            // Assert 
            Assert.Throws<InvalidOperationException>(call);

            Assert.AreEqual(1, otherModel.StochasticSoilProfiles.Count());
            Assert.AreEqual(addedProfile, otherModel.StochasticSoilProfiles.First());

            Assert.AreEqual(2, model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEqual(new[]
            {
                existingStochasticSoilProfileOne,
                existingStochasticSoilProfileTwo
            }, model.StochasticSoilProfiles);
        }

        private static PipingStochasticSoilProfile CreateStochasticSoilProfile()
        {
            var random = new Random(21);
            return new PipingStochasticSoilProfile(random.NextDouble(), PipingSoilProfileTestFactory.CreatePipingSoilProfile(string.Empty));
        }

        private static IEnumerable<PipingSoilLayer> CreateLayers()
        {
            return new[]
            {
                new PipingSoilLayer(2)
            };
        }

        private static PipingStochasticSoilModel CreateValidModel(IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles)
        {
            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            return new PipingStochasticSoilModel("name", geometry, stochasticSoilProfiles);
        }
    }
}