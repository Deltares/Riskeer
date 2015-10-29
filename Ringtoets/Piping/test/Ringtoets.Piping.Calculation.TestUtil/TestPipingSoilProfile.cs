using System.Collections.ObjectModel;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.TestUtil
{
    public class TestPipingSoilProfile : PipingSoilProfile
    {
        public TestPipingSoilProfile() : base("", 0.0, new Collection<PipingSoilLayer>
        {
            new PipingSoilLayer(0.0)
            {
                IsAquifer = true
            }
        }) {}
    }
}