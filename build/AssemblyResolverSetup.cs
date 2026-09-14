// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Runtime.CompilerServices;

namespace AssemblyResolver
{
    /// <summary>
    /// Installs the assembly resolver from a module initializer.
    /// </summary>
    /// <remarks>
    /// This is a fallback for runs where the <c>AssemblyResolverStartupHook</c> is not active (i.e. when
    /// <c>DOTNET_STARTUP_HOOKS</c> is not set). A module initializer only runs on first member access on the module,
    /// which is too late for test discovery, hence the startup hook. Both paths share the same idempotency guard, so
    /// whichever runs first wins.
    /// </remarks>
    internal static class AssemblyResolverSetup
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            AssemblyResolverInstaller.Install();
        }
    }
}
