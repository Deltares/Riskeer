// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
            var hasSurfaceLineProperty = mockRepository.Stub<IHasSurfaceLineProperty>();

            hasSurfaceLineProperty.Stub(hslp => hslp.SurfaceLine).Return(new PipingSurfaceLine("1"));
            hasSurfaceLineProperty.Stub(hslp => hslp.GetAvailableSurfaceLines()).Return(new[]
            {
                new PipingSurfaceLine("2")
            });

            var editor = new PipingInputContextSurfaceLineSelectionEditor<IHasSurfaceLineProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasSurfaceLineProperty);

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
            var hasSurfaceLineProperty = mockRepository.Stub<IHasSurfaceLineProperty>();
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            hasSurfaceLineProperty.Stub(hslp => hslp.SurfaceLine).Return(surfaceLine);
            hasSurfaceLineProperty.Stub(hslp => hslp.GetAvailableSurfaceLines()).Return(new[]
            {
                surfaceLine
            });

            var editor = new PipingInputContextSurfaceLineSelectionEditor<IHasSurfaceLineProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasSurfaceLineProperty);

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
    }
}