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
using System.Collections.Generic;
using System.ComponentModel;
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Registry used to keep track of items when exporting data.
    /// </summary>
    internal class MacroStabilityInwardsExportRegistry
    {
        private readonly Dictionary<MacroStabilityInwardsExportStageType, string> settings;
        private readonly Dictionary<IMacroStabilityInwardsSoilLayer, string> soils;
        private readonly Dictionary<IMacroStabilityInwardsSoilLayer, string> geometries;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsExportRegistry"/>.
        /// </summary>
        public MacroStabilityInwardsExportRegistry()
        {
            settings = new Dictionary<MacroStabilityInwardsExportStageType, string>();
            soils = new Dictionary<IMacroStabilityInwardsSoilLayer, string>();
            geometries = new Dictionary<IMacroStabilityInwardsSoilLayer, string>();
        }

        /// <summary>
        /// Gets the created <see cref="PersistableCalculationSettings"/> and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, string> Settings => settings;

        /// <summary>
        /// Gets the soils and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<IMacroStabilityInwardsSoilLayer, string> Soils => soils;

        /// <summary>
        /// Gets the geometries and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<IMacroStabilityInwardsSoilLayer, string> Geometries => geometries;

        /// <summary>
        /// Adds a created <see cref="PersistableCalculationSettings"/> to the registry.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the settings for.</param>
        /// <param name="id">The id of the settings.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        public void Add(MacroStabilityInwardsExportStageType stageType, string id)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsExportStageType), stageType))
            {
                throw new InvalidEnumArgumentException(nameof(stageType),
                                                       (int) stageType,
                                                       typeof(MacroStabilityInwardsExportStageType));
            }

            settings.Add(stageType, id);
        }

        /// <summary>
        /// Adds an <see cref="IMacroStabilityInwardsSoilLayer"/> to the registry.
        /// </summary>
        /// <param name="soilLayer">The soil layer to register.</param>
        /// <param name="id">The id of the settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/>
        /// is <c>null</c>.</exception>
        public void AddSoil(IMacroStabilityInwardsSoilLayer soilLayer, string id)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            soils.Add(soilLayer, id);
        }

        /// <summary>
        /// Adds an <see cref="IMacroStabilityInwardsSoilLayer"/> to the registry.
        /// </summary>
        /// <param name="geometryLayer">The geometry layer to register.</param>
        /// <param name="id">The id of the settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometryLayer"/>
        /// is <c>null</c>.</exception>
        public void AddGeometry(IMacroStabilityInwardsSoilLayer geometryLayer, string id)
        {
            if (geometryLayer == null)
            {
                throw new ArgumentNullException(nameof(geometryLayer));
            }

            geometries.Add(geometryLayer, id);
        }
    }
}