// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Reflection;
using Components.Persistence.Stability.Data;
using Core.Common.Util.Reflection;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.IO.Properties;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableProjectInfo"/>.
    /// </summary>
    internal static class PersistableProjectInfoFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="PersistableProjectInfo"/>.
        /// </summary>
        /// <param name="calculation">The calculation to use.</param>
        /// <param name="filePath">The file path to use.</param>
        /// <returns>A created <see cref="PersistableProjectInfo"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        public static PersistableProjectInfo Create(MacroStabilityInwardsCalculation calculation, string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            return new PersistableProjectInfo
            {
                Path = filePath,
                Project = calculation.Name,
                CrossSection = calculation.InputParameters.SurfaceLine.Name,
                ApplicationCreated = string.Format(Resources.PersistableProjectInfoFactory_Create_Riskeer_Version_0,
                                                   AssemblyUtils.GetAssemblyInfo(Assembly.GetAssembly(typeof(PersistableProjectInfoFactory))).Version),
                Remarks = Resources.PersistableProjectInfoFactory_Create_Export_from_Riskeer,
                Created = DateTime.Now,
                IsDataValidated = true
            };
        }
    }
}