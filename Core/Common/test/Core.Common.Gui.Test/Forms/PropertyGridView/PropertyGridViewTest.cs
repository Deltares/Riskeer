// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.PropertyGridView
{
    [TestFixture]
    public class PropertyGridViewTest
    {
        [Test]
        public void Constructor_PropertyResolverIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new Gui.Forms.PropertyGridView.PropertyGridView(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyResolver", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var propertyResolver = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            // Call
            using (var propertyGridView = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolver))
            {
                // Assert
                Assert.IsInstanceOf<PropertyGrid>(propertyGridView);
                Assert.IsInstanceOf<IView>(propertyGridView);
                Assert.IsNull(propertyGridView.Data);
                Assert.AreEqual(PropertySort.Categorized, propertyGridView.PropertySort);

                ToolStrip toolStrip = propertyGridView.Controls.OfType<ToolStrip>().First();
                Assert.AreEqual("Gecategoriseerd", toolStrip.Items[0].ToolTipText);
                Assert.AreEqual("Alfabetisch", toolStrip.Items[1].ToolTipText);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Show_ValidParameter_ExpectedProperties()
        {
            // Setup
            var mockRepository = new MockRepository();
            var propertyResolver = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            using (var form = new Form())
            using (var propertyGridView = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolver))
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

            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNull_UpdateView()
        {
            // Setup
            var dataObject = new object();

            var mockRepository = new MockRepository();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject)).Return(null);
            propertyResolver.Expect(prs => prs.GetObjectProperties(null)).Return(null);
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetSameDataObject_NoRedundantViewUpdate()
        {
            // Setup
            var dataObject = new object();

            var mockRepository = new MockRepository();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject)).Return(null);
            mockRepository.ReplayAll();

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

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertyGridViewWithDisposableDataSet_WhenNewDataObjectSet_ThenPreviousDataDisposed()
        {
            // Given
            var mockRepository = new MockRepository();
            var dataObject = new object();
            object dataObjectProperties = mockRepository.StrictMultiMock(typeof(IDisposable), typeof(IObjectProperties));
            dataObjectProperties.Expect(d => ((IDisposable) d).Dispose());
            dataObjectProperties.Expect(d => ((IObjectProperties) d).RefreshRequired += null).IgnoreArguments();
            dataObjectProperties.Expect(d => ((IObjectProperties) d).RefreshRequired -= null).IgnoreArguments();
            dataObjectProperties.Stub(d => ((IObjectProperties) d).Data).Return(dataObject);

            var newDataObject = new object();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject)).Return(new DynamicPropertyBag(dataObjectProperties));
            propertyResolver.Expect(prs => prs.GetObjectProperties(newDataObject)).Return(null);
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver))
            {
                propertyGridView.Data = dataObject;

                // When
                propertyGridView.Data = newDataObject;
            }

            // Then
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertyGridViewWithObservableDataSet_WhenNewDataObjectSet_ThenPreviousDataObserverDetached()
        {
            // Given
            var mockRepository = new MockRepository();
            var observableDataObject = mockRepository.StrictMock<IObservable>();
            observableDataObject.Expect(d => d.Attach(null)).IgnoreArguments();
            observableDataObject.Expect(d => d.Detach(null)).IgnoreArguments();
            var dataObjectProperties = mockRepository.Stub<IObjectProperties>();
            dataObjectProperties.Data = observableDataObject;

            var newDataObject = new object();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(observableDataObject)).Return(new DynamicPropertyBag(dataObjectProperties));
            propertyResolver.Expect(prs => prs.GetObjectProperties(newDataObject)).Return(null);
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver))
            {
                propertyGridView.Data = observableDataObject;

                // When
                propertyGridView.Data = newDataObject;
            }

            // Then
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertyGridViewWithDisposableDataSet_WhenDisposing_ThenObjectPropertiesCorrectlyDisposed()
        {
            // Given
            var mockRepository = new MockRepository();
            var dataObject = new object();
            object dataObjectProperties = mockRepository.StrictMultiMock(typeof(IDisposable), typeof(IObjectProperties));
            dataObjectProperties.Expect(d => ((IDisposable) d).Dispose());
            dataObjectProperties.Expect(d => ((IObjectProperties) d).RefreshRequired += null).IgnoreArguments();
            dataObjectProperties.Expect(d => ((IObjectProperties) d).RefreshRequired -= null).IgnoreArguments();
            dataObjectProperties.Stub(d => ((IObjectProperties) d).Data).Return(dataObject);

            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject)).Return(new DynamicPropertyBag(dataObjectProperties));
            mockRepository.ReplayAll();

            var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject
            };

            // When
            propertyGridView.Dispose();

            // Then
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertyGridViewWithDataSet_WhenRefreshRequiredEventRaised_ThenRefreshTriggered()
        {
            // Given
            var dataObject = new object();

            var mockRepository = new MockRepository();
            var objectProperties = mockRepository.Stub<IObjectProperties>();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject)).Return(new DynamicPropertyBag(objectProperties));
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject
            })
            {
                // When
                objectProperties.Raise(p => p.RefreshRequired += null,
                                       objectProperties,
                                       EventArgs.Empty);

                // Then
                Assert.AreEqual(1, propertyGridView.RefreshCalled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenDisposedPropertyGridViewWithDataSet_WhenRefreshRequiredEventRaised_ThenRefreshNotTriggered()
        {
            // Given
            var dataObject = new object();

            var mockRepository = new MockRepository();
            var objectProperties = mockRepository.Stub<IObjectProperties>();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject)).Return(new DynamicPropertyBag(objectProperties));
            mockRepository.ReplayAll();

            var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject
            };

            propertyGridView.Dispose();

            // When
            objectProperties.Raise(p => p.RefreshRequired += null,
                                   objectProperties,
                                   EventArgs.Empty);

            // Then
            Assert.AreEqual(0, propertyGridView.RefreshCalled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertyGridViewWithNewDataSet_WhenRefreshRequiredEventRaisedOnNewlySetData_ThenRefreshTriggered()
        {
            // Given
            var dataObject1 = new object();
            var dataObject2 = new object();

            var mockRepository = new MockRepository();
            var objectProperties1 = mockRepository.Stub<IObjectProperties>();
            var objectProperties2 = mockRepository.Stub<IObjectProperties>();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject1)).Return(new DynamicPropertyBag(objectProperties1));
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject2)).Return(new DynamicPropertyBag(objectProperties2));
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject1
            })
            {
                propertyGridView.Data = dataObject2;

                // When
                objectProperties2.Raise(p => p.RefreshRequired += null,
                                        objectProperties2,
                                        EventArgs.Empty);

                // Then
                Assert.AreEqual(1, propertyGridView.RefreshCalled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertyGridViewWithNewDataSet_WhenRefreshRequiredEventRaisedOnPreviouslySetData_ThenRefreshNotTriggered()
        {
            // Given
            var dataObject1 = new object();
            var dataObject2 = new object();

            var mockRepository = new MockRepository();
            var objectProperties1 = mockRepository.Stub<IObjectProperties>();
            var objectProperties2 = mockRepository.Stub<IObjectProperties>();
            var propertyResolver = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject1)).Return(new DynamicPropertyBag(objectProperties1));
            propertyResolver.Expect(prs => prs.GetObjectProperties(dataObject2)).Return(new DynamicPropertyBag(objectProperties2));
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolver)
            {
                Data = dataObject1
            })
            {
                propertyGridView.Data = dataObject2;

                // When
                objectProperties1.Raise(p => p.RefreshRequired += null,
                                        objectProperties1,
                                        EventArgs.Empty);

                // Then
                Assert.AreEqual(0, propertyGridView.RefreshCalled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var propertyResolver = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () =>
            {
                using (var control = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolver))
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
            mockRepository.VerifyAll();
        }

        private class TestGuiPropertyGridView : Gui.Forms.PropertyGridView.PropertyGridView
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