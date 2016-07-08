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
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test.Property
{
    [TestFixture]
    public class UrlPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ReturnsInstanceOfObjectProperties()
        {
            // Call
            var urlProperties = new WebLinkProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WebLink>>(urlProperties);

        }

        [Test]
        public void Name_WithData_ReturnsName()
        {
            // Setup
            var someName = "some name";
            var otherName = "some other name";

            var url = new WebLink(someName, new Uri("https://www.google.nl"));
            var properties = new WebLinkProperties
            {
                Data = url
            };

            // Call & Assert
            Assert.AreEqual(someName, properties.Name);

            // Call
            properties.Name = otherName;

            // Assert
            Assert.AreEqual(otherName, url.Name);
        }

        [Test]
        public void Path_WithData_ReturnsPath()
        {
            // Setup
            var somePath = "http://www.deltares.nl";
            var otherPath = "http://www.google.nl";

            var url = new WebLink(string.Empty, new Uri(somePath));
            var properties = new WebLinkProperties
            {
                Data = url
            };

            // Call & Assert
            Assert.AreEqual(new Uri(somePath), properties.Path);

            // Call
            properties.Path = otherPath;

            // Assert
            Assert.AreEqual(new Uri(otherPath), url.Path);
        }

        [Test]
        public void GetProperties_Always_ReturnsTwoProperty()
        {
            // Setup
            var someName = "some name";
            var somePath = "http://www.google.nl";
            var url = new WebLink(someName, new Uri(somePath));

            var bag = new DynamicPropertyBag(new WebLinkProperties
            {
                Data = url
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(2, properties.Count);
        }
    }
}