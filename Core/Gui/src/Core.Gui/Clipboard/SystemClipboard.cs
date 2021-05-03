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
using FormsClipboard = System.Windows.Forms.Clipboard;

namespace Core.Gui.Clipboard
{
    /// <summary>
    /// Implementation of <see cref="IClipboard"/> based on the system <see cref="Clipboard"/>.
    /// </summary>
    internal class SystemClipboard : IClipboard
    {
        public void SetDataObject(object data, bool copy = false)
        {
            FormsClipboard.SetDataObject(data, copy);
        }

        public IDataObject GetDataObject()
        {
            return FormsClipboard.GetDataObject();
        }

        public string GetText()
        {
            return FormsClipboard.GetText();
        }
    }
}