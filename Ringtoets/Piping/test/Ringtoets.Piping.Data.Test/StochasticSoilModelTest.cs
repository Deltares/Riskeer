// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelTest
    {
        [Test]
        [TestCase(1234L, "", "")]
        [TestCase(5678L, "segmentSoilModelName", "segmentName")]
        public void Constructor_Always_ExpectedValues(long segmentSoilModelId, string segmentSoilModelName, string segmentName)
        {
            // Call
            var stochasticSoilModel = new StochasticSoilModel(segmentSoilModelId, segmentSoilModelName, segmentName);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilModel);
            Assert.AreEqual(segmentSoilModelId, stochasticSoilModel.Id);
            Assert.AreEqual(segmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreEqual(segmentName, stochasticSoilModel.SegmentName);
            CollectionAssert.IsEmpty(stochasticSoilModel.Geometry);
            CollectionAssert.IsEmpty(stochasticSoilModel.StochasticSoilProfiles);
        }

        [Test]
        public void PropertySegmentPoints_Always_ReturnsExpectedValues()
        {
            // Setup
            const long expectedSegmentSoilModelId = 1234L;
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            const string expectedSegmentName = "someSegmentName";
            var stochasticSoilModel = new StochasticSoilModel(expectedSegmentSoilModelId, expectedSegmentSoilModelName, expectedSegmentName);
            var point2D = new Point2D(1.0, 2.0);

            // Call
            stochasticSoilModel.Geometry.Add(point2D);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModel.Id);
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreEqual(expectedSegmentName, stochasticSoilModel.SegmentName);
            Assert.AreEqual(1, stochasticSoilModel.Geometry.Count);
            Assert.AreEqual(point2D, stochasticSoilModel.Geometry[0]);
        }

        [Test]
        public void PropertyStochasticSoilProfileProbabilities_Always_ReturnsExpectedValues()
        {
            // Setup
            const long expectedSegmentSoilModelId = 1234L;
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            const string expectedSegmentName = "someSegmentName";
            var stochasticSoilModel = new StochasticSoilModel(expectedSegmentSoilModelId, expectedSegmentSoilModelName, expectedSegmentName);

            var mockRepository = new MockRepository();
            var stochasticSoilProfileProbabilityMock = mockRepository.StrictMock<StochasticSoilProfile>(1.0, null, null);
            mockRepository.ReplayAll();

            // Call
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfileProbabilityMock);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModel.Id);
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModel.Name);
            Assert.AreEqual(expectedSegmentName, stochasticSoilModel.SegmentName);
            Assert.AreEqual(1, stochasticSoilModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(stochasticSoilProfileProbabilityMock, stochasticSoilModel.StochasticSoilProfiles[0]);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var model = new StochasticSoilModel(1234, "name", "segment");

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
            var model = new StochasticSoilModel(1234, "name", "segment");
            model.Geometry.AddRange(new[]
            {
                new Point2D(1, 2),
                new Point2D(4, 5)
            });

            const int expectedId = 1236;
            const string expectedName = "otherName";
            const string expectedSegmentName = "otherSegmentName";
            var expectedGeometry = new[]
            {
                new Point2D(4, 2)
            };

            var otherModel = new StochasticSoilModel(expectedId, expectedName, expectedSegmentName);
            otherModel.Geometry.AddRange(expectedGeometry);

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(expectedId, model.Id);
            Assert.AreEqual(expectedName, model.Name);
            Assert.AreEqual(expectedSegmentName, model.SegmentName);
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
                SoilProfile = new TestPipingSoilProfile()
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
                SoilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D, -5)
            };
            StochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);
            StochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new PipingSoilProfile(profileName, -1, CreateLayers(), SoilProfileType.SoilProfile1D, -5)
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
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D, -5);
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
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D, -5);
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
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D, -5);
            var expectedRemovedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = soilProfile
            };
            var newProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile2D, 3)
            {
                SoilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile2D, -5)
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
                SoilProfile = new TestPipingSoilProfile(equalProfileName)
            };
            var stochasticProfileB = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -52)
            {
                SoilProfile = new TestPipingSoilProfile("nameB")
            };
            model.StochasticSoilProfiles.Add(stochasticProfileA);
            model.StochasticSoilProfiles.Add(stochasticProfileB);

            const string otherName = "other name";
            const string otherSegmentName = "other segment";
            var otherModel = new StochasticSoilModel(41, otherName, otherSegmentName);

            var otherPointA = new Point2D(2, 0);
            var otherPointB = new Point2D(3, 0);
            otherModel.Geometry.Add(otherPointA);
            otherModel.Geometry.Add(otherPointB);

            var otherStochasticProfileA = new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new PipingSoilProfile(equalProfileName, -1, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, -1)
            };
            var otherStochasticProfileB = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, -2)
            {
                SoilProfile = new TestPipingSoilProfile("other profile name")
            };
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileA);
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileB);

            // Call
            StochasticSoilModelProfileDifference difference = model.Update(otherModel);

            // Assert
            Assert.AreEqual(otherName, model.Name);
            Assert.AreEqual(otherSegmentName, model.SegmentName);
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
            var addedProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(addedProfile);

            var existingProfile = new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 3)
            {
                SoilProfile = new TestPipingSoilProfile()
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
            var stochasticSoilModel = new StochasticSoilModel(1, name, "segmentName");

            // Call & Assert
            Assert.AreEqual(name, stochasticSoilModel.ToString());
        }

        private IEnumerable<PipingSoilLayer> CreateLayers()
        {
            return new[]
            {
                new PipingSoilLayer(2)
            };
        }

        private StochasticSoilModel CreateEmptyModel()
        {
            var model = new StochasticSoilModel(1234, "name", "segment");
            return model;
        }
    }
}