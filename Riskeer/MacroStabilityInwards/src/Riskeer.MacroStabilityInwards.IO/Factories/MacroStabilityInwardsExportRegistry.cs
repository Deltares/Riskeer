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
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Registry used to keep track of items when exporting data.
    /// </summary>
    internal class MacroStabilityInwardsExportRegistry
    {
        private readonly Dictionary<MacroStabilityInwardsExportStageType, string> settings;
        private readonly Dictionary<MacroStabilityInwardsSoilLayer2D, string> soils;
        private readonly Dictionary<MacroStabilityInwardsExportStageType, string> geometries;
        private readonly Dictionary<MacroStabilityInwardsExportStageType, Dictionary<MacroStabilityInwardsSoilLayer2D, string>> geometryLayers;
        private readonly Dictionary<MacroStabilityInwardsExportStageType, string> soilLayers;
        private readonly Dictionary<MacroStabilityInwardsExportStageType, string> waternets;
        private readonly Dictionary<MacroStabilityInwardsExportStageType, string> waternetCreatorSettings;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsExportRegistry"/>.
        /// </summary>
        public MacroStabilityInwardsExportRegistry()
        {
            settings = new Dictionary<MacroStabilityInwardsExportStageType, string>();
            soils = new Dictionary<MacroStabilityInwardsSoilLayer2D, string>();
            geometries = new Dictionary<MacroStabilityInwardsExportStageType, string>();
            geometryLayers = new Dictionary<MacroStabilityInwardsExportStageType, Dictionary<MacroStabilityInwardsSoilLayer2D, string>>();
            soilLayers = new Dictionary<MacroStabilityInwardsExportStageType, string>();
            waternets = new Dictionary<MacroStabilityInwardsExportStageType, string>();
            waternetCreatorSettings = new Dictionary<MacroStabilityInwardsExportStageType, string>();
        }

        /// <summary>
        /// Gets the calculation settings and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, string> Settings => settings;

        /// <summary>
        /// Gets the soils and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsSoilLayer2D, string> Soils => soils;

        /// <summary>
        /// Gets the geometries and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, string> Geometries => geometries;

        /// <summary>
        /// Gets the geometry layers and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, Dictionary<MacroStabilityInwardsSoilLayer2D, string>> GeometryLayers => geometryLayers;

        /// <summary>
        /// Gets the soil layers and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, string> SoilLayers => soilLayers;

        /// <summary>
        /// Gets the waternets and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, string> Waternets => waternets;
        
        /// <summary>
        /// Gets the waternet creator settings and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<MacroStabilityInwardsExportStageType, string> WaternetCreatorSettings => waternetCreatorSettings;

        /// <summary>
        /// Adds calculation settings to the registry.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the settings for.</param>
        /// <param name="id">The id of the settings.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        public void AddSettings(MacroStabilityInwardsExportStageType stageType, string id)
        {
            ValidateStageType(stageType);

            settings.Add(stageType, id);
        }

        /// <summary>
        /// Adds an <see cref="MacroStabilityInwardsSoilLayer2D"/> to the registry.
        /// </summary>
        /// <param name="soilLayer">The soil layer to register.</param>
        /// <param name="id">The id of the soil.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/>
        /// is <c>null</c>.</exception>
        public void AddSoil(MacroStabilityInwardsSoilLayer2D soilLayer, string id)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            soils.Add(soilLayer, id);
        }

        /// <summary>
        /// Adds a geometry layer to the registry.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the geometry layer for.</param>
        /// <param name="geometryLayer">The geometry layer to register.</param>
        /// <param name="id">The id of the geometry layer.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometryLayer"/>
        /// is <c>null</c>.</exception>
        public void AddGeometryLayer(MacroStabilityInwardsExportStageType stageType, MacroStabilityInwardsSoilLayer2D geometryLayer, string id)
        {
            ValidateStageType(stageType);

            if (geometryLayer == null)
            {
                throw new ArgumentNullException(nameof(geometryLayer));
            }

            if (!geometryLayers.ContainsKey(stageType))
            {
                geometryLayers.Add(stageType, new Dictionary<MacroStabilityInwardsSoilLayer2D, string>(new LayerComparer()));
            }

            geometryLayers[stageType].Add(geometryLayer, id);
        }

        /// <summary>
        /// Adds a geometry to the register.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the geometry for.</param>
        /// <param name="id">The id of the geometry.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        public void AddGeometry(MacroStabilityInwardsExportStageType stageType, string id)
        {
            ValidateStageType(stageType);

            geometries.Add(stageType, id);
        }

        /// <summary>
        /// Adds a soil layer to the register.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the soil layer for.</param>
        /// <param name="id">The id of the soil layer.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        public void AddSoilLayer(MacroStabilityInwardsExportStageType stageType, string id)
        {
            ValidateStageType(stageType);

            soilLayers.Add(stageType, id);
        }

        /// <summary>
        /// Adds a waternet to the register.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the waternet for.</param>
        /// <param name="id">The id of the waternet.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        public void AddWaternet(MacroStabilityInwardsExportStageType stageType, string id)
        {
            ValidateStageType(stageType);

            waternets.Add(stageType, id);
        }

        /// <summary>
        /// Adds waternet creator settings to the register.
        /// </summary>
        /// <param name="stageType">The <see cref="MacroStabilityInwardsExportStageType"/>
        /// to register the waternet creator settings for.</param>
        /// <param name="id">The id of the waternet creator settings.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        public void AddWaternetCreatorSettings(MacroStabilityInwardsExportStageType stageType, string id)
        {
            ValidateStageType(stageType);

            waternetCreatorSettings.Add(stageType, id);
        }

        /// <summary>
        /// Validates the <see cref="MacroStabilityInwardsExportStageType"/>.
        /// </summary>
        /// <param name="stageType">The stage type to validate.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stageType"/>
        /// has an invalid value.</exception>
        private static void ValidateStageType(MacroStabilityInwardsExportStageType stageType)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsExportStageType), stageType))
            {
                throw new InvalidEnumArgumentException(nameof(stageType),
                                                       (int) stageType,
                                                       typeof(MacroStabilityInwardsExportStageType));
            }
        }

        private class LayerComparer : IEqualityComparer<MacroStabilityInwardsSoilLayer2D>
        {
            public bool Equals(MacroStabilityInwardsSoilLayer2D x, MacroStabilityInwardsSoilLayer2D y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(MacroStabilityInwardsSoilLayer2D obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}