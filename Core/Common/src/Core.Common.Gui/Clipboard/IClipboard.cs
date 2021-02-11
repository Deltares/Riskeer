// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Windows.Forms;

namespace Core.Common.Gui.Clipboard
{
    /// <summary>
    /// Interface representing the clipboard.
    /// </summary>
    public interface IClipboard
    {
        /// <summary>
        /// Clears the clipboard, places data on it and specifies whether the data should remain after the
        /// application exits.
        /// </summary>
        /// <param name="data">The data to place on the clipboard.</param>
        /// <param name="copy"><c>true</c> if the data must remain on the clipboard after the application
        /// exits, <c>false</c> otherwise.</param>
        void SetDataObject(object data, bool copy = false);

        /// <summary>
        /// Retrieves the data that is currently on the clipboard.
        /// </summary>
        /// <returns>The <see cref="IDataObject"/> that is on the clipboard, or <c>null</c> if the clipboard
        /// does not contain any data.</returns>
        IDataObject GetDataObject();

        /// <summary>
        /// Retrieves the textual data that is currently on the clipboard.
        /// </summary>
        /// <returns>The textual data that is on the clipboard, or <c>null</c> if the clipboard does not
        /// contain any textual data.</returns>
        string GetText();
    }
}