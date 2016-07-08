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
using System.Collections.Generic;
using System.IO;

using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Reads the settings file defined at <see cref="IAssessmentSection"/> level.
    /// </summary>
    public class AssessmentSectionSettingsReader
    {
        private const int assessmentSectionIdColumnIndex = 0;
        private const int lengthEffectColumnIndex = 1;
        private const char separatorCharacter = ';';
        private const string duneAssessmentSectionFlag = "Duin";

        /// <summary>
        /// Reads the settings.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="filePath"/> is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="filePath"/> is an empty string.</exception>
        /// <exception cref="FileNotFoundException">When <paramref name="filePath"/> points
        /// to a file that doesn't exist.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="IOException">When either
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> includes an incorrect
        /// or invalid syntax for file name, directory name, or volume label.</item>
        /// <item>When an I/O error occurs while reading the file.</item>
        /// </list></exception>
        /// <exception cref="OutOfMemoryException">When there is insufficient memory to allocate
        /// a buffer to a line in the file.</exception>
        /// <exception cref="IndexOutOfRangeException">When reading a line that does not have
        /// at least 2 columns or when the line doesn't have columns aren't separated by a ';'.</exception>
        /// <exception cref="FormatException">When reading a line where the second column text
        /// doesn't represent a number.</exception>
        /// <exception cref="OverflowException">When reading a line where the second column text
        /// represents a number that is too big or too small to be stored in a <see cref="double"/>.</exception>
        public AssessmentSectionSettings[] ReadSettings(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine();// Skip header

                var list = new List<AssessmentSectionSettings>();
                string currentLine;
                while (null != (currentLine = reader.ReadLine()))
                {
                    AssessmentSectionSettings settingsDefinition = ReadAssessmentSectionSettings(currentLine);
                    list.Add(settingsDefinition);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Reads the assessment section settings from a line of text.
        /// </summary>
        /// <param name="lineToParse">The line to be parsed.</param>
        /// <returns>The initialized <see cref="AssessmentSectionSettings"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">When <paramref name="lineToParse"/>
        /// does not have at least 2 columns or when the columns aren't separated by a ';'.</exception>
        /// <exception cref="FormatException">When the second column text doesn't represent a number.</exception>
        /// <exception cref="OverflowException">When the second column text represents a number
        /// that is too big or too small to be stored in a <see cref="double"/>.</exception>
        private static AssessmentSectionSettings ReadAssessmentSectionSettings(string lineToParse)
        {
            string[] lineValues = lineToParse.Split(new[]
            {
                separatorCharacter
            }, StringSplitOptions.None);
            string assessmentSectionId = lineValues[assessmentSectionIdColumnIndex];
            string nValue = lineValues[lengthEffectColumnIndex];

            if (nValue == duneAssessmentSectionFlag)
            {
                return AssessmentSectionSettings.CreateDuneAssessmentSectionSettings(assessmentSectionId);
            }

            double n = Double.Parse(nValue);
            return AssessmentSectionSettings.CreateDikeAssessmentSectionSettings(assessmentSectionId, n);
        }
    }
}