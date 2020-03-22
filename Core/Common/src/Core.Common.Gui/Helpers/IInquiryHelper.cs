// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Core.Common.Gui.Helpers
{
    /// <summary>
    /// Specifies the interface for classes that can be used to inquire information from
    /// the user.
    /// </summary>
    public interface IInquiryHelper
    {
        /// <summary>
        /// Returns the path of an existing file that the user has chosen.
        /// </summary>
        /// <param name="fileFilter">A filter to which the path returned complies.</param>
        /// <returns>A file location, or <c>null</c> if no location was chosen.</returns>
        string GetSourceFileLocation(string fileFilter);

        /// <summary>
        /// Returns the path to a file, which may or may not exist yet, that the user has chosen.
        /// </summary>
        /// <param name="fileFilter">A filter to which the path returned complies.</param>
        /// <param name="suggestedFileName">The initial file name the user can choose.</param>
        /// <returns>A path to a file, which may or may not exist yet, or <c>null</c> if no location
        /// was chosen.</returns>
        string GetTargetFileLocation(string fileFilter, string suggestedFileName);

        /// <summary>
        /// Gets the confirmation of a user.
        /// </summary>
        /// <param name="query">The query to which the user needs to answer.</param>
        /// <returns><c>true</c> if the user confirmed, <c>false</c> otherwise.</returns>
        bool InquireContinuation(string query);

        /// <summary>
        /// Checks with the user if a certain optional step in some workflow should be
        /// performed or not.
        /// </summary>
        /// <param name="workflowDescription">The short descriptive text on the workflow
        /// currently being performed.</param>
        /// <param name="query">The query to which the user needs to answer.</param>
        /// <returns>How the workflow should continue.</returns>
        OptionalStepResult InquirePerformOptionalStep(string workflowDescription, string query);
    }
}