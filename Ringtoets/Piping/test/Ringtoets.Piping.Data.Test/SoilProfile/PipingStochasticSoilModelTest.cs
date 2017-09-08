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
using Core.Common.Base;
using Core.Common.Base.Geometry;
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
            TestDelegate test = () => new PipingStochasticSoilModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var model = new PipingStochasticSoilModel(name);

            // Assert
            Assert.IsInstanceOf<Observable>(model);
            Assert.IsInstanceOf<IMechanismStochasticSoilModel>(model);

            Assert.AreEqual(name, model.Name);
            CollectionAssert.IsEmpty(model.Geometry);
            CollectionAssert.IsEmpty(model.StochasticSoilProfiles);
            Assert.AreEqual(name, model.ToString());
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var model = new PipingStochasticSoilModel("name");

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
            var model = new PipingStochasticSoilModel("name");
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

            var otherModel = new PipingStochasticSoilModel(expectedName);
            otherModel.Geometry.AddRange(expectedGeometry);

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
            PipingStochasticSoilModel model = CreateEmptyModel();
            PipingStochasticSoilModel otherModel = CreateEmptyModel();
            var expectedAddedProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            otherModel.StochasticSoilProfiles.Add(expectedAddedProfile);

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var expectedUpdatedProfile = new PipingStochasticSoilProfile(
                0.2,
                new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D));
            PipingStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);
            PipingStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new PipingStochasticSoilProfile(
                                                      0.2,
                                                      new PipingSoilProfile(profileName, -1, CreateLayers(), SoilProfileType.SoilProfile1D)));

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var soilProfile = new PipingSoilProfile(profileName, -2, CreateLayers(), SoilProfileType.SoilProfile1D);
            var expectedUpdatedProfile = new PipingStochasticSoilProfile(0.2, soilProfile);
            PipingStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedUpdatedProfile);

            PipingStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(new PipingStochasticSoilProfile(0.5, soilProfile));

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
            PipingStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedRemovedProfile);

            PipingStochasticSoilModel otherModel = CreateEmptyModel();

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
            PipingStochasticSoilModel model = CreateEmptyModel();
            model.StochasticSoilProfiles.Add(expectedRemovedProfile);

            PipingStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(newProfile);

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
            PipingStochasticSoilModel model = CreateEmptyModel();

            var stochasticProfileA = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile(equalProfileName));
            var stochasticProfileB = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("nameB"));
            model.StochasticSoilProfiles.Add(stochasticProfileA);
            model.StochasticSoilProfiles.Add(stochasticProfileB);

            const string otherName = "other name";
            var otherModel = new PipingStochasticSoilModel(otherName);

            var otherPointA = new Point2D(2, 0);
            var otherPointB = new Point2D(3, 0);
            otherModel.Geometry.Add(otherPointA);
            otherModel.Geometry.Add(otherPointB);

            var otherStochasticProfileA = new PipingStochasticSoilProfile(
                0.7, new PipingSoilProfile(equalProfileName, -1, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D));
            var otherStochasticProfileB = new PipingStochasticSoilProfile(0.3, PipingSoilProfileTestFactory.CreatePipingSoilProfile("other profile name"));
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileA);
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileB);

            // Call
            PipingStochasticSoilModelProfileDifference difference = model.Update(otherModel);

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
            var addedProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            PipingStochasticSoilModel otherModel = CreateEmptyModel();
            otherModel.StochasticSoilProfiles.Add(addedProfile);

            var existingProfile = new PipingStochasticSoilProfile(0.2, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            PipingStochasticSoilModel model = CreateEmptyModel();
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

        private static IEnumerable<PipingSoilLayer> CreateLayers()
        {
            return new[]
            {
                new PipingSoilLayer(2)
            };
        }

        private static PipingStochasticSoilModel CreateEmptyModel()
        {
            return new PipingStochasticSoilModel("name");
        }
    }
}