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

namespace Ringtoets.Common.IO.TestUtil
{
    /// <summary>
    /// Class that holds expected progress notification data.
    /// </summary>
    public class ExpectedProgressNotification
    {
        /// <summary>
        /// Gets or sets the text that is expected.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the current step that is expected.
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// Gets or sets the total number of steps that are expected.
        /// </summary>
        public int TotalNumberOfSteps { get; set; }
    }
}