using System;
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableDataModel"/>.
    /// </summary>
    internal static class PersistableDataModelFactory
    {
        /// <summary>
        /// Creates a new <see cref="PersistableDataModel"/>.
        /// </summary>
        /// <param name="calculation">The calculation to get the data from.</param>
        /// <param name="filePath">The filePath that is used.</param>
        /// <returns>A created <see cref="PersistableDataModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculation"/>
        /// has no output.</exception>
        public static PersistableDataModel Create(MacroStabilityInwardsCalculation calculation, string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (!calculation.HasOutput)
            {
                throw new InvalidOperationException("Calculation must have output.");
            }

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            return new PersistableDataModel
            {
                Info = PersistableProjectInfoFactory.Create(calculation, filePath),
                CalculationSettings = PersistableCalculationSettingsFactory.Create(calculation.Output.SlidingCurve, idFactory, registry),
                Soils = PersistableSoilCollectionFactory.Create(calculation.InputParameters.StochasticSoilProfile.SoilProfile, idFactory, registry),
                Stages = PersistableStageFactory.Create(idFactory, registry)
            };
        }
    }
}