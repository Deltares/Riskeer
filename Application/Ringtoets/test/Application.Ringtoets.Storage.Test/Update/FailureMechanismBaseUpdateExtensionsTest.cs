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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class FailureMechanismBaseUpdateExtensionsTest
    {
        [Test]
        public void UpdateFailureMechanismSections_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.UpdateFailureMechanismSections(null, new FailureMechanismEntity(), ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void UpdateFailureMechanismSections_WithoutEntity_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.UpdateFailureMechanismSections(new UpdateConversionCollector(), null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void UpdateFailureMechanismSections_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.UpdateFailureMechanismSections(new UpdateConversionCollector(), new FailureMechanismEntity(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_ContextWithNewFailureMechanismSections_FailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                StorageId = 1
            };
            failureMechanism.AddSection(new FailureMechanismSection("", new [] { new Point2D(0,0) }));
            
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            // Call
            failureMechanism.UpdateFailureMechanismSections(new UpdateConversionCollector(), failureMechanismEntity, ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedFailureMechanismSections_NoNewFailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                StorageId = 1
            };
            var testName = "testName";
            failureMechanism.AddSection(new FailureMechanismSection(testName, new[] { new Point2D(0, 0) })
            {
                StorageId = 1
            });

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1,
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                FailureMechanismSectionEntities = 
                {
                    failureMechanismSectionEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.FailureMechanismSectionEntities.Add(failureMechanismSectionEntity);

            // Call
            failureMechanism.UpdateFailureMechanismSections(new UpdateConversionCollector(), failureMechanismEntity, ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(testName, failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0).Name);

            mocks.VerifyAll();
        } 
    }

    public class TestFailureMechanism : FailureMechanismBase
    {
        public TestFailureMechanism() : base("name", "code")
        {}

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}