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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

namespace Core.Common.Gui
{
    /// <summary>
    /// Container for settings in the graphical user interface.
    /// </summary>
    public class GuiCoreSettings
    {
        /// <summary>
        /// The start page url to use in the graphical user interface.
        /// </summary>
        public string StartPageUrl { get; set; }

        /// <summary>
        /// The support email address to show in the graphical user interface.
        /// </summary>
        public string SupportEmailAddress { get; set; }

        /// <summary>
        /// The support phone number to show in the graphical user interface.
        /// </summary>
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// The copyright to show in the graphical user interface.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The license description to show in the graphical user interface.
        /// </summary>
        public string LicenseDescription { get; set; }

        /// <summary>
        /// The title to show in the main window of the graphical user interface.
        /// </summary>
        public string MainWindowTitle { get; set; }

        /// <summary>
        /// The path of the license file to use in the graphical interface.
        /// </summary>
        public string LicenseFilePath { get; set; }

        /// <summary>
        /// The path of the manual file to use in the graphical interface.
        /// </summary>
        public string ManualFilePath { get; set; }
    }
}
