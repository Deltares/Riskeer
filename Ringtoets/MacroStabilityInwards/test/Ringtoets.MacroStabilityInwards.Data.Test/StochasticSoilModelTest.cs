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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelTest
    {
        [Test]
        [TestCase("")]
        [TestCase("segmentSoilModelName")]
        public void Constructor_Always_ExpectedValues(string segmentSoilModelName)
        {
            // Call
            var stochasticSoilModel = new StochasticSoilModel(segmentSoilModelName);

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
            var stochasticSoilModel = new StochasticSoilModel(expectedSegmentSoilModelName);
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
            var stochasticSoilModel = new StochasticSoilModel(expectedSegmentSoilModelName);

            var mockRepository = new MockRepository();
            var stochasticSoilProfileProbabilityMock = mockRepository.StrictMock<StochasticSoilProfile>(1.0, null, null);
            mockRepository.ReplayAll();

            // Call
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfileProbabilityMock);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreEqual(1, stochasticSoilModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(stochasticSoilProfileProbabilityMock, stochasticSoilModel.StochasticSoilProfiles[0]);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var model = new StochasticSoilModel("name");

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
            var model = new StochasticSoilModel("name");
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

            var otherModel = new StochasticSoilModel(expectedName);
            otherModel.Geometry.AddRange(expectedGeometry);

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            StochasticSoilModel model = CreateEmptyModel();
            StochasticSoilModel otherModel = CreateEmptyModel();
            var expectedAddedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestSoilProfile()
            };
            otherModel.StochasticSoilProfiles.Add(expectedAddedProfile);

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var expectedUpdatedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -2, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5)
            };
            StochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);
            StochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(profileName, -1, CreateLayers1D(), SoilProfileType.SoilProfile1D, -5)
            });

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var expectedUpdatedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            StochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);

            StochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            });

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var expectedRemovedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            StochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedRemovedProfile);

            StochasticSoilModel otherModel = CreateEmptyModel();

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var expectedRemovedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            var newProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile2D, 3)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile2D(profileName, CreateLayers2D(), SoilProfileType.SoilProfile2D, -5)
            };
            StochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedRemovedProfile);

            StochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(newProfile);

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            StochasticSoilModel model = CreateEmptyModel();

            var stochasticProfileA = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -11)
            {
                SoilProfile = CreateMacroStabilityInwardsSoilProfile1D(equalProfileName)
            };
            var stochasticProfileB = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -52)
            {
                SoilProfile = CreateMacroStabilityInwardsSoilProfile1D("nameB")
            };
            model.StochasticSoilProfiles.Add(stochasticProfileA);
            model.StochasticSoilProfiles.Add(stochasticProfileB);

            const string otherName = "other name";
            var otherModel = new StochasticSoilModel(otherName);

            var otherPointA = new Point2D(2, 0);
            var otherPointB = new Point2D(3, 0);
            otherModel.Geometry.Add(otherPointA);
            otherModel.Geometry.Add(otherPointB);

            var otherStochasticProfileA = new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(equalProfileName, -1, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(0)
                }, SoilProfileType.SoilProfile1D, -1)
            };
            var otherStochasticProfileB = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, -2)
            {
                SoilProfile = CreateMacroStabilityInwardsSoilProfile1D("other profile name")
            };
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileA);
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileB);

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

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

        private static ISoilProfile CreateMacroStabilityInwardsSoilProfile1D(string name)
        {
            return new MacroStabilityInwardsSoilProfile1D(name, 0.0, new Collection<MacroStabilityInwardsSoilLayer1D>
            {
                new MacroStabilityInwardsSoilLayer1D(0.0)
                {
                    IsAquifer = true
                }
            }, SoilProfileType.SoilProfile1D, 0);
        }

        [Test]
        public void Update_ModelsWithAddedProfilesWithSameNames_ThrowsInvalidOperationException()
        {
            // Setup 
            var addedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile1D()
            };
            StochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(addedProfile);

            var existingProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestMacroStabilityInwardsSoilProfile1D()
            };
            StochasticSoilModel model = CreateEmptyModel();
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(name);

            // Call & Assert
            Assert.AreEqual(name, stochasticSoilModel.ToString());
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

        private static StochasticSoilModel CreateEmptyModel()
        {
            var model = new StochasticSoilModel("name");
            return model;
        }
    }
}