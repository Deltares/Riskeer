using Deltares.WTIPiping;

namespace Wti.Calculation.Piping
{
    /// <summary>
    /// Creates <see cref="PipingProfile"/> instances which are required by the <see cref="PipingCalculation"/>.
    /// </summary>
    internal class PipingProfileCreator
    {
        public PipingProfile Create()
        {
            var profile = new PipingProfile();
            var layer = new PipingLayer();
            layer.IsAquifer = true;
            profile.Layers.Add(layer);

            return profile;
        }
    }
}