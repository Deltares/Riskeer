using System;
using System.Linq;
using Deltares.WTIPiping;
using Ringtoets.Piping.Calculation.Properties;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Piping
{
    /// <summary>
    /// Creates <see cref="PipingProfile"/> instances which are required by the <see cref="PipingCalculation"/>.
    /// </summary>
    internal static class PipingProfileCreator
    {
        /// <summary>
        /// Creates a simple <see cref="PipingProfile"/> with a single default constructed <see cref="PipingLayer"/>.
        /// </summary>
        /// <param name="soilProfile"></param>
        /// <returns></returns>
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
                    IsAquifer = layer.IsAquifer
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