using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableStage"/>.
    /// </summary>
    internal static class PersistableStageFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="PersistableStage"/>.
        /// </summary>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableStage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public static IEnumerable<PersistableStage> Create(IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var stages = new List<PersistableStage>();

            for (var i = 0; i < 2; i++)
            {
                stages.Add(new PersistableStage
                {
                    Id = idFactory.Create(),
                    CalculationSettingsId = registry.Settings.ElementAt(i).Value
                });
            }

            return stages;
        }
    }
}