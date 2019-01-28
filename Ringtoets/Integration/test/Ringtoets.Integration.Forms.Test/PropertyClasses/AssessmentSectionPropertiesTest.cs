// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new AssessmentSectionProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IAssessmentSection>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "TestId",
                Name = "Test"
            };

            var properties = new AssessmentSectionProperties
            {
                Data = assessmentSection
            };

            // Call & Assert
            Assert.AreEqual(assessmentSection.Id, properties.Id);
            Assert.AreEqual(assessmentSection.Name, properties.Name);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.Attach(projectObserver);

            var properties = new AssessmentSectionProperties
            {
                Data = assessmentSection
            };

            const string newName = "Test";

            // Call
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, assessmentSection.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new AssessmentSectionProperties
            {
                Data = new AssessmentSection(AssessmentSectionComposition.Dike)
            };

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
        }
    }
}