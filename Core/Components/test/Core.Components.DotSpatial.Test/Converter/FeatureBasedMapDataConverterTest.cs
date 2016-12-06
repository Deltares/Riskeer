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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class FeatureBasedMapDataConverterTest
    {
        [Test]
        public void ConvertLayerFeatures_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(null, new MapPointLayer());

            // Assert
            const string expectedMessage = "Null data cannot be converted into a feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerFeatures_TargetLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<TestFeatureBasedMapData>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(testFeatureBasedMapData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(null, new MapPointLayer());

            // Assert
            const string expectedMessage = "Null data cannot be converted into a feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_TargetLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<TestFeatureBasedMapData>();
            var testFeatureBasedMapData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(testFeatureBasedMapData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_MapData_NameSetToLayer()
        {
            // Setup
            var name = "<Some name>";
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class(name);
            var layer = new MapPointLayer();

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.AreEqual(name, layer.Name);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ConvertLayerProperties_MapData_ShowLabelsSetToLayer(bool showLabels)
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class("test data")
            {
                ShowLabels = showLabels
            };
            var layer = new MapPointLayer();

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.AreEqual(showLabels, layer.ShowLabels);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ConvertLayerProperties_MapData_IsVisibleSetToLayer(bool isVisible)
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class("test data")
            {
                IsVisible = isVisible
            };
            var layer = new MapPointLayer();

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.AreEqual(isVisible, layer.IsVisible);
        }

        [Test]
        public void ConvertLayerProperties_LayerWithoutDataColumns_DefaultLabelLayerSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            { "Id", 1.1 },
                            { "Name", "Feature" }
                        }
                    }
                },
                SelectedMetaDataAttribute = "Name"
            };
            var layer = new MapPointLayer();

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Unknown")]
        public void ConvertLayerProperties_SelectedMetaDataAttributeEmptyOrUnknown_DefaultLabelLayerSetToLayer(string selectedMetaDataAttribute)
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            { "Id", 1.1 },
                            { "Name", "Feature" }
                        }
                    }
                },
                SelectedMetaDataAttribute = selectedMetaDataAttribute
            };
            var layer = new MapPointLayer
            {
                DataSet =
                {
                    DataTable =
                    {
                        Columns =
                        {
                            "1",
                            "2"
                        }
                    }
                }
            };

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithoutMetaData_DefaultLabelLayerSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                SelectedMetaDataAttribute = "Name"
            };
            var layer = new MapPointLayer
            {
                DataSet =
                {
                    DataTable =
                    {
                        Columns =
                        {
                            "1",
                            "2"
                        }
                    }
                }
            };

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithMetaAndSelectedMetaDataAttributeInDataColumns_CustomLabelLayerSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter<Class>();
            var data = new Class("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            { "Id", 1.1 },
                            { "Name", "Feature" }
                        }
                    }
                },
                SelectedMetaDataAttribute = "Name"
            };
            var layer = new MapPointLayer
            {
                DataSet =
                {
                    DataTable =
                    {
                        Columns =
                        {
                            "1",
                            "2"
                        }
                    }
                }
            };

            // Call
            testConverter.ConvertLayerProperties(data, layer);

            // Assert
            Assert.IsNotNull(layer.LabelLayer);
            ILabelCategory labelCategory = layer.LabelLayer.Symbology.Categories[0];
            Assert.AreEqual("FID", labelCategory.Symbolizer.PriorityField);
            Assert.AreEqual(ContentAlignment.MiddleRight, labelCategory.Symbolizer.Orientation);
            Assert.AreEqual(5, labelCategory.Symbolizer.OffsetX);
            Assert.AreEqual(string.Format("[{0}]", "2"), labelCategory.Expression);
        }

        private class Class : FeatureBasedMapData
        {
            public Class(string name) : base(name) {}
        }

        private class TestFeatureBasedMapDataConverter<TFeatureBasedMapData> : FeatureBasedMapDataConverter<TFeatureBasedMapData, MapPointLayer>
            where TFeatureBasedMapData : FeatureBasedMapData
        {
            protected override IFeatureSymbolizer CreateSymbolizer(TFeatureBasedMapData mapData)
            {
                return new PointSymbolizer();
            }

            protected override MapPointLayer CreateLayer()
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
            {
                throw new NotImplementedException();
            }
        }
    }
}