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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Components.Persistence.Stability;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Geometry;
using Core.Common.Util;
using log4net;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.Properties;
using Riskeer.MacroStabilityInwards.Primitives;
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
        private readonly Func<RoundedDouble> getNormativeAssessmentLevelFunc;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationExporter"/>.
        /// </summary>
        /// <param name="calculation">The calculation to export.</param>
        /// <param name="persistenceFactory">The persistence factory to use.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{TResult}"/>
        /// for obtaining the normative assessment level.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>,
        /// <paramref name="persistenceFactory"/> or <paramref name="getNormativeAssessmentLevelFunc"/> is <c>null</c>.</exception>
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
                                                        string filePath, Func<RoundedDouble> getNormativeAssessmentLevelFunc)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (persistenceFactory == null)
            {
                throw new ArgumentNullException(nameof(persistenceFactory));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            IOUtils.ValidateFilePath(filePath);

            this.calculation = calculation;
            this.persistenceFactory = persistenceFactory;
            this.filePath = filePath;
            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;
        }

        /// <inheritdoc />
        /// <remarks>
        /// To avoid problems with writing to an existing file, the export is done to a temp file first.
        /// When the export was successful the file is moved to the given <see cref="filePath"/>.
        /// </remarks>
        public bool Export()
        {
            ValidateData();

            PersistableDataModel persistableDataModel = PersistableDataModelFactory.Create(calculation, getNormativeAssessmentLevelFunc, filePath);

            string tempFilePath = $"{filePath}.temp";

            try
            {
                using (IPersister persister = persistenceFactory.CreateArchivePersister(tempFilePath, persistableDataModel))
                {
                    persister.Persist();
                }

                MoveTempFileToFinal(tempFilePath);
            }
            catch (Exception)
            {
                File.Delete(tempFilePath);

                log.ErrorFormat("{0} {1}", string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), Resources.MacroStabilityInwardsCalculationExporter_Export_no_stability_project_exported);
                return false;
            }

            return true;
        }

        private void ValidateData()
        {
            if (Math.Abs(calculation.InputParameters.MaximumSliceWidth - 1) > 1e-3)
            {
                log.WarnFormat(Resources.MacroStabilityInwardsCalculationExporter_ValidateData_DGeoSuite_only_supports_MaximumSliceWidth_one_but_calculation_has_MaximumSliceWidth_0,
                               calculation.InputParameters.MaximumSliceWidth.ToString(null, CultureInfo.CurrentCulture));
            }

            MacroStabilityInwardsSoilLayer2D[] layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(calculation.InputParameters.SoilProfileUnderSurfaceLine.Layers).ToArray();
            if (layers.Count(l => l.Data.IsAquifer) > 1)
            {
                log.Warn(Resources.MacroStabilityInwardsCalculationExporter_ValidateData_Multiple_aquifer_layers_not_supported_no_aquifer_layer_exported);
            }

            IDictionary<MacroStabilityInwardsSoilLayer2D, List<IMacroStabilityInwardsPreconsolidationStress>> layersWithStresses =
                GetLayersWithPreconsolidationStresses(layers, calculation.InputParameters.SoilProfileUnderSurfaceLine.PreconsolidationStresses);

            if (layersWithStresses.Any(lws => lws.Value.Count > 1 || lws.Value.Count == 1 && lws.Key.Data.UsePop && HasValidPop(lws.Key.Data.Pop)))
            {
                log.Warn(Resources.MacroStabilityInwardsCalculationExporter_ValidateData_Multiple_stress_points_not_supported_no_stress_points_exported);
            }
        }

        private static bool HasValidPop(IVariationCoefficientDistribution pop)
        {
            return pop.Mean != RoundedDouble.NaN
                   && pop.CoefficientOfVariation != RoundedDouble.NaN;
        }

        private static IDictionary<MacroStabilityInwardsSoilLayer2D, List<IMacroStabilityInwardsPreconsolidationStress>> GetLayersWithPreconsolidationStresses(
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IEnumerable<IMacroStabilityInwardsPreconsolidationStress> preconsolidationStresses)
        {
            var dictionary = new Dictionary<MacroStabilityInwardsSoilLayer2D, List<IMacroStabilityInwardsPreconsolidationStress>>();

            foreach (MacroStabilityInwardsSoilLayer2D layer in layers)
            {
                foreach (IMacroStabilityInwardsPreconsolidationStress preconsolidationStress in preconsolidationStresses)
                {
                    if (AdvancedMath2D.PointInPolygon(preconsolidationStress.Location, layer.OuterRing.Points, layer.NestedLayers.Select(l => l.OuterRing.Points)))
                    {
                        AddToDictionary(dictionary, layer, preconsolidationStress);
                    }
                }
            }

            return dictionary;
        }

        private static void AddToDictionary(IDictionary<MacroStabilityInwardsSoilLayer2D, List<IMacroStabilityInwardsPreconsolidationStress>> dictionary,
                                            MacroStabilityInwardsSoilLayer2D layer, IMacroStabilityInwardsPreconsolidationStress preconsolidationStress)
        {
            if (!dictionary.ContainsKey(layer))
            {
                dictionary.Add(layer, new List<IMacroStabilityInwardsPreconsolidationStress>());
            }

            dictionary[layer].Add(preconsolidationStress);
        }

        private void MoveTempFileToFinal(string tempFilePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.Move(tempFilePath, filePath);
        }
    }
}