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

using System;
using Core.Common.Util.Settings;

namespace Core.Common.Util.TestUtil.Settings
{
    /// <summary>
    /// Configures <see cref="SettingsHelper.Instance"/> to temporarily use a different
    /// instance.
    /// </summary>
    public class UseCustomSettingsHelper : IDisposable
    {
        private readonly ISettingsHelper originalHelper;
        private bool disposed;

        /// <summary>
        /// Creates a new instance of <see cref="UseCustomSettingsHelper"/>.
        /// </summary>
        /// <param name="settingsHelper">The temporary helper to be used.</param>
        public UseCustomSettingsHelper(ISettingsHelper settingsHelper)
        {
            originalHelper = SettingsHelper.Instance;
            SettingsHelper.Instance = settingsHelper;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            SettingsHelper.Instance = originalHelper;
            disposed = true;
        }
    }
}