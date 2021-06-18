// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing;
using Core.Gui.Forms.Main;
using Rhino.Mocks;

namespace Core.Gui.TestUtil
{
    /// <summary>
    /// Helper class for dealing with <see cref="IMainWindow"/> dependencies.
    /// </summary>
    public static class MainWindowTestHelper
    {
        /// <summary>
        /// Creates a new <see cref="IMainWindow"/> stub.
        /// </summary>
        /// <param name="mockRepository">The <see cref="MockRepository"/> to use.</param>
        /// <returns>The <see cref="IMainWindow"/> stub.</returns>
        public static IMainWindow CreateMainWindowStub(MockRepository mockRepository)
        {
            var mainWindow = mockRepository.Stub<IMainWindow>();

            mainWindow.Stub(mw => mw.ApplicationIcon).Return(SystemIcons.Application);
            mainWindow.Stub(mw => mw.Handle).Return(IntPtr.Zero);

            return mainWindow;
        }
    }
}