using Deltares.WTIPiping;

namespace Wti.Calculation.Piping
{
    /// <summary>
    /// Creates <see cref="PipingProfile"/> instances which are required by the <see cref="PipingCalculation"/>.
    /// </summary>
    internal static class PipingProfileCreator
    {
        /// <summary>
        /// Creates a simple <see cref="PipingProfile"/> with a single default constructed <see cref="PipingLayer"/>.
        /// </summary>
        /// <returns></returns>
        public static PipingProfile Create()
        {
            var profile = new PipingProfile();
            var layer = new PipingLayer
            {
                IsAquifer = true
            };
            profile.Layers.Add(layer);

            return profile;
        }
    }
}