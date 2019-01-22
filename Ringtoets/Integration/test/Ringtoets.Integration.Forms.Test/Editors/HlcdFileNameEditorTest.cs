// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using NUnit.Framework;
using Ringtoets.Integration.Forms.Editors;

namespace Ringtoets.Integration.Forms.Test.Editors
{
    [TestFixture]
    public class HlcdFileNameEditorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var editor = new HlcdFileNameEditor();

            // Assert
            Assert.IsInstanceOf<FileNameEditor>(editor);
        }

        [Test]
        public void InitializeDialog_Always_SetsOpenFileDialogFilter()
        {
            // Setup
            var editor = new HlcdFileNameEditor();
            MethodInfo methodInfo = editor.GetType().GetMethod("InitializeDialog", BindingFlags.NonPublic | BindingFlags.Instance);
            var dialog = new OpenFileDialog();

            // Precondition
            Assert.IsNotNull(methodInfo, "No method available");
            Assert.IsEmpty(dialog.Filter);

            // Call
            methodInfo.Invoke(editor, new object[]
            {
                dialog
            });

            // Assert
            Assert.AreEqual("HLCD bestand|*.sqlite", dialog.Filter);
        }
    }
}