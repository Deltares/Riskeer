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
using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses.StandAlone;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses.StandAlone
{
    [TestFixture]
    public class StandAloneFailureMechanismPropertiesTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StandAloneFailureMechanismProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var properties = new StandAloneFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithIsRelevant_ReturnCorrectPropertyValues(bool isRelevant)
        {
            // Setup
            var failureMechanism = new TestFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new StandAloneFailureMechanismProperties(failureMechanism);

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(isRelevant, properties.IsRelevant);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(bool isRelevant)
        {
            // Setup
            var failureMechanism = new TestFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new StandAloneFailureMechanismProperties(failureMechanism);

            // Assert
            const string generalCategory = "Algemeen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);
        }
    }
}