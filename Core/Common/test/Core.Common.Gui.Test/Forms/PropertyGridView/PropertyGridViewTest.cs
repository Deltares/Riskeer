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
using Core.Common.Gui.Selection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.PropertyGridView
{
    [TestFixture]
    public class PropertyGridViewTest
    {
        [Test]
        public void Constructor_ApplicationSelectionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var propertyResolverStub = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new Gui.Forms.PropertyGridView.PropertyGridView(null,
                                                                                      propertyResolverStub);
            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("applicationSelection", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyResolverIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var applicationSelectionStub = mockRepository.Stub<IApplicationSelection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new Gui.Forms.PropertyGridView.PropertyGridView(applicationSelectionStub,
                                                                                      null);
            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyResolver", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var applicationSelectionStub = mockRepository.Stub<IApplicationSelection>();
            var propertyResolverStub = mockRepository.Stub<IPropertyResolver>();
            mockRepository.ReplayAll();

            // Call
            using (var propertyGridView = new Gui.Forms.PropertyGridView.PropertyGridView(
                applicationSelectionStub,
                propertyResolverStub))
            {
                // Assert
                Assert.IsInstanceOf<PropertyGrid>(propertyGridView);
                Assert.IsInstanceOf<IView>(propertyGridView);
                Assert.IsInstanceOf<IObserver>(propertyGridView);
                Assert.IsNull(propertyGridView.Data);
            }
            mockRepository.VerifyAll();
        }
    }
}