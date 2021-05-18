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

using System;
using Core.Gui.Settings;

namespace Core.Gui.Forms.Backstage
{
    /// <summary>
    /// ViewModel for <see cref="SupportBackstagePage"/>.
    /// </summary>
    public class SupportViewModel : IBackstagePageViewModel
    {
        private readonly GuiCoreSettings settings;

        /// <summary>
        /// Creates a new instance of <see cref="SupportViewModel"/>.
        /// </summary>
        /// <param name="settings">The application settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="settings"/> is <c>null</c>.</exception>
        public SupportViewModel(GuiCoreSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.settings = settings;
        }

        /// <summary>
        /// Gets the header of the support text.
        /// </summary>
        public string SupportHeader => settings.SupportHeader;

        /// <summary>
        /// Gets the support text.
        /// </summary>
        public string SupportText => settings.SupportText;
    }
}