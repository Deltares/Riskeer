using Deltares.WTIPiping;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Piping
{
    /// <summary>
    /// Creates <see cref="PipingProfile"/> instances which are required by the <see cref="PipingCalculation"/>.
    /// </summary>
    internal static class PipingProfileCreator
    {
        /// <summary>
        /// Creates a <see cref="PipingProfile"/> based on information contained in the provided <paramref name="soilProfile"/>,
        /// which can then be used in the <see cref="PipingCalculation"/>.
        /// </summary>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> from which to take the information.</param>
        /// <returns>A new <see cref="PipingProfile"/> with information taken from the <paramref name="soilProfile"/>.</returns>
        public static PipingProfile Create(PipingSoilProfile soilProfile)
        {
            var profile = new PipingProfile
            {
                BottomLevel = soilProfile.Bottom
            };
            foreach (PipingSoilLayer layer in soilProfile.Layers)
            {
                var pipingLayer = new PipingLayer
                {
                    TopLevel = layer.Top,
                    IsAquifer = layer.IsAquifer.Equals(1.0)
                };
                if (layer.AbovePhreaticLevel.HasValue)
                {
                    pipingLayer.AbovePhreaticLevel = layer.AbovePhreaticLevel.Value;
                }
                if(layer.BelowPhreaticLevel.HasValue)
                {
                    pipingLayer.BelowPhreaticLevel = layer.BelowPhreaticLevel.Value;
                }
                if(layer.DryUnitWeight.HasValue)
                {
                    pipingLayer.DryUnitWeight = layer.DryUnitWeight.Value;
                }
                profile.Layers.Add(pipingLayer);
            }

            return profile;
        }
    }
}