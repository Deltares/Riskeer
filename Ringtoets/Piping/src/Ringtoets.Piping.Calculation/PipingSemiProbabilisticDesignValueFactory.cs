using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Calculation
{
    /// <summary>
    /// Factory for creating design variables based on probabilistic distributions.
    /// </summary>
    public static class PipingSemiProbabilisticDesignValueFactory
    {
        #region General parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.ThicknessCoverageLayer"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetThicknessCoverageLayer(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.ThicknessCoverageLayer, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.PhreaticLevelExit"/>.
        /// </summary>
        public static DesignVariable<NormalDistribution> GetPhreaticLevelExit(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.PhreaticLevelExit, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.DampingFactorExit"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDampingFactorExit(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.DampingFactorExit, 0.95);
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.SeepageLength"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetSeepageLength(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.SeepageLength, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.Diameter70"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDiameter70(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.Diameter70, 0.05);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.DarcyPermeability"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetDarcyPermeability(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.DarcyPermeability, 0.95);
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingInput.ThicknessAquiferLayer"/>.
        /// </summary>
        public static DesignVariable<LognormalDistribution> GetThicknessAquiferLayer(PipingInput parameters)
        {
            return CreateDesignVariable(parameters.ThicknessAquiferLayer, 0.95);
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