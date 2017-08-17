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
    /// Defines the table and column names of the table 'StochasticSoilProfile' in the D-Soil Model database.
    /// </summary>
    internal static class SoilProfileTableDefinitions
    {
        /// <summary>
        /// The name of the database identifier column.
        /// </summary>
        public const string SoilProfileId = "SoilProfileId";

        /// <summary>
        /// The name of the alias used whether the layer is an aquifer.
        /// </summary>
        public const string IsAquifer = "IsAquifer";

        /// <summary>
        /// The name of the profile name column.
        /// </summary>
        public const string ProfileName = "ProfileName";

        /// <summary>
        /// The name of the profile intersection column.
        /// </summary>
        public const string IntersectionX = "IntersectionX";

        /// <summary>
        /// The name of the profile bottom column.
        /// </summary>
        public const string Bottom = "Bottom";

        /// <summary>
        /// The name of the profile top column.
        /// </summary>
        public const string Top = "Top";

        /// <summary>
        /// The name of the profile color column.
        /// </summary>
        public const string Color = "Color";

        /// <summary>
        /// The name of the material name column.
        /// </summary>
        public const string MaterialName = "MaterialName";

        /// <summary>
        /// The name of the profile layer geometry column.
        /// </summary>
        public const string LayerGeometry = "LayerGeometry";

        /// <summary>
        /// The name of the below phreatic level distribution column.
        /// </summary>
        public const string BelowPhreaticLevelDistribution = "BelowPhreaticLevelDistribution";

        /// <summary>
        /// The name of the below phreatic level shift column.
        /// </summary>
        public const string BelowPhreaticLevelShift = "BelowPhreaticLevelShift";

        /// <summary>
        /// The name of the below phreatic level mean column.
        /// </summary>
        public const string BelowPhreaticLevelMean = "BelowPhreaticLevelMean";

        /// <summary>
        /// The name of the below phreatic level deviation column.
        /// </summary>
        public const string BelowPhreaticLevelDeviation = "BelowPhreaticLevelDeviation";

        /// <summary>
        /// The name of the permeability distribution column.
        /// </summary>
        public const string PermeabilityDistribution = "PermeabKxDistribution";

        /// <summary>
        /// The name of the permeability shift column.
        /// </summary>
        public const string PermeabilityShift = "PermeabKxShift";

        /// <summary>
        /// The name of the permeability mean column.
        /// </summary>
        public const string PermeabilityMean = "PermeabKxMean";

        /// <summary>
        /// The name of the permeability coefficient of variation column.
        /// </summary>
        public const string PermeabilityCoefficientOfVariation = "PermeabKxCoefficientOfVariation";

        /// <summary>
        /// The name of the diameter D70 distribution column.
        /// </summary>
        public const string DiameterD70Distribution = "DiameterD70Distribution";

        /// <summary>
        /// The name of the diameter D70 shift column.
        /// </summary>
        public const string DiameterD70Shift = "DiameterD70Shift";

        /// <summary>
        /// The name of the diameter D70 mean column.
        /// </summary>
        public const string DiameterD70Mean = "DiameterD70Mean";

        /// <summary>
        /// The name of the diameter D70 coefficient of variation column.
        /// </summary>
        public const string DiameterD70CoefficientOfVariation = "DiameterD70CoefficientOfVariation";

        /// <summary>
        /// The name of the alias used for the number of layers that can be read.
        /// </summary>
        public const string LayerCount = "LayerCount";
    }
}