﻿using System;
using System.Linq;
using Core.Common.TestUtil;
using Core.Gis.NetTopologySuite.Extensions.Test.TestObjects;
using Core.GIS.NetTopologySuite.Extensions.Features;
using NUnit.Framework;

namespace Core.Gis.NetTopologySuite.Extensions.Test.Features
{
    [TestFixture]
    public class FeatureAttributeAccessorHelperTest
    {
        [Test]
        public void GetAllAttributes()
        {
            var testFeature = new TestFeature
            {
                Attributes = new DictionaryFeatureAttributeCollection
                {
                    {
                        "attrib", "blah"
                    }
                }
            };

            var allAttributes = FeatureAttributeAccessorHelper.GetFeatureAttributeNames(testFeature);
            Assert.AreEqual(new[]
            {
                "attrib",
                "Other",
                "Name"
            }, allAttributes);
        }

        [Test]
        public void GetUndefinedAttributeSafe()
        {
            var testFeature = new TestFeature();
            testFeature.Attributes = new DictionaryFeatureAttributeCollection();
            var value = FeatureAttributeAccessorHelper.GetAttributeValue(testFeature, "unknown", false);

            Assert.IsNull(value);
        }

        [Test]
        public void GetAttributeShouldBeFast()
        {
            var testFeature = new TestFeature();
            testFeature.Attributes = new DictionaryFeatureAttributeCollection();

            TestHelper.AssertIsFasterThan(70, () =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    FeatureAttributeAccessorHelper.GetAttributeValue(testFeature, "Other", false);
                }
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Can't find attribute unknown for type TestFeature")]
        public void GetUndefinedAttributeUnsafe()
        {
            var testFeature = new TestFeature();
            testFeature.Attributes = new DictionaryFeatureAttributeCollection();
            var value = FeatureAttributeAccessorHelper.GetAttributeValue(testFeature, "unknown");

            Assert.IsNull(value);
        }

        [Test]
        public void GetAttributeNamesOfType()
        {
            var actual = FeatureAttributeAccessorHelper.GetFeatureAttributeNames(typeof(TestFeature)).ToList();
            Assert.AreEqual(new[]
            {
                "Other",
                "Name"
            }, actual);

            var actualInstance = FeatureAttributeAccessorHelper.GetFeatureAttributeNames(new TestFeature()).ToList();
            Assert.AreEqual(new[]
            {
                "Other",
                "Name"
            }, actualInstance);
        }

        [Test]
        public void GetAttributeDisplayName()
        {
            var displayName = FeatureAttributeAccessorHelper.GetPropertyDisplayName(typeof(TestFeature), "Other");
            Assert.AreEqual("Kees", displayName);

            var displayNameInstance = FeatureAttributeAccessorHelper.GetAttributeDisplayName(new TestFeature(), "Other");
            Assert.AreEqual("Kees", displayNameInstance);
        }

        [Test]
        public void GetAttributeExportName()
        {
            var exportName = FeatureAttributeAccessorHelper.GetAttributeExportName(new TestFeature(), "Name");
            Assert.AreEqual("Name", exportName);

            exportName = FeatureAttributeAccessorHelper.GetAttributeExportName(new TestFeature(), "Other");
            Assert.AreEqual("Piet", exportName);
        }

        [Test]
        public void GetAttributeDisplayNameInAttributeCollection()
        {
            var feature = new TestFeature();
            feature.Attributes = new DictionaryFeatureAttributeCollection();
            feature.Attributes["Jan"] = 3;

            var displayName = FeatureAttributeAccessorHelper.GetAttributeDisplayName(feature, "Jan");
            Assert.AreEqual("Jan", displayName);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException))]
        public void GetAttributeDisplayNameNonExistentProperty()
        {
            var feature = new TestFeature();
            feature.Attributes = new DictionaryFeatureAttributeCollection();
            feature.Attributes["Jan"] = 3;

            FeatureAttributeAccessorHelper.GetAttributeDisplayName(feature, "Piet");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException))]
        public void GetAttributeDisplayNameThrowExceptionForNonExistingProperties()
        {
            FeatureAttributeAccessorHelper.GetPropertyDisplayName(typeof(TestFeature), "Blabla");
        }

        [Test]
        public void GetAttributeNamesOfSubclass()
        {
            var actual =
                FeatureAttributeAccessorHelper.GetFeatureAttributeNames(new TestFeatureSubClass()).ToList();
            Assert.AreEqual(new[]
            {
                "Other",
                "Name"
            }, actual);
        }
    }
}