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
        public static DesignVariable GetThicknessCoverageLayer(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.ThicknessCoverageLayer,
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.PhreaticLevelExit"/>.
        /// </summary>
        public static DesignVariable GetPhreaticLevelExit(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.PhreaticLevelExit,
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.DampingFactorExit"/>.
        /// </summary>
        public static DesignVariable GetDampingFactorExit(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.DampingFactorExit,
                Percentile = 0.95
            };
        }

        #endregion

        #region Piping parameters

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.SeepageLength"/>.
        /// </summary>
        public static DesignVariable GetSeepageLength(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.SeepageLength,
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.Diameter70"/>.
        /// </summary>
        public static DesignVariable GetDiameter70(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.Diameter70,
                Percentile = 0.05
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.DarcyPermeability"/>.
        /// </summary>
        public static DesignVariable GetDarcyPermeability(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.DarcyPermeability,
                Percentile = 0.95
            };
        }

        /// <summary>
        /// Creates the design variable for <see cref="PipingData.ThicknessAquiferLayer"/>.
        /// </summary>
        public static DesignVariable GetThicknessAquiferLayer(PipingData pipingData)
        {
            return new DesignVariable
            {
                Distribution = pipingData.ThicknessAquiferLayer,
                Percentile = 0.95
            };
        }

        #endregion
    }
}