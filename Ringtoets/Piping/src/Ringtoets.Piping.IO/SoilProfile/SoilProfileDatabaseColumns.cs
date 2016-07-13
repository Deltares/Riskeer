﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class contains the column names that are used when querying the DSoil-Model database.
    /// </summary>
    internal static class SoilProfileDatabaseColumns
    {
        internal const string SoilProfileId = "SoilProfileId";
        internal const string ProfileCount = "nrOfRows";
        internal const string Dimension = "Dimension";
        internal const string IsAquifer = "IsAquifer";
        internal const string ProfileName = "ProfileName";
        internal const string IntersectionX = "IntersectionX";
        internal const string Bottom = "Bottom";
        internal const string Top = "Top";
        internal const string Color = "Color";
        internal const string MaterialName = "MaterialName";
        internal const string LayerGeometry = "LayerGeometry";
        internal const string BelowPhreaticLevelDistribution = "BelowPhreaticLevelDistribution";
        internal const string BelowPhreaticLevelShift = "BelowPhreaticLevelShift";
        internal const string BelowPhreaticLevelMean = "BelowPhreaticLevelMean";
        internal const string BelowPhreaticLevelDeviation = "BelowPhreaticLevelDeviation";
        internal const string PermeabilityDistribution = "PermeabKxDistribution";
        internal const string PermeabilityShift = "PermeabKxShift";
        internal const string PermeabilityMean = "PermeabKxMean";
        internal const string PermeabilityDeviation = "PermeabKxDeviation";
        internal const string DiameterD70Distribution = "DiameterD70Distribution";
        internal const string DiameterD70Shift = "DiameterD70Shift";
        internal const string DiameterD70Mean = "DiameterD70Mean";
        internal const string DiameterD70Deviation = "DiameterD70Deviation";
        internal const string LayerCount = "LayerCount";
    }
}