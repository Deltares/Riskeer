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

        private const string idPattern = @"^ID\s+(?<id>\w+)$";
        private const string versionPattern = @"^VERSIE\s+(?<version>\S+)$";
        private const string orientationPattern = @"^RICHTING\s+(?<orientation>\d+(?:\.\d+)?)$";
        private const string damTypePattern = @"^DAM\s+(?<damtype>\d)$";
        private const string profileTypePattern = @"^DAMWAND\s+(?<profiletype>\d)$";
        private const string damHeightPattern = @"^DAMHOOGTE\s+(?<damheight>-?\d+(?:\.\d+)?)$";
        private const string crestLevelPattern = @"^KRUINHOOGTE\s+(?<crestlevel>-?\d+(?:\.\d+)?)$";
        private const string dikeGeometryPattern = @"^DIJK\s+(?<dikegeometry>\d+)$";
        private const string foreshoreGeometryPattern = @"^VOORLAND\s+(?<foreshoregeometry>\d+)$";
        private const string roughnessSectionPattern = @"^(?<localx>-?\d+(?:\.\d+)?)\s+(?<localz>-?\d+(?:\.\d+)?)\s+(?<roughness>-?\d+(?:\.\d+)?)$";
        private const string memoPattern = @"^MEMO$";

        private string fileBeingRead;

        /// <summary>
        /// Reads the *.prfl file for dike profile data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The read dike profile data.</returns>
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
            ParametersFoundInFile readParameters = ParametersFoundInFile.None;
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

                    Match versionMatch = new Regex(versionPattern).Match(text);
                    if (versionMatch.Success)
                    {
                        //versionMatch.Groups["version"].Value;
                        // TODO: Validate version (needs to be 4.0)
                        readParameters |= ParametersFoundInFile.VERSIE;
                        continue;
                    }

                    Match idMatch = new Regex(idPattern).Match(text);
                    if (idMatch.Success)
                    {
                        data.Id = idMatch.Groups["id"].Value;
                        // TODO: Validate id (needs different regex)
                        readParameters |= ParametersFoundInFile.ID;
                        continue;
                    }
                    Match orientationMatch = new Regex(orientationPattern).Match(text);
                    if (orientationMatch.Success)
                    {
                        data.Orientation = double.Parse(orientationMatch.Groups["orientation"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate orientation [0, 360] range
                        // TODO: Validate can be parsed
                        readParameters |= ParametersFoundInFile.RICHTING;
                        continue;
                    }
                    Match damTypeMatch = new Regex(damTypePattern).Match(text);
                    if (damTypeMatch.Success)
                    {
                        data.DamType = (DamType)int.Parse(damTypeMatch.Groups["damtype"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate damtype [0, 3] range
                        // TODO: Validate can be parsed
                        readParameters |= ParametersFoundInFile.DAM;
                        continue;
                    }
                    Match profileTypeMatch = new Regex(profileTypePattern).Match(text);
                    if (profileTypeMatch.Success)
                    {
                        data.ProfileType = (ProfileType)int.Parse(profileTypeMatch.Groups["profiletype"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate profile type [0, 2] range
                        // TODO: Validate can be parsed
                        readParameters |= ParametersFoundInFile.DAMWAND;
                        continue;
                    }
                    Match damHeightMatch = new Regex(damHeightPattern).Match(text);
                    if (damHeightMatch.Success)
                    {
                        data.DamHeight = double.Parse(damHeightMatch.Groups["damheight"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate can be parsed
                        readParameters |= ParametersFoundInFile.DAMHOOGTE;
                        continue;
                    }
                    Match crestLevelMatch = new Regex(crestLevelPattern).Match(text);
                    if (crestLevelMatch.Success)
                    {
                        data.CrestLevel = double.Parse(crestLevelMatch.Groups["crestlevel"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate can be parsed
                        readParameters |= ParametersFoundInFile.KRUINHOOGTE;
                        continue;
                    }
                    Match dikeGeometryMatch = new Regex(dikeGeometryPattern).Match(text);
                    if (dikeGeometryMatch.Success)
                    {
                        int numberOfElements = int.Parse(dikeGeometryMatch.Groups["dikegeometry"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate dikegeometry count > 0
                        // TODO: Validate can be parsed
                        if (numberOfElements == 0)
                        {
                            readParameters |= ParametersFoundInFile.DIJK;
                            continue;
                        }
                        data.DikeGeometry = new RoughnessProfileSection[numberOfElements - 1];

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
                        readParameters |= ParametersFoundInFile.DIJK;
                        continue;
                    }
                    Match foreshoreGeometryMatch = new Regex(foreshoreGeometryPattern).Match(text);
                    if (foreshoreGeometryMatch.Success)
                    {
                        int numberOfElements = int.Parse(foreshoreGeometryMatch.Groups["foreshoregeometry"].Value, CultureInfo.InvariantCulture);
                        // TODO: Validate foreshore geometry count > 0
                        // TODO: Validate can be parsed
                        if (numberOfElements == 0)
                        {
                            readParameters |= ParametersFoundInFile.VOORLAND;
                            continue;
                        }
                        data.ForeshoreGeometry = new RoughnessProfileSection[numberOfElements - 1];
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
                        readParameters |= ParametersFoundInFile.VOORLAND;
                        continue;
                    }
                    Match memoMatch = new Regex(memoPattern).Match(text);
                    if (memoMatch.Success)
                    {
                        data.Memo = reader.ReadToEnd();
                        readParameters |= ParametersFoundInFile.MEMO;
                        continue;
                    }
                }
            }
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
            string[] missingParameters = requiredParameters.Where(z => !readParameters.HasFlag(z)).Select(z => z.ToString()).ToArray();
            if (missingParameters.Any())
            {
                string criticalErrorMessage = string.Format("De volgende parameter(s) zijn niet aanwezig in het bestand: {0}",
                                                            String.Join(", ", missingParameters));
                var message = new FileReaderErrorMessageBuilder(fileBeingRead)
                    .Build(criticalErrorMessage);
                throw new CriticalFileReadException(message);
            }
            return data;
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
    }
}