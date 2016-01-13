﻿using System;
using System.ComponentModel;
using Core.Common.Gui;
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
            var urlProperties = new UrlProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<Url>>(urlProperties);

        }

        [Test]
        public void Name_WithData_ReturnsName()
        {
            // Setup
            var someName = "some name";
            var otherName = "some other name";

            var url = new Url(someName, string.Empty);
            var properties = new UrlProperties
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
            var somePath = "some path";
            var otherPath = "some path";

            var url = new Url(string.Empty, somePath);
            var properties = new UrlProperties
            {
                Data = url
            };

            // Call & Assert
            Assert.AreEqual(otherPath, properties.Path);

            // Call
            properties.Path = otherPath;

            // Assert
            Assert.AreEqual(otherPath, url.Path);
        }

        [Test]
        public void GetProperties_Always_ReturnsTwoProperty()
        {
            // Setup
            var someName = "some name";
            var somePath = "some path";
            var url = new Url(someName, somePath);

            var bag = new DynamicPropertyBag(new UrlProperties
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