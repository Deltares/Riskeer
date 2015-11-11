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
        /// Creates the design variable for <see cref="PipingData.ThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetThicknessCoverageLayer(PipingData pipingData)
        {
            return CreateDesignVariable(pipingData.ThicknessCoverageLayer, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.PhreaticLevelExit"/>.
        /// </summary>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingData pipingData)
        {
            return CreateDesignVariable(pipingData.PhreaticLevelExit, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.DampingFactorExit"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDampingFactorExit(PipingData pipingData)
        {
            return CreateDesignVariable(pipingData.DampingFactorExit, 0.95);
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.SeepageLength"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetSeepageLength(PipingData pipingData)
        {
            return CreateDesignVariable(pipingData.SeepageLength, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.Diameter70"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDiameter70(PipingData pipingData)
        {
            return CreateDesignVariable(pipingData.Diameter70, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.DarcyPermeability"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDarcyPermeability(PipingData pipingData)
        {
            return CreateDesignVariable(pipingData.DarcyPermeability, 0.95);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.ThicknessAquiferLayer"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetThicknessAquiferLayer(PipingData pipingData)
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