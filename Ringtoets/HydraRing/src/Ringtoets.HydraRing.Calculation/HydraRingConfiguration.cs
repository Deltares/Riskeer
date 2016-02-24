// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Settings;
using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Container for all configurations that are necessary for performing a Hydra-Ring calculation.
    /// The following Hydra-Ring features are not exposed (yet):
    /// - Combination of multiple dike sections
    /// - Coupling two hydraulic boundary stations
    /// - Performing revetment calculations (DesignTables > LayerId)
    /// - Performing piping calculations (DesignTables > AlternativeId)
    /// - Type 3 computations (DesignTables > Method)
    /// </summary>
    public class HydraRingConfiguration
    {
        private readonly IList<HydraRingCalculationData> hydraRingCalculations;
        private IEnumerable<FixedFailureMechanismSettings> fixedFailureMechanismSettings;
        private readonly IEnumerable<FailureMechanismSettings> defaultFailureMechanismSettings;
        private readonly IEnumerable<SubMechanismSettings> defaultSubMechanismSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingConfiguration"/> class.
        /// </summary>
        public HydraRingConfiguration()
        {
            hydraRingCalculations = new List<HydraRingCalculationData>();

            InitializeFixedFailureMechanismSettings();

            defaultFailureMechanismSettings = new[]
            {
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel,
                    ValueMin = 0,
                    ValueMax = 50
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.WaveHeight,
                    ValueMin = 0,
                    ValueMax = 50
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.WavePeakPeriod,
                    ValueMin = 0,
                    ValueMax = 50
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.WaveSpectralPeriod,
                    ValueMin = 0,
                    ValueMax = 50
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.QVariant,
                    ValueMin = 0,
                    ValueMax = 50
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesOvertopping,
                    ValueMin = double.NaN,
                    ValueMax = double.NaN
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesPiping,
                    ValueMin = double.NaN,
                    ValueMax = double.NaN
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresOvertopping,
                    ValueMin = double.NaN,
                    ValueMax = double.NaN
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    ValueMin = double.NaN,
                    ValueMax = double.NaN
                },
                new FailureMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    ValueMin = double.NaN,
                    ValueMax = double.NaN
                }
            };

            defaultSubMechanismSettings = new[]
            {
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel,
                    SubMechanismId = 1,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.WaveHeight,
                    SubMechanismId = 11,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.WavePeakPeriod,
                    SubMechanismId = 14,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.WaveSpectralPeriod,
                    SubMechanismId = 16,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.QVariant,
                    SubMechanismId = 3,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.QVariant,
                    SubMechanismId = 4,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.QVariant,
                    SubMechanismId = 5,
                    CalculationTechniqueId = 4,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesOvertopping,
                    SubMechanismId = 102,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesOvertopping,
                    SubMechanismId = 103,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesPiping,
                    SubMechanismId = 311,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesPiping,
                    SubMechanismId = 313,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.DikesPiping,
                    SubMechanismId = 314,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresOvertopping,
                    SubMechanismId = 421,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresOvertopping,
                    SubMechanismId = 422,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresOvertopping,
                    SubMechanismId = 423,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    SubMechanismId = 422,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    SubMechanismId = 424,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    SubMechanismId = 425,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    SubMechanismId = 426,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    SubMechanismId = 427,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 422,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 424,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 425,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 430,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 431,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 432,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 433,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 1,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 434,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                },
                new SubMechanismSettings
                {
                    FailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    SubMechanismId = 435,
                    CalculationTechniqueId = 1,
                    FormStartMethod = 4,
                    FormNumberOfIterations = 50,
                    FormRelaxationFactor = 0.15,
                    FormEpsBeta = 0.01,
                    FormEpsHOH = 0.01,
                    FormEpsZFunc = 0.01,
                    DsStartMethod = 2,
                    DsMinNumberOfIterations = 10000,
                    DsMaxNumberOfIterations = 20000,
                    DsVarCoefficient = 0.1,
                    NiUMin = -6.0,
                    NiUMax = 6.0,
                    NiNumberSteps = 25
                }
            };
        }

        /// <summary>
        /// Initializes some fixed settings on a per <see cref="HydraRingFailureMechanismType"/> basis.
        /// </summary>
        /// <remarks>
        /// These fixed settings cannot be overruled and just reflect:
        /// - some supported ids within Hydra-Ring;
        /// - a WTI 2017 specific configuration of Hydra-Ring.
        /// </remarks>
        private void InitializeFixedFailureMechanismSettings()
        {
            fixedFailureMechanismSettings = new[]
            {
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.AssessmentLevel, 1, 2, 26, new[]
                {
                    1
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.WaveHeight, 11, 2, 28, new[]
                {
                    11
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.WavePeakPeriod, 11, 2, 29, new[]
                {
                    14
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.WaveSpectralPeriod, 11, 2, 29, new[]
                {
                    16
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.QVariant, 3, 6, 114, new[]
                {
                    3,
                    4,
                    5
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.DikesOvertopping, 101, 1, 1, new[]
                {
                    102,
                    103
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.DikesPiping, 103, 1, 44, new[]
                {
                    311,
                    313,
                    314
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.StructuresOvertopping, 110, 1, 60, new[]
                {
                    421,
                    422,
                    423
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.StructuresClosure, 111, 1, 65, new[]
                {
                    422,
                    424,
                    425,
                    426,
                    427
                }),
                new FixedFailureMechanismSettings(HydraRingFailureMechanismType.StructuresStructuralFailure, 112, 1, 65, new[]
                {
                    422,
                    424,
                    425,
                    430,
                    431,
                    432,
                    433,
                    434,
                    435
                })
            };
        }

        /// <summary>
        /// Gets or sets the <see cref="HydraRingTimeIntegrationSchemeType"/>.
        /// </summary>
        public HydraRingTimeIntegrationSchemeType? TimeIntegrationSchemeType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HydraRingUncertaintiesType"/>.
        /// </summary>
        public HydraRingUncertaintiesType? UncertaintiesType { get; set; }

        /// <summary>
        /// Adds a Hydra-Ring calculation to the <see cref="HydraRingConfiguration"/>.
        /// </summary>
        /// <param name="hydraRingCalculationData">The container that holds all data for configuring the calculation.</param>
        public void AddHydraRingCalculation(HydraRingCalculationData hydraRingCalculationData)
        {
            hydraRingCalculations.Add(hydraRingCalculationData);
        }

        /// <summary>
        /// Generates a database creation script that can be used to perform a Hydra-Ring calculation.
        /// </summary>
        /// <returns>The database creation script.</returns>
        /// <exception cref="InvalidOperationException">Thrown when one of the relevant input properties is not set.</exception>
        public string GenerateDataBaseCreationScript()
        {
            var configurationDictionary = new Dictionary<string, List<OrderedDictionary>>();

            InitializeHydraulicModelsConfiguration(configurationDictionary);
            InitializeSectionsConfiguration(configurationDictionary);
            InitializeDesignTablesConfiguration(configurationDictionary);
            InitializeNumericsConfiguration(configurationDictionary);
            InitializeAreasConfiguration(configurationDictionary);
            InitializeProjectsConfiguration(configurationDictionary);

            return GenerateDataBaseCreationScript(configurationDictionary);
        }

        private void InitializeHydraulicModelsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["HydraulicModels"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "TimeIntegrationSchemeID", (int?) TimeIntegrationSchemeType
                    },
                    {
                        "UncertaintiesID", (int?) UncertaintiesType
                    },
                    {
                        "DataSetName", "WTI 2017" // Fixed: use the WTI 2017 set of station locations
                    }
                }
            };
        }

        private void InitializeSectionsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculation in hydraRingCalculations)
            {
                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", 999 // TODO: Dike section integration
                    },
                    {
                        "PresentationId", 1 // Fixed: no support for combination of multiple dike sections
                    },
                    {
                        "MainMechanismId", 1 // Fixed: no support for combination of multiple dike sections
                    },
                    {
                        "Name", "HydraRingLocation" // TODO: Dike section integration
                    },
                    {
                        "Description", "HydraRingLocation" // TODO: Dike section integration
                    },
                    {
                        "RingCoordinateBegin", null // TODO: Dike section integration
                    },
                    {
                        "RingCoordinateEnd", null // TODO: Dike section integration
                    },
                    {
                        "XCoordinate", null // TODO: Dike cross section integration
                    },
                    {
                        "YCoordinate", null // TODO: Dike cross section integration
                    },
                    {
                        "StationId1", hydraRingCalculation.HydraulicBoundaryLocationId
                    },
                    {
                        "StationId2", hydraRingCalculation.HydraulicBoundaryLocationId // Same as "StationId1": no support for coupling two stations
                    },
                    {
                        "Relative", 100.0 // Fixed: no support for coupling two stations
                    },
                    {
                        "Normal", null // TODO: Dike cross section integration
                    },
                    {
                        "Length", null // TODO: Dike section integration
                    }
                });
            }

            configurationDictionary["Sections"] = orderedDictionaries;
        }

        private void InitializeDesignTablesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculation in hydraRingCalculations)
            {
                var fixedFailureMechanismSettingsForCalculation = fixedFailureMechanismSettings.First(ffms => ffms.FailureMechanismType == hydraRingCalculation.FailureMechanismType);
                var defaultFailureMechanismSettingsForCalculation = defaultFailureMechanismSettings.First(fms => fms.FailureMechanismType == hydraRingCalculation.FailureMechanismType);

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", 999 // TODO: Dike section integration
                    },
                    {
                        "MechanismId", fixedFailureMechanismSettingsForCalculation.MechanismId
                    },
                    {
                        "LayerId", null // Fixed: no support for revetments
                    },
                    {
                        "AlternativeId", null // Fixed: no support for piping
                    },
                    {
                        "Method", fixedFailureMechanismSettingsForCalculation.CalculationTypeId
                    },
                    {
                        "VariableId", fixedFailureMechanismSettingsForCalculation.VariableId
                    },
                    {
                        "LoadVariableId", null // Fixed: not relevant
                    },
                    {
                        "TableMin", null // Fixed: no support for type 3 computations (see "Method")
                    },
                    {
                        "TableMax", null // Fixed: no support for type 3 computations (see "Method")
                    },
                    {
                        "TableStepSize", null // Fixed: no support for type 3 computations (see "Method")
                    },
                    {
                        "ValueMin", defaultFailureMechanismSettingsForCalculation.ValueMin
                    },
                    {
                        "ValueMax", defaultFailureMechanismSettingsForCalculation.ValueMax
                    },
                    {
                        "Beta", !double.IsNaN(hydraRingCalculation.Beta) ? (double?) hydraRingCalculation.Beta : null
                    }
                });
            }

            configurationDictionary["DesignTables"] = orderedDictionaries;
        }

        private void InitializeNumericsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculation in hydraRingCalculations)
            {
                var fixedFailureMechanismSettingsForCalculation = fixedFailureMechanismSettings.First(ffms => ffms.FailureMechanismType == hydraRingCalculation.FailureMechanismType);

                foreach (var subMechanimsId in fixedFailureMechanismSettingsForCalculation.SubMechanismIds)
                {
                    var configurationForSubMechanism = defaultSubMechanismSettings.First(cs => cs.FailureMechanismType == hydraRingCalculation.FailureMechanismType && cs.SubMechanismId == subMechanimsId);

                    orderDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", 999 // TODO: Dike section integration
                        },
                        {
                            "MechanismId", fixedFailureMechanismSettingsForCalculation.MechanismId
                        },
                        {
                            "LayerId", null // Fixed: no support for revetments
                        },
                        {
                            "AlternativeId", null // Fixed: no support for piping
                        },
                        {
                            "SubMechanismId", configurationForSubMechanism.SubMechanismId
                        },
                        {
                            "Method", configurationForSubMechanism.CalculationTechniqueId
                        },
                        {
                            "FormStartMethod", configurationForSubMechanism.FormStartMethod
                        },
                        {
                            "FormNumberOfIterations", configurationForSubMechanism.FormNumberOfIterations
                        },
                        {
                            "FormRelaxationFactor", configurationForSubMechanism.FormRelaxationFactor
                        },
                        {
                            "FormEpsBeta", configurationForSubMechanism.FormEpsBeta
                        },
                        {
                            "FormEpsHOH", configurationForSubMechanism.FormEpsHOH
                        },
                        {
                            "FormEpsZFunc", configurationForSubMechanism.FormEpsZFunc
                        },
                        {
                            "DsStartMethod", configurationForSubMechanism.DsStartMethod
                        },
                        {
                            "DsIterationmethod", 1 // Fixed: not relevant
                        },
                        {
                            "DsMinNumberOfIterations", configurationForSubMechanism.DsMinNumberOfIterations
                        },
                        {
                            "DsMaxNumberOfIterations", configurationForSubMechanism.DsMaxNumberOfIterations
                        },
                        {
                            "DsVarCoefficient", configurationForSubMechanism.DsVarCoefficient
                        },
                        {
                            "NiUMin", configurationForSubMechanism.NiUMin
                        },
                        {
                            "NiUMax", configurationForSubMechanism.NiUMax
                        },
                        {
                            "NiNumberSteps", configurationForSubMechanism.NiNumberSteps
                        }
                    });
                }
            }

            configurationDictionary["Numerics"] = orderDictionaries;
        }

        private void InitializeAreasConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Areas"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "aDefault", 1 // Fixed: not relevant
                    },
                    {
                        "bDefault", "1" // Fixed: not relevant
                    },
                    {
                        "cDefault", "Nederland" // Fixed: not relevant
                    }
                }
            };
        }

        private void InitializeProjectsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Projects"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "aDefault", 1 // Fixed: not relevant
                    },
                    {
                        "bDefault", "Sprint" // Fixed: not relevant
                    },
                    {
                        "cDefault", "Hydra-Ring Sprint" // Fixed: not relevant
                    }
                }
            };
        }

        private static string GenerateDataBaseCreationScript(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var lines = new List<string>();

            foreach (var tableName in configurationDictionary.Keys)
            {
                lines.Add("DELETE FROM [" + tableName + "];");

                if (configurationDictionary[tableName].Count <= 0)
                {
                    continue;
                }

                foreach (var orderedDictionary in configurationDictionary[tableName])
                {
                    var valueStrings = new List<string>();

                    foreach (var val in orderedDictionary.Values)
                    {
                        if (val == null)
                        {
                            valueStrings.Add("NULL");
                            continue;
                        }

                        if (val is string)
                        {
                            valueStrings.Add("'" + val + "'");
                            continue;
                        }

                        if (val is double)
                        {
                            valueStrings.Add(((double) val).ToString(CultureInfo.InvariantCulture));
                            continue;
                        }

                        valueStrings.Add(val.ToString());
                    }

                    var valuesString = string.Join(", ", valueStrings);

                    lines.Add("INSERT INTO [" + tableName + "] VALUES (" + valuesString + ");");
                }

                lines.Add("");
            }

            return string.Join(Environment.NewLine, lines);
        }

        # region Nested types

        /// <summary>
        /// Container of default Hydra-Ring settings for a specific <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        private class FixedFailureMechanismSettings
        {
            private readonly int variableId;
            private readonly int mechanismId;
            private readonly int calculationTypeId;
            private readonly IEnumerable<int> subMechanismIds;
            private readonly HydraRingFailureMechanismType failureMechanismType;

            /// <summary>
            /// Creates a new instance of the <see cref="FixedFailureMechanismSettings"/> class.
            /// </summary>
            /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> the specify the fixed settings for.</param>
            /// <param name="mechanismId">The corresponding mechanism id.</param>
            /// <param name="calculationTypeId">The corresponding calculation type id.</param>
            /// <param name="variableId">The corresponding variable id.</param>
            /// <param name="subMechanismIds">The corresponding sub mechanism ids.</param>
            public FixedFailureMechanismSettings(HydraRingFailureMechanismType failureMechanismType, int mechanismId, int calculationTypeId, int variableId, IEnumerable<int> subMechanismIds)
            {
                this.failureMechanismType = failureMechanismType;
                this.mechanismId = mechanismId;
                this.variableId = variableId;
                this.calculationTypeId = calculationTypeId;
                this.subMechanismIds = subMechanismIds;
            }

            public HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return failureMechanismType;
                }
            }

            /// <summary>
            /// Gets the mechanism id that corresponds to a specific <see cref="HydraRingFailureMechanismType"/>.
            /// </summary>
            public int MechanismId
            {
                get
                {
                    return mechanismId;
                }
            }

            /// <summary>
            /// Gets the calculation type id that is applicable for a specific <see cref="HydraRingFailureMechanismType"/>.
            /// </summary>
            public int CalculationTypeId
            {
                get
                {
                    return calculationTypeId;
                }
            }

            /// <summary>
            /// Gets the id of the variable that is relevant for a specific <see cref="HydraRingFailureMechanismType"/>.
            /// </summary>
            public int VariableId
            {
                get
                {
                    return variableId;
                }
            }

            /// <summary>
            /// Gets the sub mechanism ids that are applicable for a specific <see cref="HydraRingFailureMechanismType"/>.
            /// </summary>
            public IEnumerable<int> SubMechanismIds
            {
                get
                {
                    return subMechanismIds;
                }
            }
        }

        # endregion
    }
}