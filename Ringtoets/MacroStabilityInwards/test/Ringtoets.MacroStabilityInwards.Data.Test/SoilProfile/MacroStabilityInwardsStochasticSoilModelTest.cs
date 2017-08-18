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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelTest
    {
        [Test]
        public void Constructor_WithoutName_ExpectedValues()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("segmentSoilModelName")]
        public void Constructor_WithName_ExpectedValues(string segmentSoilModelName)
        {
            // Call
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel(segmentSoilModelName);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilModel);
            Assert.AreEqual(segmentSoilModelName, stochasticSoilModel.Name);
            CollectionAssert.IsEmpty(stochasticSoilModel.Geometry);
            CollectionAssert.IsEmpty(stochasticSoilModel.StochasticSoilProfiles);
        }

        [Test]
        public void PropertySegmentPoints_Always_ReturnsExpectedValues()
        {
            // Setup
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel(expectedSegmentSoilModelName);
            var point2D = new Point2D(1.0, 2.0);

            // Call
            stochasticSoilModel.Geometry.Add(point2D);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreEqual(1, stochasticSoilModel.Geometry.Count);
            Assert.AreEqual(point2D, stochasticSoilModel.Geometry[0]);
        }

        [Test]
        public void PropertyStochasticSoilProfileProbabilities_Always_ReturnsExpectedValues()
        {
            // Setup
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel(expectedSegmentSoilModelName);

            var mockRepository = new MockRepository();
            var stochasticSoilProfileProbability = mockRepository.StrictMock<MacroStabilityInwardsStochasticSoilProfile>(1.0, null, null);
            mockRepository.ReplayAll();

            // Call
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfileProbability);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreEqual(1, stochasticSoilModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(stochasticSoilProfileProbability, stochasticSoilModel.StochasticSoilProfiles[0]);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var model = new MacroStabilityInwardsStochasticSoilModel("name");

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
            var model = new MacroStabilityInwardsStochasticSoilModel("name");
            model.Geometry.AddRange(new[]
            {
                new Point2D(1, 2),
                new Point2D(4, 5)
            });

            const string expectedName = "otherName";
            var expectedGeometry = new[]
            {
                new Point2D(4, 2)
            };

            var otherModel = new MacroStabilityInwardsStochasticSoilModel(expectedName);
            otherModel.Geometry.AddRange(expectedGeometry);

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();
            MacroStabilityInwardsStochasticSoilModel otherModel = CreateEmptyModel();
            var expectedAddedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestSoilProfile()
            };
            otherModel.StochasticSoilProfiles.Add(expectedAddedProfile);

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(1, otherModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(expectedAddedProfile, otherModel.StochasticSoilProfiles[0]);

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
            var expectedUpdatedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5)
            };
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);
            MacroStabilityInwardsStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -1, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5)
            });

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(1, otherModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(expectedUpdatedProfile, otherModel.StochasticSoilProfiles[0]);

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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5);
            var expectedUpdatedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);

            MacroStabilityInwardsStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5);
            var expectedRemovedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedRemovedProfile);

            MacroStabilityInwardsStochasticSoilModel otherModel = CreateEmptyModel();

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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5);
            var expectedRemovedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            var newProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile2D, 3)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile2D(profileName, CreateLayers2D(), SoilProfileType.SoilProfile2D, -5)
            };
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedRemovedProfile);

            MacroStabilityInwardsStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(newProfile);

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
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();

            var stochasticProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -11)
            {
                SoilProfile = CreateMacroStabilityInwardsSoilProfile1D(equalProfileName)
            };
            var stochasticProfileB = new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -52)
            {
                SoilProfile = CreateMacroStabilityInwardsSoilProfile1D("nameB")
            };
            model.StochasticSoilProfiles.Add(stochasticProfileA);
            model.StochasticSoilProfiles.Add(stochasticProfileB);

            const string otherName = "other name";
            var otherModel = new MacroStabilityInwardsStochasticSoilModel(otherName);

            var otherPointA = new Point2D(2, 0);
            var otherPointB = new Point2D(3, 0);
            otherModel.Geometry.Add(otherPointA);
            otherModel.Geometry.Add(otherPointB);

            var otherStochasticProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(equalProfileName, -1, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(0)
                }, SoilProfileType.SoilProfile1D, -1)
            };
            var otherStochasticProfileB = new MacroStabilityInwardsStochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, -2)
            {
                SoilProfile = CreateMacroStabilityInwardsSoilProfile1D("other profile name")
            };
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileA);
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileB);

            // Call
            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(otherName, model.Name);
            Assert.AreSame(otherPointA, model.Geometry[0]);
            Assert.AreSame(otherPointB, model.Geometry[1]);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            Assert.AreSame(stochasticProfileA, model.StochasticSoilProfiles[0]);
            Assert.AreSame(otherStochasticProfileA.SoilProfile, model.StochasticSoilProfiles[0].SoilProfile);
            Assert.AreNotSame(stochasticProfileB, model.StochasticSoilProfiles[1]);
            Assert.AreSame(otherStochasticProfileB.SoilProfile, model.StochasticSoilProfiles[1].SoilProfile);

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
            var addedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile1D()
            };
            MacroStabilityInwardsStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(addedProfile);

            var existingProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile1D()
            };
            MacroStabilityInwardsStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(existingProfile);
            model.StochasticSoilProfiles.Add(existingProfile);

            // Call 
            TestDelegate call = () => model.Update(otherModel);

            // Assert 
            Assert.Throws<InvalidOperationException>(call);

            Assert.AreEqual(1, otherModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(addedProfile, otherModel.StochasticSoilProfiles[0]);

            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            CollectionAssert.AreEqual(new[]
            {
                existingProfile,
                existingProfile
            }, model.StochasticSoilProfiles);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel(name);

            // Call & Assert
            Assert.AreEqual(name, stochasticSoilModel.ToString());
        }

        private static ISoilProfile CreateMacroStabilityInwardsSoilProfile1D(string name)
        {
            return new MacroStabilityInwardsSoilProfile1D(name, 0.0, new Collection<MacroStabilityInwardsSoilLayer1D>
            {
                new MacroStabilityInwardsSoilLayer1D(0.0)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                }
            }, SoilProfileType.SoilProfile1D, 0);
        }

        private class TestSoilProfile : ISoilProfile
        {
            public string Name { get; } = "";
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
                new MacroStabilityInwardsSoilLayer2D(outerRing, Enumerable.Empty<Ring>().ToArray())
            };
        }

        private static MacroStabilityInwardsStochasticSoilModel CreateEmptyModel()
        {
            var model = new MacroStabilityInwardsStochasticSoilModel("name");
            return model;
        }
    }
}