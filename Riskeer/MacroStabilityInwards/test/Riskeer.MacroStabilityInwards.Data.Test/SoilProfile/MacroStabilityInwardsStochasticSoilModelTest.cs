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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelTest
    {
        [Test]
        public void Constructor_WithoutName_ExpectedValues()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilModel(null, new[]
                                                                                   {
                                                                                       new Point2D(random.NextDouble(), random.NextDouble())
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
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsStochasticSoilModel(string.Empty, null, new[]
            {
                CreateStochasticSoilProfile()
            });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryEmpty_ThrowsArgumentException()
        {
            // Setup
            const string name = "modelName";

            // Call
            TestDelegate call = () => new MacroStabilityInwardsStochasticSoilModel(name, Enumerable.Empty<Point2D>(), new[]
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
            var random = new Random(21);
            const string name = "name";
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            TestDelegate call = () => new MacroStabilityInwardsStochasticSoilModel(name, geometry, null);

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
            TestDelegate call = () => new MacroStabilityInwardsStochasticSoilModel(name,
                                                                                   geometry,
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilProfile>());

            // Assert
            string expectedMessage = $"Er zijn geen ondergrondschematisaties gevonden in het stochastische ondergrondmodel '{name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase("")]
        [TestCase("segmentSoilModelName")]
        public void Constructor_WithValidParameters_ExpectedValues(string segmentSoilModelName)
        {
            // Setup
            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles = new[]
            {
                CreateStochasticSoilProfile()
            };

            // Call

            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel(segmentSoilModelName, geometry, stochasticSoilProfiles);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilModel);
            Assert.AreEqual(segmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreSame(geometry, stochasticSoilModel.Geometry);
            CollectionAssert.AreEqual(stochasticSoilProfiles, stochasticSoilModel.StochasticSoilProfiles);
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
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
            var model = new MacroStabilityInwardsStochasticSoilModel("name", new[]
            {
                new Point2D(1, 2),
                new Point2D(4, 5)
            }, new[]
            {
                CreateStochasticSoilProfile()
            });

            const string expectedName = "otherName";
            var otherModel = new MacroStabilityInwardsStochasticSoilModel(expectedName, new[]
            {
                new Point2D(4, 2)
            }, new[]
            {
                CreateStochasticSoilProfile()
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            AssertStochasticSoilModelAreEqual(otherModel, model);

            CollectionAssert.IsEmpty(difference.AddedProfiles);
            CollectionAssert.IsEmpty(difference.UpdatedProfiles);
            CollectionAssert.IsEmpty(difference.RemovedProfiles);
        }

        [Test]
        public void Update_ModelWithAddedProfile_ProfileAdded()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile()
            });

            var expectedAddedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, new TestSoilProfile("Added Profile"));
            MacroStabilityInwardsStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile(),
                expectedAddedProfile
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            AssertStochasticSoilModelAreEqual(otherModel, model);

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
            var expectedUpdatedProfile = new MacroStabilityInwardsStochasticSoilProfile(
                0.2, new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D()));
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
            {
                expectedUpdatedProfile
            });
            MacroStabilityInwardsStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(
                    0.2, new MacroStabilityInwardsSoilProfile1D(profileName, -1, CreateLayers1D()))
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            AssertStochasticSoilModelAreEqual(otherModel, model);
            Assert.AreEqual(expectedUpdatedProfile, otherModel.StochasticSoilProfiles.ElementAt(0));

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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D());
            var expectedUpdatedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
            {
                expectedUpdatedProfile
            });

            MacroStabilityInwardsStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, soilProfile)
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D());
            var expectedRemovedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile(),
                expectedRemovedProfile
            });

            MacroStabilityInwardsStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                CreateStochasticSoilProfile()
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D());
            var expectedRemovedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            var newProfile = new MacroStabilityInwardsStochasticSoilProfile(
                0.2,
                new MacroStabilityInwardsSoilProfile2D(profileName, CreateLayers2D(), Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>()));
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
            {
                expectedRemovedProfile
            });

            MacroStabilityInwardsStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                newProfile
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var stochasticProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.5, CreateMacroStabilityInwardsSoilProfile(equalProfileName));
            var stochasticProfileB = new MacroStabilityInwardsStochasticSoilProfile(0.5, CreateMacroStabilityInwardsSoilProfile("nameB"));
            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
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
            var otherStochasticProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.7, new TestSoilProfile(equalProfileName));
            var otherStochasticProfileB = new MacroStabilityInwardsStochasticSoilProfile(0.3, CreateMacroStabilityInwardsSoilProfile("other profile name"));
            var otherModel = new MacroStabilityInwardsStochasticSoilModel(otherName, otherGeometry, new[]
            {
                otherStochasticProfileA,
                otherStochasticProfileB
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            AssertStochasticSoilModelAreEqual(otherModel, model);
            Assert.AreSame(otherGeometry, model.Geometry);

            MacroStabilityInwardsStochasticSoilProfile[] stochasticSoilProfiles = model.StochasticSoilProfiles.ToArray();
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
            const string profileName = "Name of Profile";

            MacroStabilityInwardsSoilProfile1D soilProfileOne =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D(profileName);
            var addedStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfileOne);
            MacroStabilityInwardsStochasticSoilModel otherModel = CreateValidModel(new[]
            {
                addedStochasticSoilProfile
            });

            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D(profileName);
            var existingStochasticSoilProfileOne = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            var existingStochasticSoilProfileTwo = new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile);

            MacroStabilityInwardsStochasticSoilModel model = CreateValidModel(new[]
            {
                existingStochasticSoilProfileOne,
                existingStochasticSoilProfileTwo
            });

            // Call 
            TestDelegate call = () => model.Update(otherModel);

            // Assert 
            Assert.Throws<InvalidOperationException>(call);

            Assert.AreEqual(1, otherModel.StochasticSoilProfiles.Count());
            Assert.AreEqual(addedStochasticSoilProfile, otherModel.StochasticSoilProfiles.First());

            Assert.AreEqual(2, model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEqual(new[]
            {
                existingStochasticSoilProfileOne,
                existingStochasticSoilProfileTwo
            }, model.StochasticSoilProfiles);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var random = new Random(21);
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel(name,
                                                                                   new[]
                                                                                   {
                                                                                       new Point2D(random.NextDouble(), random.NextDouble())
                                                                                   },
                                                                                   new[]
                                                                                   {
                                                                                       new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), new TestSoilProfile())
                                                                                   });

            // Call & Assert
            Assert.AreEqual(name, stochasticSoilModel.ToString());
        }

        private static IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> CreateMacroStabilityInwardsSoilProfile(string name)
        {
            return new TestSoilProfile(name);
        }

        private class TestSoilProfile : IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>
        {
            public TestSoilProfile() {}

            public TestSoilProfile(string name)
            {
                Name = name;
            }

            public string Name { get; } = "";

            public IEnumerable<IMacroStabilityInwardsSoilLayer> Layers { get; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((TestSoilProfile) obj);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            private bool Equals(TestSoilProfile other)
            {
                return string.Equals(Name, other.Name);
            }
        }

        private static IEnumerable<MacroStabilityInwardsSoilLayer1D> CreateLayers1D()
        {
            return new[]
            {
                new MacroStabilityInwardsSoilLayer1D(2)
            };
        }

        private static IEnumerable<MacroStabilityInwardsSoilLayer2D> CreateLayers2D()
        {
            var outerRing = new Ring(new[]
            {
                new Point2D(3, 2),
                new Point2D(3, 5)
            });
            return new[]
            {
                new MacroStabilityInwardsSoilLayer2D(outerRing)
            };
        }

        private static MacroStabilityInwardsStochasticSoilModel CreateValidModel(IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles)
        {
            var random = new Random(21);
            return new MacroStabilityInwardsStochasticSoilModel("name",
                                                                new[]
                                                                {
                                                                    new Point2D(random.NextDouble(), random.NextDouble())
                                                                }, stochasticSoilProfiles);
        }

        private static MacroStabilityInwardsStochasticSoilProfile CreateStochasticSoilProfile()
        {
            var random = new Random(21);
            return new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), new TestSoilProfile());
        }

        private static void AssertStochasticSoilModelAreEqual(MacroStabilityInwardsStochasticSoilModel expected,
                                                              MacroStabilityInwardsStochasticSoilModel actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            CollectionAssert.AreEqual(expected.Geometry, actual.Geometry);
            CollectionAssert.AreEqual(expected.StochasticSoilProfiles, actual.StochasticSoilProfiles);
        }
    }
}