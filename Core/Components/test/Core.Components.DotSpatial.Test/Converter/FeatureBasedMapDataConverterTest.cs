// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using NUnit.Framework;
using Rhino.Mocks;
using Point = DotSpatial.Topology.Point;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class FeatureBasedMapDataConverterTest
    {
        [Test]
        public void ConvertLayerFeatures_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapLayer = new TestFeatureLayer();

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(null, mapLayer);

            // Assert
            const string expectedMessage = "Null data cannot be converted into feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerFeatures_TargetLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertLayerFeatures(mapData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapLayer = new TestFeatureLayer();

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(null, mapLayer);

            // Assert
            const string expectedMessage = "Null data cannot be converted into feature layer data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerProperties_TargetLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertLayerProperties(mapData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertLayerFeatures_MapDataWithMetaData_MetaDataSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapLayer = new TestFeatureLayer();
            var mapData = new TestFeatureBasedMapData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                "Id", 1.1
                            },
                            {
                                "Name", "Feature 1"
                            },
                            {
                                "Extra 1", "Feature 1 extra"
                            }
                        }
                    },
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                "Id", 2.2
                            },
                            {
                                "Name", "Feature 2"
                            },
                            {
                                "Extra 2", "Feature 2 extra"
                            }
                        }
                    }
                }
            };

            // Call
            testConverter.ConvertLayerFeatures(mapData, mapLayer);

            // Assert
            DataColumnCollection dataColumnCollection = mapLayer.DataSet.DataTable.Columns;
            Assert.AreEqual(4, dataColumnCollection.Count);
            Assert.AreEqual("1", dataColumnCollection[0].ToString());
            Assert.AreEqual("2", dataColumnCollection[1].ToString());
            Assert.AreEqual("3", dataColumnCollection[2].ToString());
            Assert.AreEqual("4", dataColumnCollection[3].ToString());

            DataRowCollection dataRowCollection = mapLayer.DataSet.DataTable.Rows;
            Assert.AreEqual(2, dataRowCollection.Count);
            Assert.AreEqual(1.1.ToString(CultureInfo.CurrentCulture), dataRowCollection[0][0]);
            Assert.AreEqual("Feature 1", dataRowCollection[0][1]);
            Assert.AreEqual("Feature 1 extra", dataRowCollection[0][2]);
            Assert.IsEmpty(dataRowCollection[0][3].ToString());
            Assert.AreEqual(2.2.ToString(CultureInfo.CurrentCulture), dataRowCollection[1][0]);
            Assert.AreEqual("Feature 2", dataRowCollection[1][1]);
            Assert.IsEmpty(dataRowCollection[1][2].ToString());
            Assert.AreEqual("Feature 2 extra", dataRowCollection[1][3]);
        }

        [Test]
        public void ConvertLayerFeatures_LayerInSameCoordinateSystemAsMapData_CreatedPointIsNotReprojected()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapLayer = new TestFeatureLayer
            {
                Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem
            };
            var mapData = new TestFeatureBasedMapData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>()) // TestFeatureBasedMapDataConverter will generate a Point feature at (1.1, 2.2)
                }
            };

            // Call
            testConverter.ConvertLayerFeatures(mapData, mapLayer);

            // Assert
            Assert.AreEqual(1, mapLayer.FeatureSet.Features.Count);
            Assert.AreEqual(1.1, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
            Assert.AreEqual(2.2, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
            Assert.AreEqual(MapDataConstants.FeatureBasedMapDataCoordinateSystem, mapLayer.Projection);
        }

        [Test]
        public void ConvertLayerFeatures_LayerWithoutCoordinateSystem_CreatePointFeatureInMapDataCoordinateSystem()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapLayer = new TestFeatureLayer
            {
                Projection = null
            };
            var mapData = new TestFeatureBasedMapData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>()) // TestFeatureBasedMapDataConverter will generate a Point feature at (1.1, 2.2)
                }
            };

            // Call
            testConverter.ConvertLayerFeatures(mapData, mapLayer);

            // Assert
            Assert.AreEqual(1, mapLayer.FeatureSet.Features.Count);
            Assert.AreEqual(1.1, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
            Assert.AreEqual(2.2, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
            Assert.AreEqual(MapDataConstants.FeatureBasedMapDataCoordinateSystem, mapLayer.Projection);
        }

        [Test]
        public void ConvertLayerFeatures_LayerInDifferentSystemAsMapData_CreatedPointIsReprojected()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            ProjectionInfo coordinateSystem = KnownCoordinateSystems.Geographic.World.WGS1984;
            var mapLayer = new TestFeatureLayer
            {
                Projection = coordinateSystem
            };
            var mapData = new TestFeatureBasedMapData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>()) // TestFeatureBasedMapDataConverter will generate a Point feature at (1.1, 2.2)
                }
            };

            // Call
            testConverter.ConvertLayerFeatures(mapData, mapLayer);

            // Assert
            Assert.AreEqual(1, mapLayer.FeatureSet.Features.Count);
            Assert.AreEqual(3.3135717854013329, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
            Assert.AreEqual(47.974786294874853, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
            Assert.AreEqual(coordinateSystem, mapLayer.Projection);
        }

        [Test]
        public void ConvertLayerFeatures_LayerWithDataInDifferentCoordinateSystem_CreatedPointIsNotReprojected()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapLayer = new TestFeatureLayer
            {
                Projection = KnownCoordinateSystems.Geographic.World.WGS1984
            };
            var mapData = new TestFeatureBasedMapData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>()) // TestFeatureBasedMapDataConverter will generate a Point feature at (1.1, 2.2)
                }
            };

            testConverter.ConvertLayerFeatures(mapData, mapLayer);
            mapLayer.Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;

            // Call
            testConverter.ConvertLayerFeatures(mapData, mapLayer);

            // Assert
            Assert.AreEqual(1, mapLayer.FeatureSet.Features.Count);
            Assert.AreEqual(1.1, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
            Assert.AreEqual(2.2, mapLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
            Assert.AreEqual(MapDataConstants.FeatureBasedMapDataCoordinateSystem, mapLayer.Projection);
        }

        [Test]
        public void GivenLayerWithConvertedProperties_WhenConvertingLayerFeatures_ThenOnlyDefaultCategoryAdded()
        {
            // Given
            var mocks = new MockRepository();
            var defaultCategory = mocks.Stub<IPointCategory>();
            var categoryOne = mocks.Stub<IPointCategory>();
            var categoryTwo = mocks.Stub<IPointCategory>();
            mocks.ReplayAll();

            const string metadataAttributeName = "Meta";
            var theme = new MapTheme<TestCategoryTheme>(metadataAttributeName, new[]
            {
                new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion())
            });
            var mapData = new TestFeatureBasedMapData("test data", theme)
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                metadataAttributeName, new object()
                            }
                        }
                    }
                }
            };

            var scheme = new PointScheme();
            scheme.Categories.Clear();
            scheme.Categories.Add(categoryOne);
            scheme.Categories.Add(categoryTwo);

            var mapLayer = new TestFeatureLayer
            {
                Symbology = scheme
            };

            var testConverter = new TestFeatureBasedMapDataConverter
            {
                CreatedDefaultCategory = defaultCategory
            };

            // Precondition
            Assert.AreEqual(2, mapLayer.Symbology.Categories.Count);

            // When
            mapData.Features = Enumerable.Empty<MapFeature>();
            testConverter.ConvertLayerFeatures(mapData, mapLayer);

            // Then
            Assert.AreSame(defaultCategory, mapLayer.Symbology.Categories.Single());
        }

        [Test]
        public void ConvertLayerProperties_MapData_NameSetToLayer()
        {
            // Setup
            const string name = "<Some name>";
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData(name);
            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.AreEqual(name, mapLayer.Name);
        }

        [Test]
        public void ConvertLayerProperties_LayerWithoutDataColumns_DefaultLabelLayerSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                "Id", 1.1
                            },
                            {
                                "Name", "Feature"
                            }
                        }
                    }
                },
                SelectedMetaDataAttribute = "Name"
            };
            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.IsNotNull(mapLayer.LabelLayer);
            Assert.AreEqual("FID", mapLayer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(mapLayer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithoutMetaData_DefaultLabelLayerSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                SelectedMetaDataAttribute = "Name"
            };
            var mapLayer = new TestFeatureLayer
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
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.IsNotNull(mapLayer.LabelLayer);
            Assert.AreEqual("FID", mapLayer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(mapLayer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithMetaAndSelectedMetaDataAttributeInDataColumns_CustomLabelLayerSetToLayer()
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                "Id", 1.1
                            },
                            {
                                "Name", "Feature"
                            }
                        }
                    }
                },
                SelectedMetaDataAttribute = "Name"
            };
            var mapLayer = new TestFeatureLayer
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
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.IsNotNull(mapLayer.LabelLayer);
            ILabelCategory labelCategory = mapLayer.LabelLayer.Symbology.Categories[0];
            Assert.AreEqual("FID", labelCategory.Symbolizer.PriorityField);
            Assert.AreEqual(ContentAlignment.MiddleRight, labelCategory.Symbolizer.Orientation);
            Assert.AreEqual(5, labelCategory.Symbolizer.OffsetX);
            Assert.AreEqual("[2]", labelCategory.Expression);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConvertLayerProperties_MapData_ShowLabelsSetToLayer(bool showLabels)
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data")
            {
                ShowLabels = showLabels
            };
            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.AreEqual(showLabels, mapLayer.ShowLabels);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConvertLayerProperties_MapData_IsVisibleSetToLayer(bool isVisible)
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data")
            {
                IsVisible = isVisible
            };
            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.AreEqual(isVisible, mapLayer.IsVisible);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Unknown")]
        public void ConvertLayerProperties_SelectedMetaDataAttributeEmptyOrUnknown_DefaultLabelLayerSetToLayer(string selectedMetaDataAttribute)
        {
            // Setup
            var testConverter = new TestFeatureBasedMapDataConverter();
            var mapData = new TestFeatureBasedMapData("test data")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                "Id", 1.1
                            },
                            {
                                "Name", "Feature"
                            }
                        }
                    }
                },
                SelectedMetaDataAttribute = selectedMetaDataAttribute
            };
            var mapLayer = new TestFeatureLayer
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
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.IsNotNull(mapLayer.LabelLayer);
            Assert.AreEqual("FID", mapLayer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(mapLayer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithMapTheme_UsesCorrectInputsForConversion()
        {
            // Setup
            const string metadataAttributeName = "metaData";
            var theme = new MapTheme<TestCategoryTheme>(metadataAttributeName, new[]
            {
                new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion()),
                new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion())
            });
            var mapData = new TestFeatureBasedMapData("test data", theme)
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                metadataAttributeName, new object()
                            }
                        }
                    }
                }
            };

            var testConverter = new TestFeatureBasedMapDataConverter
            {
                CreatedFeatureScheme = new PointScheme(),
                CreatedDefaultCategory = new PointCategory(),
                CreatedCategoryThemeCategory = new PointCategory()
            };

            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                mapData
            }, testConverter.DefaultCategoryInputs);
            CollectionAssert.AreEqual(theme.CategoryThemes, testConverter.CategoryThemeInputs);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithMapThemeAndNonExistingMetaDataAttribute_SetsExpectedSymbology()
        {
            // Setup
            var mapLayer = new TestFeatureLayer();
            var theme = new MapTheme<TestCategoryTheme>("NonExistingMetaAttribute", new[]
            {
                new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion())
            });
            var mapData = new TestFeatureBasedMapData("test data", theme)
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            var scheme = new PointScheme();
            var defaultCategory = new PointCategory();
            var testConverter = new TestFeatureBasedMapDataConverter
            {
                CreatedFeatureScheme = scheme,
                CreatedDefaultCategory = defaultCategory
            };

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            IPointScheme symbology = mapLayer.Symbology;
            Assert.AreSame(scheme, symbology);
            CollectionAssert.AreEqual(new[]
            {
                defaultCategory
            }, symbology.Categories);
        }

        [Test]
        public void ConvertLayerProperties_MapDataWithMapThemeAndExistingMetaData_SetsExpectedSymbology()
        {
            // Setup
            const string metadataAttributeName = "metaData";
            var theme = new MapTheme<TestCategoryTheme>(metadataAttributeName, new[]
            {
                new TestCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion())
            });
            var mapData = new TestFeatureBasedMapData("test data", theme)
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                metadataAttributeName, new object()
                            }
                        }
                    }
                }
            };

            var scheme = new PointScheme();
            var defaultCategory = new PointCategory();
            var categoryTheme = new PointCategory();
            var testConverter = new TestFeatureBasedMapDataConverter
            {
                CreatedFeatureScheme = scheme,
                CreatedDefaultCategory = defaultCategory,
                CreatedCategoryThemeCategory = categoryTheme
            };

            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            IPointScheme symbology = mapLayer.Symbology;
            Assert.AreSame(scheme, symbology);
            CollectionAssert.AreEqual(new[]
            {
                defaultCategory,
                categoryTheme
            }, symbology.Categories);
        }

        [Test]
        [TestCase(ValueCriterionOperator.EqualValue, "[1] = '{0}'", TestName = "EqualValue")]
        [TestCase(ValueCriterionOperator.UnequalValue, "NOT [1] = '{0}'", TestName = "UnequalValue")]
        public void ConvertLayerProperties_MapDataWithMapThemeAndVaryingValueCriteria_SetsCorrectFilterExpression(ValueCriterionOperator criterionOperator,
                                                                                                                  string expressionFormat)
        {
            // Setup
            const string metadataAttributeName = "Meta";
            const string value = "test value";

            var valueCriterion = new ValueCriterion(criterionOperator,
                                                    value);
            var theme = new MapTheme<TestCategoryTheme>(metadataAttributeName, new[]
            {
                new TestCategoryTheme(valueCriterion)
            });

            var mapData = new TestFeatureBasedMapData("test data", theme)
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                    {
                        MetaData =
                        {
                            {
                                metadataAttributeName, new object()
                            }
                        }
                    }
                }
            };

            var featureScheme = new PointScheme();
            var defaultCategory = new PointCategory();
            var category = new PointCategory();
            var testConverter = new TestFeatureBasedMapDataConverter
            {
                CreatedFeatureScheme = featureScheme,
                CreatedDefaultCategory = defaultCategory,
                CreatedCategoryThemeCategory = category
            };

            var mapLayer = new TestFeatureLayer();

            // Call
            testConverter.ConvertLayerProperties(mapData, mapLayer);

            // Assert
            Assert.IsNull(defaultCategory.FilterExpression);
            string expectedFilterExpression = string.Format(expressionFormat, value);
            Assert.AreEqual(expectedFilterExpression, category.FilterExpression);
        }

        private class TestFeatureLayer : MapPointLayer
        {
            public TestFeatureLayer() : base(new FeatureSet())
            {
                Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;
            }
        }

        private class TestCategoryTheme : CategoryTheme
        {
            public TestCategoryTheme(ValueCriterion criterion) : base(criterion) {}
        }

        private class TestFeatureBasedMapData : FeatureBasedMapData<TestCategoryTheme>
        {
            public TestFeatureBasedMapData(string name) : base(name) {}

            public TestFeatureBasedMapData(string name, MapTheme<TestCategoryTheme> theme) : base(name, theme) {}
        }

        private class TestFeatureBasedMapDataConverter : FeatureBasedMapDataConverter<TestFeatureBasedMapData, TestFeatureLayer, TestCategoryTheme>
        {
            private readonly List<TestCategoryTheme> categoryThemeInputs;

            private readonly List<TestFeatureBasedMapData> defaultCategoryInputs;

            public TestFeatureBasedMapDataConverter()
            {
                categoryThemeInputs = new List<TestCategoryTheme>();
                defaultCategoryInputs = new List<TestFeatureBasedMapData>();
            }

            public IFeatureCategory CreatedDefaultCategory { private get; set; }

            public IFeatureScheme CreatedFeatureScheme { private get; set; }

            public IFeatureCategory CreatedCategoryThemeCategory { private get; set; }

            public IEnumerable<TestCategoryTheme> CategoryThemeInputs
            {
                get
                {
                    return categoryThemeInputs;
                }
            }

            public IEnumerable<TestFeatureBasedMapData> DefaultCategoryInputs
            {
                get
                {
                    return defaultCategoryInputs;
                }
            }

            protected override IFeatureSymbolizer CreateSymbolizer(TestFeatureBasedMapData mapData)
            {
                return new PointSymbolizer();
            }

            protected override IFeatureCategory CreateDefaultCategory(TestFeatureBasedMapData mapData)
            {
                defaultCategoryInputs.Add(mapData);
                return CreatedDefaultCategory;
            }

            protected override IFeatureScheme CreateScheme()
            {
                return CreatedFeatureScheme;
            }

            protected override IFeatureCategory CreateFeatureCategory(TestCategoryTheme categoryTheme)
            {
                categoryThemeInputs.Add(categoryTheme);
                return CreatedCategoryThemeCategory;
            }

            protected override IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature)
            {
                yield return new Feature(new Point(1.1, 2.2));
            }
        }
    }
}