using System;
using System.Collections.Generic;
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableCalculationSettings"/>.
    /// </summary>
    internal static class PersistableCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="PersistableCalculationSettings"/>.
        /// </summary>
        /// <param name="slidingCurve">The sliding curve to use.</param>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableCalculationSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<PersistableCalculationSettings> Create(MacroStabilityInwardsSlidingCurve slidingCurve, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (slidingCurve == null)
            {
                throw new ArgumentNullException(nameof(slidingCurve));
            }

            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            PersistableCalculationSettings emptySettings = Create(idFactory, registry);
            PersistableCalculationSettings filledSettings = Create(idFactory, registry);
            filledSettings.AnalysisType = PersistableAnalysisType.UpliftVan;
            filledSettings.UpliftVan = new PersistableUpliftVanSettings
            {
                SlipPlane = new PersistableTwoCirclesOnTangentLine
                {
                    FirstCircleCenter = new PersistablePoint(slidingCurve.LeftCircle.Center.X,
                                                             slidingCurve.LeftCircle.Center.Y),
                    FirstCircleRadius = slidingCurve.LeftCircle.Radius,
                    SecondCircleCenter = new PersistablePoint(slidingCurve.RightCircle.Center.X,
                                                              slidingCurve.RightCircle.Center.Y)
                }
            };
            filledSettings.CalculationType = PersistableCalculationType.Deterministic;

            return new[]
            {
                emptySettings,
                filledSettings
            };
        }

        private static PersistableCalculationSettings Create(IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            var emptySettings = new PersistableCalculationSettings
            {
                Id = idFactory.Create()
            };

            registry.Add(emptySettings, emptySettings.Id);
            return emptySettings;
        }
    }
}