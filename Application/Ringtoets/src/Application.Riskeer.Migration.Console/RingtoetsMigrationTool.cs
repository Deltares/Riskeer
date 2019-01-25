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

namespace Application.Riskeer.Migration.Console
{
    /// <summary>
    /// Entry point to the console application that can migrate a Ringtoets database file to a newer version.
    /// </summary>
    public static class RingtoetsMigrationTool
    {
        /// <summary>
        /// Main Ringtoets migration application.
        /// </summary>
        /// <param name="args">Arguments </param>
        /// <remarks>Accepted commands:<list type="bullet">
        /// <item>--help Shows help menu,</item>
        /// <item>--supported Returns if the database file is supported,</item>
        /// <item>--migrate Migrates the database file to a newer version.</item>
        /// </list></remarks>
        public static void Main(string[] args)
        {
            var ringtoetsMigrationTool = new RingtoetsMigrationConsole();
            ringtoetsMigrationTool.ExecuteConsoleTool(args);
        }
    }
}