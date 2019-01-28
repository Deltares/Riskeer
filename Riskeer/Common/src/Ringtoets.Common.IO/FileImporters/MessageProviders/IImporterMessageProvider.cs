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

namespace Ringtoets.Common.IO.FileImporters.MessageProviders
{
    /// <summary>
    /// Interface for providing messages for the importers.
    /// </summary>
    public interface IImporterMessageProvider
    {
        /// <summary>
        /// Gets the progress text to be displayed when adding data to the model.
        /// </summary>
        /// <returns>The progress text.</returns>
        string GetAddDataToModelProgressText();

        /// <summary>
        /// Gets the cancelled log message text to be displayed when cancelling an
        /// importer action.
        /// </summary>
        /// <param name="typeDescriptor">The type descriptor of the items that were to 
        /// be imported.</param>
        /// <returns>The log message.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeDescriptor"/>
        /// is <c>null</c>.</exception>
        string GetCancelledLogMessageText(string typeDescriptor);

        /// <summary>
        /// Gets the log message when an importer action failed to update the data.
        /// </summary>
        /// <param name="typeDescriptor">The type descriptor of the items that were to 
        /// be imported.</param>
        /// <returns>The log message.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeDescriptor"/>
        /// is <c>null</c>.</exception>
        string GetUpdateDataFailedLogMessageText(string typeDescriptor);
    }
}