using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.ComponentModel;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using GeoAPI.Extensions.Feature;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Extensions.Features;
using NUnit.Framework;
using SharpMap.Data.Providers;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class VectorLayerAttributeTableViewTest
    {
        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void DeleteOfCustomRowObjectMustDeleteOriginalFeature()
        {   
            var features = new List<City>
                                {
                                    new City {Name = "Amsterdam", Population = 1000000, Geometry = new Point(0,0)},
                                    new City {Name = "The Hague", Population = 90000, Geometry = new Point(-20,-40)}
                                };

            var layer = new VectorLayer { DataSource = new FeatureCollection { Features = features } };

            var view = new VectorLayerAttributeTableView { Data = layer };
            view.SetCreateFeatureRowFunction(feature => new CityProperties((City)feature));

            var featureRowObjects = (IList) view.TableView.Data;

            WindowsFormsTestHelper.ShowModal(view, f =>
                {
                    Assert.AreEqual(2, features.Count);
                    featureRowObjects.Remove(featureRowObjects[0]);
                    Assert.AreEqual(1, features.Count);
                });
        }

        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void AddingFeatureMustAlsoAddCustomRowObject()
        {
            var features = new EventedList<City>
                                {
                                    new City {Name = "Amsterdam", Population = 1000000, Geometry = new Point(0,0)}
                                };

            var featureCollection = new FeatureCollection {Features = features};
            var layer = new VectorLayer { DataSource = featureCollection };

            var view = new VectorLayerAttributeTableView { Data = layer };
            view.SetCreateFeatureRowFunction(feature => new CityProperties((City)feature));

            WindowsFormsTestHelper.ShowModal(view, (f) =>
            {
                Assert.AreEqual(1, ((IList)view.TableView.Data).Count);
                featureCollection.Add(new City { Name = "The Hague", Population = 90000, Geometry = new Point(-20, -40) });
                Assert.AreEqual(2, ((IList)view.TableView.Data).Count);
            });
        }

        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void ShowAndCheckDynamicReadOnly()
        {
            var features = new []
                                {
                                    new State {Name = "Amsterdam", Gouvernor = "Piet"},
                                    new State {Name = "The Hague", Gouvernor = "Jan", ReadOnly = true}
                                };

            var layer = new VectorLayer {DataSource = new FeatureCollection {Features = features}};
            var view = new VectorLayerAttributeTableView { Data = layer };

            WindowsFormsTestHelper.ShowModal(view, f =>
                {
                    Assert.IsFalse(view.TableView.CellIsReadOnly(0, view.TableView.Columns[1]));
                    Assert.IsTrue(view.TableView.CellIsReadOnly(1, view.TableView.Columns[1]));
                });
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
            private City city;

            public CityProperties(City city)
            {
                this.city = (City) city;
            }

            [DisplayName("Name (read-only)")]
            public string Name { get { return city.Name; } }

            [DisplayFormat("0 people")]
            public int Population { get { return city.Population; } set { city.Population = value; } }

            public IFeature GetFeature()
            {
                return city;
            }
        }
    }
}