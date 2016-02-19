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
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Container for all configurations that are necessary for performing a Hydra-Ring calculation.
    /// </summary>
    public class HydraRingConfiguration
    {
        private IEnumerable<HydraRingConfigurationSettings> configurationSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingConfiguration"/> class.
        /// </summary>
        public HydraRingConfiguration()
        {
            configurationSettings = new[]
            {
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel,
                    MethodId = 1,
                    VariableId = 26
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.WaveHeight,
                    MethodId = 1,
                    VariableId = 28
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.WavePeakPeriod,
                    MethodId = 1,
                    VariableId = 29
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.WaveSpectralPeriod,
                    MethodId = 1,
                    VariableId = 29
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.QVariant,
                    MethodId = 6,
                    VariableId = 114
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.DikesOvertopping,
                    MethodId = 1,
                    VariableId = 1
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.DikesPiping,
                    MethodId = 1,
                    VariableId = 44
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.StructuresOvertopping,
                    MethodId = 1,
                    VariableId = 60
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.StructuresClosure,
                    MethodId = 1,
                    VariableId = 65
                },
                new HydraRingConfigurationSettings
                {
                    HydraRingFailureMechanismType = HydraRingFailureMechanismType.StructuresStructuralFailure,
                    MethodId = 1,
                    VariableId = 65
                }
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
        /// Gets or sets the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public HydraRingFailureMechanismType? FailureMechanismType { get; set; }

        /// <summary>
        /// Generates a database creation script that can be used to perform a Hydra-Ring calculation.
        /// </summary>
        /// <returns>The database creation script.</returns>
        /// <exception cref="InvalidOperationException">Thrown when one of the relevant input properties is not set.</exception>
        public string GenerateDataBaseCreationScript()
        {
            var configurationDictionary = new Dictionary<string, List<OrderedDictionary>>();

            ValidateDataBaseCreationScriptInput();

            InitializeHydraulicModelsConfiguration(configurationDictionary);
            InitializeSectionsConfiguration(configurationDictionary);
            InitializeDesignTablesConfiguration(configurationDictionary);
            InitializeAreasConfiguration(configurationDictionary);
            InitializeProjectsConfiguration(configurationDictionary);

            return GenerateDataBaseCreationScript(configurationDictionary);
        }

        private void ValidateDataBaseCreationScriptInput()
        {
            var formattedExceptionMessage = "Cannot generate database creation script: {0} unspecified.";

            if (TimeIntegrationSchemeType == null)
            {
                throw new InvalidOperationException(string.Format(formattedExceptionMessage, "TimeIntegrationSchemeType"));
            }

            if (UncertaintiesType == null)
            {
                throw new InvalidOperationException(string.Format(formattedExceptionMessage, "UncertaintiesType"));
            }

            if (HydraulicBoundaryLocation == null)
            {
                throw new InvalidOperationException(string.Format(formattedExceptionMessage, "HydraulicBoundaryLocation"));
            }

            if (FailureMechanismType == null)
            {
                throw new InvalidOperationException(string.Format(formattedExceptionMessage, "FailureMechanismType"));
            }
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
            configurationDictionary["Sections"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
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
                        "StationId1", HydraulicBoundaryLocation.Id
                    },
                    {
                        "StationId2", HydraulicBoundaryLocation.Id // Same as "StationId1": no support for coupling two stations
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
                }
            };
        }

        private void InitializeDesignTablesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var configurationSettingsForFailureMechanism = configurationSettings.First(cs => cs.HydraRingFailureMechanismType == FailureMechanismType);

            configurationDictionary["DesignTables"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "SectionId", 999 // TODO: Dike section integration
                    },
                    {
                        "MechanismId", (int?) FailureMechanismType
                    },
                    {
                        "LayerId", null // Fixed: no support for revetments
                    },
                    {
                        "AlternativeId", null // Fixed: no support for piping
                    },
                    {
                        "Method", configurationSettingsForFailureMechanism.MethodId
                    },
                    {
                        "VariableId", configurationSettingsForFailureMechanism.VariableId
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
                        "ValueMin", null // Fixed: no support for type 2 computations (see "Method")
                    },
                    {
                        "ValueMax", null // Fixed: no support for type 2 computations (see "Method")
                    },
                    {
                        "Beta", null // Fixed: no support for type 2 computations (see "Method")
                    }
                }
            };
        }

        private void InitializeAreasConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Areas"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "aDefault", 1 // Fixed: Not relevant
                    },
                    {
                        "bDefault", "1" // Fixed: Not relevant
                    },
                    {
                        "cDefault", "Nederland" // Fixed: Not relevant
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
                        "aDefault", 1 // Fixed: Not relevant
                    },
                    {
                        "bDefault", "Sprint" // Fixed: Not relevant
                    },
                    {
                        "cDefault", "Hydra-Ring Sprint" // Fixed: Not relevant
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