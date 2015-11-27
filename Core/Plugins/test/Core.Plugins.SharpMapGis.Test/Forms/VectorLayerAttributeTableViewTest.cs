using System.Collections;
using System.Collections.Generic;
using Core.Common.Utils;
using Core.Common.Utils.Collections.Generic;
using Core.Common.Utils.ComponentModel;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.Plugins.SharpMapGis.Gui.Forms;
using NUnit.Framework;

namespace Core.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class VectorLayerAttributeTableViewTest
    {
        [Test]
        public void DeleteOfCustomRowObjectMustDeleteOriginalFeature()
        {
            var features = new List<City>
            {
                new City
                {
                    Name = "Amsterdam", Population = 1000000, Geometry = new Point(0, 0)
                },
                new City
                {
                    Name = "The Hague", Population = 90000, Geometry = new Point(-20, -40)
                }
            };

            var layer = new VectorLayer
            {
                DataSource = new FeatureCollection
                {
                    Features = features
                }
            };

            var view = new VectorLayerAttributeTableView
            {
                Data = layer
            };
            view.SetCreateFeatureRowFunction(feature => new CityProperties((City) feature));

            var featureRowObjects = (IList) view.TableView.Data;

            Assert.AreEqual(2, features.Count);
            featureRowObjects.Remove(featureRowObjects[0]);
            Assert.AreEqual(1, features.Count);
        }

        [Test]
        public void AddingFeatureMustAlsoAddCustomRowObject()
        {
            var features = new EventedList<City>
            {
                new City
                {
                    Name = "Amsterdam", Population = 1000000, Geometry = new Point(0, 0)
                }
            };

            var featureCollection = new FeatureCollection
            {
                Features = features
            };
            var layer = new VectorLayer
            {
                DataSource = featureCollection
            };

            var view = new VectorLayerAttributeTableView
            {
                Data = layer
            };
            view.SetCreateFeatureRowFunction(feature => new CityProperties((City) feature));

            Assert.AreEqual(1, ((IList) view.TableView.Data).Count);
            featureCollection.Add(new City
            {
                Name = "The Hague", Population = 90000, Geometry = new Point(-20, -40)
            });
            Assert.AreEqual(2, ((IList) view.TableView.Data).Count);
        }

        [Test]
        public void ShowAndCheckDynamicReadOnly()
        {
            var features = new[]
            {
                new State
                {
                    Name = "Amsterdam", Gouvernor = "Piet"
                },
                new State
                {
                    Name = "The Hague", Gouvernor = "Jan", ReadOnly = true
                }
            };

            var layer = new VectorLayer
            {
                DataSource = new FeatureCollection
                {
                    Features = features
                }
            };
            var view = new VectorLayerAttributeTableView
            {
                Data = layer
            };

            Assert.IsFalse(view.TableView.CellIsReadOnly(0, view.TableView.Columns[1]));
            Assert.IsTrue(view.TableView.CellIsReadOnly(1, view.TableView.Columns[1]));
        }

        public class City : Feature
        {
            [FeatureAttribute]
            public string Name { get; set; }

            [DisplayFormat("0 people")]
            [FeatureAttribute]
            public int Population { get; set; }
        }

        public class State : Feature
        {
            [FeatureAttribute]
            public string Name { get; set; }

            [FeatureAttribute]
            [DynamicReadOnly]
            public string Gouvernor { get; set; }

            public bool ReadOnly { get; set; }

            [DynamicReadOnlyValidationMethod]
            public bool IsReadOnly(string propertyName)
            {
                return ReadOnly;
            }
        }

        public class CityProperties : IFeatureRowObject
        {
            private readonly City city;

            public CityProperties(City city)
            {
                this.city = city;
            }

            public string Name
            {
                get
                {
                    return city.Name;
                }
            }

            public int Population
            {
                get
                {
                    return city.Population;
                }
                set
                {
                    city.Population = value;
                }
            }

            public IFeature GetFeature()
            {
                return city;
            }
        }
    }
}