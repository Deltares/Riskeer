using System;
using System.Collections.Generic;
using Components.Persistence.Stability.Data;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Registry used to keep track of items when exporting data.
    /// </summary>
    public class MacroStabilityInwardsPersistenceRegistry
    {
        private readonly Dictionary<PersistableCalculationSettings, string> settings;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPersistenceRegistry"/>.
        /// </summary>
        public MacroStabilityInwardsPersistenceRegistry()
        {
            settings = new Dictionary<PersistableCalculationSettings, string>();
        }

        /// <summary>
        /// Gets the created <see cref="PersistableCalculationSettings"/> and their unique identifiers.
        /// </summary>
        public IReadOnlyDictionary<PersistableCalculationSettings, string> Settings => settings;

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
    }
}