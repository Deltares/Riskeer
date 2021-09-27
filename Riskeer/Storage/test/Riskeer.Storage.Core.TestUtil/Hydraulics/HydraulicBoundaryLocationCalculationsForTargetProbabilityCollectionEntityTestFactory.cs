using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil.Hydraulics
{
    /// <summary>
    /// Factory for creating <see cref="HydraulicLocationCalculationForTargetProbabilityCollectionEntity"/>
    /// which can be used for testing.
    /// </summary>
    public static class HydraulicLocationCalculationForTargetProbabilityCollectionEntityTestFactory
    {
        /// <summary>
        /// Creates a minimal <see cref="HydraulicLocationCalculationForTargetProbabilityCollectionEntity"/> with no calculations
        /// and a valid probability.
        /// </summary>
        /// <returns>A valid <see cref="HydraulicLocationCalculationForTargetProbabilityCollectionEntity"/>.</returns>
        public static HydraulicLocationCalculationForTargetProbabilityCollectionEntity CreateHydraulicLocationCalculationForTargetProbabilityCollectionEntity()
        {
            return new HydraulicLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = 0.05
            };
        }
    }
}