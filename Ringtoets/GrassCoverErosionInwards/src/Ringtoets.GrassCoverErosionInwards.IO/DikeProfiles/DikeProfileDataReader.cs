// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using Core.Common.Base.Geometry;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles
{
    /// <summary>
    /// Reader responsible for reading the data for a dike profile from a .prfl file.
    /// </summary>
    public class DikeProfileDataReader
    {
        private const string idPattern = @"^ID\s+(?<id>\w+)$";
        private const string orientationPattern = @"^RICHTING\s+(?<orientation>\d+(?:\.\d+)?)$";
        private const string damTypePattern = @"^DAM\s+(?<damtype>\d)$";
        private const string profileTypePattern = @"^DAMWAND\s+(?<profiletype>\d)$";
        private const string damHeightPattern = @"^DAMHOOGTE\s+(?<damheight>-?\d+(?:\.\d+)?)$";
        private const string crestLevelPattern = @"^KRUINHOOGTE\s+(?<crestlevel>-?\d+(?:\.\d+)?)$";
        private const string dikeGeometryPattern = @"^DIJK\s+(?<dikegeometry>\d+)$";
        private const string foreshoreGeometryPattern = @"^VOORLAND\s+(?<foreshoregeometry>\d+)$";
        private const string roughnessSectionPattern = @"^(?<localx>-?\d+(?:\.\d+)?)\s+(?<localz>-?\d+(?:\.\d+)?)\s+(?<roughness>-?\d+(?:\.\d+)?)$";
        private const string memoPattern = @"^MEMO$";

        /// <summary>
        /// Reads the *.prfl file for dike profile data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The read dike profile data.</returns>
        public DikeProfileData ReadDikeProfileData(string filePath)
        {
            var data = new DikeProfileData();
            using (var reader = new StreamReader(filePath))
            {
                string text;
                while ((text = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    // TODO: Read Version
                    // TODO: Validate version
                    // TODO: Ensure Version in file

                    Match idMatch = new Regex(idPattern).Match(text);
                    if (idMatch.Success)
                    {
                        data.Id = idMatch.Groups["id"].Value;
                        // TODO: Validate id (needs different regex)
                        // TODO: Ensure ID in file
                        continue;
                    }
                    Match orientationMatch = new Regex(orientationPattern).Match(text);
                    if (orientationMatch.Success)
                    {
                        data.Orientation = double.Parse(orientationMatch.Groups["orientation"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate orientation [0, 360] range
                        // TODO: Validate can be parsed
                        // TODO: Ensure orientation in file
                        continue;
                    }
                    Match damTypeMatch = new Regex(damTypePattern).Match(text);
                    if (damTypeMatch.Success)
                    {
                        data.DamType = (DamType)int.Parse(damTypeMatch.Groups["damtype"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate damtype [0, 3] range
                        // TODO: Validate can be parsed
                        // TODO: Ensure in file
                        continue;
                    }
                    Match profileTypeMatch = new Regex(profileTypePattern).Match(text);
                    if (profileTypeMatch.Success)
                    {
                        data.ProfileType = (ProfileType)int.Parse(profileTypeMatch.Groups["profiletype"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate profile type [0, 2] range
                        // TODO: Validate can be parsed
                        // TODO: Ensure in file
                        continue;
                    }
                    Match damHeightMatch = new Regex(damHeightPattern).Match(text);
                    if (damHeightMatch.Success)
                    {
                        data.DamHeight = double.Parse(damHeightMatch.Groups["damheight"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate can be parsed
                        // TODO: Ensure in file
                        continue;
                    }
                    Match crestLevelMatch = new Regex(crestLevelPattern).Match(text);
                    if (crestLevelMatch.Success)
                    {
                        data.CrestLevel = double.Parse(crestLevelMatch.Groups["crestlevel"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate can be parsed
                        // TODO: Ensure in file
                        continue;
                    }
                    Match dikeGeometryMatch = new Regex(dikeGeometryPattern).Match(text);
                    if (dikeGeometryMatch.Success)
                    {
                        int numberOfElements = int.Parse(dikeGeometryMatch.Groups["dikegeometry"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate dikegeometry count > 0
                        // TODO: Validate can be parsed
                        // TODO: Ensure in file
                        if (numberOfElements == 0)
                        {
                            continue;
                        }
                        data.DikeGeometry = new RoughnessProfileSection[numberOfElements-1];

                        double previousLocalX = 0, previousLocalZ = 0, previousRoughness = 0;
                        for (int i = 0; i < numberOfElements; i++)
                        {
                            text = reader.ReadLine();
                            Match roughnessSectionDataMatch = new Regex(roughnessSectionPattern).Match(text);
                            // TODO: Validate roughness [0.0, 1.0] range
                            // TODO: Validate all can be parsed
                            // TODO: Ensure all in file
                            double localX = double.Parse(roughnessSectionDataMatch.Groups["localx"].Value, CultureInfo.InvariantCulture);
                            double localZ = double.Parse(roughnessSectionDataMatch.Groups["localz"].Value, CultureInfo.InvariantCulture);
                            double roughness = double.Parse(roughnessSectionDataMatch.Groups["roughness"].Value, CultureInfo.InvariantCulture);
                            if (i > 0)
                            {
                                data.DikeGeometry[i - 1] = new RoughnessProfileSection(new Point2D(previousLocalX, previousLocalZ), new Point2D(localX, localZ), previousRoughness);
                            }
                            previousLocalX = localX;
                            previousLocalZ = localZ;
                            previousRoughness = roughness;
                        }
                        continue;
                    }
                    Match foreshoreGeometryMatch = new Regex(foreshoreGeometryPattern).Match(text);
                    if (foreshoreGeometryMatch.Success)
                    {
                        int numberOfElements = int.Parse(foreshoreGeometryMatch.Groups["foreshoregeometry"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate foreshore geometry count > 0
                        // TODO: Validate can be parsed
                        // TODO: Ensure in file
                        if (numberOfElements == 0)
                        {
                            continue;
                        }
                        data.ForeshoreGeometry = new RoughnessProfileSection[numberOfElements-1];
                        double previousLocalX = 0, previousLocalZ = 0, previousRoughness = 0;
                        for (int i = 0; i < numberOfElements; i++)
                        {
                            text = reader.ReadLine();
                            Match roughnessSectionDataMatch = new Regex(roughnessSectionPattern).Match(text);
                            // TODO: Validate roughness [0.0, 1.0] range
                            // TODO: Validate all can be parsed
                            // TODO: Ensure all in file
                            double localX = double.Parse(roughnessSectionDataMatch.Groups["localx"].Value, CultureInfo.InvariantCulture);
                            double localZ = double.Parse(roughnessSectionDataMatch.Groups["localz"].Value, CultureInfo.InvariantCulture);
                            double roughness = double.Parse(roughnessSectionDataMatch.Groups["roughness"].Value, CultureInfo.InvariantCulture);
                            if (i > 0)
                            {
                                data.ForeshoreGeometry[i - 1] = new RoughnessProfileSection(new Point2D(previousLocalX, previousLocalZ), new Point2D(localX, localZ), previousRoughness);
                            }
                            previousLocalX = localX;
                            previousLocalZ = localZ;
                            previousRoughness = roughness;
                        }
                        continue;
                    }
                    Match memoMatch = new Regex(memoPattern).Match(text);
                    if (memoMatch.Success)
                    {
                        // TODO: Ensure in file
                        data.Memo = reader.ReadToEnd();
                    }
                }
            }
            return data;
        }
    }
}