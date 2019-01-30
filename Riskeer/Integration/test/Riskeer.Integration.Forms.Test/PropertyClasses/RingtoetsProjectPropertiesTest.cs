// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class RingtoetsProjectPropertiesTest
    {
        [Test]
        public void Name_WithData_ReturnsName()
        {
            // Setup
            var project = new RiskeerProject();
            var properties = new RingtoetsProjectProperties
            {
                Data = project
            };

            const string testName = "some name";
            project.Name = testName;

            // Call
            string result = properties.Name;

            // Assert
            Assert.AreEqual(testName, result);
        }

        [Test]
        public void Description_WithData_ReturnsDescription()
        {
            // Setup
            var project = new RiskeerProject();
            var properties = new RingtoetsProjectProperties
            {
                Data = project
            };

            const string testDescription = "some description";
            const string anotherDescription = "another description";

            project.Description = testDescription;

            // Call
            string result = properties.Description;

            // Assert
            Assert.AreEqual(testDescription, result);

            // Call
            properties.Description = anotherDescription;

            // Assert
            Assert.AreEqual(anotherDescription, project.Description);
        }

        [Test]
        public void GetProperties_Always_ReturnsTwoProperties()
        {
            // Setup
            var properties = new RingtoetsProjectProperties
            {
                Data = new RiskeerProject()
            };

            // Call
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            // Assert
            Assert.AreEqual(2, dynamicProperties.Count);
        }
    }
}