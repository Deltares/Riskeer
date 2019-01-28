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

using System.Collections.Generic;
using System.IO;
using Core.Common.Util.Settings;

namespace Riskeer.Integration.Forms
{
    /// <summary>
    /// Class that defines helper methods related to Ringtoets settings.
    /// </summary>
    public class RingtoetsSettingsHelper : SettingsHelper
    {
        /// <summary>
        /// Gets the directory of the "NBPW" shape file within the common documents directory.
        /// </summary>
        /// <returns>Directory path where the "NBPW" shape file can be found.</returns>
        public static string GetCommonDocumentsRingtoetsShapeFileDirectory()
        {
            string commonDocuments = new RingtoetsSettingsHelper().GetCommonDocumentsDirectory();
            return Path.Combine(commonDocuments, "NBPW");
        }

        public override string GetCommonDocumentsDirectory(params string[] subPath)
        {
            var ringtoetsCommonDocumentsPath = new List<string>
            {
                "WTI",
                "Ringtoets"
            };
            ringtoetsCommonDocumentsPath.AddRange(subPath);
            return base.GetCommonDocumentsDirectory(ringtoetsCommonDocumentsPath.ToArray());
        }

        public override string GetApplicationLocalUserSettingsDirectory(params string[] subPath)
        {
            var ringtoetsApplicationPath = new List<string>
            {
                "WTI",
                "Ringtoets"
            };
            ringtoetsApplicationPath.AddRange(subPath);
            return base.GetApplicationLocalUserSettingsDirectory(ringtoetsApplicationPath.ToArray());
        }
    }
}