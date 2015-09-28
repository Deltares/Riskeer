using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DelftTools.TestUtils;
using DelftTools.Utils.ComponentModel;
using DelftTools.Utils.PropertyBag;
using DelftTools.Utils.PropertyBag.Dynamic;
using NUnit.Framework;
using SharpTestsEx;
using CategoryAttribute = System.ComponentModel.CategoryAttribute;

namespace DelftTools.Utils.Tests.PropertyBag
{
    [TestFixture]
    public class DynamicDataItemPropertyBagTest
    {
        #region Test Classes

        class TestDataItem
        {
            public bool NameIsReadOnly { get; set; }
            public string Name;
        }

        class TestDataItemProperties
        {
            public TestDataItem Item;

            public TestDataItemProperties(TestDataItem item)
            {
                Name = "DataItemName";
                Item = item;
                Linked = "Complex link with something";
            }

            [DynamicReadOnlyValidationMethod] 
            public bool ValidationMethod(string propertyName)
            {
                Assert.IsTrue(propertyName == "Name");

                return Item.NameIsReadOnly;
            }

            [DynamicReadOnly]
            [System.ComponentModel.Category("DataItem")]
            public string Name { get; set; }

            [System.ComponentModel.Category("DataItem")]
            public string Linked { get; set; }
        }

        class TestProperties : INotifyPropertyChange
        {
            /// <summary>
            /// Method checks if propertyName property is read-only (setter can be used).
            /// </summary>
            /// <param name="propertyName"></param>
            /// <returns></returns>
            [DynamicReadOnlyValidationMethod]
            public bool DynamicReadOnlyValidationMethod(string propertyName)
            {
                Assert.IsTrue(propertyName == "Name");

                return IsNameReadOnly;
            }

            /// <summary>
            /// Dynamic property. ReadOnly when IsNameReadOnly true.
            /// </summary>
            [DynamicReadOnly]
            [System.ComponentModel.Category("NameCategory")]
            public string Name { get; set; }

            private bool isNameReadOnly;

            public bool IsNameReadOnly
            {
                get { return isNameReadOnly; }
                set
                {
                    isNameReadOnly = value;

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsNameReadOnly"));
                }
            }

            public bool Visible { get; set; }

            [ReadOnly(true)] //one static property
            public string Description { get; set; }

            public TestProperties()
            {
                Name = "my name";
                Description = "short description";
                Visible = true;
                IsNameReadOnly = true;
            }           

            public event PropertyChangedEventHandler PropertyChanged;
            public event PropertyChangingEventHandler PropertyChanging;
            bool INotifyPropertyChange.HasParent { get; set; }
        }

        #endregion

        [Test]
        public void ReturnsCorrectProperties()
        {
            var testDataItem = new TestDataItem();

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(new TestDataItemProperties(testDataItem), new TestProperties());

            dynamicDataItemPropertyBag.Properties.Count
                .Should("Expected property count wrong").Be.EqualTo(5);
        }

        [Test]
        public void CopiesStaticAttributes()
        {
            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(new TestDataItemProperties(new TestDataItem()), new TestProperties());

            PropertySpec nameProperty = null;
            PropertySpec descriptionProperty = null;

            foreach (PropertySpec spec in dynamicDataItemPropertyBag.Properties)
            {
                if (spec.Name.Equals("Description"))
                {
                    descriptionProperty = spec;
                }
                else if (spec.Name.Equals("Name"))
                {
                    nameProperty = spec;
                }
            }

            // asserts
            nameProperty.Attributes.Any(x => x.GetType() == typeof(CategoryAttribute))
                .Should("Static Category attribute not copied!").Be.True();
            
            descriptionProperty.Attributes
                .Should("Static ReadOnly attribute not copied!").Contain(ReadOnlyAttribute.Yes);
        }

        [Test]
        public void ResolvesDynamicAttributesToNothing()
        {
            var testProperties = new TestProperties {IsNameReadOnly = false};
            var testDataItem = new TestDataItem();
            var testDataItemProperties = new TestDataItemProperties(testDataItem);

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(testDataItemProperties, testProperties);

            var propertyDescriptorCollection = ((ICustomTypeDescriptor) dynamicDataItemPropertyBag).GetProperties();

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);
            var namePropertyValue = namePropertyDescriptor.GetValue(dynamicDataItemPropertyBag);

            // asserts
            namePropertyDescriptor.Attributes.Matches(new DynamicReadOnlyAttribute())
                .Should("Dynamic ReadOnly attribute was not added").Be.True();
            
            namePropertyDescriptor.Attributes.Matches(new ReadOnlyAttribute(true))
                .Should("Inactive dynamic ReadOnly attribute was resolved to static attribute: wrong.").Be.False();
        }

