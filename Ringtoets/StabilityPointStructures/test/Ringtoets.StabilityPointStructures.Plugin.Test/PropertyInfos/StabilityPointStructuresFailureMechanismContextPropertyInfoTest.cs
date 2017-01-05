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

using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismContextPropertyInfoTest
    {
        private StabilityPointStructuresPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new StabilityPointStructuresPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(StabilityPointStructuresFailureMechanismProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(StabilityPointStructuresFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(StabilityPointStructuresFailureMechanismProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_Always_NewPropertiesWithFailureMechanismContextAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var context = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructuresFailureMechanismProperties>(objectProperties);
            Assert.AreSame(failureMechanism, objectProperties.Data);

            mocks.VerifyAll();
        }
    }
}