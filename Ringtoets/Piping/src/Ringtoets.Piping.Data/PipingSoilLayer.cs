namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on be used to create the
    /// necessary input for executing a piping calculation.
    /// </summary>
    public class PipingSoilLayer
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilLayer"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top"></param>
        public PipingSoilLayer(double top)
        {
            Top = top;
        }

        /// <summary>
        /// Gets the top level of the <see cref="PipingSoilLayer"/>.
        /// </summary>
        public double Top { get; private set; }

        /// <summary>
        /// Gets or sets the boolean value which represents whether the <see cref="PipingSoilLayer"/> is an aquifer.
        /// </summary>
        public bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the <see cref="PipingSoilLayer"/> above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? AbovePhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the dry unit weight for the <see cref="PipingSoilLayer"/>.
        /// </summary>
        public double? DryUnitWeight { get; set; }
    }
}