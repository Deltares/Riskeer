// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.IO.SoilProfile.Schema
{
    /// <summary>
    /// Defines the table and column names of the table 'StochasticSoilProfile' in the DSoil-Model database.
    /// </summary>
    internal static class SoilProfileTableDefinitions
    {
        public const string SoilProfileId = "SoilProfileId";
        public const string Dimension = "Dimension";
        public const string IsAquifer = "IsAquifer";
        public const string ProfileName = "ProfileName";
        public const string IntersectionX = "IntersectionX";
        public const string Bottom = "Bottom";
        public const string Top = "Top";
        public const string Color = "Color";
        public const string MaterialName = "MaterialName";
        public const string LayerGeometry = "LayerGeometry";
        public const string BelowPhreaticLevelDistribution = "BelowPhreaticLevelDistribution";
        public const string BelowPhreaticLevelShift = "BelowPhreaticLevelShift";
        public const string BelowPhreaticLevelMean = "BelowPhreaticLevelMean";
        public const string BelowPhreaticLevelDeviation = "BelowPhreaticLevelDeviation";
        public const string PermeabilityDistribution = "PermeabKxDistribution";
        public const string PermeabilityShift = "PermeabKxShift";
        public const string PermeabilityMean = "PermeabKxMean";
        public const string PermeabilityCoefficientOfVariation = "PermeabKxCoefficientOfVariation";
        public const string DiameterD70Distribution = "DiameterD70Distribution";
        public const string DiameterD70Shift = "DiameterD70Shift";
        public const string DiameterD70Mean = "DiameterD70Mean";
        public const string DiameterD70CoefficientOfVariation = "DiameterD70CoefficientOfVariation";

        public const string LayerCount = "LayerCount";
        public const string ProfileCount = "nrOfRows";
    }
}