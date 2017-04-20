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
            var propertyResolverStub = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            // Call
            using (var propertyGridView = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolverStub))
            {
                // Assert
                Assert.IsInstanceOf<PropertyGrid>(propertyGridView);
                Assert.IsInstanceOf<IView>(propertyGridView);
                Assert.IsInstanceOf<IObserver>(propertyGridView);
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
            var propertyResolverStub = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            using (var form = new Form())
            using (var propertyGridView = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolverStub))
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
            var propertyResolverMock = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolverMock.Expect(prs => prs.GetObjectProperties(dataObject)).Return(null);
            propertyResolverMock.Expect(prs => prs.GetObjectProperties(null)).Return(null);
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolverMock))
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
            var propertyResolverMock = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolverMock.Expect(prs => prs.GetObjectProperties(dataObject)).Return(null);
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolverMock))
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
        public void GivenPropertyGridViewWithObservableSet_WhenNotifyObserverCalled_ThenRefreshTriggered()
        {
            // Given
            var dataObject = new object();
            var observerable = new SimpleObservable();

            var mockRepository = new MockRepository();
            var objectPropertiesStub = mockRepository.Stub<IObjectProperties>();
            objectPropertiesStub.Data = observerable;

            var propertyResolverMock = mockRepository.StrictMock<IPropertyResolver>();
            propertyResolverMock.Expect(prs => prs.GetObjectProperties(dataObject)).Return(new DynamicPropertyBag(objectPropertiesStub));
            mockRepository.ReplayAll();

            using (var propertyGridView = new TestGuiPropertyGridView(propertyResolverMock))
            {
                // When
                propertyGridView.Data = dataObject;

                // Then
                observerable.NotifyObservers();
                Assert.AreEqual(1, propertyGridView.RefreshCalled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var propertyResolverStub = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () =>
            {
                using (var control = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolverStub))
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

        private class SimpleObservable : Observable {}
    }
}