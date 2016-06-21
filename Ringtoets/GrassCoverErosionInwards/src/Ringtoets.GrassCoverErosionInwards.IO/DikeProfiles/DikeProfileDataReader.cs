﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;

using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles
{
    /// <summary>
    /// Reader responsible for reading the data for a dike profile from a .prfl file.
    /// </summary>
    public class DikeProfileDataReader
    {
        [Flags]
        private enum ParametersFoundInFile
        {
            None = 0,
            VERSIE = 1,
            ID = 2,
            RICHTING = 4,
            DAM = 8,
            DAMHOOGTE = 16,
            VOORLAND = 32,
            DAMWAND = 64,
            KRUINHOOGTE = 128,
            DIJK = 256,
            MEMO = 512,
            All = 1023
        }

        private const string idPattern = @"^ID(\s+(?<id>.+?)?)?\s*$";
        private const string versionPattern = @"^VERSIE(\s+(?<version>.+?)?)?\s*$";
        private const string orientationPattern = @"^RICHTING(\s+(?<orientation>.+?)?)?\s*$";
        private const string damTypePattern = @"^DAM(\s+(?<damtype>.+?)?)?\s*$";
        private const string profileTypePattern = @"^DAMWAND(\s+(?<profiletype>.+?)?)?\s*$";
        private const string damHeightPattern = @"^DAMHOOGTE(\s+(?<damheight>.+?)?)?\s*$";
        private const string crestLevelPattern = @"^KRUINHOOGTE(\s+(?<crestlevel>.+?)?)?\s*$";
        private const string dikeGeometryPattern = @"^DIJK(\s+(?<dikegeometry>.+?)?)?\s*$";
        private const string foreshoreGeometryPattern = @"^VOORLAND(\s+(?<foreshoregeometry>.+?)?)?\s*$";
        private const string roughnessPointPattern = @"^(\s*)?(?<localx>.+?)?(\s+(?<localz>.+?)?(\s+(?<roughness>.+?)?)?)?\s*$";
        private const string memoPattern = @"^MEMO\s*$";

        private string fileBeingRead;
        private ParametersFoundInFile readParameters;

        /// <summary>
        /// Reads the *.prfl file for dike profile data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The read dike profile data.</returns>
        /// <exception cref="CriticalFileReadException">When an error occurs like:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> refers to a file that doesn't exist.</item>
        /// <item>A piece of text from the file cannot be converted in the expected variable type.</item>
        /// <item>A converted value is invalid.</item>
        /// <item>The file is not complete.</item>
        /// </list></exception>
        public DikeProfileData ReadDikeProfileData(string filePath)
        {
            FileUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            // TODO: Duplicate entries
            var data = new DikeProfileData();
            readParameters = ParametersFoundInFile.None;
            using (var reader = new StreamReader(filePath))
            {
                fileBeingRead = filePath;
                string text;
                int lineNumber = 0;
                while ((text = ReadLineAndHandleIOExceptions(reader, 1)) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    if (TryReadVersion(text, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadId(text, data, lineNumber))
                        continue;

                    if (TryReadOrientation(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDamType(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadProfileType(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDamHeight(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadCrestLevel(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDikeRoughnessPoints(text, data, reader, ref lineNumber))
                        continue;

                    if (TryReadForeshoreRoughnessPoints(text, data, reader, ref lineNumber))
                        continue;

                    if (TryReadMemo(text, data, reader))
                    {
                        continue;
                    }
                }
            }

            ValidateNoMissingParameters();

            return data;
        }

        /// <summary>
        /// Attempts to match the given text to a VERSIE key-value pair. If a match is found,
        /// the value is parsed and validated.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a VERSIE key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the VERSIE key does
        /// not represent a valid version code or the file version is not supported by this reader.</exception>
        private bool TryReadVersion(string text, int lineNumber)
        {
            Match versionMatch = new Regex(versionPattern).Match(text);
            if (versionMatch.Success)
            {
                string readVersionText = versionMatch.Groups["version"].Value;
                Version fileVersion = ParseVersion(lineNumber, readVersionText);

                if (!fileVersion.Equals(new Version(4, 0)))
                {
                    string message = Resources.DikeProfileDataReader_TryReadVersion_Only_version_four_zero_supported;
                    throw CreateCriticalFileReadException(lineNumber, message);
                }

                readParameters |= ParametersFoundInFile.VERSIE;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the version code from a piece of text representing a VERSIE key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readVersionText">The VERSIE key-value pair text.</param>
        /// <returns>The read version code.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readVersionText"/>
        /// does not represent a valid version code.</exception>
        private Version ParseVersion(int lineNumber, string readVersionText)
        {
            try
            {
                return new Version(readVersionText);
            }
            catch (FormatException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseVersion_Invalid_version_0_,
                                            readVersionText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseVersion_Version_0_overflows,
                                            readVersionText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (ArgumentException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseVersion_Invalid_version_0_,
                                            readVersionText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a ID key-value pair. If a match is found,
        /// the value is parsed and validated.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a ID key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the ID key does
        /// not represent a id.</exception>
        private bool TryReadId(string text, DikeProfileData data, int lineNumber)
        {
            Match idMatch = new Regex(idPattern).Match(text);
            if (idMatch.Success)
            {
                string readIdText = idMatch.Groups["id"].Value;
                if (string.IsNullOrWhiteSpace(readIdText))
                {
                    string message = string.Format(Resources.DikeProfileDataReader_TryReadId_Id_0_not_valid,
                                                   readIdText);
                    throw CreateCriticalFileReadException(lineNumber, message);
                }
                if (new Regex(@"\s").IsMatch(readIdText))
                {
                    string message = string.Format(Resources.DikeProfileDataReader_TryReadId_Id_0_has_unsupported_white_space,
                                                   readIdText);
                    throw CreateCriticalFileReadException(lineNumber, message);
                }
                data.Id = readIdText;
                readParameters |= ParametersFoundInFile.ID;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to match the given text to a RICHTING key-value pair. If a match is
        /// found, the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a RICHTING key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the RICHTING key does
        /// not represent a valid number or the orientation value is not in the range [0, 360].</exception>
        private bool TryReadOrientation(string text, DikeProfileData data, int lineNumber)
        {
            Match orientationMatch = new Regex(orientationPattern).Match(text);
            if (orientationMatch.Success)
            {
                string readOrientationText = orientationMatch.Groups["orientation"].Value;
                double orientation = ParseOrientation(lineNumber, readOrientationText);

                if (orientation < 0.0 || orientation > 360.0)
                {
                    string message = string.Format(Resources.DikeProfileDataReader_TryReadOrientation_Orientation_0_must_be_in_range,
                                                   orientation);
                    throw CreateCriticalFileReadException(lineNumber, message);
                }
                data.Orientation = orientation;
                readParameters |= ParametersFoundInFile.RICHTING;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the orientation from a piece representing a RICHTING key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readOrientationText">The RICHTING key-value pair text.</param>
        /// <returns>The value corresponding to the RICHTING key.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readOrientationText"/>
        /// does not represent a valid number.</exception>
        private double ParseOrientation(int lineNumber, string readOrientationText)
        {
            try
            {
                return double.Parse(readOrientationText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseOrientation_Orientation_0_not_double,
                                            readOrientationText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseOrientation_Orientation_0_overflows,
                                            readOrientationText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a DAM key-value pair. If a match is found,
        /// the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a DAM key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DAM key does
        /// not represent a valid <see cref="DamType"/> value.</exception>
        private bool TryReadDamType(string text, DikeProfileData data, int lineNumber)
        {
            Match damTypeMatch = new Regex(damTypePattern).Match(text);
            if (damTypeMatch.Success)
            {
                string readDamTypeText = damTypeMatch.Groups["damtype"].Value;
                DamType damType = ParseDamType(lineNumber, readDamTypeText);

                data.DamType = damType;
                readParameters |= ParametersFoundInFile.DAM;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the dam-type from a piece of text representing a DAM key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readDamTypeText">The DAM key-value pair text.</param>
        /// <returns>The read dam-type.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readDamTypeText"/>
        /// does not represent a <see cref="DamType"/>.</exception>
        private DamType ParseDamType(int lineNumber, string readDamTypeText)
        {
            int damTypeValue;
            try
            {
                damTypeValue = int.Parse(readDamTypeText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseDamType_DamType_0_is_not_integer,
                                            readDamTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                var message = string.Format(Resources.DikeProfileDataReader_ParseDamType_DamType_0_overflows,
                                            readDamTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }

            if (!CanSafelyCastToEnum<DamType>(damTypeValue))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDamType_DamType_0_must_be_in_range,
                                               damTypeValue);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
            DamType damType = (DamType)damTypeValue;
            return damType;
        }

        /// <summary>
        /// Attempts to match the given text to a DAMWAND key-value pair. If a match is
        /// found, the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a DAMWAND key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DAMWAND key
        /// does not represent a valid <see cref="ProfileType"/> value.</exception>
        private bool TryReadProfileType(string text, DikeProfileData data, int lineNumber)
        {
            Match profileTypeMatch = new Regex(profileTypePattern).Match(text);
            if (profileTypeMatch.Success)
            {
                string readProfileTypeText = profileTypeMatch.Groups["profiletype"].Value;
                ProfileType profileType = ParseProfileType(lineNumber, readProfileTypeText);

                data.ProfileType = profileType;
                readParameters |= ParametersFoundInFile.DAMWAND;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the profile-type from a piece of text representing a DAMWAND key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readProfileTypeText">The DAMWAND key-value pair text.</param>
        /// <returns>The read profile-type.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readProfileTypeText"/>
        /// does not represent a <see cref="ProfileType"/>.</exception>
        private ProfileType ParseProfileType(int lineNumber, string readProfileTypeText)
        {
            int profileTypeValue;
            try
            {
                profileTypeValue = int.Parse(readProfileTypeText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseProfileType_ProfileType_0_not_integer,
                                               readProfileTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseProfileType_ProfileType_0_overflows,
                                               readProfileTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }

            if (!CanSafelyCastToEnum<ProfileType>(profileTypeValue))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ReadDikeProfileData_ProfileType_0_must_be_in_range,
                                               profileTypeValue);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
            return (ProfileType)profileTypeValue;
        }

        /// <summary>
        /// Attempts to match the given text to a DAMHOOGTE key-value pair. If a match is
        /// found, the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a DAMHOOGTE key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DAMHOOGTE key
        /// does not represent a valid number.</exception>
        private bool TryReadDamHeight(string text, DikeProfileData data, int lineNumber)
        {
            Match damHeightMatch = new Regex(damHeightPattern).Match(text);
            if (damHeightMatch.Success)
            {
                string readDamHeightText = damHeightMatch.Groups["damheight"].Value;
                var damHeight = ParseDamHeight(readDamHeightText, lineNumber);

                data.DamHeight = damHeight;
                readParameters |= ParametersFoundInFile.DAMHOOGTE;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the height of the dam from a piece of text representing a DAMHOOGTE
        /// key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readDamHeightText">The DAMHOOGTE key-value pair text.</param>
        /// <returns>The height of the dam.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readDamHeightText"/>
        /// does not represent a number.</exception>
        private double ParseDamHeight(string readDamHeightText, int lineNumber)
        {
            try
            {
                return double.Parse(readDamHeightText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDamHeight_DamHeight_0_not_number,
                                               readDamHeightText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDamHeight_DamHeight_0_overflows,
                                               readDamHeightText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a KRUINHOOGTE key-value pair. If a match is
        /// found, the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a KRUINHOOGTE key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the KRUINHOOGTE key
        /// does not represent a valid number.</exception>
        private bool TryReadCrestLevel(string text, DikeProfileData data, int lineNumber)
        {
            Match crestLevelMatch = new Regex(crestLevelPattern).Match(text);
            if (crestLevelMatch.Success)
            {
                string readCrestLevelText = crestLevelMatch.Groups["crestlevel"].Value;
                double crestLevel = ParseCrestLevel(lineNumber, readCrestLevelText);

                data.CrestLevel = crestLevel;
                readParameters |= ParametersFoundInFile.KRUINHOOGTE;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the height of the dike from a piece of text representing a KRUINHOOGTE
        /// key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readCrestLevelText">The KRUINHOOGTE key-value pair text.</param>
        /// <returns>The height of the dike.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readCrestLevelText"/>
        /// does not represent a number.</exception>
        private double ParseCrestLevel(int lineNumber, string readCrestLevelText)
        {
            try
            {
                return double.Parse(readCrestLevelText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseCrestLevel_CrestLevel_0_not_number,
                                               readCrestLevelText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseCrestLevel_CrestLevel_0_overflows,
                                               readCrestLevelText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a DIJK key-value pair. If a match is
        /// found, the data block is being read. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text of the DIJK key-value pair.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="reader">The reader of the file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a DIJK key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DIJK key
        /// does not represent a valid number or any of read the parameters in the following
        /// data block is invalid.</exception>
        private bool TryReadDikeRoughnessPoints(string text, DikeProfileData data, TextReader reader, ref int lineNumber)
        {
            Match dikeGeometryMatch = new Regex(dikeGeometryPattern).Match(text);
            if (dikeGeometryMatch.Success)
            {
                string readDikeGeometryCountText = dikeGeometryMatch.Groups["dikegeometry"].Value;
                int numberOfElements = ParseNumberOfDikeElements(lineNumber, readDikeGeometryCountText);

                if (numberOfElements < 0)
                {
                    string message = string.Format(Resources.DikeProfileDataReader_ReadDikeProfileData_DikeCount_cannot_be_negative,
                                                   numberOfElements);
                    throw CreateCriticalFileReadException(lineNumber, message);
                }
                if (numberOfElements == 0)
                {
                    readParameters |= ParametersFoundInFile.DIJK;
                    return true;
                }
                data.DikeGeometry = new RoughnessPoint[numberOfElements];

                for (int i = 0; i < numberOfElements; i++)
                {
                    lineNumber++;
                    text = ReadLineAndHandleIOExceptions(reader, lineNumber);
                    if (text == null)
                    {
                        string message = string.Format(Resources.DikeProfileDataReader_TryReadDikeRoughnessPoints_DikeCount_0_does_not_correspond_ExpectedCount_1_,
                                                       i, numberOfElements);
                        throw CreateCriticalFileReadException(lineNumber, message);
                    }

                    RoughnessPoint roughnessPoint = ReadRoughnessPoint(text, lineNumber);
                    data.DikeGeometry[i] = roughnessPoint;
                }
                readParameters |= ParametersFoundInFile.DIJK;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the number of dike roughness points from a piece of text representing
        /// a DIJK key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readDikeGeometryCountText">The DIJK key-value pair text.</param>
        /// <returns>The number of dike roughness points.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readDikeGeometryCountText"/>
        /// does not represent a number.</exception>
        private int ParseNumberOfDikeElements(int lineNumber, string readDikeGeometryCountText)
        {
            try
            {
                return int.Parse(readDikeGeometryCountText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseNumberOfDikeElements_DijkCount_0_not_integer,
                                               readDikeGeometryCountText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseNumberOfDikeElements_DikeCount_0_overflows,
                                               readDikeGeometryCountText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a roughness-point value triplet. If a match
        /// is found, the values are parsed and validated. If valid, the resulting <see cref="RoughnessPoint"/>
        /// based on the values will be returned.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a roughness-point value triplet and
        /// has been validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">Any parameter value in the roughness-point
        /// value triplet does not represent a valid number or the roughness parameter
        /// is not valid.</exception>
        private RoughnessPoint ReadRoughnessPoint(string text, int lineNumber)
        {
            Match roughnessSectionDataMatch = new Regex(roughnessPointPattern).Match(text);

            string readLocalXText = roughnessSectionDataMatch.Groups["localx"].Value;
            double localX = ParseRoughnessPointParameter(readLocalXText, 
                Resources.DikeProfileDataReader_ReadRoughnessPoint_X_DisplayName, 
                lineNumber);

            string readLocalYText = roughnessSectionDataMatch.Groups["localz"].Value;
            double localY = ParseRoughnessPointParameter(readLocalYText, 
                Resources.DikeProfileDataReader_ReadRoughnessPoint_Y_DisplayName, 
                lineNumber);

            string readRoughnessText = roughnessSectionDataMatch.Groups["roughness"].Value;
            double roughness = ParseRoughnessPointParameter(readRoughnessText, 
                Resources.DikeProfileDataReader_ReadRoughnessPoint_Roughness_DisplayName, 
                lineNumber);

            if (roughness < 0.5 || roughness > 1.0)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ReadRoughnessPoint_Roughness_0_must_be_in_range_LowerLimit_1_,
                                               roughness, 0.5);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
            return new RoughnessPoint(new Point2D(localX, localY), roughness);
        }

        /// <summary>
        /// Parses some <c>double</c> parameter from a piece of text.
        /// </summary>
        /// <param name="parameterName">The name of the parameter as shown to the user.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readParameterText">The value text to be parsed.</param>
        /// <returns>The parameter value.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readParameterText"/>
        /// does not represent a number.</exception>
        private double ParseRoughnessPointParameter(string readParameterText, string parameterName, int lineNumber)
        {
            try
            {
                return double.Parse(readParameterText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseRoughnessPointParameter_ParameterName_0_Value_1_not_number,
                                               parameterName, readParameterText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseRoughnessPointParameter_ParameterName_0_Value_1_overflows,
                                               parameterName, readParameterText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a VOORLAND key-value pair. If a match is
        /// found, the data block is being read. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text of the VOORLAND key-value pair.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="reader">The reader of the file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>True</c> if the text matches a VOORLAND key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the VOORLAND key
        /// does not represent a valid number or any of read the parameters in the following
        /// data block is invalid.</exception>
        private bool TryReadForeshoreRoughnessPoints(string text, DikeProfileData data, TextReader reader, ref int lineNumber)
        {
            Match foreshoreGeometryMatch = new Regex(foreshoreGeometryPattern).Match(text);
            if (foreshoreGeometryMatch.Success)
            {
                string readForeshoreCountText = foreshoreGeometryMatch.Groups["foreshoregeometry"].Value;
                var numberOfElements = ParseNumberOfForeshoreElements(readForeshoreCountText, lineNumber);

                if (numberOfElements < 0)
                {
                    string message = string.Format(Resources.DikeProfileDataReader_ReadDikeProfileData_ForeshoreCount_0_cannot_be_negative,
                                                   numberOfElements);
                    throw CreateCriticalFileReadException(lineNumber, message);
                }
                if (numberOfElements == 0)
                {
                    readParameters |= ParametersFoundInFile.VOORLAND;
                    return true;
                }

                data.ForeshoreGeometry = new RoughnessPoint[numberOfElements];
                for (int i = 0; i < numberOfElements; i++)
                {
                    lineNumber++;
                    text = ReadLineAndHandleIOExceptions(reader, lineNumber);
                    if (text == null)
                    {
                        string message = string.Format(Resources.DikeProfileDataReader_TryReadForeshoreRoughnessPoints_ForeshoreCount_0_does_not_correspond_ExpectedCount_1_,
                                                       i, numberOfElements);
                        throw CreateCriticalFileReadException(lineNumber, message);
                    }

                    RoughnessPoint roughnessPoint = ReadRoughnessPoint(text, lineNumber);
                    data.ForeshoreGeometry[i] = roughnessPoint;
                }
                readParameters |= ParametersFoundInFile.VOORLAND;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the number of foreshore roughness points from a piece of text representing
        /// a VOORLAND key-value pair.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readForeshoreCountText">The VOORLAND key-value pair text.</param>
        /// <returns>The number of foreshore roughness points.</returns>
        /// <exception cref="CriticalFileReadException">When <paramref name="readForeshoreCountText"/>
        /// does not represent a number.</exception>
        private int ParseNumberOfForeshoreElements(string readForeshoreCountText, int lineNumber)
        {
            try
            {
                return int.Parse(readForeshoreCountText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseNumberOfForeshoreElements_ForeshoreCount_0_not_integer,
                                               readForeshoreCountText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseNumberOfForeshoreElements_ForeshoreCount_0_overflows,
                                               readForeshoreCountText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        private bool TryReadMemo(string text, DikeProfileData data, StreamReader reader)
        {
            Match memoMatch = new Regex(memoPattern).Match(text);
            if (memoMatch.Success)
            {
                data.Memo = reader.ReadToEnd();
                readParameters |= ParametersFoundInFile.MEMO;
                return true;
            }
            return false;
        }

        private static bool CanSafelyCastToEnum<TEnum>(int parameterValue) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            return Enum.GetValues(typeof(TEnum))
                       .OfType<TEnum>()
                       .Select(dt => Convert.ToInt32(dt))
                       .Any(i => i.Equals(parameterValue));
        }

        /// <summary>
        /// Reads the next line and handles I/O exceptions.
        /// </summary>
        /// <param name="reader">The opened text file reader.</param>
        /// <param name="currentLine">Row number for error messaging.</param>
        /// <returns>The read line, or null when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">An critical I/O exception occurred.</exception>
        private string ReadLineAndHandleIOExceptions(TextReader reader, int currentLine)
        {
            try
            {
                return reader.ReadLine();
            }
            catch (OutOfMemoryException e)
            {
                throw CreateCriticalFileReadException(currentLine, UtilsResources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                var message = new FileReaderErrorMessageBuilder(fileBeingRead).Build(string.Format(UtilsResources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="CriticalFileReadException"/> with message and inner exception set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(UtilsResources.TextFile_On_LineNumber_0_, currentLine);
            var message = new FileReaderErrorMessageBuilder(fileBeingRead).WithLocation(locationDescription)
                                                                          .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }


        private void ValidateNoMissingParameters()
        {
            string[] missingParameters = GetMissingParameterNames();
            if (missingParameters.Any())
            {
                string criticalErrorMessage = string.Format(Resources.DikeProfileDataReader_ReadDikeProfileData_List_mising_parameters_0_,
                                                            String.Join(", ", missingParameters));
                var message = new FileReaderErrorMessageBuilder(fileBeingRead)
                    .Build(criticalErrorMessage);
                throw new CriticalFileReadException(message);
            }
        }

        private string[] GetMissingParameterNames()
        {
            var requiredParameters = new[]
            {
                ParametersFoundInFile.VERSIE,
                ParametersFoundInFile.ID,
                ParametersFoundInFile.RICHTING,
                ParametersFoundInFile.DAM,
                ParametersFoundInFile.DAMHOOGTE,
                ParametersFoundInFile.VOORLAND,
                ParametersFoundInFile.DAMWAND,
                ParametersFoundInFile.KRUINHOOGTE,
                ParametersFoundInFile.DIJK,
                ParametersFoundInFile.MEMO
            };
            return requiredParameters.Where(z => !readParameters.HasFlag(z))
                                     .Select(z => z.ToString())
                                     .ToArray();
        }
    }
}