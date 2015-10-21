using System.Collections.ObjectModel;
using Wti.Data;

namespace Wti.Calculation.Test.Piping.Stub
{
    public class TestPipingSoilProfile : PipingSoilProfile
    {
        public TestPipingSoilProfile() : base("", 0.0, new Collection<PipingSoilLayer> {new PipingSoilLayer(0.0)}) { }
    }
}