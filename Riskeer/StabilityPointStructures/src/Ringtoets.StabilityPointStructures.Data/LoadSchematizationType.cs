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

using Core.Common.Util.Attributes;
using Ringtoets.StabilityPointStructures.Data.Properties;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Defines the types of load representations that can be used 
    /// for the schematization of <see cref="StabilityPointStructure"/>.
    /// </summary>
    public enum LoadSchematizationType
    {
        /// <summary>
        /// A linear schematization. 
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LoadSchematizationType_Linear_DisplayName))]
        Linear = 1,

        /// <summary>
        /// A quadratic schematization.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LoadSchematizationType_Quadratic_DisplayName))]
        Quadratic = 2
    }
}