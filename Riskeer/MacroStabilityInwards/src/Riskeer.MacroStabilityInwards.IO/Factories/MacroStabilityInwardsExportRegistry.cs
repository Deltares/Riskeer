using System;
using System.Collections.Generic;
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Registry used to keep track of items when exporting data.
    /// </summary>
    internal class MacroStabilityInwardsExportRegistry
    {
        private readonly Dictionary<PersistableCalculationSettings, string> settings;
        private readonly Dictionary<IMacroStabilityInwardsSoilLayer, string> soils;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsExportRegistry"/>.
        /// </summary>
        public MacroStabilityInwardsExportRegistry()
        {
            settings = new Dictionary<PersistableCalculationSettings, string>();
            soils = new Dictionary<IMacroStabilityInwardsSoilLayer, string>();
        }

        /// <summary>
        /// Gets the created <see cref="PersistableCalculationSettings"/> and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<PersistableCalculationSettings, string> Settings => settings;

        /// <summary>
        /// Gets the soils and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<IMacroStabilityInwardsSoilLayer, string> Soils => soils;

        /// <summary>
        /// Adds a created <see cref="PersistableCalculationSettings"/> to the registry.
        /// </summary>
        /// <param name="createdSettings">The settings to register.</param>
        /// <param name="id">The id of the settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="createdSettings"/>
        /// is <c>null</c>.</exception>
        public void Add(PersistableCalculationSettings createdSettings, string id)
        {
            if (createdSettings == null)
            {
                throw new ArgumentNullException(nameof(createdSettings));
            }

            settings.Add(createdSettings, id);
        }

        /// <summary>
        /// Adds an <see cref="IMacroStabilityInwardsSoilLayer"/> to the registry.
        /// </summary>
        /// <param name="soilLayer">The soil layer to register.</param>
        /// <param name="id">The id of the settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/>
        /// is <c>null</c>.</exception>
        public void Add(IMacroStabilityInwardsSoilLayer soilLayer, string id)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            soils.Add(soilLayer, id);
        }
    }
}