        [Test]
        public void ResolvesDynamicAttributes()
        {
            var testProperties = new TestProperties { IsNameReadOnly = false };
            var testDataItem = new TestDataItem { NameIsReadOnly = true }; //explicit conflicting bool flags
            var testDataItemProperties = new TestDataItemProperties(testDataItem);

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(testDataItemProperties, testProperties);

            var propertyDescriptorCollection = ((ICustomTypeDescriptor)dynamicDataItemPropertyBag).GetProperties();

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            var nameValue = namePropertyDescriptor.GetValue(dynamicDataItemPropertyBag);

            // asserts
            namePropertyDescriptor.Attributes.Matches(new DynamicReadOnlyAttribute())
                .Should("Dynamic ReadOnly attribute was not added").Be.True();
            
            namePropertyDescriptor.Attributes.Matches(new ReadOnlyAttribute(true))
                .Should("Dynamic ReadOnly attribute was resolved to static attribute: wrong.").Be.False();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        [Ignore("Only used for visual inspection")]
        public void ShowsInForm()
        {
            var propertyGrid = new PropertyGrid();
            
            var testProperties = new TestProperties();

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(new TestDataItemProperties(new TestDataItem()), testProperties);
            propertyGrid.SelectedObject = dynamicDataItemPropertyBag;

            testProperties.PropertyChanged += delegate { propertyGrid.SelectedObject = dynamicDataItemPropertyBag; };

            propertyGrid.Dock = DockStyle.Fill;

            WindowsFormsTestHelper.ShowModal(propertyGrid);
        }

        [Test]
        public void PropagatesValueSetterToDataItem()
        {
            var propertyGrid = new PropertyGrid();
            
            var testProperties = new TestProperties {Name = "name"};
            var testDataItemProperties = new TestDataItemProperties(new TestDataItem());

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(testDataItemProperties, testProperties);

            propertyGrid.SelectedObject = dynamicDataItemPropertyBag;
            
            var expected = "newLinked";

            var propertyGridRoot = propertyGrid.SelectedGridItem.Parent.Parent;

            testDataItemProperties.Linked
                .Should("Linked not correctly initialized").Not.Be.EqualTo(expected);

            propertyGridRoot.GridItems[0].GridItems[0].PropertyDescriptor.SetValue(null, expected);

            testDataItemProperties.Linked
                .Should("Linked not correctly set").Be.EqualTo(expected);
        }

        [Test]
        public void PropagatesValueSetterToUnderlyingObjectOnDuplicateProperty()
        {
            var propertyGrid = new PropertyGrid();

            var testProperties = new TestProperties { Name = "name" };
            var testDataItemProperties = new TestDataItemProperties(new TestDataItem());

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(testDataItemProperties, testProperties);

            propertyGrid.SelectedObject = dynamicDataItemPropertyBag;

            var expected = "newName";

            var propertyGridRoot = propertyGrid.SelectedGridItem.Parent.Parent;

            testProperties.Name
                .Should("Name not correctly initialized").Not.Be.EqualTo(expected);

            propertyGridRoot.GridItems[2].GridItems[0].PropertyDescriptor.SetValue(null, expected);

            testProperties.Name
                .Should("Name not correctly set").Be.EqualTo(expected);
        }

        [Test]
        public void PropagatesValueSetterToObject()
        {
            var propertyGrid = new PropertyGrid();

            var testProperties = new TestProperties { Name = "name" };

            testProperties.Visible = false;
            
            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(new TestDataItemProperties(new TestDataItem()), testProperties);

            propertyGrid.SelectedObject = dynamicDataItemPropertyBag;
            
            var propertyGridRoot = propertyGrid.SelectedGridItem.Parent.Parent;

            testProperties.Visible
                .Should("Visible not correctly initialized").Be.False();

            // set visible using property bag
            var items = propertyGridRoot.GridItems[1].GridItems.Cast<GridItem>();

            items.First(item => item.PropertyDescriptor.Name == "Visible").PropertyDescriptor.SetValue(null, true);

            testProperties.Visible
                .Should("Visible not correctly set").Be.True();
        }

        [Test]
        public void AttributesOfActualObjectShouldBeUsedOverDataItemAttributes()
        {
            var testProperties = new TestProperties { IsNameReadOnly = false };
            var testDataItem = new TestDataItem { NameIsReadOnly = true }; //explicit conflicting bool flags
            var testDataItemProperties = new TestDataItemProperties(testDataItem);

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(testDataItemProperties, testProperties);

            var propertyDescriptorCollection = ((ICustomTypeDescriptor)dynamicDataItemPropertyBag).GetProperties();

            var namePropertyDescriptor = propertyDescriptorCollection.Find("Name", false);

            namePropertyDescriptor.Attributes.Matches(new CategoryAttribute("NameCategory")).Should("wrong category taken").Be.True();
        }

        [Test]
        public void PropagatesValueGetterToUnderlyingObjectForDuplicateProperty()
        {
            var propertyGrid = new PropertyGrid();

            var testProperties = new TestProperties { Name = "name" };
            var testDataItemProperties = new TestDataItemProperties(new TestDataItem());

            var dynamicDataItemPropertyBag = new DynamicDataItemPropertyBag(testDataItemProperties, testProperties);

            propertyGrid.SelectedObject = dynamicDataItemPropertyBag;

            var expected = testProperties.Name;

            var propertyGridRoot = propertyGrid.SelectedGridItem.Parent.Parent;

            propertyGridRoot.GridItems[2].GridItems[0].PropertyDescriptor.GetValue(null).
                Should("Name not retrieved from dataitem properties").Be.EqualTo(expected);
        }
    }
}
