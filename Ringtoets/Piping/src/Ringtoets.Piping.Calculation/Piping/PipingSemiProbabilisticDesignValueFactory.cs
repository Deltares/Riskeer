using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Calculation.Piping
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions.
    /// </summary>
    public static class PipingSemiProbabilisticDesignValueFactory
    {
        #region General parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.ThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetThicknessCoverageLayer(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.ThicknessCoverageLayer, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.PhreaticLevelExit"/>.
        /// </summary>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.PhreaticLevelExit, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.DampingFactorExit"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDampingFactorExit(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.DampingFactorExit, 0.95);
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.SeepageLength"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetSeepageLength(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.SeepageLength, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.Diameter70"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDiameter70(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.Diameter70, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.DarcyPermeability"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDarcyPermeability(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.DarcyPermeability, 0.95);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingCalculationData.ThicknessAquiferLayer"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetThicknessAquiferLayer(PipingCalculationData pipingData)
        {
            return CreateDesignVariable(pipingData.ThicknessAquiferLayer, 0.95);
        }

        #endregion

        private static DesignVariable<NormalDistribution> CreateDesignVariable(NormalDistribution distribution, double percentile)
        {
            return new NormalDistributionDesignVariable(distribution)
            {
                Percentile = percentile
            };
        }

        private static DesignVariable<LognormalDistribution> CreateDesignVariable(LognormalDistribution distribution, double percentile)
        {
            return new LognormalDistributionDesignVariable(distribution)
            {
                Percentile = percentile
            };
        }
    }
}