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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class FailureMechanismBaseCreateExtensions
    {
        [Test]
        public void CreateFailureMechanismSections_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.CreateFailureMechanismSections(null, new FailureMechanismEntity());

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void CreateFailureMechanismSections_WithoutEntity_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.CreateFailureMechanismSections(new CreateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void CreateFailureMechanismSections_WithoutSections_EmptyFailureMechanismSectionEntities()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var failureMechanismEntity = new FailureMechanismEntity();

            // Call
            failureMechanism.CreateFailureMechanismSections(new CreateConversionCollector(), failureMechanismEntity);

            // Assert
            Assert.IsEmpty(failureMechanismEntity.FailureMechanismSectionEntities);
        }

        [Test]
        public void CreateFailureMechanismSections_WithSections_FailureMechanismSectionEntitiesCreated()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new [] { new Point2D(0,0) }));
            var failureMechanismEntity = new FailureMechanismEntity();

            // Call
            failureMechanism.CreateFailureMechanismSections(new CreateConversionCollector(), failureMechanismEntity);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
        }
    }

    public class TestFailureMechanism : FailureMechanismBase {
        public TestFailureMechanism() : base("", "")
        {}

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ICalculationGroup CalculationsGroup { get; protected set; }
    }
}