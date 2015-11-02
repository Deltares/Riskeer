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
        private const bool dSoilModelDatabaseContainsAquiferOnLayerLevel = false;

        /// <summary>
        /// Creates a simple <see cref="PipingProfile"/> with a single default constructed <see cref="PipingLayer"/>.
        /// </summary>
        /// <param name="soilProfile"></param>
        /// <returns></returns>
        public static PipingProfile Create(PipingSoilProfile soilProfile)
        {
            ValidateForAquiferLayer(soilProfile);
            var profile = new PipingProfile
            {
                BottomLevel = soilProfile.Bottom
            };
            foreach (PipingSoilLayer layer in soilProfile.Layers)
            {
                profile.Layers.Add(new PipingLayer
                {
                    TopLevel = layer.Top
                });
            }
            var max = profile.Layers.Max(l => l.TopLevel);
            var topLayer = profile.Layers.FirstOrDefault(l => Math.Abs(l.TopLevel - max) < 1e-6);
            if (topLayer != null)
            {
                topLayer.IsAquifer = true;
            }

            return profile;
        }

        private static void ValidateForAquiferLayer(PipingSoilProfile soilProfile)
        {
            if (!soilProfile.Layers.Any(l => l.IsAquifer))
            {
                if (dSoilModelDatabaseContainsAquiferOnLayerLevel)
                {
                    var message = String.Format(Resources.PipingProfileCreator_No_Aquifer_Layer, soilProfile.Name);
                    throw new PipingProfileCreatorException(message);
                }
                soilProfile.Layers.First().IsAquifer = true;
            }
        }
    }
}