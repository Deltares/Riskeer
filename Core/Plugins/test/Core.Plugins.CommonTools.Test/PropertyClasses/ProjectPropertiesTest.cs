// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Plugins.CommonTools.PropertyClasses;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test.PropertyClasses
{
    [TestFixture]
    public class ProjectPropertiesTest
    {
        [Test]
        public void Name_WithData_ReturnsName()
        {
            // Setup
            var project = new TestProject();
            var properties = new ProjectProperties
            {
                Data = project
            };

            var testName = "some name";
            project.Name = testName;

            // Call
            var result = properties.Name;

            // Assert
            Assert.AreEqual(testName, result);
        }

        [Test]
        public void Description_WithData_ReturnsDescription()
        {
            // Setup
            var project = new TestProject();
            var properties = new ProjectProperties
            {
                Data = project
            };

            var testDescription = "some description";
            var anotherDescription = "another description";

            project.Description = testDescription;

            // Call
            var result = properties.Description;

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
            var data = new TestProject();

            var bag = new DynamicPropertyBag(new ProjectProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(2, properties.Count);
        }

        private class TestProject : IProject {
            public long StorageId { get; set; }
            public void Attach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void Detach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void NotifyObservers()
            {
                throw new NotImplementedException();
            }

            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}