﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        private readonly SubMechanismSettingsProvider subMechanismSettingsProvider = new SubMechanismSettingsProvider();
        private readonly FailureMechanismSettingsProvider failureMechanismSettingsProvider = new FailureMechanismSettingsProvider();
        private readonly FailureMechanismDefaultsProvider failureMechanismDefaultsProvider = new FailureMechanismDefaultsProvider();

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingConfiguration"/> class.
        /// </summary>
        public HydraRingConfiguration()
        {
            hydraRingCalculations = new List<HydraRingCalculationData>();
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
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculation.FailureMechanismType);
                var failureMechanismSettings = failureMechanismSettingsProvider.GetFailureMechanismSettings(hydraRingCalculation.FailureMechanismType);

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", 999 // TODO: Dike section integration
                    },
                    {
                        "MechanismId", failureMechanismDefaults.MechanismId
                    },
                    {
                        "LayerId", null // Fixed: no support for revetments
                    },
                    {
                        "AlternativeId", null // Fixed: no support for piping
                    },
                    {
                        "Method", failureMechanismDefaults.CalculationTypeId
                    },
                    {
                        "VariableId", failureMechanismDefaults.VariableId
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
                        "ValueMin", failureMechanismSettings.ValueMin
                    },
                    {
                        "ValueMax", failureMechanismSettings.ValueMax
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
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculation.FailureMechanismType);

                foreach (var subMechanimsId in failureMechanismDefaults.SubMechanismIds)
                {
                    var subMechanismSettings = subMechanismSettingsProvider.GetSubMechanismSettings(hydraRingCalculation.FailureMechanismType, subMechanimsId);

                    orderDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", 999 // TODO: Dike section integration
                        },
                        {
                            "MechanismId", failureMechanismDefaults.MechanismId
                        },
                        {
                            "LayerId", null // Fixed: no support for revetments
                        },
                        {
                            "AlternativeId", null // Fixed: no support for piping
                        },
                        {
                            "SubMechanismId", subMechanimsId
                        },
                        {
                            "Method", subMechanismSettings.CalculationTechniqueId
                        },
                        {
                            "FormStartMethod", subMechanismSettings.FormStartMethod
                        },
                        {
                            "FormNumberOfIterations", subMechanismSettings.FormNumberOfIterations
                        },
                        {
                            "FormRelaxationFactor", subMechanismSettings.FormRelaxationFactor
                        },
                        {
                            "FormEpsBeta", subMechanismSettings.FormEpsBeta
                        },
                        {
                            "FormEpsHOH", subMechanismSettings.FormEpsHoh
                        },
                        {
                            "FormEpsZFunc", subMechanismSettings.FormEpsZFunc
                        },
                        {
                            "DsStartMethod", subMechanismSettings.DsStartMethod
                        },
                        {
                            "DsIterationmethod", 1 // Fixed: not relevant
                        },
                        {
                            "DsMinNumberOfIterations", subMechanismSettings.DsMinNumberOfIterations
                        },
                        {
                            "DsMaxNumberOfIterations", subMechanismSettings.DsMaxNumberOfIterations
                        },
                        {
                            "DsVarCoefficient", subMechanismSettings.DsVarCoefficient
                        },
                        {
                            "NiUMin", subMechanismSettings.NiUMin
                        },
                        {
                            "NiUMax", subMechanismSettings.NiUMax
                        },
                        {
                            "NiNumberSteps", subMechanismSettings.NiNumberSteps
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
    }
}