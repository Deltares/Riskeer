// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;
using UtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.DikeProfiles
{
    /// <summary>
    /// Reader responsible for reading the data for a dike profile from a .prfl file.
    /// </summary>
    public class DikeProfileDataReader
    {
        [Flags]
        private enum Keywords
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
            MEMO = 512
        }

        private static readonly Range<double> orientationValidityRange = new Range<double>(0, 360);
        private static readonly Range<double> roughnessValidityRange = new Range<double>(0.5, 1.0);

        private readonly string[] acceptedIds;

        private string fileBeingRead;
        private Keywords readKeywords;

        /// <summary>
        /// Initializes a new instance of <see cref="DikeProfileDataReader"/>.
        /// </summary>
        /// <param name="acceptedIds">The accepted ids the file id must have to pass validation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="acceptedIds"/> is <c>null</c>.</exception>
        public DikeProfileDataReader(string[] acceptedIds)
        {
            if (acceptedIds == null)
            {
                throw new ArgumentNullException(nameof(acceptedIds));
            }

            this.acceptedIds = acceptedIds;
        }

        /// <summary>
        /// Reads a *.prfl file containing dike profile data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The read <see cref="DikeProfileData"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> refers to a file that doesn't exist.</item>
        /// <item>A piece of text from the file cannot be converted into the expected variable type.</item>
        /// <item>A critical I/O exception occurred while attempting to read a line in the file.</item>
        /// <item>A converted value is invalid.</item>
        /// <item>The file is incomplete.</item>
        /// <item>A keyword is defined more then once.</item>
        /// <item>The geometry points for either the dike or foreshore do not have monotonically
        /// increasing X-coordinates.</item>
        /// <item>An unexpected piece of text has been encountered in the file.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileValidationException">Thrown when the read id is not an accepted id.</exception>
        public DikeProfileData ReadDikeProfileData(string filePath)
        {
            IOUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            var data = new DikeProfileData();
            readKeywords = Keywords.None;
            using (var reader = new StreamReader(filePath))
            {
                fileBeingRead = filePath;
                string text;
                var lineNumber = 0;
                while ((text = ReadLineAndHandleIOExceptions(reader, ++lineNumber)) != null)
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    if (TryReadVersion(text, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadId(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadOrientation(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDamType(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadSheetPileType(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDamHeight(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDikeHeight(text, data, lineNumber))
                    {
                        continue;
                    }

                    if (TryReadDikeRoughnessPoints(text, data, reader, ref lineNumber))
                    {
                        continue;
                    }

                    if (TryReadForeshoreRoughnessPoints(text, data, reader, ref lineNumber))
                    {
                        continue;
                    }

                    if (TryReadMemo(text, data, reader))
                    {
                        continue;
                    }

                    HandleUnexpectedText(text, lineNumber);
                }
            }

            ValidateNoMissingKeywords();

            return data;
        }

        /// <summary>
        /// Attempts to match the given text to a VERSIE key-value pair. If a match is found,
        /// the value is parsed and validated.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches a VERSIE key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the VERSIE key does
        /// not represent a valid version code or has already been defined or the version
        /// is not supported by this reader.</exception>
        private bool TryReadVersion(string text, int lineNumber)
        {
            Match versionMatch = new Regex(@"^VERSIE(\s+(?<version>.+?)?)?\s*$").Match(text);
            if (versionMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.VERSIE, lineNumber);

                string readVersionText = versionMatch.Groups["version"].Value;
                Version fileVersion = ParseVersion(lineNumber, readVersionText);

                ValidateVersion(fileVersion, lineNumber);

                readKeywords |= Keywords.VERSIE;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the version code from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readVersionText">The text to parse.</param>
        /// <returns>The read version code.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readVersionText"/>
        /// does not represent a valid version code.</exception>
        private Version ParseVersion(int lineNumber, string readVersionText)
        {
            try
            {
                return new Version(readVersionText);
            }
            catch (FormatException e)
            {
                string message = Resources.DikeProfileDataReader_ValidateVersion_Only_version_four_zero_supported;
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = Resources.DikeProfileDataReader_ValidateVersion_Only_version_four_zero_supported;
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (ArgumentException e)
            {
                string message = Resources.DikeProfileDataReader_ValidateVersion_Only_version_four_zero_supported;
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Validates the version.
        /// </summary>
        /// <param name="fileVersion">The file version.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="fileVersion"/>
        /// is not supported by this reader.</exception>
        private void ValidateVersion(Version fileVersion, int lineNumber)
        {
            if (!fileVersion.Equals(new Version(4, 0)))
            {
                string message = Resources.DikeProfileDataReader_ValidateVersion_Only_version_four_zero_supported;
                throw CreateCriticalFileReadException(lineNumber, message);
            }
        }

        /// <summary>
        /// Attempts to match the given text to an ID key-value pair. If a match is found,
        /// the value is parsed and validated.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches an ID key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the ID key does
        /// not represent an id or has already been defined.</exception>
        private bool TryReadId(string text, DikeProfileData data, int lineNumber)
        {
            Match idMatch = new Regex(@"^ID(\s+(?<id>.+?)?)?\s*$").Match(text);
            if (idMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.ID, lineNumber);

                string readIdText = idMatch.Groups["id"].Value;
                ValidateId(readIdText, lineNumber);

                data.Id = readIdText;
                readKeywords |= Keywords.ID;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the identifier.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="id"/> is not
        /// present or has a whitespace.</exception>
        /// <exception cref="CriticalFileValidationException">Thrown when <paramref name="id"/> is not an accepted id.</exception>
        private void ValidateId(string id, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ValidateId_Id_0_not_valid,
                                               id);
                throw CreateCriticalFileReadException(lineNumber, message);
            }

            if (id.Any(char.IsWhiteSpace))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ValidateId_Id_0_has_unsupported_white_space,
                                               id);
                throw CreateCriticalFileReadException(lineNumber, message);
            }

            if (!acceptedIds.Contains(id))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ValidateId_Id_0_not_valid,
                                               id);
                throw new CriticalFileValidationException(message);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a RICHTING key-value pair. If a match is
        /// found, the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches a RICHTING key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the RICHTING key does
        /// not represent a valid number or has already been defined or the orientation value
        /// is not in the range [0, 360].</exception>
        private bool TryReadOrientation(string text, DikeProfileData data, int lineNumber)
        {
            Match orientationMatch = new Regex(@"^RICHTING(\s+(?<orientation>.+?)?)?\s*$").Match(text);
            if (orientationMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.RICHTING, lineNumber);

                string readOrientationText = orientationMatch.Groups["orientation"].Value;
                double orientation = ParseOrientation(lineNumber, readOrientationText);

                ValidateOrientation(orientation, lineNumber);

                data.Orientation = orientation;
                readKeywords |= Keywords.RICHTING;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the orientation from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readOrientationText">The text.</param>
        /// <returns>The value corresponding to the RICHTING key.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readOrientationText"/>
        /// does not represent a valid number.</exception>
        private double ParseOrientation(int lineNumber, string readOrientationText)
        {
            try
            {
                return double.Parse(readOrientationText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseOrientation_Orientation_0_not_double,
                                               readOrientationText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseOrientation_Orientation_0_overflows,
                                               readOrientationText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
        }

        /// <summary>
        /// Validates the orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="orientation"/>
        /// is outside the [0, 360] range.</exception>
        private void ValidateOrientation(double orientation, int lineNumber)
        {
            if (!orientationValidityRange.InRange(orientation))
            {
                string rangeText = orientationValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture);
                string message = string.Format(Resources.DikeProfileDataReader_ValidateOrientation_Orientation_0_must_be_in_Range_1_,
                                               orientation, rangeText);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
        }

        /// <summary>
        /// Attempts to match the given text to a DAM key-value pair. If a match is found,
        /// the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches a DAM key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DAM key does
        /// not represent a valid <see cref="DamType"/> value or has already been defined.</exception>
        private bool TryReadDamType(string text, DikeProfileData data, int lineNumber)
        {
            Match damTypeMatch = new Regex(@"^DAM(\s+(?<damtype>.+?)?)?\s*$").Match(text);
            if (damTypeMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.DAM, lineNumber);

                string readDamTypeText = damTypeMatch.Groups["damtype"].Value;
                DamType damType = ParseDamType(lineNumber, readDamTypeText);

                data.DamType = damType;
                readKeywords |= Keywords.DAM;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the dam-type from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readDamTypeText">The text.</param>
        /// <returns>The read dam-type.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readDamTypeText"/>
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
                string message = string.Format(Resources.DikeProfileDataReader_ParseDamType_DamType_0_must_be_in_range,
                                               readDamTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDamType_DamType_0_must_be_in_range,
                                               readDamTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }

            if (!CanSafelyCastToEnum<DamType>(damTypeValue))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDamType_DamType_0_must_be_in_range,
                                               damTypeValue);
                throw CreateCriticalFileReadException(lineNumber, message);
            }

            return (DamType) damTypeValue;
        }

        /// <summary>
        /// Attempts to match the given text to a DAMWAND key-value pair. If a match is
        /// found, the value is parsed and validated. If valid, the value is stored.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches a DAMWAND key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DAMWAND key
        /// does not represent a valid <see cref="SheetPileType"/> value or has already been defined.</exception>
        private bool TryReadSheetPileType(string text, DikeProfileData data, int lineNumber)
        {
            Match profileTypeMatch = new Regex(@"^DAMWAND(\s+(?<sheetpiletype>.+?)?)?\s*$").Match(text);
            if (profileTypeMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.DAMWAND, lineNumber);

                string readSheetPileTypeText = profileTypeMatch.Groups["sheetpiletype"].Value;
                SheetPileType sheetPileType = ParseSheetPileType(lineNumber, readSheetPileTypeText);

                data.SheetPileType = sheetPileType;
                readKeywords |= Keywords.DAMWAND;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the sheet piling type from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readSheetPileTypeText">The text.</param>
        /// <returns>The read sheet piling type.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readSheetPileTypeText"/>
        /// does not represent a <see cref="SheetPileType"/>.</exception>
        private SheetPileType ParseSheetPileType(int lineNumber, string readSheetPileTypeText)
        {
            int sheetPileTypeValue;
            try
            {
                sheetPileTypeValue = int.Parse(readSheetPileTypeText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseSheetPileType_SheetPileType_0_must_be_in_range,
                                               readSheetPileTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseSheetPileType_SheetPileType_0_must_be_in_range,
                                               readSheetPileTypeText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }

            if (!CanSafelyCastToEnum<SheetPileType>(sheetPileTypeValue))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseSheetPileType_SheetPileType_0_must_be_in_range,
                                               sheetPileTypeValue);
                throw CreateCriticalFileReadException(lineNumber, message);
            }

            return (SheetPileType) sheetPileTypeValue;
        }

        /// <summary>
        /// Attempts to match the given text to a DAMHOOGTE key-value pair. If a match is
        /// found, the value is parsed.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches a DAMHOOGTE key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the DAMHOOGTE key
        /// does not represent a valid number or has already been defined.</exception>
        private bool TryReadDamHeight(string text, DikeProfileData data, int lineNumber)
        {
            Match damHeightMatch = new Regex(@"^DAMHOOGTE(\s+(?<damheight>.+?)?)?\s*$").Match(text);
            if (damHeightMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.DAMHOOGTE, lineNumber);

                string readDamHeightText = damHeightMatch.Groups["damheight"].Value;
                double damHeight = ParseDamHeight(readDamHeightText, lineNumber);

                data.DamHeight = damHeight;
                readKeywords |= Keywords.DAMHOOGTE;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the height of the dam from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readDamHeightText">The text.</param>
        /// <returns>The height of the dam.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readDamHeightText"/>
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
        /// found, the value is parsed.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="data">The data to be updated.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns><c>true</c> if the text matches a KRUINHOOGTE key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">The value after the KRUINHOOGTE key
        /// does not represent a valid number or has already been defined.</exception>
        private bool TryReadDikeHeight(string text, DikeProfileData data, int lineNumber)
        {
            Match crestLevelMatch = new Regex(@"^KRUINHOOGTE(\s+(?<dikeheight>.+?)?)?\s*$").Match(text);
            if (crestLevelMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.KRUINHOOGTE, lineNumber);

                string readDikeHeightText = crestLevelMatch.Groups["dikeheight"].Value;
                double crestLevel = ParseDikeHeight(lineNumber, readDikeHeightText);

                data.DikeHeight = crestLevel;
                readKeywords |= Keywords.KRUINHOOGTE;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the height of the dike from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readDikeHeightText">The text.</param>
        /// <returns>The height of the dike.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readDikeHeightText"/>
        /// does not represent a number.</exception>
        private double ParseDikeHeight(int lineNumber, string readDikeHeightText)
        {
            try
            {
                return double.Parse(readDikeHeightText, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDikeHeight_DikeHeight_0_not_number,
                                               readDikeHeightText);
                throw CreateCriticalFileReadException(lineNumber, message, e);
            }
            catch (OverflowException e)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ParseDikeHeight_DikeHeight_0_overflows,
                                               readDikeHeightText);
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
        /// <returns><c>true</c> if the text matches a DIJK key-value pair and has been
        /// validated successfully; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when
        /// <list type="bullet">
        /// <item>The value after the DIJK key does not represent a valid number.</item>
        /// <item>Any of read the parameters in the following data block is invalid.</item>
        /// <item>The keyword has already been defined.</item>
        /// <item>The X-coordinates of the dike are not monotonically increasing.</item>
        /// </list></exception>
        private bool TryReadDikeRoughnessPoints(string text, DikeProfileData data, TextReader reader, ref int lineNumber)
        {
            Match dikeGeometryMatch = new Regex(@"^DIJK(\s+(?<dikegeometry>.+?)?)?\s*$").Match(text);
            if (dikeGeometryMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.DIJK, lineNumber);

                string readDikeGeometryCountText = dikeGeometryMatch.Groups["dikegeometry"].Value;
                int numberOfPoints = ParseNumberOfDikePoints(lineNumber, readDikeGeometryCountText);

                ValidateDikePointCount(numberOfPoints, lineNumber);

                if (numberOfPoints == 0)
                {
                    readKeywords |= Keywords.DIJK;
                    return true;
                }

                data.DikeGeometry = new RoughnessPoint[numberOfPoints];
                for (var i = 0; i < numberOfPoints; i++)
                {
                    lineNumber++;
                    text = ReadLineAndHandleIOExceptions(reader, lineNumber);
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        string message = string.Format(Resources.DikeProfileDataReader_TryReadDikeRoughnessPoints_DikeCount_0_does_not_correspond_ExpectedCount_1_,
                                                       i, numberOfPoints);
                        throw CreateCriticalFileReadException(lineNumber, message);
                    }

                    RoughnessPoint roughnessPoint = ReadRoughnessPoint(text, lineNumber);
                    data.DikeGeometry[i] = roughnessPoint;

                    if (i > 0)
                    {
                        ValidateDikePointsAreMonotonicallyIncreasing(roughnessPoint.Point,
                                                                     data.DikeGeometry[i - 1].Point,
                                                                     lineNumber);
                    }
                }

                readKeywords |= Keywords.DIJK;
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
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readDikeGeometryCountText"/>
        /// does not represent a number.</exception>
        private int ParseNumberOfDikePoints(int lineNumber, string readDikeGeometryCountText)
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
        /// Validates the number of dike points to be read from file.
        /// </summary>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="numberOfElements"/>
        /// is negative.</exception>
        private void ValidateDikePointCount(int numberOfElements, int lineNumber)
        {
            if (numberOfElements < 0)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ReadDikeProfileData_DikeCount_cannot_be_negative,
                                               numberOfElements);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
        }

        /// <summary>
        /// Matches the given text to a value triplet representing a roughness point. If 
        /// a match is found, the values are parsed and validated. If valid, the resulting
        /// <see cref="RoughnessPoint"/> based on the values will be returned.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>A <see cref="RoughnessPoint"/> instance built using data in <paramref name="text"/>.</returns>
        /// <exception cref="CriticalFileReadException">Any parameter value in the roughness
        /// point triplet does not represent a valid number or the roughness parameter is
        /// not valid.</exception>
        private RoughnessPoint ReadRoughnessPoint(string text, int lineNumber)
        {
            Match roughnessSectionDataMatch = new Regex(@"^(\s*)?(?<localx>\S+)\s+(?<localz>\S+)\s+(?<roughness>\S+)\s*$").Match(text);
            if (!roughnessSectionDataMatch.Success)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ReadRoughnessPoint_Line_0_not_x_y_roughness_definition,
                                               text);
                throw CreateCriticalFileReadException(lineNumber, message);
            }

            string readLocalXText = roughnessSectionDataMatch.Groups["localx"].Value;
            double localX = ParseRoughnessPointParameter(readLocalXText,
                                                         Resources.DikeProfileDataReader_ReadRoughnessPoint_X_DisplayName,
                                                         lineNumber);

            string readLocalZText = roughnessSectionDataMatch.Groups["localz"].Value;
            double localZ = ParseRoughnessPointParameter(readLocalZText,
                                                         Resources.DikeProfileDataReader_ReadRoughnessPoint_Z_DisplayName,
                                                         lineNumber);

            string readRoughnessText = roughnessSectionDataMatch.Groups["roughness"].Value;
            double roughness = ParseRoughnessPointParameter(readRoughnessText,
                                                            Resources.DikeProfileDataReader_ReadRoughnessPoint_Roughness_DisplayName,
                                                            lineNumber);

            ValidateRoughness(roughness, lineNumber);

            return new RoughnessPoint(new Point2D(localX, localZ), roughness);
        }

        /// <summary>
        /// Parses some <c>double</c> parameter from a piece of text.
        /// </summary>
        /// <param name="parameterName">The name of the parameter as shown to the user.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readParameterText">The value text to be parsed.</param>
        /// <returns>The parameter value.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readParameterText"/>
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
        /// Validates the roughness.
        /// </summary>
        /// <param name="roughness">The roughness.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="roughness"/>
        /// is outside the range [0.5, 1].</exception>
        private void ValidateRoughness(double roughness, int lineNumber)
        {
            if (!roughnessValidityRange.InRange(roughness))
            {
                string rangeText = roughnessValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture);
                string message = string.Format(Resources.DikeProfileDataReader_ReadRoughnessPoint_Roughness_0_must_be_Range_1_,
                                               roughness, rangeText);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
        }

        /// <summary>
        /// Validates that two dike points are monotonically increasing.
        /// </summary>
        /// <param name="currentPoint">The current point.</param>
        /// <param name="previousPoint">The previous point.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="currentPoint"/>
        /// has an X-coordinate before or equal to that of <paramref name="previousPoint"/>.</exception>
        private void ValidateDikePointsAreMonotonicallyIncreasing(Point2D currentPoint, Point2D previousPoint, int lineNumber)
        {
            if (currentPoint.X <= previousPoint.X)
            {
                throw CreateCriticalFileReadException(lineNumber, Resources.DikeProfileDataReader_ValidateDikePointsAreMonotonicallyIncreasing_Error_message);
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
        /// <exception cref="CriticalFileReadException">Thrown when
        /// <list type="bullet">
        /// <item>The value after the VOORLAND key does not represent a valid number.</item>
        /// <item>Any of read the parameters in the following data block is invalid.</item>
        /// <item>The keyword has already been defined.</item>
        /// <item>The X-coordinates of the foreshore are not monotonically increasing.</item>
        /// </list></exception>
        private bool TryReadForeshoreRoughnessPoints(string text, DikeProfileData data, TextReader reader, ref int lineNumber)
        {
            Match foreshoreGeometryMatch = new Regex(@"^VOORLAND(\s+(?<foreshoregeometry>.+?)?)?\s*$").Match(text);
            if (foreshoreGeometryMatch.Success)
            {
                ValidateNoPriorParameterDefinition(Keywords.VOORLAND, lineNumber);

                string readForeshoreCountText = foreshoreGeometryMatch.Groups["foreshoregeometry"].Value;
                int numberOfElements = ParseNumberOfForeshoreElements(readForeshoreCountText, lineNumber);

                ValidateForeshorePointCount(numberOfElements, lineNumber);

                if (numberOfElements == 0)
                {
                    readKeywords |= Keywords.VOORLAND;
                    return true;
                }

                data.ForeshoreGeometry = new RoughnessPoint[numberOfElements];
                for (var i = 0; i < numberOfElements; i++)
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

                    if (i > 0)
                    {
                        ValidateForeshorePointsAreMonotonicallyIncreasing(roughnessPoint.Point,
                                                                          data.ForeshoreGeometry[i - 1].Point,
                                                                          lineNumber);
                    }
                }

                readKeywords |= Keywords.VOORLAND;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the number of foreshore roughness points from a piece of text.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="readForeshoreCountText">The text.</param>
        /// <returns>The number of foreshore roughness points.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="readForeshoreCountText"/>
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

        /// <summary>
        /// Validates the number of foreshore points.
        /// </summary>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="numberOfElements"/>
        /// is negative.</exception>
        private void ValidateForeshorePointCount(int numberOfElements, int lineNumber)
        {
            if (numberOfElements < 0)
            {
                string message = string.Format(Resources.DikeProfileDataReader_ReadDikeProfileData_ForeshoreCount_0_cannot_be_negative,
                                               numberOfElements);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
        }

        /// <summary>
        /// Validates that two foreshore points are monotonically increasing.
        /// </summary>
        /// <param name="currentPoint">The current point.</param>
        /// <param name="previousPoint">The previous point.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="currentPoint"/>
        /// has an X-coordinate before or equal to that of <paramref name="previousPoint"/>.</exception>
        private void ValidateForeshorePointsAreMonotonicallyIncreasing(Point2D currentPoint, Point2D previousPoint, int lineNumber)
        {
            if (currentPoint.X <= previousPoint.X)
            {
                throw CreateCriticalFileReadException(lineNumber, Resources.DikeProfileDataReader_ValidateForeshorePointsAreMonotonicallyIncreasing_Error_message);
            }
        }

        private bool TryReadMemo(string text, DikeProfileData data, StreamReader reader)
        {
            Match memoMatch = new Regex(@"^MEMO\s*$").Match(text);
            if (memoMatch.Success)
            {
                data.Memo = reader.ReadToEnd();
                readKeywords |= Keywords.MEMO;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates that there is no prior parameter definition of a given parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="parameter"/>
        /// already has been defined in the file at a line prior to <paramref name="lineNumber"/>.</exception>
        private void ValidateNoPriorParameterDefinition(Keywords parameter, int lineNumber)
        {
            if (readKeywords.HasFlag(parameter))
            {
                string message = string.Format(Resources.DikeProfileDataReader_ValidateNoPriorParameterDefinition_Parameter_0_already_defined,
                                               parameter);
                throw CreateCriticalFileReadException(lineNumber, message);
            }
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
        /// <returns>The read line, or <c>null</c> when at the end of the file.</returns>
        /// <exception cref="CriticalFileReadException">An critical I/O exception occurred.</exception>
        private string ReadLineAndHandleIOExceptions(TextReader reader, int currentLine)
        {
            try
            {
                return reader.ReadLine();
            }
            catch (OutOfMemoryException e)
            {
                throw CreateCriticalFileReadException(currentLine, UtilResources.Error_Line_too_big_for_RAM, e);
            }
            catch (IOException e)
            {
                string message = new FileReaderErrorMessageBuilder(fileBeingRead).Build(string.Format(UtilResources.Error_General_IO_ErrorMessage_0_, e.Message));
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Returns a configured instance of <see cref="CriticalFileReadException"/>.
        /// </summary>
        /// <param name="currentLine">The line number being read.</param>
        /// <param name="criticalErrorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>New <see cref="CriticalFileReadException"/> with message and inner exception set.</returns>
        private CriticalFileReadException CreateCriticalFileReadException(int currentLine, string criticalErrorMessage, Exception innerException = null)
        {
            string locationDescription = string.Format(UtilResources.TextFile_On_LineNumber_0_, currentLine);
            string message = new FileReaderErrorMessageBuilder(fileBeingRead).WithLocation(locationDescription)
                                                                             .Build(criticalErrorMessage);
            return new CriticalFileReadException(message, innerException);
        }

        /// <summary>
        /// Handles the error-case that the file has unexpected text.
        /// </summary>
        /// <param name="text">The unexpected text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <exception cref="CriticalFileReadException">Thrown when calling this method, due to
        /// having read an unexpected piece of text from the file.</exception>
        private void HandleUnexpectedText(string text, int lineNumber)
        {
            string message = string.Format(Resources.DikeProfileDataReader_HandleUnexpectedText_Line_0_is_invalid,
                                           text);
            throw CreateCriticalFileReadException(lineNumber, message);
        }

        private void ValidateNoMissingKeywords()
        {
            string[] missingKeywords = GetMissingKeywords();
            if (missingKeywords.Any())
            {
                string criticalErrorMessage = string.Format(Resources.DikeProfileDataReader_ValidateNoMissingKeywords_List_mising_keywords_0_,
                                                            string.Join(", ", missingKeywords));
                string message = new FileReaderErrorMessageBuilder(fileBeingRead)
                    .Build(criticalErrorMessage);
                throw new CriticalFileReadException(message);
            }
        }

        private string[] GetMissingKeywords()
        {
            var requiredKeywords = new[]
            {
                Keywords.VERSIE,
                Keywords.ID,
                Keywords.RICHTING,
                Keywords.DAM,
                Keywords.DAMHOOGTE,
                Keywords.VOORLAND,
                Keywords.DAMWAND,
                Keywords.KRUINHOOGTE,
                Keywords.DIJK,
                Keywords.MEMO
            };
            return requiredKeywords.Where(z => !readKeywords.HasFlag(z))
                                   .Select(z => z.ToString())
                                   .ToArray();
        }
    }
}