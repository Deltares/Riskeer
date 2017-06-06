// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Core.Common.Gui.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.UITypeEditors
{
    [TestFixture]
    public class ColorEditorTest
    {
        [Test]
        public void DefaultConstructor_ReturnsNewInstance()
        {
            // Call
            var editor = new ColorEditor();

            // Assert
            Assert.IsInstanceOf<UITypeEditor>(editor);
        }

        [Test]
        public void GetEditStyle_Always_ReturnUITypeEditorEditStyleModal()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var editor = new ColorEditor();

            // Call
            UITypeEditorEditStyle editStyle = editor.GetEditStyle(context);

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.Modal, editStyle);
            mocks.VerifyAll();
        }

        [Test]
        public void EditValue_WithoutService_ReturnEqualValue()
        {
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            var provider = mocks.Stub<IServiceProvider>();
            mocks.ReplayAll();

            var editor = new ColorEditor();
            Color color = Color.Beige;

            // Call
            object value = editor.EditValue(context, provider, color);

            // Assert
            Assert.AreEqual(color, value);
            mocks.VerifyAll();
        }

        [Test]
        public void GetPaintValueSupported_Always_ReturnTrue()
        {
            // Setup
            var editor = new ColorEditor();

            // Call
            bool paintValueSupported = editor.GetPaintValueSupported();

            // Assert
            Assert.IsTrue(paintValueSupported);
        }
    }
}
