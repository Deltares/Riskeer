using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// This class represents objects which were imported from a DSoilModel database. Instances of this class are transient and are not to be used
    /// once the DSoilModel database has been imported.
    /// </summary>
    internal class SoilLayer1D
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer1D"/>.
        /// </summary>
        public SoilLayer1D(double top)
        {
            Top = top;
        }

        /// <summary>
        /// Gets the top level of the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double Top { get; private set; }

        /// <summary>
        /// Gets or sets the double which represents whether the <see cref="SoilLayer1D"/> is an aquifer.
        /// </summary>
        public double? IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the <see cref="SoilLayer1D"/> above the phreatic level.
        /// [kN/m&#179;]
        /// </summary>
        public double? AbovePhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the <see cref="SoilLayer1D"/> below the phreatic level.
        /// [kN/m&#179;]
        /// </summary>
        public double? BelowPhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the dry unit weight for the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double? DryUnitWeight { get; set; }

        /// <summary>
        /// Constructs a (1D) <see cref="PipingSoilLayer"/> based on the properties set for the <see cref="SoilLayer1D"/>.
        /// </summary>
        /// <returns>The <see cref="PipingSoilLayer"/> with properties corresponding to those set on the <see cref="SoilLayer1D"/>.</returns>
        internal PipingSoilLayer AsPipingSoilLayer()
        {
            return new PipingSoilLayer(Top)
            {
                AbovePhreaticLevel = AbovePhreaticLevel,
                BelowPhreaticLevel = BelowPhreaticLevel,
                DryUnitWeight = DryUnitWeight,
                IsAquifer = IsAquifer.HasValue && IsAquifer.Value.Equals(1.0)
            };
        }
    }
}