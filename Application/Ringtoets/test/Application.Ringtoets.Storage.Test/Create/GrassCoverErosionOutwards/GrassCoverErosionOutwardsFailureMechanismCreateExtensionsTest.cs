﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionOutwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionOutwards
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollectorAndPropertiesSet_ReturnsFailureMechanismEntityWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = isRelevant,
                Comments = "Some text"
            };
            var registry = new PersistenceRegistry();

            // Call
            var entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) FailureMechanismType.GrassRevetmentErosionOutwards, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.AreEqual(failureMechanism.Comments, entity.Comments);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalComments = "Some text";
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Comments = originalComments
            };
            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.AreNotSame(originalComments, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(originalComments, entity.Comments);
        }

        [Test]
        public void Create_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsEmpty(entity.FailureMechanismSectionEntities);
        }

        [Test]
        public void Create_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new TestFailureMechanismSection());

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities).Count());
        }

        [Test]
        public void Create_WithoutForeshoreProfiles_EmptyForeshoreProfilesEntities()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.IsEmpty(entity.ForeshoreProfileEntities);
        }

        [Test]
        public void Create_WithForeshoreProfiles_ForeshoreProfilesEntitiesCreated()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.ForeshoreProfiles.Add(new TestForeshoreProfile());

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(1, entity.ForeshoreProfileEntities.Count);
        }

        [Test]
        public void Create_WithUpdatedN_FailureMechanismMetaUpdated()
        {
            // Setup
            var n = new Random(21).Next(1, 20);
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.GeneralInput.N = n;

            // Call
            var entity = failureMechanism.Create(new PersistenceRegistry());

            // Assert
            Assert.AreEqual(n, entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.First().N);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCalculationGroup_ReturnFailureMechanismEntityWithCalculationGroupEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup("A", true));
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup("B", true));

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.WaveConditionsCalculationGroup.Name, entity.CalculationGroupEntity.Name);
            Assert.AreEqual(Convert.ToByte(failureMechanism.WaveConditionsCalculationGroup.IsNameEditable), entity.CalculationGroupEntity.IsEditable);
            Assert.AreEqual(0, entity.CalculationGroupEntity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity.CalculationGroupEntity1
                                                                .OrderBy(cge => cge.Order)
                                                                .ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual("A", childGroupEntities[0].Name);
            Assert.AreEqual(1, childGroupEntities[0].IsEditable);
            Assert.AreEqual(0, childGroupEntities[0].Order);
            Assert.AreEqual("B", childGroupEntities[1].Name);
            Assert.AreEqual(1, childGroupEntities[1].IsEditable);
            Assert.AreEqual(1, childGroupEntities[1].Order);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithGrassCoverErosionOutwardHydraulicBoundaryLocation_ReturnFailureMechanismEntityWithCalculationGroupEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.HydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(0, "A", 0, 0));
            failureMechanism.HydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(1, "B", 0, 0));

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.GrassCoverErosionOutwardsHydraulicLocationEntities.Count);

            var firstLocation = entity.GrassCoverErosionOutwardsHydraulicLocationEntities.ElementAt(0);
            var secondLocation = entity.GrassCoverErosionOutwardsHydraulicLocationEntities.ElementAt(1);
            Assert.AreEqual("A", firstLocation.Name);
            Assert.AreEqual(0, firstLocation.LocationId);
            Assert.AreEqual(0, firstLocation.Order);
            Assert.AreEqual("B", secondLocation.Name);
            Assert.AreEqual(1, secondLocation.LocationId);
            Assert.AreEqual(1, secondLocation.Order);
        }
    }
}