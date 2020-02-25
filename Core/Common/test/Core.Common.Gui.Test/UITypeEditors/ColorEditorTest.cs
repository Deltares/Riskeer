// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing.Design;
using Core.Common.Gui.UITypeEditors;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Gui.Test.UITypeEditors
{
    [TestFixture]
    public class ColorEditorTest : NUnitFormTestWithHiddenDesktop
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
            var editor = new ColorEditor();

            // Call
            UITypeEditorEditStyle editStyle = editor.GetEditStyle(null);

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.Modal, editStyle);
        }

        [Test]
        public void EditValue_WithOtherValue_ReturnSameValue()
        {
            // Setup
            var editor = new ColorEditor();
            var value = new object();

            // Call
            object editedValue = editor.EditValue(null, null, value);

            // Assert
            Assert.AreSame(value, editedValue);
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