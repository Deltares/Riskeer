using System.Collections.ObjectModel;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.TestUtil
{
    public class TestPipingSoilProfile : PipingSoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestPipingSoilProfile"/>, which is a <see cref="PipingSoilProfile"/>
        /// which has:
        /// <list type="bullet">
        /// <item><see cref="PipingSoilProfile.Name"/> set to <see cref="string.Empty"/></item>
        /// <item><see cref="PipingSoilProfile.Bottom"/> set to <c>0.0</c></item>
        /// <item><see cref="PipingSoilProfile.Layers"/> set to a collection with a single <see cref="PipingSoilLayer"/>
        /// with <see cref="PipingSoilLayer.Top"/> set to <c>0.0</c>.</item>
        /// </list>
        /// </summary>
        public TestPipingSoilProfile() : base("", 0.0, new Collection<PipingSoilLayer>
        {
            new PipingSoilLayer(0.0)
            {
                IsAquifer = true
            }
        }, 0) {}
    }
}