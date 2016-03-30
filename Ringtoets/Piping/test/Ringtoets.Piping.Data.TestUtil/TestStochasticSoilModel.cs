using System;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Data.TestUtil
{
    public class TestStochasticSoilModel : StochasticSoilModel
    {
        public TestStochasticSoilModel() : base(0, String.Empty, String.Empty)
        {
            StochasticSoilProfiles.AddRange(new []
            {
                new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0)
                {
                    SoilProfile = new TestPipingSoilProfile()
                },
                new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0)
                {
                    SoilProfile = new TestPipingSoilProfile()
                }
            });
        }
    }
}