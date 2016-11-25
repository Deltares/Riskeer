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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.UITypeEditors
{
    [TestFixture]
    public class SelectionEditorTest
    {
        [Test]
        public void GetEditStyle_Always_ReturnDropDown()
        {
            // Setup
            var editorNoDomainType = new SelectionEditor<IObjectProperties, object>();
            var editorWithDomainType = new SelectionEditor<IObjectProperties, object, object>();

            // Call
            var editStyleNoDomainType = editorNoDomainType.GetEditStyle();
            var editStyleWithDomainType = editorWithDomainType.GetEditStyle();

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.DropDown, editStyleNoDomainType);
            Assert.AreEqual(UITypeEditorEditStyle.DropDown, editStyleWithDomainType);
        }

        [Test]
        public void EditValue_NoProviderNoContext_ReturnsOriginalValue()
        {
            // Setup
            var editorNoDomainType = new SelectionEditor<IObjectProperties, object>();
            var editorWithDomainType = new SelectionEditor<IObjectProperties, object, object>();
            var someValue = new object();

            // Call
            var editStyleNoDomainType = editorNoDomainType.EditValue(null, null, someValue);
            var editStyleWithDomainType = editorWithDomainType.EditValue(null, null, someValue);

            // Assert
            Assert.AreSame(someValue, editStyleNoDomainType);
            Assert.AreSame(someValue, editStyleWithDomainType);
        }

        [Test]
        public void EditValue_NoContextAndNoDomainType_ReturnsOriginalValue()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();
            var mockRepository = new MockRepository();
            var provider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(null, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_NoContextAndWithDomainType_ReturnsOriginalValue()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object, object>();
            var mockRepository = new MockRepository();
            var provider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(null, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_NoDomainTypeAlways_ReturnsOriginalValue()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();
            var mockRepository = new MockRepository();
            var provider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var context = mockRepository.StrictMock<ITypeDescriptorContext>();
            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            mockRepository.ReplayAll();

            var someValue = new object();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithDomainTypeAlways_ReturnsOriginalValueInDomainType()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, TestClass, object>();
            var mockRepository = new MockRepository();
            var provider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var context = mockRepository.StrictMock<ITypeDescriptorContext>();
            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            mockRepository.ReplayAll();

            var someValue = new TestClass();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
            Assert.IsTrue(result.GetType() == typeof(TestClass));

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_NullItem_ReturnsNull()
        {
            var nullItem = new object();
            var editor = new TestSelectionEditor(nullItem);

            var mockRepository = new MockRepository();
            var provider = mockRepository.StrictMock<IServiceProvider>();
            var service = mockRepository.StrictMock<IWindowsFormsEditorService>();
            var context = mockRepository.StrictMock<ITypeDescriptorContext>();
            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            service.Expect(s => s.CloseDropDown());
            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, nullItem);

            // Assert
            Assert.IsNull(result);
            mockRepository.VerifyAll();
        }

        private class TestSelectionEditor : SelectionEditor<IObjectProperties, object, object>
        {
            public TestSelectionEditor(object nullItem)
            {
                NullItem = nullItem;
            }
        }

        private class TestClass {}
    }
}