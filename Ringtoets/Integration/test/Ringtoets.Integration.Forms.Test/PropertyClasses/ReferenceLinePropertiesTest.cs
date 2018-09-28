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

using System;
using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ReferenceLinePropertiesTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReferenceLineProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var properties = new ReferenceLineProperties(assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IAssessmentSection>>(properties);
            TestHelper.AssertTypeConverter<ReferenceLineProperties, ExpandableArrayConverter>(
                nameof(ReferenceLineProperties.Geometry));

            Assert.AreSame(assessmentSection, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_AssessmentSectionWithReferenceLine_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var referenceLine = new ReferenceLine();
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            referenceLine.SetGeometry(geometry);
            assessmentSection.ReferenceLine = referenceLine;
            mocks.ReplayAll();

            // Call
            var properties = new ReferenceLineProperties(assessmentSection);

            // Assert
            Assert.AreEqual(2, properties.Length.NumberOfDecimalPlaces);
            Assert.AreEqual(referenceLine.Length, properties.Length, properties.Length.GetAccuracy());
            CollectionAssert.AreEqual(geometry, properties.Geometry);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionWithReferenceLine_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            assessmentSection.ReferenceLine = new ReferenceLine();

            // Call
            var properties = new ReferenceLineProperties(assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor lengthProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthProperty,
                                                                            generalCategoryName,
                                                                            "Lengte* [m]",
                                                                            "Totale lengte van het traject in meters (afgerond).",
                                                                            true);

            PropertyDescriptor geometryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(geometryProperty,
                                                                            generalCategoryName,
                                                                            "Coördinaten",
                                                                            "Lijst van alle coördinaten (X-coördinaat, Y-coördinaat) " +
                                                                            "die samen de referentielijn vormen.",
                                                                            true);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionWithoutReferenceLine_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var properties = new ReferenceLineProperties(assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(0, dynamicProperties.Count);
            mocks.VerifyAll();
        }
    }
}