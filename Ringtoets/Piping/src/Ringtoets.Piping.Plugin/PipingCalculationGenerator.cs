using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin
{
    /// <summary>
    /// This class provides helpers to generate <see cref="IPipingCalculationItem"/> based on <see cref="StochasticSoilModel"/>
    /// and a <see cref="ReferenceLine"/> for given <see cref="RingtoetsPipingSurfaceLine"/>.
    /// </summary>
    public static class PipingCalculationGenerator
    {
        public static IEnumerable<IPipingCalculationItem> Generate(IEnumerable<RingtoetsPipingSurfaceLine> ringtoetsPipingSurfaceLines, IEnumerable<StochasticSoilModel> soilModels)
        {
            if (ringtoetsPipingSurfaceLines == null)
            {
                return Enumerable.Empty<IPipingCalculationItem>();
            }
            var pipingCalculationGroups = ringtoetsPipingSurfaceLines.Select(sl => CreateCalculationGroup(sl, soilModels));
            return pipingCalculationGroups;
        }

        private static IPipingCalculationItem CreateCalculationGroup(RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<StochasticSoilModel> soilModels)
        {
            var pipingCalculationGroup = new PipingCalculationGroup(surfaceLine.Name, true);
            if (soilModels != null)
            {
                foreach (var profile in PipingCalculationConfigurationHelper.GetPipingSoilProfilesForSurfaceLine(surfaceLine, soilModels))
                {
                    pipingCalculationGroup.Children.Add(CreatePipingCalculation(surfaceLine, profile));
                }
            }

            return pipingCalculationGroup;
        }

        private static IPipingCalculationItem CreatePipingCalculation(RingtoetsPipingSurfaceLine surfaceLine, PipingSoilProfile profile)
        {
            return new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    SoilProfile = profile
                }
            };
        }
    }
}