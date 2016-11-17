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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.PropertyGridView;
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
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetSameDataObject_NoRedundantViewUpdate()
        {
            // Setup
            var mockRepository = new MockRepository();
            var propertyResolverStub = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            var dataObject = new object();

            using (var propertyGridView = new Gui.Forms.PropertyGridView.PropertyGridView(propertyResolverStub))
            {
                propertyGridView.Data = dataObject;

                var selectedObject = propertyGridView.SelectedObject;

                // Call
                propertyGridView.Data = dataObject;

                // Assert
                Assert.AreSame(selectedObject, propertyGridView.SelectedObject);
            }
            mockRepository.VerifyAll();
        }
    }
}