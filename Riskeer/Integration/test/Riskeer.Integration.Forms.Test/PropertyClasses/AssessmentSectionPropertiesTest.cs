// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.ComponentModel;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionPropertiesTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Id).Return("1");
            mocks.ReplayAll();

            assessmentSection.Name = "test";

            // Call
            var properties = new AssessmentSectionProperties(assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IAssessmentSection>>(properties);
            Assert.AreSame(assessmentSection, properties.Data);
            Assert.AreEqual(assessmentSection.Id, properties.Id);
            Assert.AreEqual(assessmentSection.Name, properties.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            const string newName = "Test";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());
            mocks.ReplayAll();

            var properties = new AssessmentSectionProperties(assessmentSection);

            // Call
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, assessmentSection.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var properties = new AssessmentSectionProperties(assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor idProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            generalCategoryName,
                                                                            "ID",
                                                                            "ID van het traject.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "Naam van het traject.");
            mocks.VerifyAll();
        }
    }
}