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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismContributionContextPropertiesTest
    {
        private readonly MockRepository mocks = new MockRepository();

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new FailureMechanismContributionContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContribution>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var failureMechanismContributionProperties = new FailureMechanismContributionContextProperties();

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(failureMechanismContributionProperties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(2, dynamicProperties.Count);

            var expectedCategory = "Algemeen";

            PropertyDescriptor compositionProperty = dynamicProperties[0];
            Assert.IsNotNull(compositionProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(compositionProperty,
                                                                            expectedCategory,
                                                                            "Compositie",
                                                                            string.Empty);

            PropertyDescriptor returnPeriodProperty = dynamicProperties[1];
            Assert.IsNotNull(returnPeriodProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(returnPeriodProperty,
                                                                            expectedCategory,
                                                                            "Norm [1/jaar]",
                                                                            string.Empty);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, "Dijk")]
        [TestCase(AssessmentSectionComposition.Dune, "Duin")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, "Dijk / Duin")]
        public void GetProperties_WithData_ReturnExpectedValues(AssessmentSectionComposition composition, string expectedValue)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.Composition).Return(composition);
            mocks.ReplayAll();

            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();

            int returnPeriod = 30000;
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 1.0/returnPeriod);

            // Call
            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            };

            // Assert
            Assert.AreEqual(returnPeriod, properties.ReturnPeriod);
            Assert.AreEqual(expectedValue, properties.AssessmentSectionComposition);
            mocks.VerifyAll();
        }

        [Test]
        public void NormChangeHandler_SetNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismContributionContextProperties()
            {
                NormChangeHandler = null
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void CompositionChangeHandler_SetNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismContributionContextProperties()
            {
                CompositionChangeHandler = null
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }
    }
}