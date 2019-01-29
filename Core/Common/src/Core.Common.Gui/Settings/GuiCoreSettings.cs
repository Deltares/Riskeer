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

namespace Core.Common.Gui.Settings
{
    /// <summary>
    /// Container for settings in the graphical user interface.
    /// </summary>
    public class GuiCoreSettings
    {
        /// <summary>
        /// Gets or sets the support email address to show in the graphical user interface.
        /// </summary>
        public string SupportEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the support phone number to show in the graphical user interface.
        /// </summary>
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the title to show in the main window of the graphical user interface.
        /// </summary>
        public string MainWindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the path of the manual file to use in the graphical interface.
        /// </summary>
        public string ManualFilePath { get; set; }
    }
}