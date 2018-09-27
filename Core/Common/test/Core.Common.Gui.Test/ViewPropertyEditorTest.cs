// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Drawing.Design;
using Core.Common.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class ViewPropertyEditorTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var editor = new ViewPropertyEditor();

            // Assert
            Assert.IsInstanceOf<UITypeEditor>(editor);
        }

        [Test]
        public void GetEditStyle_Always_ReturnModal()
        {
            // Setup
            var editor = new ViewPropertyEditor();

            // Call
            UITypeEditorEditStyle style = editor.GetEditStyle();

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.Modal, style);
        }

        [Test]
        public void EditValue_Always_OpenViewForData()
        {
            // Setup
            var editor = new ViewPropertyEditor();
            var data = new object();

            var mocks = new MockRepository();
            var commands = mocks.StrictMock<IViewCommands>();
            commands.Expect(c => c.OpenView(data));
            mocks.ReplayAll();

            IViewCommands originalValue = ViewPropertyEditor.ViewCommands;
            try
            {
                ViewPropertyEditor.ViewCommands = commands;

                // Call
                editor.EditValue(null, null, data);

                // Assert
                mocks.VerifyAll(); // Expect 'OpenView' to be called.
            }
            finally
            {
                ViewPropertyEditor.ViewCommands = originalValue;
            }
        }
    }
}