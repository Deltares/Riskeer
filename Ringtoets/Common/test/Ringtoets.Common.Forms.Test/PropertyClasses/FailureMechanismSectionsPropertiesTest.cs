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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionsPropertiesTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionsProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            var properties = new FailureMechanismSectionsProperties(sections);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IEnumerable<FailureMechanismSection>>>(properties);
            Assert.AreSame(sections, properties.Data);

            TestHelper.AssertTypeConverter<FailureMechanismSectionsProperties, ExpandableArrayConverter>(
                nameof(FailureMechanismSectionsProperties.Sections));
            Assert.IsNotNull(properties.Sections);
            Assert.AreEqual(sections.Count(), properties.Sections.Length);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();

            // Call
            var properties = new FailureMechanismSectionsProperties(sections);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor sectionsProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionsProperty,
                                                                            "Algemeen",
                                                                            "Vakindeling",
                                                                            "Vakindeling waarmee de waterkering voor dit toetsspoor is geschematiseerd ten behoeve van de beoordeling.",
                                                                            true);
        }
    }
}