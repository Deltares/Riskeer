// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Security;

namespace Migration.Console
{
    /// <summary>
    /// Controls class for <see cref="Environment"/>.
    /// </summary>
    public class EnvironmentControl
    {
        private static EnvironmentControl instance = new EnvironmentControl();

        /// <summary>
        /// Gets or sets the current <see cref="EnvironmentControl"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public static EnvironmentControl Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                instance = value;
            }
        }

        /// <summary>
        /// Terminates this process and gives the underlying operating system the specified exit code.
        /// </summary>
        /// <param name="errorCode">Exit code to be given to the operating system.</param>
        /// <exception cref="SecurityException">The caller does not have sufficient security permission to perform this function.</exception>
        public virtual void Exit(ErrorCode errorCode)
        {
            Environment.Exit((int) errorCode);
        }
    }
}