using System;
using System.Reflection;
using Components.Persistence.Stability.Data;
using Core.Common.Util.Reflection;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.IO.Properties;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableDataModel"/>.
    /// </summary>
    internal static class PersistableDataModelFactory
    {
        /// <summary>
        /// Creates a new <see cref="PersistableDataModel"/>.
        /// </summary>
        /// <param name="calculation">The calculation to get the data from.</param>
        /// <param name="filePath">The filePath that is used.</param>
        /// <returns>A created <see cref="PersistableDataModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculation"/>
        /// has no output.</exception>
        public static PersistableDataModel Create(MacroStabilityInwardsCalculation calculation, string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (!calculation.HasOutput)
            {
                throw new InvalidOperationException("Calculation must have output.");
            }

            return new PersistableDataModel
            {
                Info = new PersistableProjectInfo
                {
                    Path = filePath,
                    Project = calculation.Name,
                    CrossSection = calculation.InputParameters.SurfaceLine.Name,
                    ApplicationCreated = string.Format(Resources.PersistableDataModelFactory_Create_Riskeer_Version_0,
                                                       AssemblyUtils.GetAssemblyInfo(Assembly.GetAssembly(typeof(PersistableDataModelFactory))).Version),
                    Remarks = Resources.PersistableDataModelFactory_Create_Export_from_Riskeer,
                    Created = DateTime.Now,
                    IsDataValidated = true
                },
                CalculationSettings = new[]
                {
                    new PersistableCalculationSettings
                    {
                        Id = "0"
                    },
                    new PersistableCalculationSettings
                    {
                        AnalysisType = PersistableAnalysisType.UpliftVan,
                        UpliftVan = new PersistableUpliftVanSettings
                        {
                            SlipPlane = new PersistableTwoCirclesOnTangentLine
                            {
                                FirstCircleCenter = new PersistablePoint(calculation.Output.SlidingCurve.LeftCircle.Center.X,
                                                                         calculation.Output.SlidingCurve.LeftCircle.Center.Y),
                                FirstCircleRadius = calculation.Output.SlidingCurve.LeftCircle.Radius,
                                SecondCircleCenter = new PersistablePoint(calculation.Output.SlidingCurve.RightCircle.Center.X,
                                                                          calculation.Output.SlidingCurve.RightCircle.Center.Y)
                            }
                        },
                        CalculationType = PersistableCalculationType.Deterministic,
                        Id = "1"
                    }
                },
                Stages = new[]
                {
                    new PersistableStage
                    {
                        Id = "0",
                        CalculationSettingsId = "0"
                    },
                    new PersistableStage
                    {
                        Id = "1",
                        CalculationSettingsId = "1"
                    }
                }
            };
        }
    }
}