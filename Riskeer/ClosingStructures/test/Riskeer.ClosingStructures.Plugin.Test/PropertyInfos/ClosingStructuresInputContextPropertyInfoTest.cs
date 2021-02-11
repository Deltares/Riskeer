﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.ClosingStructures.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class ClosingStructuresInputContextPropertyInfoTest
    {
        private ClosingStructuresPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(ClosingStructuresInputContextProperties));
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
            Assert.AreEqual(typeof(ClosingStructuresInputContext), info.DataType);
            Assert.AreEqual(typeof(ClosingStructuresInputContextProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_WithContext_NewPropertiesWithFailureMechanismContextAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestClosingStructuresCalculationScenario();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresInputContext(calculation.InputParameters, calculation, failureMechanism, assessmentSection);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<ClosingStructuresInputContextProperties>(objectProperties);
            Assert.AreSame(context, objectProperties.Data);

            mocks.VerifyAll();
        }
    }
}