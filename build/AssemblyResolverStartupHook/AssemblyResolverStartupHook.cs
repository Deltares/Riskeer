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

/// <summary>
/// Startup hook that installs the Riskeer assembly resolver before the entry assembly is loaded.
/// </summary>
/// <remarks>
/// The runtime requires this type to be named <c>AssemblyResolverStartupHook</c>, to reside in the global namespace and to expose a
/// parameterless, non-generic <c>Initialize</c> method. It is activated by pointing <c>DOTNET_STARTUP_HOOKS</c> at
/// the absolute path of this assembly.
/// <para>
/// This runs earlier than the module initializer in <c>AssemblyResolverSetup</c>, which only executes on first member
/// access on its module and is therefore too late for test discovery: the test host loads the test assembly and the
/// adapter immediately reflects over its types, which needs the resolver to be in place already.
/// </para>
/// <para>
/// If this install command fails, the runtime will throw an exception and terminate the process. This is desired behavior, 
/// as it prevents the test host from running without the resolver in place.
/// </para>
/// </remarks>
internal static class StartupHook
{
    public static void Initialize()
    {
        AssemblyResolver.AssemblyResolverInstaller.Install();
    }
}
