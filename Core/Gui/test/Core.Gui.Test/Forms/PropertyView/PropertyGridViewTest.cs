// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Gui.Forms.PropertyView;
using Core.Gui.PropertyBag;
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.PropertyView
{
    [TestFixture]
    public class PropertyGridViewTest
    {
        [Test]
        public void Constructor_PropertyResolverIsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PropertyGridView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("propertyResolver", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var propertyResolver = Substitute.For<IPropertyResolver>();

            // Call
            using (var propertyGridView = new PropertyGridView(propertyResolver))
            {
                // Assert
                Assert.IsInstanceOf<PropertyGrid>(propertyGridView);
                Assert.IsInstanceOf<IView>(propertyGridView);
                Assert.IsNull(propertyGridView.Data);
                Assert.AreEqual(PropertySort.Categorized, propertyGridView.PropertySort);
                Assert.AreEqual("PropertiesPanelGridView", propertyGridView.Name);

                ToolStrip toolStrip = propertyGridView.Controls.OfType<ToolStrip>().First();
                Assert.AreEqual("Gecategoriseerd", toolStrip.Items[0].ToolTipText);
                Assert.AreEqual("Alfabetisch", toolStrip.Items[1].ToolTipText);
            }
        }

        [Test]
        public void Show_ValidParameter_ExpectedProperties()
        {
            // Setup
            var propertyResolver = Substitute.For<IPropertyResolver>();

            using (var form = new Form())
            using (var propertyGridView = new PropertyGridView(propertyResolver))
            {
                form.Controls.Add(propertyGridView);

                // Call
                form.Show();

                // Assert
                ToolStrip toolStrip = propertyGridView.Controls.OfType<ToolStrip>().First();
                Assert.AreEqual(5, toolStrip.Items.Count);
                Assert.IsTrue(toolStrip.Items[0].Visible);
                Assert.IsTrue(toolStrip.Items[1].Visible);
                Assert.IsFalse(toolStrip.Items[2].Visible);
                Assert.IsFalse(toolStrip.Items[3].Visible);
                Assert.IsFalse(toolStrip.Items[4].Visible);
            }
        }

        [Test]
        public void Data_SetNull_UpdateView()
        {
            // Setup
            var dataObject = new object();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject).Returns((object) null);
            propertyResolver.GetObjectProperties(null).Returns((object) null);

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver))
            {
                propertyGridView.Data = dataObject;

                object selectedObject = propertyGridView.SelectedObject;

                // Call
                propertyGridView.Data = null;

                // Assert
                Assert.AreSame(selectedObject, propertyGridView.SelectedObject);
                Assert.AreEqual(0, propertyGridView.RefreshCalled);
            }

            propertyResolver.Received(1).GetObjectProperties(dataObject);
            propertyResolver.Received(1).GetObjectProperties(null);
        }

        [Test]
        public void Data_SetSameDataObject_NoRedundantViewUpdate()
        {
            // Setup
            var dataObject = new object();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject).Returns((object) null);

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver))
            {
                propertyGridView.Data = dataObject;

                object selectedObject = propertyGridView.SelectedObject;

                // Call
                propertyGridView.Data = dataObject;

                // Assert
                Assert.AreSame(selectedObject, propertyGridView.SelectedObject);
                Assert.AreEqual(0, propertyGridView.RefreshCalled);
            }

            propertyResolver.Received(1).GetObjectProperties(dataObject);
        }

        [Test]
        public void GivenPropertyGridViewWithDisposableDataSet_WhenNewDataObjectSet_ThenPreviousDataDisposed()
        {
            // Given
            var dataObject = new object();
            var dataObjectProperties = Substitute.For<IObjectProperties, IDisposable>();
            dataObjectProperties.Data.Returns(dataObject);

            var newDataObject = new object();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject).Returns(new DynamicPropertyBag(dataObjectProperties));
            propertyResolver.GetObjectProperties(newDataObject).Returns((object) null);

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver))
            {
                propertyGridView.Data = dataObject;

                // When
                propertyGridView.Data = newDataObject;
            }

            // Then
            ((IDisposable) dataObjectProperties).Received(1).Dispose();
        }

        [Test]
        public void GivenPropertyGridViewWithObservableDataSet_WhenNewDataObjectSet_ThenPreviousDataObserverDetached()
        {
            // Given
            var observableDataObject = Substitute.For<IObservable>();
            var dataObjectProperties = Substitute.For<IObjectProperties>();
            dataObjectProperties.Data = observableDataObject;

            var newDataObject = new object();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(observableDataObject).Returns(new DynamicPropertyBag(dataObjectProperties));
            propertyResolver.GetObjectProperties(newDataObject).Returns((object) null);

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver))
            {
                propertyGridView.Data = observableDataObject;

                // When
                propertyGridView.Data = newDataObject;
            }

            // Then
            observableDataObject.Received(1).Attach(Arg.Any<IObserver>());
            observableDataObject.Received(1).Detach(Arg.Any<IObserver>());
        }

        [Test]
        public void GivenPropertyGridViewWithDisposableDataSet_WhenDisposing_ThenObjectPropertiesCorrectlyDisposed()
        {
            // Given
            var dataObject = new object();
            var dataObjectProperties = Substitute.For<IObjectProperties, IDisposable>();
            dataObjectProperties.Data.Returns(dataObject);

            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject).Returns(new DynamicPropertyBag(dataObjectProperties));

            var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject
            };

            // When
            propertyGridView.Dispose();

            // Then
            ((IDisposable) dataObjectProperties).Received(1).Dispose();
        }

        [Test]
        public void GivenPropertyGridViewWithDataSet_WhenRefreshRequiredEventRaised_ThenRefreshTriggered()
        {
            // Given
            var dataObject = new object();

            var objectProperties = Substitute.For<IObjectProperties>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject).Returns(new DynamicPropertyBag(objectProperties));

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject
            })
            {
                // When
                objectProperties.RefreshRequired += Raise.EventWith(EventArgs.Empty);

                // Then
                Assert.AreEqual(1, propertyGridView.RefreshCalled);
            }
        }

        [Test]
        public void GivenDisposedPropertyGridViewWithDataSet_WhenRefreshRequiredEventRaised_ThenRefreshNotTriggered()
        {
            // Given
            var dataObject = new object();

            var objectProperties = Substitute.For<IObjectProperties>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject).Returns(new DynamicPropertyBag(objectProperties));

            var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject
            };

            propertyGridView.Dispose();

            // When
            objectProperties.RefreshRequired += Raise.EventWith(EventArgs.Empty);

            // Then
            Assert.AreEqual(0, propertyGridView.RefreshCalled);
        }

        [Test]
        public void GivenPropertyGridViewWithNewDataSet_WhenRefreshRequiredEventRaisedOnNewlySetData_ThenRefreshTriggered()
        {
            // Given
            var dataObject1 = new object();
            var dataObject2 = new object();

            var objectProperties1 = Substitute.For<IObjectProperties>();
            var objectProperties2 = Substitute.For<IObjectProperties>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject1).Returns(new DynamicPropertyBag(objectProperties1));
            propertyResolver.GetObjectProperties(dataObject2).Returns(new DynamicPropertyBag(objectProperties2));

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject1
            })
            {
                propertyGridView.Data = dataObject2;

                // When
                objectProperties2.RefreshRequired += Raise.EventWith(EventArgs.Empty);

                // Then
                Assert.AreEqual(1, propertyGridView.RefreshCalled);
            }
        }

        [Test]
        public void GivenPropertyGridViewWithNewDataSet_WhenRefreshRequiredEventRaisedOnPreviouslySetData_ThenRefreshNotTriggered()
        {
            // Given
            var dataObject1 = new object();
            var dataObject2 = new object();

            var objectProperties1 = Substitute.For<IObjectProperties>();
            var objectProperties2 = Substitute.For<IObjectProperties>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(dataObject1).Returns(new DynamicPropertyBag(objectProperties1));
            propertyResolver.GetObjectProperties(dataObject2).Returns(new DynamicPropertyBag(objectProperties2));

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject1
            })
            {
                propertyGridView.Data = dataObject2;

                // When
                objectProperties1.RefreshRequired += Raise.EventWith(EventArgs.Empty);

                // Then
                Assert.AreEqual(0, propertyGridView.RefreshCalled);
            }
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            var propertyResolver = Substitute.For<IPropertyResolver>();

            // Call
            TestDelegate call = () =>
            {
                using (var control = new PropertyGridView(propertyResolver))
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        private class TestGuiPropertyGridView : PropertyGridView
        {
            public TestGuiPropertyGridView(IPropertyResolver propertyResolver) : base(propertyResolver) {}

            public int RefreshCalled { get; private set; }

            public override void Refresh()
            {
                RefreshCalled++;
                base.Refresh();
            }
        }
    }
}