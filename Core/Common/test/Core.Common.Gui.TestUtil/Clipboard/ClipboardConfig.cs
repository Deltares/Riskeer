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

using System;
using System.Windows.Forms;
using Core.Common.Gui.Clipboard1;

namespace Core.Common.Gui.TestUtil.Clipboard
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="IClipboard"/> for <see cref="ClipboardProvider"/> while
    /// testing. Disposing an instance of this class will revert the <see cref="ClipboardProvider.Clipboard"/> to its
    /// original state.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new ClipboardConfig())
    /// {
    ///     IClipboard clipboard = ClipboardProvider.Clipboard;
    /// 
    ///     // Perform tests with clipboard
    /// }
    /// </code>
    /// </example>
    public class ClipboardConfig : IDisposable
    {
        private readonly IClipboard previousClipboard;

        /// <summary>
        /// Creates a new instance of <see cref="ClipboardConfig"/>. Sets a test implementation of <see cref="IClipboard"/> to 
        /// <see cref="ClipboardProvider.Clipboard"/>.
        /// </summary>
        public ClipboardConfig()
        {
            previousClipboard = ClipboardProvider.Clipboard;
            ClipboardProvider.Clipboard = new TestClipboard();
        }

        /// <summary>
        /// Reverts <see cref="ClipboardProvider.Clipboard"/> to its original state.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClipboardProvider.Clipboard = previousClipboard;
            }
        }

        private class TestClipboard : IClipboard
        {
            private object clipBoardContent;

            public void SetDataObject(object data, bool copy = false)
            {
                clipBoardContent = data;
            }

            public IDataObject GetDataObject()
            {
                return clipBoardContent as IDataObject;
            }

            public string GetText()
            {
                return clipBoardContent as string;
            }
        }
    }
}