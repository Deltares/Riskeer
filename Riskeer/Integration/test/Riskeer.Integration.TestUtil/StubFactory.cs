// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Gui;
using Core.Gui.Forms.ViewHost;
using Rhino.Mocks;

namespace Riskeer.Integration.TestUtil
{
    /// <summary>
    /// Factory for creating stubs.
    /// </summary>
    public static class StubFactory
    {
        /// <summary>
        /// Creates a basic stub of <see cref="IGui"/>.
        /// </summary>
        /// <param name="mockRepository">The repository to use for creating the stub.</param>
        /// <returns>The created <see cref="IGui"/> stub.</returns>
        public static IGui CreateGuiStub(MockRepository mockRepository)
        {
            var gui = mockRepository.Stub<IGui>();

            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.ViewHost).Return(mockRepository.Stub<IViewHost>());

            gui.Replay();

            return gui;
        }
    }
}