// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Piping.Forms.UITypeEditors;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextSurfaceLineSelectionEditorTest
    {
        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();

            var properties = new TestHasSurfaceLineProperty(new PipingSurfaceLine("1"), new[]
            {
                new PipingSurfaceLine("2")
            });

            var editor = new PipingInputContextSurfaceLineSelectionEditor<TestHasSurfaceLineProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            var properties = new TestHasSurfaceLineProperty(surfaceLine, new[]
            {
                surfaceLine
            });

            var editor = new PipingInputContextSurfaceLineSelectionEditor<TestHasSurfaceLineProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(surfaceLine, result);

            mockRepository.VerifyAll();
        }

        private class TestHasSurfaceLineProperty : IHasSurfaceLineProperty
        {
            private readonly IEnumerable<PipingSurfaceLine> availableSurfaceLines;

            public event EventHandler<EventArgs> RefreshRequired;

            public TestHasSurfaceLineProperty(PipingSurfaceLine surfaceLine, IEnumerable<PipingSurfaceLine> availableSurfaceLines)
            {
                this.availableSurfaceLines = availableSurfaceLines;
                SurfaceLine = surfaceLine;
            }

            public object Data { get; set; }

            public PipingSurfaceLine SurfaceLine { get; }

            public IEnumerable<PipingSurfaceLine> GetAvailableSurfaceLines()
            {
                return availableSurfaceLines;
            }
        }
    }
}