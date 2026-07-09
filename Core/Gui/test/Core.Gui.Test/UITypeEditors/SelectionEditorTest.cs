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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Core.Gui.PropertyBag;
using Core.Gui.UITypeEditors;
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.UITypeEditors
{
    [TestFixture]
    public class SelectionEditorTest
    {
        [Test]
        public void GetEditStyle_Always_ReturnDropDown()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();

            // Call
            UITypeEditorEditStyle editStyle = editor.GetEditStyle();

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.DropDown, editStyle);
        }

        [Test]
        public void EditValue_NoProviderNoContext_ReturnsOriginalValue()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();
            var someValue = new object();

            // Call
            object result = editor.EditValue(null, null, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_NoContext_ReturnsOriginalValue()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            provider.GetService(Arg.Any<Type>()).Returns(service);

            var someValue = new object();

            // Call
            object result = editor.EditValue(null, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            provider.Received().GetService(Arg.Any<Type>());
        }

        [Test]
        public void EditValue_Always_ReturnsOriginalValue()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            provider.GetService(Arg.Any<Type>()).Returns(service);

            var someValue = new object();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            provider.Received().GetService(Arg.Any<Type>());
            service.Received().DropDownControl(Arg.Any<Control>());
        }

        [Test]
        public void EditValue_NullItem_ReturnsNull()
        {
            var nullItem = new object();
            var editor = new TestSelectionEditor(nullItem);

            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            provider.GetService(Arg.Any<Type>()).Returns(service);

            // Call
            object result = editor.EditValue(context, provider, nullItem);

            // Assert
            Assert.IsNull(result);

            provider.Received().GetService(Arg.Any<Type>());
            service.Received().DropDownControl(Arg.Any<Control>());
            service.Received().CloseDropDown();
        }

        private class TestSelectionEditor : SelectionEditor<IObjectProperties, object>
        {
            public TestSelectionEditor(object nullItem)
            {
                NullItem = nullItem;
            }
        }
    }
}