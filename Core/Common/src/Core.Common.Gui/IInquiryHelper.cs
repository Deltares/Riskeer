// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Common.Gui
{
    /// <summary>
    /// Specifies the interface for classes that can be used to inquire information from
    /// the user.
    /// </summary>
    public interface IInquiryHelper
    {
        /// <summary>
        /// Returns a <see cref="FileResult"/> containing a path to an existing file
        /// if the user did not cancel the inquiry.
        /// </summary>
        /// <returns>A new <see cref="FileResult"/>.</returns>
        string GetSourceFileLocation();

        /// <summary>
        /// Returns a <see cref="FileResult"/> containing a path to an existing file
        /// if the user did not cancel the inquiry.
        /// </summary>
        /// <param name="filter">A filter to which the path in the returned 
        /// <see cref="FileResult"/> complies.</param>
        /// <returns>A new <see cref="FileResult"/>.</returns>
        string GetSourceFileLocation(FileFilterGenerator filter);

        /// <summary>
        /// Returns a <see cref="FileResult"/> containing a path to a (non-existing) file 
        /// if the user did not cancel the inquiry.
        /// </summary>
        /// <returns>A <see cref="FileResult"/> containing a path to a (non-existing) file.</returns>
        string GetTargetFileLocation();

        /// <summary>
        /// Gets the confirmation of a user.
        /// </summary>
        /// <param name="query">The query to which the user needs to answer.</param>
        /// <returns><c>true</c> if the user confirmed, <c>false</c> otherwise.</returns>
        bool InquireContinuation(string query);
    }
}