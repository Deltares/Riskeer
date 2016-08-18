﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.UITypeEditors;

namespace Ringtoets.Common.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextSelectionEditorTest
    {
        [Test]
        public void GetEditStyle_Always_ReturnDropDown()
        {
            // Setup
            var editor = new SelectionEditor<IObjectProperties, object>();

            // Call
            var editStyle = editor.GetEditStyle();

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
            var result = editor.EditValue(null, null, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_NoContext_ReturnsOriginalValue()
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
        public void EditValue_Always_ReturnsOriginalValue()
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
    }
}