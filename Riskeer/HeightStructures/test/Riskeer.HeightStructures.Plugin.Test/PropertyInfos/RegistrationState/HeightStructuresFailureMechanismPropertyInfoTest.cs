﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Linq;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PresentationObjects.RegistrationState;
using Riskeer.HeightStructures.Forms.PropertyClasses.RegistrationState;

namespace Riskeer.HeightStructures.Plugin.Test.PropertyInfos.RegistrationState
{
    [TestFixture]
    public class HeightStructuresFailureMechanismPropertyInfoTest
    {
        private HeightStructuresPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new HeightStructuresPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(HeightStructuresFailureMechanismProperties));
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
            Assert.AreEqual(typeof(HeightStructuresFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(HeightStructuresFailureMechanismProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_WithContext_NewPropertiesWithFailureMechanismAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<HeightStructuresFailureMechanismProperties>(objectProperties);
            Assert.AreSame(failureMechanism, objectProperties.Data);

            mocks.VerifyAll();
        }
    }
}