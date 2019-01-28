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

using System;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Reads the settings defined at <see cref="IAssessmentSection"/> level.
    /// </summary>
    public class AssessmentSectionSettingsReader
    {
        private const int assessmentSectionIdColumnIndex = 0;
        private const int lengthEffectColumnIndex = 1;
        private const char separatorCharacter = ';';
        private const string duneAssessmentSectionFlag = "Duin";

        /// <summary>
        /// Reads the settings from <see cref="Resources.IHW_filecontents"/>.
        /// </summary>
        /// <returns>The fully initialized settings.</returns>
        public AssessmentSectionSettings[] ReadSettings()
        {
            string[] ihwFileLines = Resources.IHW_filecontents.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None);
            var resultArray = new AssessmentSectionSettings[ihwFileLines.Length - 1];
            for (var i = 1; i < ihwFileLines.Length; i++)
            {
                resultArray[i - 1] = ReadAssessmentSectionSettings(ihwFileLines[i]);
            }

            return resultArray;
        }

        /// <summary>
        /// Reads the assessment section settings from a line of text.
        /// </summary>
        /// <param name="lineToParse">The line to be parsed.</param>
        /// <returns>The initialized <see cref="AssessmentSectionSettings"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="lineToParse"/>
        /// does not have at least 2 columns or when the columns are not separated by a ';'.</exception>
        /// <exception cref="FormatException">Thrown when the second column text does not represent a number.</exception>
        /// <exception cref="OverflowException">Thrown when the second column text represents a number
        /// that is too big or too small to be stored in an <see cref="int"/>.</exception>
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

            int n = int.Parse(nValue);
            return AssessmentSectionSettings.CreateDikeAssessmentSectionSettings(assessmentSectionId, n);
        }
    }
}