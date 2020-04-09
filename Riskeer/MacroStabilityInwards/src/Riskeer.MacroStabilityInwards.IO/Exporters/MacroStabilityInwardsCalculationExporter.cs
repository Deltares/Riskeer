// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Reflection;
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Reflection;
using log4net;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.IO.Properties;
using Shared.Components.Persistence;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports a macro stability inwards calculation and stores it in a separate stix file.
    /// </summary>
    public class MacroStabilityInwardsCalculationExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsCalculationExporter));

        private readonly MacroStabilityInwardsCalculation calculation;
        private readonly IPersistenceFactory persistenceFactory;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationExporter"/>.
        /// </summary>
        /// <param name="calculation">The calculation to export.</param>
        /// <param name="persistenceFactory">The persistence factory to use.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> or
        /// <see cref="persistenceFactory"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public MacroStabilityInwardsCalculationExporter(MacroStabilityInwardsCalculation calculation,
                                                        IPersistenceFactory persistenceFactory,
                                                        string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (persistenceFactory == null)
            {
                throw new ArgumentNullException(nameof(persistenceFactory));
            }

            IOUtils.ValidateFilePath(filePath);

            this.calculation = calculation;
            this.persistenceFactory = persistenceFactory;
            this.filePath = filePath;
        }

        public bool Export()
        {
            PersistableDataModel persistableDataModel = CreatePersistableDataModel();

            try
            {
                using (IPersister persister = persistenceFactory.CreateArchivePersister(filePath, persistableDataModel))
                {
                    persister.Persist();
                }
            }
            catch (Exception)
            {
                log.ErrorFormat("{0} {1}", string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), Resources.MacroStabilityInwardsCalculationExporter_Export_no_stability_project_exported);
                return false;
            }

            return true;
        }

        private PersistableDataModel CreatePersistableDataModel()
        {
            return new PersistableDataModel
            {
                Info = new PersistableProjectInfo
                {
                    Path = filePath,
                    Project = calculation.Name,
                    CrossSection = calculation.InputParameters.SurfaceLine.Name,
                    ApplicationCreated = $"Riskeer {AssemblyUtils.GetAssemblyInfo(Assembly.GetAssembly(GetType())).Version}",
                    Remarks = "Export from Riskeer",
                    Created = DateTime.Now,
                    IsDataValidated = true
                },
                CalculationSettings = new []
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
                    },
                },
                Stages = new []
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