// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using NSubstitute;
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
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var hasSurfaceLineProperty = Substitute.For<IHasSurfaceLineProperty>();

            hasSurfaceLineProperty.SurfaceLine.Returns(new PipingSurfaceLine("1"));
            hasSurfaceLineProperty.GetAvailableSurfaceLines().Returns(new[]
            {
                new PipingSurfaceLine("2")
            });

            var editor = new PipingInputContextSurfaceLineSelectionEditor<IHasSurfaceLineProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasSurfaceLineProperty);
            
            
            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var hasSurfaceLineProperty = Substitute.For<IHasSurfaceLineProperty>();
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            hasSurfaceLineProperty.SurfaceLine.Returns(surfaceLine);
            hasSurfaceLineProperty.GetAvailableSurfaceLines().Returns(new[]
            {
                surfaceLine
            });

            var editor = new PipingInputContextSurfaceLineSelectionEditor<IHasSurfaceLineProperty>();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(hasSurfaceLineProperty);

            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(surfaceLine, result);
        }
    }
}