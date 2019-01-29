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

namespace Migration.Console
{
    /// <summary>
    /// Enumeration that defines the possible exit codes of an application.
    /// </summary>
    /// <remarks>See <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms681381(v=vs.85).aspx">
    /// System Error Codes</a></remarks>
    public enum ErrorCode
    {
        /// <summary>
        /// Exit successfully.
        /// </summary>
        ErrorSuccess = 0,

        /// <summary>
        /// Exit because the command provided failed.
        /// </summary>
        ErrorBadCommand = 22,

        /// <summary>
        /// Exit because invalid command line arguments were provided.
        /// </summary>
        ErrorInvalidCommandLine = 160
    }
}