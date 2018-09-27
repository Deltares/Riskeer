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

using System.IO;
using Core.Common.Util.Settings;
using log4net.Util;

namespace Core.Common.Gui.Appenders
{
    /// <summary>
    /// Converter that injects the application user data folder.
    /// </summary>
    public class RingtoetsUserDataFolderConverter : PatternConverter // NOTE: Class might look like it's only used in tests, but it's actually created in Application.Ringtoets/app.config!
    {
        protected override void Convert(TextWriter writer, object state)
        {
            string settingsDirectory = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(Option);
            writer.Write(settingsDirectory);
        }
    }
}